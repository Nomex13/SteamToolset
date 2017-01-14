using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using CefSharp;
using Newtonsoft.Json;
using CefSharp.OffScreen;

namespace SteamToolset
{
	public class Steam
	{
		private const string field_urlLogin = "https://steamcommunity.com/login/home/";
		private const string field_urlHome = "https://steamcommunity.com/";

		private bool field_inited = false;
		private ChromiumWebBrowser field_browser;
		private volatile  bool field_isLoaded = false;
		private int field_screenshotCount = 0;
		private string field_pathCache;

		private int field_width = 1024;
		private int field_height = 768;

		public Steam()
		{
			field_pathCache = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache/");
			if (!Directory.Exists(field_pathCache))
			{
				Directory.CreateDirectory(field_pathCache);
			}

			Clear();

			//browser.RenderProcessMessageHandler = new RenderProcessMessageHandler();

            // We have to wait for something, otherwise the process will exit too soon.
			//Console.ReadKey();

            // Clean up Chromium objects.  You need to call this in your application otherwise
            // you will get a crash when closing.
		}

		public void Init()
		{
			if (field_inited)
			{
				return;
			}
			if (!Cef.IsInitialized)
			{
				if (!Directory.Exists(field_pathCache))
				{
					Directory.CreateDirectory(field_pathCache);
				}
				if (!Cef.IsInitialized)
				{
					Cef.Initialize(new CefSettings() { CachePath = field_pathCache, LogFile = "./browser.log", LogSeverity = LogSeverity.Disable }, performDependencyCheck: true, browserProcessHandler:null);
				}
				//Cef.Initialize(new CefSettings() { CachePath = field_pathCache }, performDependencyCheck: true, browserProcessHandler:null);
			}
			if (field_browser == null)
			{
				field_browser = new ChromiumWebBrowser();
				field_browser.Size = new Size(field_width, field_height);

				while (!field_browser.IsBrowserInitialized)
				{
					Thread.Sleep(10);
				}

				// An event that is fired when the first page is finished loading.
				// This returns to us from another thread.
				field_browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
				field_browser.FrameLoadEnd += BrowserOnFrameLoadEnd;
			}

			field_inited = true;
		}
		public void Deinit()
		{
			if (!field_inited)
			{
				return;
			}
			if (field_browser != null);
			{
				//field_browser.Dispose();
				field_browser = null;
			}
			//if (Cef.IsInitialized)
			//{
			//	Cef.Shutdown();
			//}

			//// HACK: Just kill it already
			////TextWriter textWriter = Console.Out;
			////Console.SetOut(TextWriter.Null);
			//System.Diagnostics.Process process = new System.Diagnostics.Process();
			//System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
			//startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			//startInfo.FileName = "cmd.exe";
			//startInfo.Arguments = "/C taskkill /IM CefSharp.BrowserSubprocess.exe /F";
			//process.StartInfo = startInfo;
			//process.Start();
			//process.WaitForExit();
			////Console.SetOut(textWriter);

			field_inited = false;
		}
		public void Clear()
		{
			Deinit();
			
			Init();

			//// Create the offscreen Chromium browser.
			//field_browser =  new ChromiumWebBrowser();
			//field_browser.Size = new Size(1024, 2048);
		}

		//// Wait for the underlying `Javascript Context` to be created, this is only called for the main frame.
		//// If the page has no javascript, no context will be created.
		//void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
		//{
		//const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";

		//frame.ExecuteJavaScriptAsync(script);
		//}
		//}

		////Wait for the page to finish loading (all resources will have been loaded, rendering is likely still happening)
		//browser.LoadingStateChanged += (sender, args) =>
		//{
		////Wait for the Page to finish loading
		//if (args.IsLoading == false)
		//{
		//browser.ExecuteJavaScriptAsync("alert('All Resources Have Loaded');");
		//}
		//}

