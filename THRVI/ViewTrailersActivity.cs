
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
using THRVI.Core;
using Android.Views.InputMethods;
using Android.Util;
using System.Net;
using Newtonsoft.Json;


namespace THRVI.Core
{
	[Activity (Label = "ViewTrailersActivity")]			
	public class ViewTrailersActivity : Activity
	{

		private List<Trailer> gTrailers;
		private ListView gListView;
		private EditText gSearch;
		private LinearLayout gContainer;
		private bool gAnimatedDown;
		private bool gIsAnimating;
		private TrailersAdapter gAdapter;

		//TextViews located on view_trailers.axml
		private TextView gTxtHeaderStockNumber;
		private TextView gTxtHeaderMake;
		private TextView gTxtHeaderModel;
		private TextView gTxtHeaderLocation;
		private TextView gTxtHeaderWhiteX;

		//Create a final working Trailers List after any sorting or searching that may occur.
		//To update a trailer, the program stores the position of the Trailer on the gTrailers List into the TextView's tag.
		//When it determines which trailer to update, the program retrieves the position from the TextView tag.
		//If the user searches or sorts the list of trailers from the GUI with LINQ, the tag position is updated in the TrailerAdapter class
		//A global list of trailers is required that properly reflects the new sorted position.
		private List<Trailer> gTrailersSorted; 
											  
		//Booleans to determines if the list of trailers is currently sorted.
		private bool gStockNumberAscending;
		private bool gMakeAscending;
		private bool gModelAscending;
		private bool gLocationAscending;
		private bool gWhiteXAscending;

		//Initial loading progressbar
		private ProgressBar gProgressBar;
		//private LinearLayout gLinearLayoutProgressBar; // This isn't woring.
		private WebClient gClient;
		private Uri gUrl;

		//Create List of locations from the server's database
		private List<Location> gLocations;
		private WebClient gLocationsClient;
		private Uri gLocationsUrl;

		//Selected LongClick stock number
		private TextView gSelectedStockNumber;
		Action<TextView> gActionUpdate;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.view_trailers);


			gListView = FindViewById<ListView> (Resource.Id.listView);
			gSearch = FindViewById<EditText> (Resource.Id.etSearch);
			gContainer = FindViewById<LinearLayout> (Resource.Id.llContainer);

			//gLinearLayoutProgressBar = FindViewById<LinearLayout>(Resource.Id.linearLayout1ProgressBar); // Linear Layout for progress bar.
			//gLinearLayoutProgressBar.BringToFront(); // This isn't woring.

			//Textviews for sorting with LINQ
			gTxtHeaderStockNumber = FindViewById<TextView> (Resource.Id.txtHeaderStockNumber);
			gTxtHeaderMake = FindViewById<TextView> (Resource.Id.txtHeaderMake);
			gTxtHeaderModel = FindViewById<TextView> (Resource.Id.txtHeaderModel);
			gTxtHeaderLocation = FindViewById<TextView> (Resource.Id.txtHeaderLocation);
			gTxtHeaderWhiteX = FindViewById<TextView> (Resource.Id.txtHeaderWhiteX);
																		
			//Click events for sorting by TextView
			gTxtHeaderStockNumber.Click += gTxtHeaderStockNumber_Click;
			gTxtHeaderMake.Click += gTxtHeaderMake_Click;
			gTxtHeaderModel.Click += gTxtHeaderModel_Click;
			gTxtHeaderLocation.Click += gTxtHeaderLocation_Click;
			gTxtHeaderWhiteX.Click += gTxtHeaderWhiteX_Click;

			//TODO: Get the progress bar working... its probably hiding under the container.
			gProgressBar = FindViewById<ProgressBar> (Resource.Id.progressBar);

			//Action for TrailerAdapter LongClick
			gActionUpdate = StockNumberSelected;

			gSearch.Alpha = 0; //Changing this value gives the search the illusion of fading in and out
			gContainer.BringToFront ();  //Keyboard is activating when attempting to click on the textview for sorting.
										//Bring Container to front to prevent that from happening.
			gSearch.TextChanged += gSearch_TextChanged; //Text change event in search bar

			Website w = new Website (); //For Website URL

			//Create inventory Locations List.. this will populate the picklist when creating and updating trailers.
			gLocationsClient = new WebClient ();
			gLocationsUrl = new Uri (w.uriGetLocationsPHP ());
			gLocationsClient.DownloadDataAsync (gLocationsUrl);
			System.Threading.Thread.Sleep(200); //Delay in download task. NOTE: I'm not entirely sure this is helping.
			gLocationsClient.DownloadDataCompleted += gLocationsClient_DownloadDataCompleted;


