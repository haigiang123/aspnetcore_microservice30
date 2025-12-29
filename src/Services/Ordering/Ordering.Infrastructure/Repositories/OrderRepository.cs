using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order, long, OrderContext>, IOrderRepository
    {
        public OrderRepository(OrderContext context, IUnitOfWork<OrderContext> unitOfWork) : base(context, unitOfWork)
        { 
            
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string userName)
        {
            return await FindByCondition(x => x.UserName.Equals(userName))
                .ToListAsync();
        }

        public async Task<Order> GetOrderByDocumentNo(string documentNo)
        {
            return await FindByCondition(x => x.DocumentNo.ToString().Equals(documentNo)).FirstOrDefaultAsync();
        }

        public async Task CreateOrder(Order order)
        {
            await CreateAsync(order);
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            await UpdateAsync(order);
            return order;
        }

        public async Task DeleteOrder(Order order)
        {
            DeleteAsync(order);
        }
    }
}
