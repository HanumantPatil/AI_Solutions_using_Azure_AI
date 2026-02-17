using System;
using System.Collections.Generic;
using System.Linq;
using Agents.Application.Wrappers;
using Agents.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Agents.WebApi.Endpoints
{
    public class DocEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetErrorCodes);
        }

        BaseResult<Dictionary<int, string>> GetErrorCodes()
            => Enum.GetValues<ErrorCode>().ToDictionary(t => (int)t, t => t.ToString());

    }
}
