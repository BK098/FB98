using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Cinemas.Domain.Entities
{
	public class CinemaHall : BaseEntity
	{
		[StringLength(255)]
		public string Name { get; set; } = null!;

		[ForeignKey("Cinema")]
		public Guid CinemaId { get; set; }
		public Cinema? Cinema { get; set; }

		public ICollection<CinemaHallSeat> Seats { get; set; } = new List<CinemaHallSeat>();
	}
}