using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using THRVI.Core;
using Android.Util;

namespace THRVI.Core
{
	public class TrailersAdapter : BaseAdapter<Trailer>
	{
		private Context gContext;
		private int gRowLayout;
		private List<Trailer> gTrailers;
		private int[] gAlternatingColors;
		private List<Location> gLocations;
		private LocationHandler gLh;// = new LocationHandler(gLocations);
		PropertyConverter gPc = new PropertyConverter();


		private Action<TextView> gActionStockNumberSelected;

		//Set TextView to global variables


		public TrailersAdapter (Context context, int rowLayout, List<Trailer> trailers, List<Location> locations, Action<TextView> stockNumberSelected)
		{
			gContext = context;
			gRowLayout = rowLayout;
			gTrailers = trailers;
			gAlternatingColors = new int[] { 0xF2F2F2, 0x009900 };
			gLocations = locations;
			gLh = new LocationHandler(gLocations);

			//Tutorial:
			gActionStockNumberSelected = stockNumberSelected;
		}

		public override int Count
		{
			get { return gTrailers.Count; }
		}

		public override Trailer this[int position]
		{
			get { return gTrailers[position]; }
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = convertView;

			if(row == null)
			{
				row = LayoutInflater.From(gContext).Inflate(gRowLayout, parent, false);
			}

			row.SetBackgroundColor(GetColorFromInteger(gAlternatingColors[position % gAlternatingColors.Length]));
			TextView stockNumber = row.FindViewById<TextView>(Resource.Id.txtStockNumber);
			stockNumber.Text = gTrailers[position].StockNumber;
			//Clear existing tag data
			stockNumber.Tag = null;
			//Set tag to store position in the gTrailers array. This will be retrieved after the invoke event
			stockNumber.Tag = position;


			TextView make = row.FindViewById<TextView>(Resource.Id.txtMake);
			make.Text = gTrailers[position].Make;
			TextView model = row.FindViewById<TextView>(Resource.Id.txtModel);
			model.Text = gTrailers[position].Model;



			TextView location = row.FindViewById<TextView>(Resource.Id.txtLocation);
			//Log.Info("TrailersAdapter", gLh.getLocationName(gTrailers[position].LocationId));
			location.Text = gLh.getLocationName(gTrailers[position].LocationId);

			TextView move = row.FindViewById<TextView>(Resource.Id.txtMove);
			move.Text = gLh.getLocationName(gTrailers[position].MoveId);

			TextView whiteX = row.FindViewById<TextView>(Resource.Id.txtWhiteX);
			whiteX.Text = gPc.getShot(gTrailers[position].WhiteX);

			if((position % 2) == 1)
			{
				//Green background, set text white
				stockNumber.SetTextColor(Color.Blue);
				stockNumber.PaintFlags = PaintFlags.UnderlineText; //Underline text for hyperlink feel
				make.SetTextColor(Color.White);
				model.SetTextColor(Color.White);
				location.SetTextColor(Color.White);
				move.SetTextColor(Color.White);
				whiteX.SetTextColor(Color.White);
			}
			else
			{
				//White background, black text
				stockNumber.SetTextColor(Color.Blue);
				stockNumber.PaintFlags = PaintFlags.UnderlineText; //Underline text for hyperlink feel
				make.SetTextColor(Color.Black);
				model.SetTextColor(Color.Black);
				location.SetTextColor(Color.Black);
				move.SetTextColor(Color.Black);
				whiteX.SetTextColor(Color.Black);
			}


			//Long Click to update an existing record
			//Log.Info("TrailersAdapter","Start Adding LongClick");

			Log.Info("TrailersAdapter","-= LongClick");
			stockNumber.LongClick -= StockNumber_LongClick;
			Log.Info("TrailersAdapter","+= LongClick");
			stockNumber.LongClick += StockNumber_LongClick;
		
			return row;
		}


		//UPDATE EXISTING RECORD
		void StockNumber_LongClick (object sender, View.LongClickEventArgs e)
		{
			/*
			TextView tv = (TextView) sender;
			int position = (int) tv.Tag;
			Log.Info ("TrailersAdapter", "Long Click " + position.ToString());
			*/

			//LongClick has been clicked.
			Log.Info ("TrailersAdapter", "Long Clicking Event Start");
			gActionStockNumberSelected.Invoke((TextView) sender);
			Log.Info ("TrailersAdapter", "Long Clicking Event FINISHED");
		}


		//ALTERNATING COLORS FOR EACH ROW
		private Color GetColorFromInteger(int color)
        {
            return Color.Rgb(Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }
	}
}

