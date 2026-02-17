using System.Threading;
using System.Threading.Tasks;
using Agents.Application.DTOs.Account.Responses;
using Agents.Application.Interfaces;
using Agents.Application.Interfaces.UserInterfaces;
using Agents.Application.Wrappers;

namespace Agents.Application.Features.Accounts.Commands.Authenticate
{
    public class AuthenticateCommandHandler(IAccountServices accountServices) : IRequestHandler<AuthenticateCommand, BaseResult<AuthenticationResponse>>
    {
        public async Task<BaseResult<AuthenticationResponse>> Handle(AuthenticateCommand request, CancellationToken cancellationToken = default)
        {
            return await accountServices.Authenticate(request);
        }
    }
}