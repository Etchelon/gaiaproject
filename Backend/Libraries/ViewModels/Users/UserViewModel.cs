using System;

namespace GaiaProject.ViewModels.Users
{
	public class UserViewModel
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public string Avatar { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime MemberSince { get; set; }
	}
}