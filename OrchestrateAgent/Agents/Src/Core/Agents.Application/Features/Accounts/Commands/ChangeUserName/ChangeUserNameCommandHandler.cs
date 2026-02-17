using System.Threading;
using System.Threading.Tasks;
using Agents.Application.Interfaces;
using Agents.Application.Interfaces.UserInterfaces;
using Agents.Application.Wrappers;

namespace Agents.Application.Features.Accounts.Commands.ChangeUserName
{
    public class ChangeUserNameCommandHandler(IAccountServices accountServices) : IRequestHandler<ChangeUserNameCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(ChangeUserNameCommand request, CancellationToken cancellationToken = default)
        {
            return await accountServices.ChangeUserName(request);
        }
    }
}