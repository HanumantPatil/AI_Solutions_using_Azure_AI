using System.Threading.Tasks;
using Agents.Application.Features.Products.Commands.CreateProduct;
using Agents.Application.Features.Products.Commands.DeleteProduct;
using Agents.Application.Features.Products.Commands.UpdateProduct;
using Agents.Application.Features.Products.Queries.GetPagedListProduct;
using Agents.Application.Features.Products.Queries.GetProductById;
using Agents.Application.Interfaces;
using Agents.Application.Wrappers;
using Agents.Domain.Products.DTOs;
using Agents.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Agents.WebApi.Endpoints
{
    public class ProductEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListProduct);

            builder.MapGet(GetProductById);

            builder.MapPost(CreateProduct).RequireAuthorization();

            builder.MapPut(UpdateProduct).RequireAuthorization();

            builder.MapDelete(DeleteProduct).RequireAuthorization();
        }

        async Task<PagedResponse<ProductDto>> GetPagedListProduct(IMediator mediator, [AsParameters] GetPagedListProductQuery model)
            => await mediator.Send<GetPagedListProductQuery, PagedResponse<ProductDto>>(model);

        async Task<BaseResult<ProductDto>> GetProductById(IMediator mediator, [AsParameters] GetProductByIdQuery model)
            => await mediator.Send<GetProductByIdQuery, BaseResult<ProductDto>>(model);

        async Task<BaseResult<long>> CreateProduct(IMediator mediator, CreateProductCommand model)
            => await mediator.Send<CreateProductCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateProduct(IMediator mediator, UpdateProductCommand model)
            => await mediator.Send<UpdateProductCommand, BaseResult>(model);

        async Task<BaseResult> DeleteProduct(IMediator mediator, [AsParameters] DeleteProductCommand model)
            => await mediator.Send<DeleteProductCommand, BaseResult>(model);

    }
}
