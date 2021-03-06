using System;
using MongoDbGenericRepository.Attributes;
using ScoreSheets.Common.Database;

namespace GaiaProject.Engine.Model.Players
{
	[CollectionName("GaiaProject.Users")]
	public class User : MongoEntity
	{
		public string Identifier { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Avatar { get; set; }
		public DateTime MemberSince { get; set; }
	}
}