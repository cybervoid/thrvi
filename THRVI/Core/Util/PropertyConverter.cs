using System;
using System.Collections.Generic;
using THRVI.Core;
using System.Linq;


namespace THRVI.Core
{
	public class PropertyConverter
	{

		/*

		public string getLocationName(int id)
		{
			//string loc = gLocationsDictionary[id];
			if(id == 0)
			{
				loc = "";
			}
			else
			{
				//TODO: Determine the location from id
				loc = "loc";
			}


			return loc;
		}

		public int getLocationId(string location)
		{
			int locId = gLocationsDictionary.FirstOrDefault( x => x.Value == location).Key;

			//TODO: Create code to reverse calculate the Id of location, based upon its name in a string		

			//Use this temporarily:
					
			return locId;
		}
		*/

		public string getShot(bool whiteX)
		{
			string wx;

			if(whiteX == true)
			{
				wx = "X";
			}
			else
			{
				wx = "";
			}
			return wx;
		}
	}
}

