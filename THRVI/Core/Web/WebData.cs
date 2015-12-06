using System;
using THRVI.Core;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using Android.Util;
using System.Threading;
using Android.App;


namespace THRVI.Core
{
	public class WebData
	{

		private List<Location> gLocations;
		private WebClient gClient;
		private Uri gUrl;

		//GOAL: Eliminate this class.

		//List of Locations
		public List<Location> locationsList()
		{
			gClient = new WebClient();
			Website w = new Website();
			gUrl = new Uri(w.uriGetLocationsPHP());
			//Uri url = new Uri("http://10.0.0.4/thrv/xta/android/GetLocations.php");
			//client.DownloadDataAsync(url);
			gClient.DownloadData(gUrl);
			Log.Info("WebData", "VlocationsList DownloadDataAsync Started");
			gClient.DownloadDataCompleted += client_DownloadDataCompleted;
			Log.Info("WebData", "^locationsList DownloadDataAsyncCompleted");
			//List<Location> localLocation = gLocations;

			/*
			int counter = 0;

			while(counter < 100 && (gLocations == null || gLocations.Count == 0))
			{
				System.Threading.Thread.Sleep(200);

				if(counter % 5 == 0)
				{
					Log.Info("Counter", counter.ToString());
				}

				counter++;
			}
			*/
			if(gLocations == null || gLocations.Count == 0)
			{
				Log.Info("locationList","gLocations is null");

			}
			foreach(Location loc in gLocations)
			{
				Log.Info("Location List", loc.Name);
			}

			return gLocations;
		}

		void client_DownloadDataCompleted (object sender, DownloadDataCompletedEventArgs e)
		{
			//Might need to run on ui thread in activity calling.
			//RunOnUiThread(() => 
			//{
			//ThreadPool.QueueUserWorkItem(o => 
			//{
				Log.Info("JSON START", "Start");
				string json = Encoding.UTF8.GetString(e.Result);
				Log.Info("JSON END", json);
				gLocations = JsonConvert.DeserializeObject<List<Location>>(json);
			//});
			//});
		}


		//List of Location in a dictionary.
		public Dictionary<int, string> locationsDictionary ()
		{
			//WebData wd = new WebData();
			Log.Info("WebData", "START locationsDictionary locationsList");
			List<Location> locations = locationsList();
			Log.Info("WebData", "END locationsDictionary locationsList");
			Dictionary<int, string> dict = new Dictionary<int, string> ();
			foreach (Location location in locations) 
			{
				int locId = location.Id;
				string locName = location.Name;
				dict.Add(locId, locName);
			}
			return dict;
		}

	}
}

