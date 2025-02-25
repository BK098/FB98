using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Cinemas.Domain.Entities
{
	public class CinemaHallSeat : BaseEntity
	{
		public byte SeatRow { get; set; }
		public byte SeatColumn { get; set; }
		public string SeatPosition { get; private set; }

		[ForeignKey("CinemaHall")]
		public Guid HallId { get; set; }
		public CinemaHall CinemaHall { get; set; }

		[ForeignKey("SeatType")]
		public Guid SeatTypeId { get; set; }
		public SeatType SeatType { get; set; }

		public void SetSeatPosition(byte row, byte col)
		{
			var rowLetter = (char)('A' + SeatRow - 1);
			SeatPosition = $"{rowLetter}{SeatColumn:D2}";
		}
	}
}