using Agents.Application.DTOs.Account.Responses;
using Agents.Application.Interfaces;
using Agents.Application.Wrappers;

namespace Agents.Application.Features.Accounts.Commands.Start
{
    public class StartCommand : IRequest<BaseResult<AuthenticationResponse>>
    {
    }
}