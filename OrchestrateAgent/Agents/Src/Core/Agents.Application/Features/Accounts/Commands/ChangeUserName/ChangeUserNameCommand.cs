using Agents.Application.DTOs.Account.Requests;
using Agents.Application.Interfaces;
using Agents.Application.Wrappers;

namespace Agents.Application.Features.Accounts.Commands.ChangeUserName
{
    public class ChangeUserNameCommand : ChangeUserNameRequest, IRequest<BaseResult>
    {
    }
}