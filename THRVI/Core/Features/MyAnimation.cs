using System;

using Android.Views;
using Android.Views.Animations;

namespace THRVI.Core
{
	public class MyAnimation : Animation
	{
		private View gView;
		private int gOriginalHeight;
		private int gTargetHeight;
		private int gGrowBy;

		public MyAnimation(View view, int targetHeight)
		{
			gView = view;
			gOriginalHeight = view.Height;
			gTargetHeight = targetHeight;
			gGrowBy = gTargetHeight - gOriginalHeight;
		}

		protected override void ApplyTransformation (float interpolatedTime, Transformation t)
		{
				gView.LayoutParameters.Height = (int)(gOriginalHeight + (gGrowBy * interpolatedTime));
				gView.RequestLayout(); //Layout is invalid, Redraw it
		}

		public override bool WillChangeBounds ()
		{
			return true;
		}

	}
}

