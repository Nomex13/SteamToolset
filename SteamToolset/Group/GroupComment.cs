using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamToolset
{
	public class GroupComment
	{
		private string field_profileName;
		public string ProfileName
		{
			get
			{
				return field_profileName;
			}
		}
		private string field_profileUrl;
		public string ProfileUrl
		{
			get
			{
				return field_profileUrl;
			}
		}
		private string field_text;
		public string Text
		{
			get
			{
				return field_text;
			}
		}
		private string field_id;
		public string Id
		{
			get
			{
				return field_id;
			}
		}
		private List<string> field_keyWords = new List<string>() { "$", "dollar", "money", "profit", "http", "code", "giveaway", "discount", "it's ez", "{link removed}" };

		public GroupComment(string param_profileName, string param_profileUrl, string param_text, string param_id = null)
		{
			field_profileName = param_profileName;
			field_profileUrl = param_profileUrl;
			field_text = param_text;
			field_id = param_id;
		}
		public bool IsSpam()
		{
			string text = field_text.ToLower();
			int count = 0;
			foreach (string keyWord in field_keyWords)
			{
				if (text.Contains(keyWord))
				{
					count++;
				}
			}
			return count >= 4;
		}
	}
}
