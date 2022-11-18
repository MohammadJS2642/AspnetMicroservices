using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Orders.Queries.GetOrderList
{
    public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, List<OrdersVm>>
    {

        private readonly IOrderRepository _orderOepository;
        private readonly IMapper _mapper;

        public GetOrderListQueryHandler(IOrderRepository orderOepository, IMapper mapper)
        {
            _orderOepository = orderOepository;
            _mapper = mapper;
        }

        public async Task<List<OrdersVm>> Handle(
            GetOrderListQuery request,
            CancellationToken cancellationToken
        )
        {
            var orderList = await _orderOepository.GetOrdersByUsername(request.UserName);
            return _mapper.Map<List<OrdersVm>>(orderList);

        }
    }
}
