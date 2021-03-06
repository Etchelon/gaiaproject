using MongoDbGenericRepository.Models;

namespace ScoreSheets.Common.Database.Abstractions
{
	public interface IMongoEntity : IDocument<string>
	{
	}
}
