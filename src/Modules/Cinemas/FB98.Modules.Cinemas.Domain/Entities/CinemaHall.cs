using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Cinemas.Domain.Entities
{
	public class CinemaHall : BaseEntity
	{
		public string Name { get; set; }

		[ForeignKey("Cinema")]
		public Guid CinemaId { get; set; }
		public Cinema Cinema { get; set; }

		public ICollection<CinemaHallSeat> Seats { get; set; } = new List<CinemaHallSeat>();
	}
}