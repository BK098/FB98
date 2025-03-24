namespace FB98.Modules.Identity.Application.ProfileManagement.EditProfile
{
	public class EditProfileDto
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public DateOnly? BirthOfDate { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public bool? Gender { get; set; }
	}
}