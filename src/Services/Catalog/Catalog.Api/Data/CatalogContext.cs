using Catalog.Api.Entities;
using MongoDB.Driver;

namespace Catalog.Api.Data
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }

        public CatalogContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var databaseName = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
            Products = databaseName.GetCollection<Product>("DatabaseSettings:CollectionName");

            CatalogContextSeed.SeedData(Products);

        }

    }
}
