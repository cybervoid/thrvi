using System;

namespace THRVI.Core
{
	public static class ExtMethods
	{
		//Make Contains not case sensitive.
		public static bool Contains(this string source, string toCheck, StringComparison comparisonType)
		{
			return (source.IndexOf(toCheck, comparisonType) >= 0);
		}
	}
}

