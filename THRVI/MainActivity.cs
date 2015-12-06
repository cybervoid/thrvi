using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace THRVI.Core
{
	[Activity (Label = "THRVI", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private Button gButtonViewTrailers;
		private Button gButtonMoveTrailers;
		private Button gButtonOptions;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			gButtonViewTrailers = FindViewById<Button>(Resource.Id.buttonViewTrailers);
			gButtonMoveTrailers = FindViewById<Button>(Resource.Id.buttonMoveTrailers);
			gButtonOptions = FindViewById<Button>(Resource.Id.buttonOptions);

			gButtonViewTrailers.Click += delegate {
				StartActivity(typeof(ViewTrailersActivity));
			};

		}
	}
}


