using System.Linq;
using System.Threading.Tasks;
using Agents.Application.DTOs;
using Agents.Application.Interfaces.Repositories;
using Agents.Domain.Products.DTOs;
using Agents.Domain.Products.Entities;
using Agents.Infrastructure.Persistence.Contexts;

namespace Agents.Infrastructure.Persistence.Repositories
{
    public class ProductRepository(ApplicationDbContext dbContext) : GenericRepository<Product>(dbContext), IProductRepository
    {
        public async Task<PaginationResponseDto<ProductDto>> GetPagedListAsync(int pageNumber, int pageSize, string name)
        {
            var query = dbContext.Products.OrderBy(p => p.Created).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            return await Paged(
                query.Select(p => new ProductDto(p)),
                pageNumber,
                pageSize);

        }
    }
}
