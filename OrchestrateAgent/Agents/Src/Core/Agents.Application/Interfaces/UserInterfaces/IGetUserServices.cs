using System.Threading.Tasks;
using Agents.Application.DTOs.Account.Requests;
using Agents.Application.DTOs.Account.Responses;
using Agents.Application.Wrappers;

namespace Agents.Application.Interfaces.UserInterfaces
{
    public interface IGetUserServices
    {
        Task<PagedResponse<UserDto>> GetPagedUsers(GetAllUsersRequest model);
    }
}
