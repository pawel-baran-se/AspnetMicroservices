using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Data
{
	public class CatalogContext : ICatalogContext
	{
		public CatalogContext(IConfiguration configuration)
		{
			var mongoClient = new MongoClient(
				configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

			var mongoDatabase = mongoClient.GetDatabase(
				configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

			Products = mongoDatabase.GetCollection<Product>(
				configuration.GetValue<string>("DatabaseSettings:CollectionName"));

			CatalogContextSeed.SeedData(Products);
		}


		public IMongoCollection<Product> Products { get; }
	}
}
