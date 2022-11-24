using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
    public static class OrderContextSeed
    {

        public static async Task SeedAsync(OrderContext orderContext)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new Order(){
                    UserName="MJS",
                    FirstName="Mohammad Javad",
                    LastName="Sardehghan",
                    EmailAddress="mohammadjs2642@gmail.com",
                    AddressLine="Tehran",
                    Country="Iran",
                    TotalPrice=1000
                }
            };
        }

    }
}
