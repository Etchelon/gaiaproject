using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDbGenericRepository.Models;
using MongoDB.Driver;

namespace MongoDbGenericRepository.Abstractions
{
	/// <summary>
	/// The IReadOnlyMongoRepository exposes the readonly functionality of the BaseMongoRepository.
	/// </summary>
	public interface IReadOnlyMongoRepository<TKey> where TKey : IEquatable<TKey>
	{
		/// <summary>
		/// The connection string.
		/// </summary>
		string ConnectionString { get; set; }
		/// <summary>
		/// The database name.
		/// </summary>
		string DatabaseName { get; set; }
		/// <summary>
		/// The database instance.
		/// </summary>
		IMongoDatabase Database { get; }

		/*
        #region Sync

        /// <summary>
        /// Returns one document given its id.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="id">The Id of the document you want to get.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        TDocument GetById<TDocument>(TKey id) where TDocument : IDocument<TKey>;

        /// <summary>
        /// Returns one document given an expression filter.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        TDocument GetOne<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;

	    /// <summary>
	    /// Returns one document given an expression filter.
	    /// </summary>
	    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="filter">A LINQ expression filter.</param>
	    /// <param name="partitionKey">An optional partition key.</param>
	    TDocument GetOne<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Returns a collection cursor.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		IFindFluent<TDocument, TDocument> GetCursor<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;

	    /// <summary>
	    /// Returns a collection cursor.
	    /// </summary>
	    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="filter">A LINQ expression filter.</param>
	    /// <param name="partitionKey">An optional partition key.</param>
	    IFindFluent<TDocument, TDocument> GetCursor<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Returns true if any of the document of the collection matches the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		bool Any<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;

	    /// <summary>
	    /// Returns true if any of the document of the collection matches the filter condition.
	    /// </summary>
	    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="filter">A LINQ expression filter.</param>
	    /// <param name="partitionKey">An optional partition key.</param>
	    bool Any<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Returns a list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		List<TDocument> GetAll<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;

	    /// <summary>
	    /// Returns a list of the documents matching the filter condition.
	    /// </summary>
	    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="filter">A LINQ expression filter.</param>
	    /// <param name="partitionKey">An optional partition key.</param>
	    List<TDocument> GetAll<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Counts how many documents match the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		long Count<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;

	    /// <summary>
	    /// Counts how many documents match the filter condition.
	    /// </summary>
	    /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="filter">A LINQ expression filter.</param>
	    /// <param name="partitionKey">An optional partition key.</param>
	    long Count<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		#endregion
		*/

		#region Async
		/// <summary>
		/// Asynchronously returns one document given its id.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="id">The Id of the document you want to get.</param>
		Task<TDocument> GetByIdAsync<TDocument>(TKey id) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns one document given an expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		Task<TDocument> GetOneAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns one document given an expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		Task<TDocument> GetOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns true if any of the document of the collection matches the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		Task<bool> AnyAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns true if any of the document of the collection matches the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		Task<bool> AnyAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns a list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="ids">List of ids of the entities to get.</param>
		Task<List<TDocument>> GetAllAsync<TDocument>(IEnumerable<TKey> ids) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns a list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		Task<List<TDocument>> GetAllAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns a list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		Task<List<TDocument>> GetAllAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously counts how many documents match the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		Task<long> CountAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously counts how many documents match the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		Task<long> CountAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;
		#endregion
	}

	public interface IReadOnlyMongoRepository : IReadOnlyMongoRepository<string> { }
}
