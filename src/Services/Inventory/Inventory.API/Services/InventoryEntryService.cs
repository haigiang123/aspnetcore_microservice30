using AutoMapper;
using Inventory.API.Extensions;
using Inventory.API.Services.Interfaces;
using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.DTOs.Inventory;
using Infrastructure.Extensions;
using Shared.Configurations;
using Infrastructure.Common;

namespace Inventory.API.Services
{
    public class InventoryEntryService : MongoDbRepository<InventoryEntry>, IInventoryEntryService
    {
        public readonly IMapper _mapper;
        public InventoryEntryService(IMongoClient client, MongoDbSettings settings, IMapper mapper) : base(client, settings)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo)
        {
            var entities = await FindAll()
                .Find(x => x.ItemNo.Equals(itemNo))
                .ToListAsync();
            var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);

            return result;
        }

        public async Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query)
        {
            var filterSearchTerm = Builders<InventoryEntry>.Filter.Empty;
            var filterItemNo = Builders<InventoryEntry>.Filter.Eq(s => s.ItemNo, query.ItemNo());
            if (!string.IsNullOrEmpty(query.SearchTerm))
                filterSearchTerm = Builders<InventoryEntry>.Filter.Eq(s => s.DocumentNo, query.SearchTerm);

            var andFilter = filterItemNo & filterSearchTerm;

            var pagedList = await Collection.PaginatedListAsync(andFilter, pageIndex: query.PageIndex, pageNumber: query.PageSize);
            var items = _mapper.Map<IEnumerable<InventoryEntryDto>>(pagedList);
            var result = new PagedList<InventoryEntryDto>(items, pagedList.GetMetaData().TotalItems, query.PageIndex,
                query.PageSize);
            return result;
        }

        public async Task<InventoryEntryDto> GetByIdAsync(string id)
        {
            FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(s => s.Id, id);
            var entity = await FindAll().Find(filter).FirstOrDefaultAsync();
            var result = _mapper.Map<InventoryEntryDto>(entity);

            return result;
        }

        public async Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model)
        {
            var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                ItemNo = itemNo,
                Quantity = model.Quantity,
                DocumentType = model.DocumentType,
            };
            var entity = _mapper.Map<InventoryEntry>(itemToAdd);
            await CreateAsync(entity);
            var result = _mapper.Map<InventoryEntryDto>(entity);

            return result;
        }
    }
}
