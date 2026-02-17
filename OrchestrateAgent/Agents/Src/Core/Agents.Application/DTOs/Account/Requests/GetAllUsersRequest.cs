using Agents.Application.Parameters;

namespace Agents.Application.DTOs.Account.Requests
{
    public class GetAllUsersRequest : PaginationRequestParameter
    {
        public string Name { get; set; }
    }
}
