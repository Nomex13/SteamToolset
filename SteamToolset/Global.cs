using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamToolset
{
	public class Global
	{
		private static Global field_instance;
		public static Global Instance
		{
			get
			{
				if (field_instance == null)
				{
					field_instance = new Global();
					field_instance.Init();
				}
				return field_instance;
			}
		}

		private Global()
		{
			;
		}
		
		private Steam field_steam;
		public Steam Steam
		{
			get
			{
				if (!field_inited)
				{
					Init();
				}
				return field_steam;
			}
		}

		private bool field_inited = false;
		public void Init()
		{
			if (field_inited)
			{
				return;
			}

			field_steam = new Steam();
			field_steam.Init();

			field_inited = true;
		}
		public void Deinit()
		{
			if (!field_inited)
			{
				return;
			}

			field_steam.Deinit();
			field_steam = null;

			field_inited = false;
		}
		public void Reinit()
		{
			Deinit();
			Init();
		}
	}
}
