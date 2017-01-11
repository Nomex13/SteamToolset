using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamToolset
{
	public class Arguments
	{
		private string field_accountLogin;
		public string AccountLogin
		{
			get
			{
				return field_accountLogin;
			}
			set
			{
				field_accountLogin = value;
			}
		}
		private string field_accountPassword;
		public string AccountPassword
		{
			get
			{
				return field_accountPassword;
			}
			set
			{
				field_accountPassword = value;
			}
		}
		private string field_accountSteamID;
		public string AccountSteamID
		{
			get
			{
				return field_accountSteamID;
			}
			set
			{
				field_accountSteamID = value;
			}
		}
		private string field_groupName;
		public string GroupName
		{
			get
			{
				return field_groupName;
			}
			set
			{
				field_groupName = value;
			}
		}
		private string field_emailToken;
		public string EmailToken
		{
			get
			{
				return field_emailToken;
			}
			set
			{
				field_emailToken = value;
			}
		}
		private int? field_browserHeight;
		public int? BrowserHeight
		{
			get
			{
				return field_browserHeight;
			}
			set
			{
				field_browserHeight = value;
			}
		}
		private int? field_browserWidth;
		public int? BrowserWidth
		{
			get
			{
				return field_browserWidth;
			}
			set
			{
				field_browserWidth = value;
			}
		}
		private bool field_help = false;
		public bool Help
		{
			get
			{
				return field_help;
			}
			set
			{
				field_help = value;
			}
		}

		public Arguments(string[] param_arguments)
		{
			for (int i = 0; i < param_arguments.Length; i++)
			{
				string key = param_arguments[i];
				string argument = i < param_arguments.Length - 1 ? param_arguments[i + 1] : null;
				if (IsKey(key, new [] { "-l", "--login" }))
				{
					AccountLogin = argument;
					i++;
					continue;
				}
				if (IsKey(key, new [] { "-p", "--password" }))
				{
					AccountPassword = argument;
					i++;
					continue;
				}
				if (IsKey(key, new [] { "-i", "--steamid" }))
				{
					AccountSteamID = argument;
					i++;
					continue;
				}
				if (IsKey(key, new [] { "-e", "--emailtoken" }))
				{
					EmailToken = argument;
					i++;
					continue;
				}
				if (IsKey(key, new [] { "-g", "--groupname" }))
				{
					GroupName = argument;
					i++;
					continue;
				}
				if (IsKey(key, new [] { "-h", "--height" }))
				{
					BrowserHeight = Int32.Parse(argument);
					i++;
					continue;
				}
				if (IsKey(key, new [] { "-w", "--width" }))
				{
					BrowserWidth = Int32.Parse(argument);
					i++;
					continue;
				}
				if (IsKey(key, new [] { "--help" }))
				{
					Help = true;
					continue;
				}
			}
		}

		private static bool IsKey(string param_string, IEnumerable<string> param_keys)
		{
			return param_keys.ToList().Find(param_key => String.Equals(param_key, param_string, StringComparison.InvariantCultureIgnoreCase)) != null;
		}
	}
}
