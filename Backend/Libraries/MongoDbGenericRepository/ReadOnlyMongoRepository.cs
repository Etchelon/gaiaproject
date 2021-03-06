using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using MongoDbGenericRepository.Models;
using MongoDbGenericRepository.Abstractions;

namespace MongoDbGenericRepository
{
	/// <summary>
	/// The ReadOnlyMongoRepository implements the readonly functionality of the IReadOnlyMongoRepository.
	/// </summary>
	public class ReadOnlyMongoRepository<TKey> : IReadOnlyMongoRepository<TKey>
		where TKey : IEquatable<TKey>
	{
		/// <inheritdoc />
		public string ConnectionString { get; set; }

		/// <inheritdoc />
		public string DatabaseName { get; set; }

		/// <inheritdoc />
		public IMongoDatabase Database => this.MongoDbContext?.Database;

		/// <summary>
		/// The MongoDbContext
		/// </summary>
		protected IMongoDbContext MongoDbContext = null;

		#region Constructors
		/// <summary>
		/// The constructor taking a connection string and a database name.
		/// </summary>
		/// <param name="connectionString">The connection string of the MongoDb server.</param>
		/// <param name="databaseName">The name of the database against which you want to perform operations.</param>
		protected ReadOnlyMongoRepository(string connectionString, string databaseName)
		{
			MongoDbContext = new MongoDbContext(connectionString, databaseName);
		}

		/// <summary>
		/// The contructor taking a <see cref="IMongoDbContext"/>.
		/// </summary>
		/// <param name="mongoDbContext">A mongodb context implementing <see cref="IMongoDbContext"/></param>
		protected ReadOnlyMongoRepository(IMongoDbContext mongoDbContext)
		{
			MongoDbContext = mongoDbContext;
		}

		/// <summary>
		/// The contructor taking a <see cref="IMongoDatabase"/>.
		/// </summary>
		/// <param name="mongoDatabase">A mongodb context implementing <see cref="IMongoDatabase"/></param>
		protected ReadOnlyMongoRepository(IMongoDatabase mongoDatabase)
		{
			MongoDbContext = new MongoDbContext(mongoDatabase);
		}
		#endregion

		/*
        #region Read Sync

        /// <summary>
        /// Returns one document given its id.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="id">The Id of the document you want to get.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public TDocument GetById<TDocument>(TKey id) where TDocument : IDocument<TKey>
        {
            var filter = Builders<TDocument>.Filter.Eq("Id", id);
            return GetCollection<TDocument>().Find(filter).FirstOrDefault();
        }

		/// <summary>
		/// Returns one document given an expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public TDocument GetOne<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
        {
            return GetCollection<TDocument>().Find(filter).FirstOrDefault();
        }

	    /// <summary>
	    /// Returns one document given an expression filter.
	    /// </summary>
	    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="filter">A LINQ expression filter.</param>
	    /// <param name="partitionKey">An optional partition key.</param>
	    public TDocument GetOne<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
	    {
		    return GetCollection<TDocument>().Find(filter).FirstOrDefault();
	    }

		/// <summary>
		/// Returns a collection cursor.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public IFindFluent<TDocument, TDocument> GetCursor<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
        {
            return GetCollection<TDocument>().Find(filter);
        }

	    /// <summary>
	    /// Returns a collection cursor.
	    /// </summary>
	    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="filter">A LINQ expression filter.</param>
	    /// <param name="partitionKey">An optional partition key.</param>
	    public IFindFluent<TDocument, TDocument> GetCursor<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
	    {
		    return GetCollection<TDocument>().Find(filter);
	    }

		/// <summary>
		/// Returns true if any of the document of the collection matches the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public bool Any<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
        {
            var count = GetCollection<TDocument>().Count(filter);
            return (count > 0);
        }

	    /// <summary>
	    /// Returns true if any of the document of the collection matches the filter condition.
	    /// </summary>
	    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="filter">A LINQ expression filter.</param>
	    /// <param name="partitionKey">An optional partition key.</param>
	    public bool Any<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
	    {
		    var count = GetCollection<TDocument>().Count(filter);
		    return (count > 0);
	    }

		/// <summary>
		/// Returns a list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public List<TDocument> GetAll<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
        {
            return GetCollection<TDocument>().Find(filter).ToList();
        }

	    /// <summary>
	    /// Returns a list of the documents matching the filter condition.
	    /// </summary>
	    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="filter">A LINQ expression filter.</param>
	    /// <param name="partitionKey">An optional partition key.</param>
	    public List<TDocument> GetAll<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
	    {
		    return GetCollection<TDocument>().Find(filter).ToList();
	    }

		/// <summary>
		/// Counts how many documents match the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partitionKey</param>
		public long Count<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
        {
            return GetCollection<TDocument>().Find(filter).Count();
        }

	    /// <summary>
	    /// Counts how many documents match the filter condition.
	    /// </summary>
	    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="filter">A LINQ expression filter.</param>
	    /// <param name="partitionKey">An optional partitionKey</param>
	    public long Count<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
	    {
		    return GetCollection<TDocument>().Find(filter).Count();
	    }

		#endregion
		*/

		#region Read Async
		/// <summary>
		/// Asynchronously returns one document given its id.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="id">The Id of the document you want to get.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<TDocument> GetByIdAsync<TDocument>(TKey id) where TDocument : IDocument<TKey>
		{
			var filter = Builders<TDocument>.Filter.Eq("Id", id);
			return await GetOneAsync(filter);
		}

		/// <summary>
		/// Asynchronously returns one document given an expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<TDocument> GetOneAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>().Find(filter).FirstOrDefaultAsync();
		}

		/// <summary>
		/// Asynchronously returns one document given an expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<TDocument> GetOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>().Find(filter).FirstOrDefaultAsync();
		}

		/// <summary>
		/// Returns true if any of the document of the collection matches the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<bool> AnyAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
		{
			return await this.CountAsync(filter) > 0;
		}

		/// <summary>
		/// Returns true if any of the document of the collection matches the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<bool> AnyAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
		{
			return await this.CountAsync(filter) > 0;
		}

		/// <summary>
		/// Asynchronously returns a list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="ids">List of ids of the entities to get.</param>
		public async Task<List<TDocument>> GetAllAsync<TDocument>(IEnumerable<TKey> ids) where TDocument : IDocument<TKey>
		{
			var filter = Builders<TDocument>.Filter.In(u => u.Id, ids);
			return await this.GetAllAsync(filter);
		}

		/// <summary>
		/// Asynchronously returns a list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		public async Task<List<TDocument>> GetAllAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>().Find(filter).ToListAsync();
		}

		/// <summary>
		/// Asynchronously returns a list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		public async Task<List<TDocument>> GetAllAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>().Find(filter).ToListAsync();
		}

		/// <summary>
		/// Asynchronously counts how many documents match the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partitionKey</param>
		public async Task<long> CountAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>().CountDocumentsAsync(filter);
		}

		/// <summary>
		/// Asynchronously counts how many documents match the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partitionKey</param>
		public async Task<long> CountAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>
		{
			return await GetCollection<TDocument>().CountDocumentsAsync(filter);
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Gets a collections for the type TDocument
		/// </summary>
		/// <typeparam name="TDocument">The document type.</typeparam>
		/// <returns></returns>
		protected IMongoCollection<TDocument> GetCollection<TDocument>() where TDocument : IDocument<TKey>
		{
			return MongoDbContext.GetCollection<TDocument>();
		}
		#endregion
	}
}
