using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using MongoDbGenericRepository.Models;
using System.Linq;
using MongoDbGenericRepository.Abstractions;
using MongoDbGenericRepository.Utils;

namespace MongoDbGenericRepository
{
	/// <summary>
	/// The base Repository, it is meant to be inherited from by your custom custom MongoRepository implementation.
	/// Its constructor must be given a connection string and a database name.
	/// </summary>
	public abstract class BaseMongoRepository<TKey> : ReadOnlyMongoRepository<TKey>, IBaseMongoRepository<TKey>
		where TKey : IEquatable<TKey>
	{
		private static readonly Random Random = new Random();

		#region Constructors
		/// <summary>
		/// The constructor taking a connection string and a database name.
		/// </summary>
		/// <param name="connectionString">The connection string of the MongoDb server.</param>
		/// <param name="databaseName">The name of the database against which you want to perform operations.</param>
		protected BaseMongoRepository(string connectionString, string databaseName) : base(connectionString, databaseName)
		{
			MongoDbContext = new MongoDbContext(connectionString, databaseName);
		}

		/// <summary>
		/// The contructor taking a <see cref="IMongoDbContext"/>.
		/// </summary>
		/// <param name="mongoDbContext">A mongodb context implementing <see cref="IMongoDbContext"/></param>
		protected BaseMongoRepository(IMongoDbContext mongoDbContext) : base(mongoDbContext)
		{
			MongoDbContext = mongoDbContext;
		}

		/// <summary>
		/// The contructor taking a <see cref="IMongoDatabase"/>.
		/// </summary>
		/// <param name="mongoDatabase">A mongodb context implementing <see cref="IMongoDatabase"/></param>
		protected BaseMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
		{
			MongoDbContext = new MongoDbContext(mongoDatabase);
		}
		#endregion

		#region Create Async
		/// <summary>
		/// Asynchronously adds a document to the collection.
		/// Populates the Id and AddedAtUtc fields if necessary.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="document">The document you want to add.</param>
		public async Task AddOneAsync<TDocument>(TDocument document) where TDocument : IDocument<TKey>
		{
			FormatDocument(document);
			await GetCollection<TDocument>().InsertOneAsync(document);
		}

		/// <summary>
		/// Asynchronously adds a list of documents to the collection.
		/// Populates the Id and AddedAtUtc fields if necessary.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documents">The documents you want to add.</param>
		public async Task AddManyAsync<TDocument>(IEnumerable<TDocument> documents) where TDocument : IDocument<TKey>
		{
			if (!documents.Any())
			{
				return;
			}
			foreach (var doc in documents)
			{
				FormatDocument(doc);
			}
			await GetCollection<TDocument>().InsertManyAsync(documents);
		}
		#endregion

		#region Update Async

		#region Update One

		/// <summary>
		/// Asynchronously Updates a document.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="modifiedDocument">The document with the modifications you want to persist.</param>
		public async Task<bool> UpdateOneAsync<TDocument>(TDocument modifiedDocument) where TDocument : IDocument<TKey>
		{
			var updateRes = await GetCollection<TDocument>().ReplaceOneAsync(x => x.Id.Equals(modifiedDocument.Id), modifiedDocument);
			return updateRes.ModifiedCount == 1;
		}

		#region Update a field
		/// <summary>
		/// Takes a document you want to modify and applies the update you have defined in MongoDb.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documentToModify">The document you want to modify.</param>
		/// <param name="update">The update definition for the document.</param>
		public async Task<bool> UpdateOneAsync<TDocument, TField>(TKey id, Expression<Func<TDocument, TField>> field, TField value)
			where TDocument : IDocument<TKey>
		{
			var filter = this.FilterById<TDocument>(id);
			return await this.UpdateOneAsync(filter, field, value);
		}

		public async Task<bool> UpdateOneAsync<TDocument, TField>(FilterDefinition<TDocument> filter, Expression<Func<TDocument, TField>> field, TField value)
			where TDocument : IDocument<TKey>
		{
			var updateRes = await GetCollection<TDocument>().UpdateOneAsync(filter, Builders<TDocument>.Update.Set(field, value));
			return updateRes.ModifiedCount == 1;
		}

		public async Task<bool> UpdateOneAsync<TDocument, TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field, TField value)
			where TDocument : IDocument<TKey>
		{
			var updateRes = await GetCollection<TDocument>().UpdateOneAsync(filter, Builders<TDocument>.Update.Set(field, value));
			return updateRes.ModifiedCount == 1;
		}
		#endregion

		#region Update with an UpdateDefinition
		/// <summary>
		/// Takes a document you want to modify and applies the update you have defined in MongoDb.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documentToModify">The document you want to modify.</param>
		/// <param name="update">The update definition for the document.</param>
		public async Task<bool> UpdateOneAsync<TDocument>(TKey id, UpdateDefinition<TDocument> update)
			where TDocument : IDocument<TKey>
		{
			var filter = this.FilterById<TDocument>(id);
			var updateRes = await GetCollection<TDocument>().UpdateOneAsync(filter, update);
			return updateRes.ModifiedCount == 1;
		}

