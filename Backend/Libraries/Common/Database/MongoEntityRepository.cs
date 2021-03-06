using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Abstractions;

namespace ScoreSheets.Common.Database
{
	public class MongoEntityRepository : BaseMongoRepository<string>
	{
		/// <summary>
		/// The contructor taking a <see cref="IMongoDatabase"/>.
		/// </summary>
		/// <param name="mongoDatabase">A mongodb context implementing <see cref="IMongoDatabase"/></param>
		public MongoEntityRepository(IMongoDbContext mongoDbContext) : base(mongoDbContext)
		{
		}

		protected override string SetIdField()
		{
			return ObjectId.GenerateNewId().ToString();
		}
	}
}
