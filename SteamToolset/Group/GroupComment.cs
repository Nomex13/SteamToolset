using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamToolset
{
	public class GroupComment
	{
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
		private List<string> field_keyWords = new List<string>() { "$", "dollar", "profit", "http", "code", "giweaway" };

		public GroupComment(string param_text, string param_id = null)
		{
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
			return count > field_keyWords.Count / 2;
		}
	}
}
