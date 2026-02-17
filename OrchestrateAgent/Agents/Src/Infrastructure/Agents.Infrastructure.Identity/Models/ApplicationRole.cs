using System;
using Microsoft.AspNetCore.Identity;

namespace Agents.Infrastructure.Identity.Models
{
    public class ApplicationRole(string name) : IdentityRole<Guid>(name)
    {
    }
}
