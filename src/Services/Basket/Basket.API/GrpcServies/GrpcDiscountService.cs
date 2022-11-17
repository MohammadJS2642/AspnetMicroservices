using Discount.Grpc.Protos;

namespace Basket.API.GrpcServies
{
    public class GrpcDiscountService
    {

        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;

        public GrpcDiscountService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            _discountProtoService = discountProtoService;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var discountRequest = new GetDiscountRequest { ProductName = productName };
            return await _discountProtoService.GetDiscountAsync(discountRequest);
        }

    }
}
