using System;
using Android.Widget;
using System.Collections.Generic;
using THRVI.Core;
using Android.Views;
using Android.App;

namespace THRVI.Core
{
	public class LocationSpinnerAdapter : BaseAdapter<Location>
	{
		List<Location> gLocations;
		Activity context;


		public LocationSpinnerAdapter(Activity context, List<Location> locations) : base()
		{
			this.context = context;
			gLocations = locations;
		}

		public override long GetItemId(int position)
		{
			return (long)gLocations[position].Id;
		}

		public override Location this[int position]
		{
			get { return gLocations[position]; }
		}

		public override int Count
		{
			get { return gLocations.Count; }
		}


		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; //Re-use existing view, if one is available

			if(view == null)
			{
				view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
			}
			view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = gLocations[position].Name;

			return view;
		}
	}
}

