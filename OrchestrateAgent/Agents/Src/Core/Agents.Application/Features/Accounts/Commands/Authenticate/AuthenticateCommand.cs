using Agents.Application.DTOs.Account.Requests;
using Agents.Application.DTOs.Account.Responses;
using Agents.Application.Interfaces;
using Agents.Application.Wrappers;

namespace Agents.Application.Features.Accounts.Commands.Authenticate
{
    public class AuthenticateCommand : AuthenticationRequest, IRequest<BaseResult<AuthenticationResponse>>
    {
    }
}