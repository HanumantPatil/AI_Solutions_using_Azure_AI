using System.Threading;
using System.Threading.Tasks;
using Agents.Application.DTOs.Account.Responses;
using Agents.Application.Interfaces;
using Agents.Application.Interfaces.UserInterfaces;
using Agents.Application.Wrappers;

namespace Agents.Application.Features.Accounts.Commands.Start
{
    public class StartCommandHandler(IAccountServices accountServices) : IRequestHandler<StartCommand, BaseResult<AuthenticationResponse>>
    {
        public async Task<BaseResult<AuthenticationResponse>> Handle(StartCommand request, CancellationToken cancellationToken = default)
        {
            var ghostUsername = await accountServices.RegisterGhostAccount();
            return await accountServices.AuthenticateByUserName(ghostUsername.Data);
        }
    }
}