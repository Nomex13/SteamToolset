using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamToolset
{
	public class Account
	{
		private string field_login;
		public string Login
		{
			get
			{
				return field_login;
			}
		}
		private string field_password;
		public string Password
		{
			get
			{
				return field_password;
			}
		}

		public Account(string param_login, string param_password)
		{
			field_login = param_login;
			field_password = param_password;
		}
	}
}
