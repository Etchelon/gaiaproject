using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDbGenericRepository.Models;
using MongoDB.Driver;

namespace MongoDbGenericRepository.Abstractions
{
	/// <summary>
	/// The IBaseMongoRepository exposes the CRUD functionality of the BaseMongoRepository.
	/// <typeparam name="TKey">The type of the Document's primary key.</typeparam>
	/// </summary>
	public interface IBaseMongoRepository<TKey> : IReadOnlyMongoRepository<TKey>
		where TKey : IEquatable<TKey>
	{
		#region Create Async
		/// <summary>
		/// Asynchronously adds a document to the collection.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="document">The document you want to add.</param>
		Task AddOneAsync<TDocument>(TDocument document) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously adds a list of documents to the collection.
		/// Populates the Id and AddedAtUtc fields if necessary.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documents">The document you want to add.</param>
		Task AddManyAsync<TDocument>(IEnumerable<TDocument> documents) where TDocument : IDocument<TKey>;
		#endregion

		#region Update Async

		#region Update One
		/// <summary>
		/// Updates a document.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="modifiedDocument">The document with the modifications you want to persist.</param>
		Task<bool> UpdateOneAsync<TDocument>(TDocument modifiedDocument) where TDocument : IDocument<TKey>;

		#region Update a field
		/// <summary>
		/// Updates the property field with the given value update a property field in entities.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="id">The document id.</param>
		/// <param name="field">The field selector.</param>
		/// <param name="value">The new value of the property field.</param>
		Task<bool> UpdateOneAsync<TDocument, TField>(TKey id, Expression<Func<TDocument, TField>> field, TField value)
			where TDocument : IDocument<TKey>;

		/// <summary>
		/// Updates the property field with the given value update a property field in entities.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="filter">The document filter.</param>
		/// <param name="field">The field selector.</param>
		/// <param name="value">The new value of the property field.</param>
		Task<bool> UpdateOneAsync<TDocument, TField>(FilterDefinition<TDocument> filter, Expression<Func<TDocument, TField>> field, TField value)
			 where TDocument : IDocument<TKey>;

		/// <summary>
		/// Updates the property field with the given value update a property field in entities.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="filter">The document filter.</param>
		/// <param name="field">The field selector.</param>
		/// <param name="value">The new value of the property field.</param>
		Task<bool> UpdateOneAsync<TDocument, TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field, TField value)
			where TDocument : IDocument<TKey>;
		#endregion

		#region Update with an UpdateDefinition
		/// <summary>
		/// Updates the property field with the given value update a property field in entities.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="id">The document id.</param>
		/// <param name="updateDefinition">The update definition,</param>
		Task<bool> UpdateOneAsync<TDocument>(TKey id, UpdateDefinition<TDocument> updateDefinition)
			where TDocument : IDocument<TKey>;

