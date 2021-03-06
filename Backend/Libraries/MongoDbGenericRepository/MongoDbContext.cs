﻿using MongoDB.Driver;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using MongoDbGenericRepository.Utils;
using System;
using System.Linq;
using System.Reflection;
using MongoDbGenericRepository.Abstractions;

namespace MongoDbGenericRepository
{
	/// <summary>
	/// The MongoDb context
	/// </summary>
	public class MongoDbContext : IMongoDbContext
	{
		/// <summary>
		/// The IMongoClient from the official MongoDb driver
		/// </summary>
		public IMongoClient Client { get; }

		/// <summary>
		/// The IMongoDatabase from the official Mongodb driver
		/// </summary>
		public IMongoDatabase Database { get; }

		static MongoDbContext()
		{
			// Avoid legacy UUID representation: use Binary 0x04 subtype.
			MongoDefaults.GuidRepresentation = MongoDB.Bson.GuidRepresentation.Standard;
		}

		/// <summary>
		/// Sets the Guid representation of the MongoDb Driver.
		/// </summary>
		/// <param name="guidRepresentation">The new value of the GuidRepresentation</param>
		public void SetGuidRepresentation(MongoDB.Bson.GuidRepresentation guidRepresentation)
		{
			MongoDefaults.GuidRepresentation = guidRepresentation;
		}

		/// <summary>
		/// The constructor of the MongoDbContext, it needs a an object implementing <see cref="IMongoDatabase"/>.
		/// </summary>
		/// <param name="mongoDatabase">An object implementing IMongoDatabase</param>
		public MongoDbContext(IMongoDatabase mongoDatabase)
		{
			Database = mongoDatabase;
			Client = Database.Client;
		}

		/// <summary>
		/// The constructor of the MongoDbContext, it needs a connection string and a database name. 
		/// </summary>
		/// <param name="connectionString">The connections string.</param>
		/// <param name="databaseName">The name of your database.</param>
		public MongoDbContext(string connectionString, string databaseName)
		{
			Client = new MongoClient(connectionString);
			Database = Client.GetDatabase(databaseName);
		}

		/// <summary>
		/// The private GetCollection method
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <returns></returns>
		public IMongoCollection<TDocument> GetCollection<TDocument>()
		{
			var collectionNameAttribute = typeof(TDocument).GetTypeInfo().GetCustomAttributes(typeof(CollectionNameAttribute)).FirstOrDefault() as CollectionNameAttribute;
			var name = collectionNameAttribute?.Name ?? Pluralize<TDocument>();
			return Database.GetCollection<TDocument>(name);
		}

		/// <summary>
		/// Returns a collection for a document type that has a partition key.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <param name="partitionKey">The value of the partition key.</param>
		public IMongoCollection<TDocument> GetCollection<TDocument>(string partitionKey) where TDocument : IDocument
		{
			var collectionNameAttribute = typeof(TDocument).GetTypeInfo().GetCustomAttributes(typeof(CollectionNameAttribute)).FirstOrDefault() as CollectionNameAttribute;
			var name = partitionKey + "-" + collectionNameAttribute?.Name ?? Pluralize<TDocument>();
			return Database.GetCollection<TDocument>(name);
		}

		/// <summary>
		/// Returns a collection for a document type that has a partition key.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <typeparam name="TKey">The type of the primary key for a Document.</typeparam>
		/// <param name="partitionKey">The value of the partition key.</param>
		public IMongoCollection<TDocument> GetCollection<TDocument, TKey>(string partitionKey)
			where TDocument : IDocument<TKey>
			where TKey : IEquatable<TKey>
		{
			var collectionNameAttribute = typeof(TDocument).GetTypeInfo().GetCustomAttributes(typeof(CollectionNameAttribute)).FirstOrDefault() as CollectionNameAttribute;
			var name = partitionKey + "-" + collectionNameAttribute?.Name ?? Pluralize<TDocument>();
			return Database.GetCollection<TDocument>(name);
		}

		/// <summary>
		/// Drops a collection, use very carefully.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		public void DropCollection<TDocument>()
		{
			var collectionNameAttribute = typeof(TDocument).GetTypeInfo().GetCustomAttributes(typeof(CollectionNameAttribute)).FirstOrDefault() as CollectionNameAttribute;
			var name = collectionNameAttribute?.Name ?? Pluralize<TDocument>();
			Database.DropCollection(name);
		}

		/// <summary>
		/// Drops a collection having a partitionkey, use very carefully.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		public void DropCollection<TDocument>(string partitionKey)
		{
			var collectionNameAttribute = typeof(TDocument).GetTypeInfo().GetCustomAttributes(typeof(CollectionNameAttribute)).FirstOrDefault() as CollectionNameAttribute;
			var name = partitionKey + "-" + collectionNameAttribute?.Name ?? Pluralize<TDocument>();
			Database.DropCollection(name);
		}

		/// <summary>
		/// Very naively pluralizes a TDocument type name.
		/// </summary>
		/// <typeparam name="TDocument">The type representing a Document.</typeparam>
		/// <returns></returns>
		private string Pluralize<TDocument>()
		{
			return (typeof(TDocument).Name.Pluralize()).Camelize();
		}
	}
}