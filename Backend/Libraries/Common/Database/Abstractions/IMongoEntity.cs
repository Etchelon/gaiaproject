using MongoDbGenericRepository.Models;

namespace GaiaProject.Common.Database.Abstractions
{
	public interface IMongoEntity : IDocument<string>
	{
	}
}
