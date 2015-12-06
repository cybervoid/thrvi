using System;
using THRVI.Core;

namespace THRVI
{
	public static class LocationCast
	{
		public static T Cast<T>(Java.Lang.Object obj) where T : Location
		{
			var propertyInfo = obj.GetType().GetProperty("Instance");
			return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;

		}
	}
}