		/// <summary>
		/// Takes a document you want to modify and applies the update you have defined in MongoDb.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documentToModify">The document you want to modify.</param>
		/// <param name="update">The update definition for the document.</param>
		public async Task<bool> UpdateOneAsync<TDocument>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update)
			where TDocument : IDocument<TKey>
		{
			var updateRes = await GetCollection<TDocument>().UpdateOneAsync(filter, update);
			return updateRes.ModifiedCount == 1;
		}

		/// <summary>
		/// Takes a document you want to modify and applies the update you have defined in MongoDb.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documentToModify">The document you want to modify.</param>
		/// <param name="update">The update definition for the document.</param>
		public async Task<bool> UpdateOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update)
			where TDocument : IDocument<TKey>
		{
			var updateRes = await GetCollection<TDocument>().UpdateOneAsync(filter, update);
			return updateRes.ModifiedCount == 1;
		}
		#endregion

		#endregion

		#region Update Many

		#region Update with an UpdateDefinition
		/// <summary>
		/// Takes a document you want to modify and applies the update you have defined in MongoDb.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documentToModify">The document you want to modify.</param>
		/// <param name="update">The update definition for the document.</param>
		public async Task<long> UpdateManyAsync<TDocument>(IEnumerable<TKey> ids, UpdateDefinition<TDocument> update)
			where TDocument : IDocument<TKey>
		{
			var filter = Builders<TDocument>.Filter.In(o => o.Id, ids);
			return await this.UpdateManyAsync(filter, update);
		}

		/// <summary>
		/// Takes a document you want to modify and applies the update you have defined in MongoDb.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documentToModify">The document you want to modify.</param>
		/// <param name="update">The update definition for the document.</param>
		public async Task<long> UpdateManyAsync<TDocument>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update)
			where TDocument : IDocument<TKey>
		{
			var updateRes = await GetCollection<TDocument>().UpdateManyAsync(filter, update);
			return updateRes.ModifiedCount;
		}

		/// <summary>
		/// Takes a document you want to modify and applies the update you have defined in MongoDb.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documentToModify">The document you want to modify.</param>
		/// <param name="update">The update definition for the document.</param>
		public async Task<long> UpdateManyAsync<TDocument>(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update)
			where TDocument : IDocument<TKey>
		{
			var updateRes = await GetCollection<TDocument>().UpdateManyAsync(filter, update);
			return updateRes.ModifiedCount;
		}
		#endregion

		#endregion

		#endregion

		#region Delete Async

		#region Delete One
		/// <summary>
		/// Asynchronously deletes a document.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="document">The document you want to delete.</param>
		/// <returns>The number of documents deleted.</returns>
		public async Task<bool> DeleteOneAsync<TDocument>(TDocument document) where TDocument : IDocument<TKey>
		{
			var filter = Builders<TDocument>.Filter.Eq(o => o.Id, document.Id);
			return await this.DeleteOneAsync(filter);
		}

		/// <summary>
		/// Asynchronously deletes a document matching the condition of the LINQ expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		public async Task<bool> DeleteOneAsync<TDocument>(TKey id) where TDocument : IDocument<TKey>
		{
			var filter = this.FilterById<TDocument>(id);
			return await this.DeleteOneAsync(filter);
		}

		/// <summary>
		/// Asynchronously deletes a document matching the condition of the LINQ expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		public async Task<bool> DeleteOneAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
		{
			var deleteRes = await GetCollection<TDocument>().DeleteOneAsync(filter);
			return deleteRes.DeletedCount == 1;
		}

		/// <summary>
		/// Asynchronously deletes a document matching the condition of the LINQ expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		public async Task<bool> DeleteOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
		{
			var deleteRes = await GetCollection<TDocument>().DeleteOneAsync(filter);
			return deleteRes.DeletedCount == 1;
		}
		#endregion

		#region Delete Many
		/// <summary>
		/// Asynchronously deletes the documents matching the condition of the LINQ expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		public async Task<long> DeleteManyAsync<TDocument>(IEnumerable<TDocument> documents) where TDocument : IDocument<TKey>
		{
			var ids = documents.Select(o => o.Id);
			var filter = Builders<TDocument>.Filter.In(o => o.Id, ids);
			return await this.DeleteManyAsync(filter);
		}

		/// <summary>
		/// Asynchronously deletes the documents matching the condition of the LINQ expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		public async Task<long> DeleteManyAsync<TDocument>(IEnumerable<TKey> ids) where TDocument : IDocument<TKey>
		{
			var filter = Builders<TDocument>.Filter.In(o => o.Id, ids);
			return await this.DeleteManyAsync(filter);
		}

		/// <summary>
		/// Asynchronously deletes the documents matching the condition of the LINQ expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		public async Task<long> DeleteManyAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
		{
			var deleteRes = await GetCollection<TDocument>().DeleteManyAsync(filter);
			return deleteRes.DeletedCount;
		}

		/// <summary>
		/// Asynchronously deletes the documents matching the condition of the LINQ expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		public async Task<long> DeleteManyAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
		{
			var deleteRes = await GetCollection<TDocument>().DeleteManyAsync(filter);
			return deleteRes.DeletedCount;
		}
		#endregion

		#endregion

		#region Project Async

		#region Project One
		/// <summary>
		/// Asynchronously returns a projected document matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
		/// <param name="filter"></param>
		/// <param name="projection">The projection expression.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<TProjection> ProjectOneAsync<TDocument, TProjection>(TKey id, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>
		{
			var filter = this.FilterById<TDocument>(id);
			return await this.ProjectOneAsync(filter, projection);
		}

		/// <summary>
		/// Asynchronously returns a projected document matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
		/// <param name="filter"></param>
		/// <param name="projection">The projection expression.</param>
		public async Task<TProjection> ProjectOneAsync<TDocument, TProjection>(FilterDefinition<TDocument> filter, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>()
				.Find(filter)
				.Project(projection)
				.FirstOrDefaultAsync();
		}

		/// <summary>
		/// Asynchronously returns a projected document matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
		/// <param name="filter"></param>
		/// <param name="projection">The projection expression.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<TProjection> ProjectOneAsync<TDocument, TProjection>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>()
				.Find(filter)
				.Project(projection)
				.FirstOrDefaultAsync();
		}
		#endregion

		#region Project Many
		/// <summary>
		/// Asynchronously returns a list of projected documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
		/// <param name="filter"></param>
		/// <param name="projection">The projection expression.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<List<TProjection>> ProjectManyAsync<TDocument, TProjection>(IEnumerable<TKey> ids, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>
		{
			var filter = Builders<TDocument>.Filter.In(o => o.Id, ids);
			return await this.ProjectManyAsync(filter, projection);
		}

		/// <summary>
		/// Asynchronously returns a list of projected documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
		/// <param name="filter"></param>
		/// <param name="projection">The projection expression.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<List<TProjection>> ProjectManyAsync<TDocument, TProjection>(FilterDefinition<TDocument> filter, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>()
				.Find(filter)
				.Project(projection)
				.ToListAsync();
		}

		/// <summary>
		/// Asynchronously returns a list of projected documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
		/// <param name="filter"></param>
		/// <param name="projection">The projection expression.</param>
		public async Task<List<TProjection>> ProjectManyAsync<TDocument, TProjection>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>()
				.Find(filter)
				.Project(projection)
				.ToListAsync();
		}
		#endregion

		#endregion

		#region Paginate Async
		/// <summary>
		/// Asynchronously returns a paginated list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter"></param>
		/// <param name="skipNumber">The number of documents you want to skip. Default value is 0.</param>
		/// <param name="takeNumber">The number of documents you want to take. Default value is 50.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<List<TDocument>> GetPaginatedAsync<TDocument>(FilterDefinition<TDocument> filter, SortDefinition<TDocument> sorter, int skipNumber = 0, int takeNumber = 50)
			where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>()
				.Find(filter)
				.Sort(sorter)
				.Skip(skipNumber)
				.Limit(takeNumber)
				.ToListAsync();
		}

		/// <summary>
		/// Asynchronously returns a paginated list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter"></param>
		/// <param name="skipNumber">The number of documents you want to skip. Default value is 0.</param>
		/// <param name="takeNumber">The number of documents you want to take. Default value is 50.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<List<TDocument>> GetPaginatedAsync<TDocument>(Expression<Func<TDocument, bool>> filter, SortDefinition<TDocument> sorter, int skipNumber = 0, int takeNumber = 50)
			where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>()
				.Find(filter)
				.Sort(sorter)
				.Skip(skipNumber)
				.Limit(takeNumber)
				.ToListAsync();
		}
		#endregion

		protected virtual TKey SetIdField()
		{
			var idTypeName = typeof(TKey).Name;
			switch (idTypeName)
			{
				case "Guid":
					return (TKey)(object)Guid.NewGuid();
				case "Int16":
					return (TKey)(object)Random.Next(1, short.MaxValue);
				case "Int32":
					return (TKey)(object)Random.Next(1, int.MaxValue);
				case "Int64":
					return (TKey)(object)(Random.NextLong(1, long.MaxValue));
				case "String":
					return (TKey)(object)Guid.NewGuid().ToString();
			}
			throw new ArgumentException($"{idTypeName} is not a supported Id type, the Id of the document cannot be set.");
		}

		/// <summary>
		/// Sets the value of the document Id if it is not set already.
		/// </summary>
		/// <typeparam name="TDocument">The document type.</typeparam>
		/// <typeparam name="TKey">The type of the primary key.</typeparam>
		/// <param name="document">The document.</param>
		protected void FormatDocument<TDocument>(TDocument document) where TDocument : IDocument<TKey>
		{
			if (document == null)
			{
				throw new ArgumentNullException(nameof(document));
			}

			var defaultTKey = default(TKey);
			if (document.Id == null || (defaultTKey != null && defaultTKey.Equals(document.Id)))
			{
				document.Id = SetIdField();
			}
		}

		protected FilterDefinition<TDocument> FilterById<TDocument>(TKey id) where TDocument : IDocument<TKey>
		{
			return Builders<TDocument>.Filter.Eq(o => o.Id, id);
		}
	}
}