using System;

namespace THRVI.Core
{
	public class Trailer
	{
		public int Id { get; set; }
		public int LocationId { get; set; }
		public int MoveId { get; set; }
		public string StockNumber { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }
		public bool WhiteX { get; set; }
		public bool Active { get; set; }
	}
}

