using Agents.Application.Interfaces;
using Agents.Application.Wrappers;
using Agents.Domain.Products.DTOs;

namespace Agents.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<BaseResult<ProductDto>>
    {
        public long Id { get; set; }
    }
}
