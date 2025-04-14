using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Shows.Domain.Entities
{
	public class Show : BaseEntity
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		[ForeignKey("ShowStatus")]
		public Guid ShowStatusId { get; set; }
		public ShowStatus? ShowStatus { get; set; }

		public Guid MovieId { get; set; }
		public string MovieTitle { get; set; } = null!;
		public int MovieRuntimeMinutes { get; set; }
		public Guid CinemaHallId { get; set; }
		[StringLength(255)]
		public string CinemaHallName { get; set; } = null!;

		public ICollection<ShowFeature> Features { get; set; } = new List<ShowFeature>();
	}
}