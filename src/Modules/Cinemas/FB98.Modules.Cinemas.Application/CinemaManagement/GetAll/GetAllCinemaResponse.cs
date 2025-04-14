namespace FB98.Modules.Cinemas.Application.CinemaManagement.GetAll
{
	public class GetAllCinemaResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string Address { get; set; } = null!;
	}
}