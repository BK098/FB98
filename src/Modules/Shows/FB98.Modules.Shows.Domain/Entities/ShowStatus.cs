using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Shows.Domain.Entities
{
	public class ShowStatus : BaseEntity
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}

	public static class ShowStatusConstants
	{
		public static Guid UpComming = Guid.Parse("9230c5f0-db09-44d5-9d33-d4a05955a3a7");
		public static Guid OnGoing = Guid.Parse("af113986-2f88-435f-bc38-8518df5a29dd");
		public static Guid Ended = Guid.Parse("4cfd3bd1-062d-442f-ad42-fb4726f061e8");
	}
}