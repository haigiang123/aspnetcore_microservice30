using Contracts.Domains.Interfaces;
using Inventory.API.Extensions;
using Inventory.Product.API.Entities;
using Shared.DTOs.Inventory;
using System.Collections.Generic;

namespace Inventory.API.Services.Interfaces
{
    public interface IInventoryEntryService : IMongoDbRepositoryBase<InventoryEntry>
    {
        Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo);
        Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query);
        Task<InventoryEntryDto> GetByIdAsync(string id);
        Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model);
        Task<InventoryEntryDto> SalesItemAsync(string itemNo, SalesProductDto model);
        Task DeleteByDocumentNoAsync(string documentNo);
        Task<string> SalesOrderAsync(SalesOrderDto model);
    }
}
