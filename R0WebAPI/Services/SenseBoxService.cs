using System;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services
{
    
// Service to handle user registration
public class SenseBoxService : ISenseBoxService
    {
        // Example of a user repository. In a real application, this could connect to a database.
        private readonly ISenseBoxRepository _boxesRepository;
        private readonly ILogger<SenseBoxService> _logger;
        private SenseBoxResponse _responseContent;
        public SenseBoxService(ISenseBoxRepository boxesRepository, ILogger<SenseBoxService> logger)
        {
            _boxesRepository = boxesRepository;
            _logger = logger;
            _responseContent = new SenseBoxResponse();
        }

        public async Task<SenseBoxResponse> SaveNewSenseBoxAsync(SenseBoxRequest senseboxRequest,string token)
        {
            //* All the below field(s) are mandatory.
            //{
            //"email": "Assessment01@test.com",
            //"name": "NewBoxForAssessment",
            //"exposure": "indoor",
            //"model": "homeV2Wifi",
            //"location": {
            //   "lat": 90,
            //   "lng": 180,
            //   "height": 100
            //   }
            //}

            // Validate inputs
            if (string.IsNullOrWhiteSpace(senseboxRequest.email))
            {
                _logger.LogError("Email is required !");
                throw new ArgumentException("Email is required", nameof(senseboxRequest.email));
            }
            if (string.IsNullOrWhiteSpace(senseboxRequest.name))
            {
                _logger.LogError("Name is required !");
                throw new ArgumentException("Name is required", nameof(senseboxRequest.name));
            }
            if (string.IsNullOrWhiteSpace(senseboxRequest.exposure))
            {
                _logger.LogError("Exposure is required !");
                throw new ArgumentException("Exposure is required", nameof(senseboxRequest.exposure));
            }
            if (string.IsNullOrWhiteSpace(senseboxRequest.model))
            {
                _logger.LogError("Model is required !");
                throw new ArgumentException("Model is required", nameof(senseboxRequest.model));
            }
            

            // To check / validate creation of a new sense box is only successful after the valid user is login / sign-in.
            if (!UserService._tokenEmail.ContainsKey(token))
            {
                _responseContent.code = "UnauthorizedAccess";
                _responseContent.message = "Creating a new sense box is only successful after the valid user is login / sign-in.";
            }
            else
            {
                // Save the SenseBox Info. to the database / web API call
                _responseContent = await _boxesRepository.SaveNewSenseBoxAsync(senseboxRequest, token);
            }

            return _responseContent;
        }

        public async Task<SenseBox> GetSenseBoxByIdAsync(string senseBoxId)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(senseBoxId))
            {
                _logger.LogError("SenseBox Id is required !");
                throw new ArgumentException("SenseBox Id is required", nameof(senseBoxId));
            }
            // Get the SenseBox Info. from the database / web API call
            return await _boxesRepository.GetSenseBoxByIdAsync(senseBoxId);

        }
               
    }

}
