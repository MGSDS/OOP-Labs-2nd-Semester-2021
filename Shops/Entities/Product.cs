using Shops.Tools;

namespace Shops.Entities
{
    public class Product
    {
        public Product(string name)
        {
            Name = name ?? throw new ShopServiceException("Product name can not be null");
        }

        public string Name { get; }
    }
}