using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.Metrics;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{

    // we create the API controller that will handle incoming HTTP requests.

    [ApiController]
    [Route("api/[controller]")]
    public class SenseBoxesController : ControllerBase
    {
        private readonly ISenseBoxService _senseboxService;
        private readonly ILogger<SenseBoxesController> _logger;

        private SenseBoxResponse _responseContent;
        private SenseBox _responseSensebox;
        public SenseBoxesController(ISenseBoxService senseboxService, ILogger<SenseBoxesController> logger)
        {
            _senseboxService = senseboxService;
            _logger = logger;
            _responseContent = new SenseBoxResponse();
        }


        /// <summary>  
        /// Requirement 3
        /// Create NewSenseBox API service method(POST) to create a new sense box in OpenSenseMap.
        /// https://docs.opensensemap.org/#api-Boxes-postNewBox
        /// Creating a new sense box is only successful after the user is login / sign-in.
        /// </summary>  
        /// <header>
        /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoidXNlciIsImlhdCI6MTc1MjEzOTY2MCwiZXhwIjoxNzUyMTQzMjYwLCJpc3MiOiJodHRwczovL2FwaS5vcGVuc2Vuc2VtYXAub3JnIiwic3ViIjoiYXNzZXNzbWVudDA3QHRlc3QuY29tIiwianRpIjoiMWRlY2YwODUtNGI2Zi00MTljLTgyNzgtYjVlNTQ3NzNkNDNmIn0.RLhbY2r4llT1cyyZ9FRrTu-VIByt68OUwb-lcz3GRVQ
        /// </header> 
        /// <param name="SenseBoxRequest">senseboxRequest</param>  
        /// <returns>SenseBoxResponse _id, name, exposure, model, email, and many more</returns>
        [HttpPost]
        [Route("newsensebox")]
        public async Task<IActionResult> NewSenseBox([FromBody] SenseBoxRequest senseboxRequest)
        {
            try
            {
                // https://api.opensensemap.org/boxes
                //Authorization Header example
                //Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoidXNlciIsImlhdCI6MTc1MjEzOTY2MCwiZXhwIjoxNzUyMTQzMjYwLCJpc3MiOiJodHRwczovL2FwaS5vcGVuc2Vuc2VtYXAub3JnIiwic3ViIjoiYXNzZXNzbWVudDA3QHRlc3QuY29tIiwianRpIjoiMWRlY2YwODUtNGI2Zi00MTljLTgyNzgtYjVlNTQ3NzNkNDNmIn0.RLhbY2r4llT1cyyZ9FRrTu-VIByt68OUwb-lcz3GRVQ

                //Example of Request Json:
                //* All the below field(s) are mandatory.
                //{
                //"email": "Assessment07@test.com",
                //"name": "NewBoxForAssessment",
                //"exposure": "indoor",
                //"model": "homeV2Wifi",
                //"location": {
                //"lat": 90,
                //"lng": 180,
                //"height": 100
                //}
                //}

                //Example of Response Json:
                //{
                //"_id" : "686f88b45e06c100080ad7f5",
                //"email": "Assessment07@test.com",
                //"name": "NewBoxForAssessment",
                //"exposure": "indoor",
                //"model": "homeV2Wifi",
                //"location": {
                //"lat": 90,
                //"lng": 180,
                //"height": 100
                //}
                //}

                // Access the Authorization header
                var authorizationHeader = Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(); // Return 401 Unauthorized if authorization header is missing or invalid.
                }

                // Assuming the header format is "Bearer <token>", you can validate and extract the token
                var token = authorizationHeader.StartsWith("Bearer ") ?
                            authorizationHeader.Substring("Bearer ".Length).Trim() : null;

                if (token == null)
                {
                    return BadRequest("Invalid Authorization header.");
                }

                // To check / validate creation of a new sense box is only successful after the valid user is login / sign-in.
                //if (UsersController._cache.TryGetValue("userToken", out string userToken))

                    //Call ISenseBoxService SaveNewSenseBoxAsync API by passing the parameter SenseBoxRequest Object with mandatory fields email, name, model, exposure, and location & valid authorization token
                    _responseContent = await _senseboxService.SaveNewSenseBoxAsync(senseboxRequest, token);

                //if the SenseBoxResponse Object data is not null
                if (_responseContent.data != null)
                {
                    //if the SenseBoxResponse Object data._id is not null or not empty or message is "Box successfully created", then the new sense box in OpenSenseMap is created successfully with an unique id.
                    if (!string.IsNullOrEmpty(_responseContent.data._id) || _responseContent.message.Equals("Box successfully created"))
                        return Ok("A new sense box in OpenSenseMap created successfully with an unique id of { " + _responseContent.data._id + " }");
                    else
                        //if (string.IsNullOrEmpty(_responseContent.data._id) || !_responseContent.message.Equals("Box successfully created"))
                        //else i.e. the SenseBoxResponse Object data._id is null or empty or message is not "Box successfully created", then the new sense box in OpenSenseMap is not created or not saved.
                        return BadRequest(_responseContent.code + " - A new sense box in OpenSenseMap was not created or not saved due to " + _responseContent.message);
                }
                else
                    //if (string.IsNullOrEmpty(_responseContent.data._id) || !_responseContent.message.Equals("Box successfully created"))
                    //else i.e. the SenseBoxResponse Object data._id is null or empty or message is not "Box successfully created", then the new sense box in OpenSenseMap is not created or not saved.
                    return BadRequest(_responseContent.code + " - A new sense box in OpenSenseMap was not created or not saved due to " + _responseContent.message);
                
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        /// <summary>  
        /// Requirement 4
        /// Create GetSenseBoxById API service method(GET) to retrieve the sense box by id OpenSenseMap.
        /// https://docs.opensensemap.org/#api-Boxes-getBox
        /// </summary>  
        /// <param name="senseBoxId">686f88b45e06c100080ad7f5</param>  
        /// <returns>SenseBoxResponse _id, name, exposure, model, email, and many more</returns>
                
        [HttpGet]
        [Route("getsenseboxbyid/{senseBoxId}")]
        public async Task<IActionResult> GetSenseBoxById(string senseBoxId)
        {
            try
            {
                //https://api.opensensemap.org/boxes/686f88b45e06c100080ad7f5?format=json

                //responseContent contains the JSON data 
                //{
                //"_id": "686f88b45e06c100080ad7f5",
                //"email": "Assessment07@test.com",
                //name": "NewBoxForAssessment",
                //"exposure": "indoor",
                //"model": "homeV2Wifi"
                //}

                //Call ISenseBoxService GetSenseBoxByIdAsync API by passing the parameter senseBoxId
                _responseSensebox = await _senseboxService.GetSenseBoxByIdAsync(senseBoxId);

                //if the SenseBoxResponse Object _id is null or empty, then the sense box by id OpenSenseMap is not found or not available.
                if (string.IsNullOrEmpty(_responseSensebox._id))
                    return BadRequest("The sense box by id {" + senseBoxId + "} OpenSenseMap is not found or not available.");
                else
                    //if (_responseContent._id.Equals(senseBoxId))
                    //else i.e. the SenseBoxResponse Object _id is not null or not empty, then the sense box by id OpenSenseMap is found or available with name, model, exposure, email, etc..
                    return Ok("The sense box by id {" + senseBoxId + "} OpenSenseMap is found or available and retrieved successfully with name as {" + _responseSensebox.name + "} and model as {" + _responseSensebox.model + "}");

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

    }
}
