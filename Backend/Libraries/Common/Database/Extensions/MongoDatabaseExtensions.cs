using System.Linq;
using System.Reflection;
using MongoDB.Driver;
using MongoDbGenericRepository.Attributes;

namespace GaiaProject.Common.Database.Extensions
{
	public static class MongoDatabaseExtensions
	{
		public static IMongoCollection<T> GetCollection<T>(this IMongoDatabase database, MongoCollectionSettings settings = null)
		{
			var collectionNameAttribute = typeof(T).GetCustomAttributes(typeof(CollectionNameAttribute)).FirstOrDefault() as CollectionNameAttribute;
			return database.GetCollection<T>(
				collectionNameAttribute != null ? collectionNameAttribute.Name : typeof(T).FullName,
				settings
			);
		}
	}
}
