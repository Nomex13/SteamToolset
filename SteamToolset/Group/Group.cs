using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamToolset
{
	public class Group
	{
		private string field_name;
		//private string field_url;
		public string Name
		{
			get
			{
				return field_name;
			}
		}
		public string Url
		{
			get
			{
				return "https://steamcommunity.com/groups/" + field_name + "/";
			}
		}

		public Group(string param_name)
		{
			field_name = param_name;
		}

		public void Filter()
		{
			//Global.Instance.Steam.
		}
	}
}