			//Setup some initial data for the view_trailer page
			gTrailers = new List<Trailer>();
			gClient = new WebClient();

			gUrl = new Uri(w.uriGetTrailersPHP());
			//Call the PHP file, download the JSON data 
			gClient.DownloadDataAsync(gUrl);
			gClient.DownloadDataCompleted += gClient_DownloadDataCompleted;

			//Add long click to edit existing unit.
			//gTxtStockNumber = FindViewById<TextView>(Resource.Id.txtStockNumber);
			//gTxtStockNumber.LongClick += gTxtStockNumber_LongClick;

		}

		//After a longclick event, run this method
		private void StockNumberSelected (TextView selectedStockNumber)
		{

			gSelectedStockNumber = selectedStockNumber;

			//Get the LongClick target's position in the global list of trailers.
			int position = (int)gSelectedStockNumber.Tag;

			//Start the fragment
			FragmentTransaction transaction = FragmentManager.BeginTransaction();
			//Launch the update trailer dialog, feeding it the required parameters.
			DialogUpdateTrailer dialogUpdateTrailer = new DialogUpdateTrailer(gLocations, gTrailersSorted[position].Id, gTrailersSorted[position].StockNumber, gTrailersSorted[position].Make, 
																			  gTrailersSorted[position].Model, gTrailersSorted[position].LocationId, gTrailersSorted[position].WhiteX);
			
			dialogUpdateTrailer.Show(transaction, "Dialog Update Trailer Fragment");

			dialogUpdateTrailer.gOnUpdateTrailerComplete += dialogUpdateTrailer_gOnUpdateTrailerComplete;

		}


		void dialogUpdateTrailer_gOnUpdateTrailerComplete (object sender, OnUpdateTrailerEventArgs e)
		{

			//Continue here. Uncomment this:
			//gTrailers.Find(t => t.Id.Equals(e.Id), t. = e.StockNumber )
			Log.Info("ViewTrailersActivity", "Completed B");
			Trailer trailer = gTrailers.FirstOrDefault ( x => x.Id == e.Id );
			Log.Info("ViewTrailersActivity", "Completed C");

			//Update the trailer on the original list of trailers
			if(trailer != null)
			{
				trailer.StockNumber = e.StockNumber;
				trailer.Make = e.Make;
				trailer.Model = e.Model;
				trailer.LocationId = e.Location;
				trailer.WhiteX = e.WhiteX;
				//trailer.Active = e.Active; //Removed ability for general employee to mark a unit as active/inactive (or sold/not sold)
			}

			//Update the trailer on the sorted list of trailers.
			Trailer trailerSorted = gTrailersSorted.FirstOrDefault(x => x.Id == e.Id);
			if(trailerSorted != null)
			{
				trailerSorted.StockNumber = e.StockNumber;
				trailerSorted.Make = e.Make;
				trailerSorted.Model = e.Model;
				trailerSorted.LocationId = e.Location;
				trailerSorted.WhiteX = e.WhiteX;
			}

			//Update Adapter / screen
			gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, gTrailersSorted, gLocations, gActionUpdate);
			gListView.Adapter = gAdapter;

		}

		void gLocationsClient_DownloadDataCompleted (object sender, DownloadDataCompletedEventArgs e)
		{
			RunOnUiThread(() =>
			{
				//Parse JSON
				string json = Encoding.UTF8.GetString(e.Result);
				//Store parsed data into a List with the datatype of Locations.
				gLocations = JsonConvert.DeserializeObject<List<Location>>(json);

				//TODO: Sometimes gLocations does not finish loading. This causes a crash when launching the activity. Figure out how to make Xamarin wait for DownloadData Task to complete.
			});
		}

		void gClient_DownloadDataCompleted (object sender, DownloadDataCompletedEventArgs e)
		{
			RunOnUiThread(() =>
			{
				string json = Encoding.UTF8.GetString(e.Result);
				//Parse JSON
				gTrailers = JsonConvert.DeserializeObject<List<Trailer>>(json);

				gTrailersSorted = gTrailers; //Store gTrailers List into the final sorted list. 
				if(gLocations == null || gLocations.Count == 0)
				{
					Log.Error("ViewTrailerActivity.cs", "gLocations is Empty");
				}

				//Refresh listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, gTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;
				//Hide progress bar
				//gLinearLayoutProgressBar.Visibility = ViewStates.Gone; // This isn't woring.
				gProgressBar.Visibility = ViewStates.Gone;

			});
		}


		void dialogCreateTrailer_gOnCreateTrailerComplete(object sender, OnCreateTrailerEventArgs e)
		{
			//This event fires when the user clicks the "Create Trailer" button on the dialog fragment.

			//Doing this temporarily:
			gTrailers.Add(new Trailer{Id = e.Id, StockNumber = e.StockNumber, Make = e.Make, Model = e.Model, LocationId = e.Location, WhiteX = e.WhiteX });

			//Refresh the ListView
			gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, gTrailers, gLocations, gActionUpdate);
			gListView.Adapter = gAdapter;

		}


		//Inflate the actionbar onto the Menu bar.
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.actionbar, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		//Add click events to the menu bar stuff.
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch(item.ItemId)
			{
				case Resource.Id.search:
					gSearch.Visibility = ViewStates.Visible; //Make searchbar visible on click.
					if(gIsAnimating)
					{
						return true;
					}
					//Search has been clicked.
					if(!gAnimatedDown)
					{
						//ListView is up
						//Animate down
						MyAnimation anim = new MyAnimation(gListView, gListView.Height - gSearch.Height);
						anim.Duration = 500;
						gListView.StartAnimation(anim);
						//Wire up a method
						anim.AnimationStart += anim_AnimationStartDown;
						//Event to toggle when animation is finished
						anim.AnimationEnd += anim_AnimationEndDown;
						//Move whole container
						gContainer.Animate().TranslationYBy(gSearch.Height).SetDuration(500).Start();	
					}

					else
					{
						//ListView is down
						//Animate up
						MyAnimation anim = new MyAnimation(gListView, gListView.Height - gSearch.Height);
						anim.Duration = 500;
						gListView.StartAnimation(anim);
						//Wire up a method
						anim.AnimationStart += anim_AnimationStartUp;
						anim.AnimationEnd += anim_AnimationEndUp;
						//Move whole container
						gContainer.Animate().TranslationYBy(-gSearch.Height).SetDuration(500).Start();	

					}
					gAnimatedDown = !gAnimatedDown;
					return true;

				case Resource.Id.add:
					FragmentTransaction transaction = FragmentManager.BeginTransaction();

					DialogCreateTrailer dialogCreateTrailer = new DialogCreateTrailer(gLocations);
					dialogCreateTrailer.Show(transaction, "Dialog Create Trailer Fragment");			
					//DialogCreateTrailer finished... subscribe to its event
					dialogCreateTrailer.gOnCreateTrailerComplete += dialogCreateTrailer_gOnCreateTrailerComplete;
					return true;
				default:
					return base.OnOptionsItemSelected(item);
			}
		}

		#region Animations

		//////////////////////////////////////////////////////
		//////////// Search EditText Animnations //////////// 
		//////////////////////////////////////////////////////
		void anim_AnimationEndUp (object sender, Android.Views.Animations.Animation.AnimationEndEventArgs e)
		{
			gIsAnimating = false;
			//Clear focus
			gSearch.ClearFocus(); 
			//Close keyboard
			InputMethodManager inputManager = (InputMethodManager) this.GetSystemService(Context.InputMethodService);
			inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
		}

		void anim_AnimationEndDown (object sender, Android.Views.Animations.Animation.AnimationEndEventArgs e)
		{
			gIsAnimating = false;
		}

		void anim_AnimationStartDown (object sender, Android.Views.Animations.Animation.AnimationStartEventArgs e)
		{
			gIsAnimating = true;
			//Search EditText fade in
			gSearch.Animate().AlphaBy(1.0f).SetDuration(500).Start();
		}

		void anim_AnimationStartUp (object sender, Android.Views.Animations.Animation.AnimationStartEventArgs e)
		{
			gIsAnimating = true;
			//Search EditText fade out
			gSearch.Animate().AlphaBy(-1.0f).SetDuration(300).Start();
		}

		#endregion

		#region Search and Sort with LINQ
		/////////////////////////////////////////////////////
		///////////// SEARCH AND SORT WITH LINQ /////////////
		/////////////////////////////////////////////////////	
		//Sort by StockNumber textview
		void gTxtHeaderStockNumber_Click (object sender, EventArgs e)
		{
			List<Trailer> filteredTrailers;
			if(!gStockNumberAscending)
			{				
				filteredTrailers = (from trailer in gTrailers 
									orderby trailer.StockNumber
									select trailer).ToList<Trailer>();
				//Refresh the listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, filteredTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;

			}
			else
			{
				filteredTrailers = (from trailer in gTrailers 
									orderby trailer.StockNumber descending
									select trailer).ToList<Trailer>();
				//Refresh the listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, filteredTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;
			}

			gTrailersSorted = filteredTrailers; //Final list of trailers.

			gStockNumberAscending = !gStockNumberAscending;

		}

		//Sort by Make textview
		void gTxtHeaderMake_Click (object sender, EventArgs e)
		{
			List<Trailer> filteredTrailers;
			if(!gMakeAscending)
			{				
				filteredTrailers = (from trailer in gTrailers 
									orderby trailer.Make
									select trailer).ToList<Trailer>();
				//Refresh the listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, filteredTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;

				gTrailersSorted = filteredTrailers; //Final list of trailers.
			}
			else
			{
				filteredTrailers = (from trailer in gTrailers 
									orderby trailer.Make descending
									select trailer).ToList<Trailer>();
				//Refresh the listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, filteredTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;

				gTrailersSorted = filteredTrailers; //Final list of trailers.
			}
			gMakeAscending = !gMakeAscending;

		}
		//Sort by Model TextView
		void gTxtHeaderModel_Click (object sender, EventArgs e)
		{
			List<Trailer> filteredTrailers;
			if(!gModelAscending)
			{				
				filteredTrailers = (from trailer in gTrailers 
									orderby trailer.Model
									select trailer).ToList<Trailer>();
				//Refresh the listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, filteredTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;

				gTrailersSorted = filteredTrailers; //Final list of trailers.
			}
			else
			{
				filteredTrailers = (from trailer in gTrailers 
									orderby trailer.Model descending
									select trailer).ToList<Trailer>();
				//Refresh the listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, filteredTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;

				gTrailersSorted = filteredTrailers; //Final list of trailers.
			}
			gModelAscending = !gModelAscending;

		}

		//Sort by Location TextView
		void gTxtHeaderLocation_Click (object sender, EventArgs e)
		{
			List<Trailer> filteredTrailers;
			if(!gLocationAscending)
			{				
				filteredTrailers = (from trailer in gTrailers 
									orderby trailer.LocationId
									select trailer).ToList<Trailer>();
				//Refresh the listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, filteredTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;

				gTrailersSorted = filteredTrailers; //Final list of trailers.
			}
			else
			{
				filteredTrailers = (from trailer in gTrailers 
									orderby trailer.LocationId descending
									select trailer).ToList<Trailer>();
				//Refresh the listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, filteredTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;

				gTrailersSorted = filteredTrailers; //Final list of trailers.
			}
			gLocationAscending = !gLocationAscending;

		}
		//Sort by WhiteX (shot) TextView
		void gTxtHeaderWhiteX_Click (object sender, EventArgs e)
		{
			List<Trailer> filteredTrailers;
			if(!gWhiteXAscending)
			{				
				filteredTrailers = (from trailer in gTrailers 
									orderby trailer.WhiteX
									select trailer).ToList<Trailer>();
				//Refresh the listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, filteredTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;

				gTrailersSorted = filteredTrailers; //Final list of trailers.
			}
			else
			{
				filteredTrailers = (from trailer in gTrailers 
									orderby trailer.WhiteX descending
									select trailer).ToList<Trailer>();
				//Refresh the listview
				gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, filteredTrailers, gLocations, gActionUpdate);
				gListView.Adapter = gAdapter;

				gTrailersSorted = filteredTrailers; //Final list of trailers.
			}
			gWhiteXAscending = !gWhiteXAscending;

		}
		//Search the list of trailers
		void gSearch_TextChanged (object sender, Android.Text.TextChangedEventArgs e)
		{
			//Return a list of trailers
			//Run LINQ query, store in list<trailer>
			List<Trailer> searchedTrailers = (from trailer in gTrailers 
											where trailer.StockNumber.Contains(gSearch.Text, StringComparison.OrdinalIgnoreCase) 
											|| trailer.Make.Contains(gSearch.Text, StringComparison.OrdinalIgnoreCase) 
											|| trailer.Model.Contains(gSearch.Text, StringComparison.OrdinalIgnoreCase)
											select trailer).ToList<Trailer>();
			//Refresh the listview
			gAdapter = new TrailersAdapter(this, Resource.Layout.row_trailer, searchedTrailers, gLocations, gActionUpdate);
			gListView.Adapter = gAdapter;

			gTrailersSorted = searchedTrailers; //Final list of trailers.

		}

		#endregion 
	}
}