		////Wait for the MainFrame to finish loading
		//browser.FrameLoadEnd += (sender, args) =>
		//{
		////Wait for the MainFrame to finish loading
		//if(args.Frame.IsMain)
		//{
		//args.Frame.ExecuteJavaScriptAsync("alert('MainFrame finished loading');");
		//}
		//};

		private void BrowserOnLoadingStateChanged(object param_sender, LoadingStateChangedEventArgs param_loadingStateChangedEventArgs)
		{
			if (param_loadingStateChangedEventArgs.IsLoading)
			{
				return;
			}

			field_isLoaded = true;
		}

		private void BrowserOnFrameLoadEnd(object param_sender, FrameLoadEndEventArgs param_frameLoadEndEventArgs)
		{
			if (!param_frameLoadEndEventArgs.Frame.IsMain)
			{
				return;
			}
			//field_isLoaded = true;
		}

		private void Load(string param_url)
		{
			Console.Write("Loading {0}", param_url);
			field_isLoaded = false;
			field_browser.Load(param_url);
			Thread.Sleep(250);
			while (!field_isLoaded || param_url != field_browser.Address)
			{
				Thread.Sleep(10);
			}
			Thread.Sleep(250);
			Execute("undefined;");
			Console.WriteLine(" ... done");
		}
		private JavascriptResponse Execute(string param_javascript)
		{
			field_isLoaded = false;
			JavascriptResponse response = ExecuteAsync(param_javascript).Result;
			Thread.Sleep(100);
			field_isLoaded = true;
			return response;
		}
		private async Task<JavascriptResponse> ExecuteAsync(string param_javascript)
		{
			return await field_browser.GetMainFrame().EvaluateScriptAsync(param_javascript);
		}
		public void Resize(int? param_width = null, int? param_height = null)
		{
			//return;
			int width = param_width.HasValue ? param_width.Value : field_browser.Size.Width;
			int height = param_height.HasValue ? param_height.Value : field_browser.Size.Height;

			field_width = width;
			field_height = height;

			field_browser.Size = new Size(field_width, field_height);
		}

		private void Screenshot(string param_name = "")
		{
			//return;
			string path = "./screenshots/" + String.Format("{0:D4}", field_screenshotCount) + (param_name == "" ? "" : ("." + param_name)) + ".png";
			try
			{
				Task<Bitmap> bitmap = field_browser.ScreenshotAsync(false);
				bitmap.Wait();
				bitmap.Result.Save(path);
				bitmap.Dispose();
			}
			catch
			{
				if (File.Exists(path))
				{
					try
					{
						File.Delete(path);
					}
					catch (Exception exception)
					{
						;
					}
				}
			}
			finally
			{
				field_screenshotCount++;
			}
			return;
		}

