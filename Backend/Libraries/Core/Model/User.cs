using System;
using GaiaProject.Common.Database;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace GaiaProject.Core.Model
{
	[CollectionName("GaiaProject.Users")]
	public class User : MongoEntity
	{
		[BsonRequired]
		public string Identifier { get; set; }

		[BsonRequired]
		public string Username { get; set; }

		[BsonRequired]
		public string Email { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Avatar { get; set; }
		public DateTime MemberSince { get; set; }
	}
}