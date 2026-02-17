using System.Threading;
using System.Threading.Tasks;
using Agents.Application.Helpers;
using Agents.Application.Interfaces;
using Agents.Application.Interfaces.Repositories;
using Agents.Application.Wrappers;
using Agents.Domain.Products.DTOs;

namespace Agents.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler(IProductRepository productRepository, ITranslator translator) : IRequestHandler<GetProductByIdQuery, BaseResult<ProductDto>>
    {
        public async Task<BaseResult<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdAsync(request.Id);

            if (product is null)
            {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_NotFound_with_id(request.Id)), nameof(request.Id));
            }

            return new ProductDto(product);
        }
    }
}
