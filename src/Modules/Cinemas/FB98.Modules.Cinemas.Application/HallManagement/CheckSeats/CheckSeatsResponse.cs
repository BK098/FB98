namespace FB98.Modules.Cinemas.Application.HallManagement.CheckSeats
{
	public class CheckSeatsResponse
	{
		public string Name { get; set; }

		/// <summary>
		///     First Guid is SeatId
		///     Second Guid is SeatTypeId
		/// </summary>
		public IList<Dictionary<Guid, Guid>> SeatIds { get; set; }
	}
}