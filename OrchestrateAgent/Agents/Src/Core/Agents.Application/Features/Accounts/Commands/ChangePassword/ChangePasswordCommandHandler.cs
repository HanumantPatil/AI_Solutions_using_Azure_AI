using System.Threading;
using System.Threading.Tasks;
using Agents.Application.Interfaces;
using Agents.Application.Interfaces.UserInterfaces;
using Agents.Application.Wrappers;

namespace Agents.Application.Features.Accounts.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler(IAccountServices accountServices) : IRequestHandler<ChangePasswordCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken = default)
        {
            return await accountServices.ChangePassword(request);
        }
    }
}