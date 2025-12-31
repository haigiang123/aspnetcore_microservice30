using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using MongoDB.Driver;
using Shared.Enums;

namespace Inventory.Product.API.Persistence
{
    public class InventoryDbSeed
    {
        public async Task SeedDataAsync(IMongoClient mongoClient, DatabaseSettings settings)
        {
            var databaseName = settings.DatabaseName;
            var database = mongoClient.GetDatabase(databaseName);
            var invventoryCollection = database.GetCollection<InventoryEntry>("InventoryEntry");
            if(await invventoryCollection.EstimatedDocumentCountAsync() == 0)
            {
                await invventoryCollection.InsertManyAsync(GetPreConfiguredInventoryEntries());
            }
        }

        private IEnumerable<InventoryEntry> GetPreConfiguredInventoryEntries()
        {
            return new List<InventoryEntry>
            {
                new()
                {
                    Quantity = 10,
                    DocumentNo = Guid.NewGuid().ToString(),
                    ItemNo = "Lotus",
                    ExternalDocumentNo = Guid.NewGuid().ToString(),
                    DocumentType = EDocumentType.Purchase
                },
                new()
                {
                    Quantity = 10,
                    DocumentNo = Guid.NewGuid().ToString(),
                    ItemNo = "Cadillac",
                    ExternalDocumentNo = Guid.NewGuid().ToString(),
                    DocumentType = EDocumentType.Purchase
                }

            };
        }
    }
}
