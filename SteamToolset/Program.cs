using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamToolset
{
	class Program
	{
		private static Arguments field_arguments;
		private static bool field_abort = false;

		static void Main(string[] param_arguments)
		{
			Console.CancelKeyPress += ConsoleOnCancelKeyPress;

			Global.Instance.Init();

			// Say hello
			PrintHello();

			// Prepare
			ClearScreenshots();

			// Parse arguments
			field_arguments = new Arguments(param_arguments);

			// If help was requested
			if (field_arguments.Help)
			{
				PrintHelp();
				Global.Instance.Deinit();
				Console.Write("Press any key");
				Console.ReadKey();
				return;
			}

			// Main cycle here
			Do();
			
			Global.Instance.Deinit();

			Console.WriteLine();
			Console.Write("Press any key");
			Console.ReadKey();
		}

		private static void ConsoleOnCancelKeyPress(object param_sender, ConsoleCancelEventArgs param_consoleCancelEventArgs)
		{
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Shutting down. May take some time.");
			
			field_abort = true;
		}

		private static void Do()
		{
			if (field_arguments.BrowserWidth.HasValue || field_arguments.BrowserHeight.HasValue)
			{
				Global.Instance.Steam.Resize(field_arguments.BrowserWidth, field_arguments.BrowserHeight);
			}

			// Ensure we've got login
			if (String.IsNullOrEmpty(field_arguments.AccountLogin))
			{
				Console.Write("Login      ");
				field_arguments.AccountLogin = Console.ReadLine();
			}
			else
			{
				Console.WriteLine("Login      " + field_arguments.AccountLogin);
			}
			// Ensure we've got password
			if (String.IsNullOrEmpty(field_arguments.AccountPassword))
			{
				Console.Write("Password   ");
				field_arguments.AccountPassword = Console.ReadLine();
				Util.ConsoleClearLinePrevious();
				Console.Write("Password   ");
				Console.Write(new string('*', field_arguments.AccountPassword.Length)); 
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine("Password   " + new string('*', field_arguments.AccountPassword.Length));
			}
			// Ensure we've got group name
			if (String.IsNullOrEmpty(field_arguments.GroupName))
			{
				Console.Write("Group name ");
				field_arguments.GroupName = Console.ReadLine();
			}
			else
			{
				Console.WriteLine("Group name " + field_arguments.GroupName);
			}

			Console.WriteLine();

			// Login
			while (!Global.Instance.Steam.IsLoggedIn())
			{
				try
				{
					Login();
				}
				catch (EmailTokenNeededException exception)
				{
					Console.Write("Email token is needed. Please type: ");
					field_arguments.EmailToken = Console.ReadLine();
				}
				catch (InvalidLoginException exception)
				{
					Console.Write("Login is invalid. Please retype: ");
					field_arguments.AccountLogin = Console.ReadLine();
				}
				catch (InvalidPasswordException exception)
				{
					Console.Write("Password is invalid. Please retype: ");
					field_arguments.AccountPassword = Console.ReadLine();
				}
				catch (Exception exception)
				{
					Console.WriteLine("Failed to login, reason unknown.");
					return;
				}
			}

			Group group = new Group(field_arguments.GroupName);
			int delay = 60000;

			while (true)
			{
				// Do stuff
				Global.Instance.Steam.Filter(group.Url);
				ClearScreenshots(32);

				// Wait until next
				if (field_abort)
				{
					break;
				}
				DateTime nextRun = DateTime.Now.AddMilliseconds(delay);
				while (DateTime.Now < nextRun)
				{
					if (field_abort)
					{
						break;
					}
					Thread.Sleep(100);
				}
			}
		}
		private static void ClearScreenshots(int param_countToKeep = 0)
		{
			if (!Directory.Exists("./screenshots/"))
			{
				Directory.CreateDirectory("./screenshots/");
			}
			string[] paths = Directory.GetFiles("./screenshots/", "*.png");
			for (int i = 0; i < (param_countToKeep > 0 ? paths.Length - param_countToKeep : paths.Length); i++)
			{
				string path = paths[i];
				try
				{
					File.Delete(path);
				}
				catch (Exception exception)
				{
					Console.WriteLine("Failed to delete {0}.", path);
				}
			}
		}
		private static void Login()
		{
			Global.Instance.Steam.Login(field_arguments.AccountLogin, field_arguments.AccountPassword, field_arguments.EmailToken);
		}
		private static void PrintHello()
		{
			//Console.WriteLine(@" ╔═╗┌┬┐┌─┐┌─┐┌┬┐╔╦╗┌─┐┌─┐┬  ┌─┐┌─┐┌┬┐ ");
			//Console.WriteLine(@" ╚═╗ │ ├┤ ├─┤│││ ║ │ ││ ││  └─┐├┤  │  ");
			//Console.WriteLine(@" ╚═╝ ┴ └─┘┴ ┴┴ ┴ ╩ └─┘└─┘┴─┘└─┘└─┘ ┴  ");
			Console.WriteLine(@"   _____ _                    _______          _          _    ");
			Console.WriteLine(@"  / ____| |                  |__   __|        | |        | |   ");
			Console.WriteLine(@" | (___ | |_ ___  __ _ _ __ ___ | | ___   ___ | |___  ___| |_  ");
			Console.WriteLine(@"  \___ \| __/ _ \/ _` | '_ ` _ \| |/ _ \ / _ \| / __|/ _ \ __| ");
			Console.WriteLine(@"  ____) | ||  __/ (_| | | | | | | | (_) | (_) | \__ \  __/ |_  ");
			Console.WriteLine(@" |_____/ \__\___|\__,_|_| |_| |_|_|\___/ \___/|_|___/\___|\__| ");
			Console.WriteLine();
			Console.WriteLine(@"                                                     by Nomex ");
			Console.WriteLine();
		}
		private static void PrintHelp()
		{
			Console.WriteLine("Version  0.1");
			Console.WriteLine("Build of {0:yyyy/MM/dd H:mm:ss zzz}", Assembly.GetExecutingAssembly().GetLinkerTime());
			Console.WriteLine();
			Console.WriteLine("Usage:");
			Console.WriteLine(" " + AppDomain.CurrentDomain.FriendlyName + " [-l login] [-p password] [-g groupname] [-e token] [-h]");
			Console.WriteLine();
			Console.WriteLine("Keys:");
			Console.WriteLine(" -l --login             account login");
			Console.WriteLine(" -p --password          account password");
			Console.WriteLine(" -e --emailtoken        email token");
			Console.WriteLine(" -g --groupname         group name");
			Console.WriteLine(" -h --help              print this help");
			Console.WriteLine();
			Console.WriteLine("Examples:");
			Console.WriteLine(" -l bot -p qwerty -e YT7FR -g nyashmyash");
			Console.WriteLine();
		}
	}
}