		public bool IsLoggedIn()
		{
			Load(field_urlHome);
			Screenshot();
			
			var result = Execute("document.getElementById(\"header_notification_link\") != null;").Result;
			return result is bool && (bool)result;
		}
		public string SteamID()
		{
			string steamId = (string)Execute("g_steamID;").Result;
			return steamId;
		}
		public void Filter(Group param_group)
		{
			Load(param_group.Url);

			Screenshot("group");

			List<GroupComment> comments = new List<GroupComment>();

			int commentsCount = (int)Execute("document.getElementsByClassName(\"commentthread_comment_text\").length;").Result;
			for (int commentPosition = 0; commentPosition < commentsCount; commentPosition++)
			{
				string commentText = (string)Execute("document.getElementsByClassName(\"commentthread_comment_text\")[" + commentPosition + "].innerText;").Result;
				string commentHtmlId = (string)Execute("document.getElementsByClassName(\"commentthread_comment_text\")[" + commentPosition + "].id;").Result;
				string commentId = commentHtmlId.Substring(16);
				string commentProfileName = (string)Execute("document.getElementById(\"comment_" + commentId + "\").getElementsByTagName(\"bdi\")[0].innerText;").Result;
				string commentProfileUrl = (string)Execute("document.getElementById(\"comment_" + commentId + "\").getElementsByClassName(\"commentthread_author_link\")[0].href;").Result;
				GroupComment comment = new GroupComment(commentProfileName, commentProfileUrl, commentText, commentId);
				comments.Add(comment);
			}
			
			string groupIdRaw = (string)Execute("document.getElementsByClassName(\"commentthread_comments\")[0].id;").Result;
			string groupId = groupIdRaw.Substring(14, 25);

			foreach (GroupComment comment in comments)
			{
				if (comment.IsSpam())
				{
					string padding = "    ";
					Console.WriteLine("Comment is considered spam and is being deleted:");
					Console.WriteLine(padding + comment.Text.Replace("\n", "\n" + padding));

					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("\r\n[Comment.Deleted]");
					stringBuilder.Append(String.Format("\r\nTimestamp = {0:yyyy/MM/dd HH:mm:ss zzz}", DateTime.Now));
					stringBuilder.Append("\r\nNickname  = ");
					stringBuilder.Append(comment.ProfileName);
					stringBuilder.Append("\r\nProfile   = ");
					stringBuilder.Append(comment.ProfileUrl);
					stringBuilder.Append("\r\nComment   = ");
					stringBuilder.Append(comment.Text.Replace("\r", "").Replace("\n", " \\ "));
					stringBuilder.Append("\r\n");

					File.AppendAllText("group.filter." + param_group.Name.ToLower() + ".txt", stringBuilder.ToString());

					Execute("javascript:CCommentThread.DeleteComment('" + groupId + "', '" + comment.Id + "');");
					Thread.Sleep(5000);
				}
			}
		}
		public bool Login(string param_login, string param_password, string param_token = "")
		{
			
			Load(field_urlLogin);

			Console.WriteLine("Trying to log in.");

			Screenshot("login.empty");

			Execute("document.getElementById(\"steamAccountName\").value=\"" + param_login + "\";" +
					"document.getElementById(\"steamPassword\").value=\"" + param_password + "\";");
					
			Screenshot("login.filled");

			Execute("document.getElementById(\"SteamLogin\").click();");

			Screenshot("login.clicked");

			Thread.Sleep(10000);
			
			Screenshot("login.waited");

			var result = Execute("document.getElementsByClassName(\"login_modal loginAuthCodeModal\")[0].style.display != \"none\";").Result;
			if (!(result is bool) || !(bool)result)
			{
				Console.WriteLine("Email token not needed.");

				const long delayInMilliseconds = 10000;
				DateTime now = DateTime.Now;
				while (field_browser.Address == field_urlLogin)
				{
					Thread.Sleep(10);
					if ((DateTime.Now - now).Ticks > delayInMilliseconds * 10000)
					{
						Console.WriteLine("Could not log in.");
						Screenshot("not.logged.in");
						return false;
					}
				}

				Load(field_urlHome);

				Console.WriteLine("Logged in.");
				Screenshot("logged.in");
				return true;
			}
			else
			{
				if ((bool)result)
				{
					Console.WriteLine("Email token needed.");
					if (String.IsNullOrEmpty(param_token))
					{
						Console.WriteLine("Email token not specified.");
						throw new EmailTokenNeededException();
					}

					Console.WriteLine("Applying email token.");

					Execute("document.getElementById(\"authcode\").value=\"" + param_token + "\";");

					Screenshot("token.filled");

					Execute("document.getElementById(\"auth_buttonset_entercode\").getElementsByClassName(\"auth_button leftbtn\")[0].click();");
				
					Screenshot("token.clicked");
					
					const long delayInMilliseconds = 10000;
					DateTime now = DateTime.Now;
					while (field_browser.Address == field_urlLogin)
					{
						Thread.Sleep(10);
						if ((DateTime.Now - now).Ticks > delayInMilliseconds * 10000)
						{
							Console.WriteLine("Could not log in.");
							Screenshot("not.logged.in");
							return false;
						}
					}

					Load(field_urlHome);

					Console.WriteLine("Logged in.");
					Screenshot("logged.in");
					return true;
				}
			}
			
			Console.WriteLine("Could not log in.");
			return false;
		}
	}
}
