using System.Threading.Tasks;

namespace Agents.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> SaveChangesAsync();
    }
}
