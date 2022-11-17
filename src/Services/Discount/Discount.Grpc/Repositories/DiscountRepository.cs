using Dapper;
using Discount.Grpc.Entities;
using Npgsql;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {

        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = ConnectionDB();

            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(@"
                SELECT * 
                FROM Coupon 
                WHERE ProductName = @ProductName
            ", param: new
            {
                ProductName = productName
            });

            if (coupon == null)
                return new Coupon()
                {
                    ProductName = "No Discount",
                    Amount = 0,
                    Description = "No Discount Desc"
                };

            return coupon;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            NpgsqlConnection connection = ConnectionDB();

            var affected = await connection.ExecuteAsync(@"
                INSERT INTO Coupon (ProductName, Description, Amount)
                VALUES(@productName, @description, @amount)
            ", param: new
            {
                productName = coupon.ProductName,
                description = coupon.Description,
                amount = coupon.Amount
            });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = ConnectionDB();
            var affected = await connection.ExecuteAsync(@"
                UPDATE Coupon 
                SET ProductName = @productName
                , Description = @description
                , Amount = @amount
                WHERE Id = @id
            ", param: new
            {
                productName = coupon.ProductName,
                description = coupon.Description,
                amount = coupon.Amount,
                id = coupon.Id
            });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = ConnectionDB();
            var affected = await connection.ExecuteAsync(@"
                DELETE FROM Coupon 
                WHERE ProductName = @productName
            ", param: new
            {
                productName
            });
            if (affected == 0)
                return false;

            return true;
        }

        private NpgsqlConnection ConnectionDB()
        {
            return new NpgsqlConnection(
                 _configuration.GetValue<string>("DatabaseSettings:ConnectionString")
             );
        }

    }
}
