using System.Threading.Tasks;
using Agents.Application.DTOs.Account.Responses;
using Agents.Application.Features.Accounts.Commands.Authenticate;
using Agents.Application.Features.Accounts.Commands.ChangePassword;
using Agents.Application.Features.Accounts.Commands.ChangeUserName;
using Agents.Application.Features.Accounts.Commands.Start;
using Agents.Application.Interfaces;
using Agents.Application.Wrappers;
using Agents.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Agents.WebApi.Endpoints
{
    public class AccountEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapPost(Authenticate);

            builder.MapPut(ChangeUserName).RequireAuthorization();

            builder.MapPut(ChangePassword).RequireAuthorization();

            builder.MapPost(Start);
        }

        async Task<BaseResult<AuthenticationResponse>> Authenticate(IMediator mediator, AuthenticateCommand model)
            => await mediator.Send<AuthenticateCommand, BaseResult<AuthenticationResponse>>(model);

        async Task<BaseResult> ChangeUserName(IMediator mediator, ChangeUserNameCommand model)
            => await mediator.Send<ChangeUserNameCommand, BaseResult>(model);

        async Task<BaseResult> ChangePassword(IMediator mediator, ChangePasswordCommand model)
            => await mediator.Send<ChangePasswordCommand, BaseResult>(model);

        async Task<BaseResult<AuthenticationResponse>> Start(IMediator mediator)
            => await mediator.Send<StartCommand, BaseResult<AuthenticationResponse>>(new StartCommand());

    }
}
