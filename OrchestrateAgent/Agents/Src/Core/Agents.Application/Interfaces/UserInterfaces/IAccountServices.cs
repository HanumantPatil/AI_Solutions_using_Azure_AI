using System.Threading.Tasks;
using Agents.Application.DTOs.Account.Requests;
using Agents.Application.DTOs.Account.Responses;
using Agents.Application.Wrappers;

namespace Agents.Application.Interfaces.UserInterfaces
{
    public interface IAccountServices
    {
        Task<BaseResult<string>> RegisterGhostAccount();
        Task<BaseResult> ChangePassword(ChangePasswordRequest model);
        Task<BaseResult> ChangeUserName(ChangeUserNameRequest model);
        Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request);
        Task<BaseResult<AuthenticationResponse>> AuthenticateByUserName(string username);

    }
}
