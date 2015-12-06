using System;
using System.Text;
using Android.Util;

namespace THRVI.Core
{
	public class Website
	{
		public static string baseUrl()
		{
			string url = "http://www.thrvc.net/thrv/xta/android/"; //Release
			//string url = "http://10.0.0.4/thrv/xta/android/"; //Debugging
			return url;
		}

		public string uriCreateMovePHP()
		{
			StringBuilder fullUrl = new StringBuilder(Website.baseUrl());
			fullUrl.Append("CreateMove.php");
			return fullUrl.ToString();
		}

		public string uriCreateTrailerPHP()
		{
			StringBuilder fullUrl = new StringBuilder(Website.baseUrl());
			fullUrl.Append("CreateTrailer.php");		
			return fullUrl.ToString();
		}

		public string uriGetLocationsPHP()
		{
			StringBuilder fullUrl = new StringBuilder(Website.baseUrl());
			fullUrl.Append("GetLocations.php");
			//Log.Info("Website", fullUrl.ToString());
			return fullUrl.ToString();
		}

		public string uriGetMovesPHP()
		{
			StringBuilder fullUrl = new StringBuilder(Website.baseUrl());
			fullUrl.Append("GetMoves.php");
			return fullUrl.ToString();
		}

		public string uriGetTrailersPHP()
		{
			StringBuilder fullUrl = new StringBuilder(Website.baseUrl());
			fullUrl.Append("GetTrailers.php");
			return fullUrl.ToString();
		}

		public string uriUpdateTrailerPHP()
		{
			StringBuilder fullUrl = new StringBuilder(Website.baseUrl());
			fullUrl.Append("UpdateTrailer.php");
			return fullUrl.ToString();
		}
	}
}

