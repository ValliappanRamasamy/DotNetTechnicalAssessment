using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public interface ISenseBoxRepository
    {
        Task<SenseBoxResponse> SaveNewSenseBoxAsync(SenseBoxRequest senseboxRequest,string token);
        Task<SenseBox> GetSenseBoxByIdAsync(string senseBoxId);
    }
}
