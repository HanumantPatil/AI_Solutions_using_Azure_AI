using System.Threading.Tasks;
using Agents.Application.DTOs;
using Agents.Domain.Products.DTOs;
using Agents.Domain.Products.Entities;

namespace Agents.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<PaginationResponseDto<ProductDto>> GetPagedListAsync(int pageNumber, int pageSize, string name);
    }
}
