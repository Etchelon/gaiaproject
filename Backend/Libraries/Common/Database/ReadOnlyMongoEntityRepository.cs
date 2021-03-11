using MongoDB.Driver;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Abstractions;

namespace GaiaProject.Common.Database
{
	public class ReadOnlyMongoEntityRepository : ReadOnlyMongoRepository<string>
	{
		/// <summary>
		/// The contructor taking a <see cref="IMongoDatabase"/>.
		/// </summary>
		/// <param name="mongoDatabase">A mongodb context implementing <see cref="IMongoDatabase"/></param>
		public ReadOnlyMongoEntityRepository(IMongoDbContext mongoDbContext) : base(mongoDbContext)
		{
		}
	}
}
