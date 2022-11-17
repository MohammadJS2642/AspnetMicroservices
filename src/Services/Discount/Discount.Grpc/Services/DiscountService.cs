using AutoMapper;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(
            IDiscountRepository repository
            , IMapper mapper
            , ILogger<DiscountService> logger
        )
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if (coupon == null)
            {
                throw new RpcException(
                    new Status(
                        StatusCode.NotFound,
                        $"Discount with ProductName={request.ProductName} is not found."
                    )
                );
            }

            _logger.LogInformation(
                "Discount is retrieved for ProductName: {productName}, Amount: {Amount}",
                coupon.ProductName, coupon.Amount
            );

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

    }
}
