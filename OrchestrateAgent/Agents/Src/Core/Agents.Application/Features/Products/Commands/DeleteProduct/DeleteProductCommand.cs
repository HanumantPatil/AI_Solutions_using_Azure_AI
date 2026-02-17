using Agents.Application.Interfaces;
using Agents.Application.Wrappers;

namespace Agents.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest<BaseResult>
    {
        public long Id { get; set; }
    }
}
