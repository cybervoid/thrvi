using System;
using Android.App;
using Android.Views;
using Android.OS;
using Android.Content;
using Android.Widget;
using Android.Graphics;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using Android.Util;
using Android.Views.InputMethods;

namespace THRVI.Core
{

	public class OnCreateTrailerEventArgs : EventArgs
	{
		private int gId; 
		private string gStockNumber;
		private string gMake;
		private string gModel;
		private int gLocation;
		private bool gWhiteX;


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

		public bool WhiteX
		{
			get { return gWhiteX; }
			set { gWhiteX = value; }
		}

		public OnCreateTrailerEventArgs(int id, string stockNumber, string make, string model, int location, bool whiteX) : base()
		{
			Id = id;
			StockNumber = stockNumber;
			Make = make;
			Model = model;
			Location = location;
			WhiteX = whiteX;
		}
	}


	/********* NEW CLASS *******/

	public class DialogCreateTrailer : DialogFragment
	{

		private EditText gTxtStockNumber;
		private EditText gTxtMake;
		private EditText gTxtModel;
		private Spinner gSpinnerLocation;
		private CheckBox gCheckboxWhiteX;
		private Button gBtnCreateTrailer;
		private int gLocationId;
		private ProgressBar gProgressBar;
		private List<Location> gLocations;
		private LocationHandler gLocationHandler;

		//Create custom event.
		public event EventHandler<OnCreateTrailerEventArgs> gOnCreateTrailerComplete;

		public DialogCreateTrailer(List<Location> locations)
		{
			Log.Info("DialogCreateTrailer", "Constructor Started");
			gLocations = locations;
			gLocationHandler = new LocationHandler(locations);
			Log.Info("DialogCreateTrailer", "Constructor Completed");
		}
				
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState)	;

			var view = inflater.Inflate(Resource.Layout.dialog_create_trailer, container, false);

			Color hintColor = Color.LightSteelBlue;

			//Initialize resource components
			gTxtStockNumber = view.FindViewById<EditText>(Resource.Id.txtStockNumber);

			//this.ShowKeyboard(gTxtStockNumber);
			gTxtMake = view.FindViewById<EditText>(Resource.Id.txtMake);
			gTxtModel = view.FindViewById<EditText>(Resource.Id.txtModel);
			gSpinnerLocation = view.FindViewById<Spinner>(Resource.Id.spinnerLocation);
			gCheckboxWhiteX = view.FindViewById<CheckBox>(Resource.Id.checkboxWhiteX);
			gBtnCreateTrailer = view.FindViewById<Button>(Resource.Id.btnCreateTrailer);
			gProgressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);

			//Set Hint Color
			gTxtStockNumber.SetHintTextColor(hintColor);
			gTxtMake.SetHintTextColor(hintColor);
			gTxtModel.SetHintTextColor(hintColor);


			Log.Info("DialogCreateTrailer", "Completed 1");
			foreach(Location location in gLocations)
			{
				Log.Info("Checking gLocations", "DialogCreateTrailer ID " + gLocationHandler.getLocationId(location.Name));
			}

			//Populate Spinner with list of locations using custom adapter
			gSpinnerLocation.Adapter = new LocationSpinnerAdapter(this.Activity, gLocations);

			gSpinnerLocation.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);

			//Register click event
			gBtnCreateTrailer.Click += gBtnCreateTrailer_Click;

			return view;
		}

		private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Log.Info("DialogCreateTrailer", "Completed 3");
			Spinner spinner = (Spinner)sender;

			Java.Lang.Object item = (Java.Lang.Object)spinner.GetItemAtPosition(e.Position);

			//Cast from Java Object to Location class using customer cast class.
			Location location = LocationCast.Cast<Location>(item);

			string locName = location.Name;

			Log.Info("DialogCreateTrailer", "String Name e.Position: " + locName);
			gLocationId = gLocationHandler.getLocationId(locName);
			/*
			string toast = string.Format("{0}", spinner.GetItemAtPosition(e.Position));
			Toast.MakeText(this.Activity.ApplicationContext, toast, ToastLength.Long).Show();
			*/

			Log.Info("spinner_ItemSelected", "Location ID: " + gLocationId.ToString());

		}



		void gBtnCreateTrailer_Click (object sender, EventArgs e)
		{
			//Set Progressbar to visible to provide user feedback
			gProgressBar.Visibility = ViewStates.Visible;
			gBtnCreateTrailer.Clickable = false;

			//User has clicked create trailer button
			WebClient client = new WebClient ();
			Website w = new Website (); //List of website addresses
			Uri uri = new Uri (w.uriCreateTrailerPHP ());
			//Create namevaluecollection for uploading data to web server
			NameValueCollection parameters = new NameValueCollection ();

			//Add information to name value collection.
			parameters.Add ("StockNumber", gTxtStockNumber.Text);
			parameters.Add ("Make", gTxtMake.Text); 
			parameters.Add ("Model", gTxtModel.Text); 
			parameters.Add ("LocationId", gLocationId.ToString ()); //Convert ID to String because NameValueCollections suck and require strings.
			parameters.Add ("WhiteX", gCheckboxWhiteX.Checked.ToString ()); //Convert bool to string because NameValueCollections can't use bool.

			//if (parameters["StockNumber"] != "0" && parameters["Make"] != "0" && parameters["Model"] != "0") 
			//{
			client.UploadValuesCompleted += client_UploadValuesCompleted;
			client.UploadValuesAsync (uri, parameters);
			/*
			}
			else
			{				
				string error = "There was an error, please try again.";
				//Tell user there was an error
				Toast.MakeText(this.Activity.ApplicationContext, error, ToastLength.Long).Show();
				//Make Create Button clickable again
				gBtnCreateTrailer.Clickable = true;
				//Dismiss the Fragment
				this.Dismiss();
			}

			*/


		}

		void client_UploadValuesCompleted (object sender, UploadValuesCompletedEventArgs e)
		{

			Activity.RunOnUiThread(() => 
			{
				//Retrieve new database ID
				string id = Encoding.UTF8.GetString(e.Result);
				int newID = 0;
				//Parse id string into integer
				int.TryParse(id, out newID);

				//Send broadcast			
				if (gOnCreateTrailerComplete != null) 
				{
					gOnCreateTrailerComplete.Invoke (this, new OnCreateTrailerEventArgs (newID, gTxtStockNumber.Text, gTxtMake.Text, gTxtModel.Text, gLocationId, gCheckboxWhiteX.Checked));
				}

				//Re-enable clickability.
				gBtnCreateTrailer.Clickable = true;
				//Close dialog when done.
				this.Dismiss();

			});


		}


		//Show the keyboard
		//NOT USED CURRENTLY
		public void ShowKeyboard(View view)
		{
			view.RequestFocus();
			Application a = new Application();
			InputMethodManager inputMethodManager = a.GetSystemService(Context.InputMethodService) as InputMethodManager;
			inputMethodManager.ShowSoftInput(view, ShowFlags.Forced);
			inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
		}

		//Hide Keyboard
		//NOT USED CURRENTLY
		public void HideKeyboard(View view)
		{
			Application a = new Application();
			InputMethodManager inputMethodManager = a.GetSystemService(Context.InputMethodService) as InputMethodManager;
			inputMethodManager.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.None);
		}



		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
			base.OnActivityCreated(savedInstanceState);
		}

		//Continue here when ready to create new trailer dialog functionalities: 
		//https://www.youtube.com/watch?v=C4xodPCCmkU&list=PLCuRg51-gw5VqYchUekCqxUS9hEZkDf6l&index=9
	}
}

