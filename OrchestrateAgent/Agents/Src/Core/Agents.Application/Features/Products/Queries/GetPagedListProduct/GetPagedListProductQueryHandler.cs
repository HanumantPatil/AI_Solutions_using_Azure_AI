using System.Threading;
using System.Threading.Tasks;
using Agents.Application.Interfaces;
using Agents.Application.Interfaces.Repositories;
using Agents.Application.Wrappers;
using Agents.Domain.Products.DTOs;

namespace Agents.Application.Features.Products.Queries.GetPagedListProduct
{
    public class GetPagedListProductQueryHandler(IProductRepository productRepository) : IRequestHandler<GetPagedListProductQuery, PagedResponse<ProductDto>>
    {
        public async Task<PagedResponse<ProductDto>> Handle(GetPagedListProductQuery request, CancellationToken cancellationToken)
        {
            return await productRepository.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name);
        }
    }
}
