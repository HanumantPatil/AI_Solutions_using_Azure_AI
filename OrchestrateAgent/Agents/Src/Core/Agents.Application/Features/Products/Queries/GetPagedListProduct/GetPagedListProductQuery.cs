using Agents.Application.Interfaces;
using Agents.Application.Parameters;
using Agents.Application.Wrappers;
using Agents.Domain.Products.DTOs;

namespace Agents.Application.Features.Products.Queries.GetPagedListProduct
{
    public class GetPagedListProductQuery : PaginationRequestParameter, IRequest<PagedResponse<ProductDto>>
    {
        public string Name { get; set; }
    }
}
