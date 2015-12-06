using System;
using System.Collections.Generic;
using THRVI.Core;
using System.Linq;
using Android.Util;

namespace THRVI.Core
{
	public class LocationHandler
	{
		Dictionary<int, string> gLocationsDictionary;
		List<Location> gLocations;

		public LocationHandler (List<Location> locations)
		{
			Log.Info("LH Constr", "Start");
			gLocationsDictionary = locationsDictionary(locations);
			gLocations = locations;
			Log.Info("LH Constr", "End");
		}


		public string getLocationName (int id)
		{
			
			string loc;

			if (id == 0) 
			{
				loc = "";
			} 
			else 
			{
				loc = gLocationsDictionary [id];
			}
			return loc;
		}

		public int getLocationId(string location)
		{
			//int locId = gLocationsDictionary.FirstOrDefault( x => x.Value == location).Key;

			int locId = gLocationsDictionary.FirstOrDefault(x => x.Value.Contains(location)).Key;
					
			return locId;
		}

		//List of Location in a dictionary.
		public Dictionary<int, string> locationsDictionary (List<Location> locations)
		{
			Dictionary<int, string> dict = new Dictionary<int, string> ();
			foreach (Location location in locations) 
			{
				int locId = location.Id;
				string locName = location.Name;
				Log.Info("Adding to Dictionary", locName);
				dict.Add(locId, locName);
			}
			return dict;
		}

		/// <summary>
		/// Gets the location position from the list of Locations.
		/// </summary>
		/// <returns>The location position.</returns>
		/// <param name="id">Database ID #.</param>
		public int getLocationPosition(int id)
		{
			int position = gLocations.FindIndex(t => t.Id == id);
			return position;
		}


		/// <summary>
		/// Gets the location position from the List of Locations.
		/// </summary>
		/// <returns>The location position.</returns>
		/// <param name="name">Location Name</param>
		public int getLocationPosition(string name)
		{
			int position = gLocations.FindIndex(t => t.Name == name);
			return position;
		}

	}
}

