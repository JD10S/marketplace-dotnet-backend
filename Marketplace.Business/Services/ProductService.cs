using Marketplace.Business.Interfaces;
using Marketplace.Data.Interfaces;
using Marketplace.Entities.Entities;

namespace Marketplace.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IEnumerable<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product? GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public void Create(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new Exception("Product name is required");

            if (product.Price <= 0)
                throw new Exception("Price must be greater than zero");

            product.CreatedAt = DateTime.UtcNow;
            _productRepository.Create(product);
        }

        public void Update(Product product)
        {
            _productRepository.Update(product);
        }

        public void Delete(int id)
        {
            _productRepository.Delete(id);
        }
    }
}
