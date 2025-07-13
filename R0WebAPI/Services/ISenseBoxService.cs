using WebAPI.Models;

namespace WebAPI.Services
{
   
    public interface ISenseBoxService
    {
        Task<SenseBoxResponse> SaveNewSenseBoxAsync(SenseBoxRequest senseboxRequest, string token);
        Task<SenseBox> GetSenseBoxByIdAsync(string senseBoxId);
    }

}