		/// <summary>
		/// Updates the property field with the given value update a property field in entities.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="filter">The filter definition.</param>
		/// <param name="updateDefinition">The update definition,</param>
		Task<bool> UpdateOneAsync<TDocument>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> updateDefinition)
			where TDocument : IDocument<TKey>;

		/// <summary>
		/// Updates the property field with the given value update a property field in entities.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="filter">The filter selector.</param>
		/// <param name="updateDefinition">The update definition</param>
		Task<bool> UpdateOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> updateDefinition)
			where TDocument : IDocument<TKey>;
		#endregion
		#endregion

		#region Update Many

		#region Update with an UpdateDefinition
		/// <summary>
		/// Updates the property field with the given value update a property field in entities.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="ids">The documents' ids.</param>
		/// <param name="updateDefinition">The update definition,</param>
		Task<long> UpdateManyAsync<TDocument>(IEnumerable<TKey> ids, UpdateDefinition<TDocument> updateDefinition)
			where TDocument : IDocument<TKey>;

		/// <summary>
		/// Updates the property field with the given value update a property field in entities.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="filter">The filter definition.</param>
		/// <param name="updateDefinition">The update definition,</param>
		Task<long> UpdateManyAsync<TDocument>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> updateDefinition)
			where TDocument : IDocument<TKey>;

		/// <summary>
		/// Updates the property field with the given value update a property field in entities.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TField">The type of the field.</typeparam>
		/// <param name="filter">The filter selector.</param>
		/// <param name="updateDefinition">The update definition</param>
		Task<long> UpdateManyAsync<TDocument>(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> updateDefinition)
			where TDocument : IDocument<TKey>;
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
		Task<bool> DeleteOneAsync<TDocument>(TDocument document) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously deletes a document.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="id">The document's id.</param>
		/// <returns>The number of documents deleted.</returns>
		Task<bool> DeleteOneAsync<TDocument>(TKey id) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously deletes a document matching the condition of the FilterDefinition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		Task<bool> DeleteOneAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously deletes a document matching the condition of the LINQ expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		Task<bool> DeleteOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;
		#endregion

		#region Delete Many
		/// <summary>
		/// Asynchronously deletes a list of documents.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documents">The list of documents to delete.</param>
		/// <returns>The number of documents deleted.</returns>
		Task<long> DeleteManyAsync<TDocument>(IEnumerable<TDocument> documents) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously deletes a list of documents.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="documents">The list of documents to delete.</param>
		/// <returns>The number of documents deleted.</returns>
		Task<long> DeleteManyAsync<TDocument>(IEnumerable<TKey> ids) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously deletes the documents matching the condition of the LINQ expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		Task<long> DeleteManyAsync<TDocument>(FilterDefinition<TDocument> filter) where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously deletes the documents matching the condition of the LINQ expression filter.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter">A LINQ expression filter.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		/// <returns>The number of documents deleted.</returns>
		Task<long> DeleteManyAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument<TKey>;
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
		Task<TProjection> ProjectOneAsync<TDocument, TProjection>(TKey id, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns a projected document matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
		/// <param name="filter"></param>
		/// <param name="projection">The projection expression.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		Task<TProjection> ProjectOneAsync<TDocument, TProjection>(FilterDefinition<TDocument> filter, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns a projected document matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
		/// <param name="filter"></param>
		/// <param name="projection">The projection expression.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		Task<TProjection> ProjectOneAsync<TDocument, TProjection>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>;
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
		Task<List<TProjection>> ProjectManyAsync<TDocument, TProjection>(IEnumerable<TKey> ids, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns a list of projected documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
		/// <param name="filter"></param>
		/// <param name="projection">The projection expression.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		Task<List<TProjection>> ProjectManyAsync<TDocument, TProjection>(FilterDefinition<TDocument> filter, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns a list of projected documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
		/// <param name="filter"></param>
		/// <param name="projection">The projection expression.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		Task<List<TProjection>> ProjectManyAsync<TDocument, TProjection>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TProjection>> projection)
			where TDocument : IDocument<TKey>;
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
		Task<List<TDocument>> GetPaginatedAsync<TDocument>(FilterDefinition<TDocument> filter, SortDefinition<TDocument> sorter, int skipNumber = 0, int takeNumber = 50)
			 where TDocument : IDocument<TKey>;

		/// <summary>
		/// Asynchronously returns a paginated list of the documents matching the filter condition.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="filter"></param>
		/// <param name="skipNumber">The number of documents you want to skip. Default value is 0.</param>
		/// <param name="takeNumber">The number of documents you want to take. Default value is 50.</param>
		/// <param name="partitionKey">An optional partition key.</param>
		Task<List<TDocument>> GetPaginatedAsync<TDocument>(Expression<Func<TDocument, bool>> filter, SortDefinition<TDocument> sorter, int skipNumber = 0, int takeNumber = 50)
			 where TDocument : IDocument<TKey>;
		#endregion
	}

	public interface IBaseMongoRepository : IBaseMongoRepository<string> { }
}
