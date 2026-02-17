using Agents.Application.DTOs.Account.Requests;
using Agents.Application.Interfaces;
using Agents.Application.Wrappers;

namespace Agents.Application.Features.Accounts.Commands.ChangePassword
{
    public class ChangePasswordCommand : ChangePasswordRequest, IRequest<BaseResult>
    {
    }
}