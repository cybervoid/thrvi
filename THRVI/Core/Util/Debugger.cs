using System;
using Android.Util;
namespace THRVI
{
	public class Debugger
	{
		public void debugInfoLogcat(string tag, string msg)
		{
			Log.Info(tag, msg);
		}

		public void debugWarnLogcat(string tag, string msg)
		{
			Log.Warn (tag, msg);
		}

		public void debugErrorLogcat(string tag, string msg)
		{
			Log.Error(tag, msg);
		}
	}

}

