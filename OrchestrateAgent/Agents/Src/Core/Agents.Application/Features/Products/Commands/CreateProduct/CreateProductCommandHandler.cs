using System.Threading;
using System.Threading.Tasks;
using Agents.Application.Interfaces;
using Agents.Application.Interfaces.Repositories;
using Agents.Application.Wrappers;
using Agents.Domain.Products.Entities;

namespace Agents.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Name, request.Price, request.BarCode);

            await productRepository.AddAsync(product);
            await unitOfWork.SaveChangesAsync();

            return product.Id;
        }
    }
}
