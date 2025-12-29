using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;
using Serilog;
using AutoMapper;
using Ordering.Application.Common.Interfaces;

namespace Ordering.Application.Features.V1.Orders.Queries.GetOrders
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, ApiResult<List<OrderDto>>>
    {
        private readonly string MethodName = "GetOrdersQueryHandler";
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _repository;

        public GetOrdersQueryHandler(ILogger logger, IMapper mapper, IOrderRepository repository) 
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(repository));
        }


        public async Task<ApiResult<List<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken token)
        {
            _logger.Information($"BEGIN: {MethodName} - Username: {request.UserName}");

            var orderEntities = await _repository.GetOrdersByUserNameAsync(request.UserName);
            var orders = _mapper.Map<List<OrderDto>>(orderEntities);

            _logger.Information($"END: {MethodName} - Username: {request.UserName}");

            return new ApiSuccessResult<List<OrderDto>>(orders);
        }
    }
}
