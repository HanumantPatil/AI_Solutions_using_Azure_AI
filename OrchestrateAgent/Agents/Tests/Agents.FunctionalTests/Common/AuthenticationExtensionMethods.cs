using Agents.Application.DTOs.Account.Responses;
using Agents.Application.Wrappers;

namespace Agents.FunctionalTests.Common
{
    public static class AuthenticationExtensionMethods
    {
        public static async Task<AuthenticationResponse> GetGhostAccount(this HttpClient client)
        {
            var url = ApiRoutes.Account.Start;

            var result = await client.PostAndDeserializeAsync<BaseResult<AuthenticationResponse>>(url);

            return result.Data;
        }
    }
}
