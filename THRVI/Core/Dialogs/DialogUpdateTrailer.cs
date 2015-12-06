
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Net;
using System.Collections.Specialized;

namespace THRVI.Core
{

	public class OnUpdateTrailerEventArgs : EventArgs
	{
		private int gId; 
		private string gStockNumber;
		private string gMake;
		private string gModel;
		private int gLocation;
		private bool gWhiteX;
		private bool gActive;


		public int Id 
		{
			get	{ return gId; }
			set { gId = value; }
		}
					
		public string StockNumber
		{
			get { return gStockNumber; }
			set { gStockNumber = value; }
		}

		public string Make
		{
			get { return gMake; }
			set { gMake = value; }
		}

		public string Model
		{
			get { return gModel; }
			set { gModel = value; }
		}

		public int Location
		{
			get { return gLocation; }
			set { gLocation = value; }
		}

		//White X is used to signify if the marketing team has photo'd and video'd the trailer.
		public bool WhiteX
		{
			get { return gWhiteX; }
			set { gWhiteX = value; }
		}


		public bool Active
		{
			get { return gActive; }
			set { gActive = value; }
		}

		public OnUpdateTrailerEventArgs(int id, string stockNumber, string make, string model, int location, bool whiteX)
		{
			Id = id;
			StockNumber = stockNumber;
			Make = make;
			Model = model;
			Location = location;
			WhiteX = whiteX;
		}
	}



	public class DialogUpdateTrailer : DialogFragment
	{

		private EditText gTxtUpdateStockNumber;
		private EditText gTxtUpdateMake;
		private EditText gTxtUpdateModel;
		private Spinner gSpinnerUpdateLocation;
		private CheckBox gCheckboxUpdateWhiteX;
		private Button gBtnUpdateTrailer;
		private int gLocationId; //Value defined in the spinner adapter
		private ProgressBar gProgressBarUpdate;
		private List<Location> gLocations;
		private LocationHandler gLocationHandler;


		private string gStockNumber;
		private string gMake;
		private string gModel;
		private int gLocation; //Value define in the constructor
		private bool gWhiteX;
		private int gId;

		//Create custom event
		public event EventHandler<OnUpdateTrailerEventArgs> gOnUpdateTrailerComplete;

		public DialogUpdateTrailer(List<Location> locations, int id, string stockNumber, string make, string model, int location, bool whiteX /*, bool active*/)
		{
			Log.Info("DialogUpdateTrailer", "Constructor Started");
			//Location for spinner
			gLocations = locations;
			gLocationHandler = new LocationHandler(locations);

			//Location for default values
			gStockNumber = stockNumber;
			gMake = make;
			gModel = model;
			gLocation = location; 
			gWhiteX = whiteX;
			//gActive = active;
			gId = id;
			Log.Info("DialogUpdateTrailer", "Constructor FINISHED");
		}


		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			Log.Info("DialogUpdateTrailer", "Complete 1");


			base.OnCreateView(inflater, container, savedInstanceState);
			Log.Info("DialogUpdateTrailer", "Complete 2");
			var view = inflater.Inflate(Resource.Layout.dialog_update_trailer, container, false);
			Log.Info("DialogUpdateTrailer", "Complete 3");
			Log.Info("DialogUpdateTrailer", "Variable Check - gStockNumber: " + gStockNumber + " - MAKE: " + gMake);
			//Initialize resource components and set default values
			gTxtUpdateStockNumber = view.FindViewById<EditText>(Resource.Id.txtUpdateStockNumber);
			gTxtUpdateStockNumber.Text = gStockNumber;

			Log.Info("DialogUpdateTrailer", "Complete 4");

			gTxtUpdateMake = view.FindViewById<EditText>(Resource.Id.txtUpdateMake);
			gTxtUpdateMake.Text = gMake;

			gTxtUpdateModel = view.FindViewById<EditText>(Resource.Id.txtUpdateModel);
			gTxtUpdateModel.Text = gModel;

			gSpinnerUpdateLocation = view.FindViewById<Spinner>(Resource.Id.spinnerUpdateLocation);


			gCheckboxUpdateWhiteX = view.FindViewById<CheckBox>(Resource.Id.checkboxUpdateWhiteX);
			gCheckboxUpdateWhiteX.Checked = gWhiteX;

			/* Removing sold checkbox so users don't accidently mark something as sold
			gCheckboxUpdateActive = view.FindViewById<CheckBox>(Resource.Id.checkboxUpdateActive);
			gCheckboxUpdateActive.Checked = !gActive;
			*/
			gBtnUpdateTrailer = view.FindViewById<Button>(Resource.Id.btnUpdateTrailer);
			gProgressBarUpdate = view.FindViewById<ProgressBar>(Resource.Id.progressBarUpdate);


			//Populate Spinner with list of locations using custom adapter
			gSpinnerUpdateLocation.Adapter = new LocationSpinnerAdapter(this.Activity, gLocations);
			gSpinnerUpdateLocation.SetSelection(gLocationHandler.getLocationPosition(gLocation));
			gSpinnerUpdateLocation.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);

			gBtnUpdateTrailer.Click += gBtnUpdateTrailer_Click;
			Log.Info("DialogUpdateTrailer", "Complete 5");
			return view;

		}

		//Load locations into spinner
		private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			
			Spinner spinner = (Spinner)sender;

			Java.Lang.Object item = (Java.Lang.Object)spinner.GetItemAtPosition(e.Position);

			//Cast from Java Object to Location class using customer cast class.
			Location location = LocationCast.Cast<Location>(item);
			string locName = location.Name;
			gLocationId = gLocationHandler.getLocationId(locName);
		}


		void gBtnUpdateTrailer_Click (object sender, EventArgs e)
		{
			//Set progressbar to visible to provide user feedback
			gProgressBarUpdate.Visibility = ViewStates.Visible;
			//Disable Update Button so people cannot double click.
			gBtnUpdateTrailer.Clickable = false;
			//User has clicked update trailer button
			WebClient client = new WebClient();
			Website w = new Website(); //load website urls
			Uri uri = new Uri(w.uriUpdateTrailerPHP());

			//Create name value collection
			NameValueCollection parameters = new NameValueCollection();

			//Add data to namevalue collection
			parameters.Add("Id",gId.ToString());
			parameters.Add("StockNumber", gTxtUpdateStockNumber.Text);
			parameters.Add("Make", gTxtUpdateMake.Text);
			parameters.Add("Model", gTxtUpdateModel.Text);
			parameters.Add("LocationId", gLocationId.ToString()); //Convert id to string because name value collections require string
			parameters.Add("WhiteX", gCheckboxUpdateWhiteX.Checked.ToString());

			//May add a sold column in database, will need to change this if so.

			client.UploadValuesCompleted += client_UploadValuesCompleted;
			client.UploadValuesAsync (uri, parameters);

		}

		void client_UploadValuesCompleted (object sender, UploadValuesCompletedEventArgs e)
		{
			Activity.RunOnUiThread(() =>
			{
				//Send Broadcast
				if(gOnUpdateTrailerComplete != null)
				{
					gOnUpdateTrailerComplete.Invoke(this, new OnUpdateTrailerEventArgs(gId, gTxtUpdateStockNumber.Text, gTxtUpdateMake.Text, gTxtUpdateModel.Text, 
																						gLocationId, gCheckboxUpdateWhiteX.Checked));
																											
				}
				//Make Create Button clickable again
				gBtnUpdateTrailer.Clickable = true;
				this.Dismiss();

			});
		}

	}
}

