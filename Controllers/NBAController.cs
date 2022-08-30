using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NBAProcessingAPI.Models.NBAAPIResponse;
using NBAProcessingAPI.Models.ReturnModels;
using NBAProcessingAPI.Models.Shared;
using NBAProcessingAPI.Services;
using RestSharp;
using System.Linq;
using System.Net;

namespace NBAProcessingAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class NBAController : Controller
    {
        //External API calls as a Service to aid in Mocking
        private readonly NBAService _NBAService = new NBAService();

        /**
         * Hypothetical API Key, in a realworld example these would be associated with 
         * individual users. This is useful for authentication of Paid/ Premium users as well
         * as a first step towards rate limiting
         * **/
        private const string API_KEY = "1234";

        //Returned Constants
        private const string VERSION = "0.0.1";
        private const string SOURCE = "sports-api";

        [EnableCors("GoodrPolicy")]
        [HttpGet]
        public IActionResult Get(int season, int teamOneId, int teamTwoId, string key)
        {
            try{ //try-catch for 500:Something Went Wrong
                if (string.IsNullOrEmpty(key))
                {
                    //401-Unauthorized: No key is present in the request
                    var _resp = new GenericAPIResponse<string>()
                    {
                        Version = VERSION,
                        Source = SOURCE,
                        Message = "Unauthorized Request",
                        Status = 401,
                        Content = "Unauthorized Request"
                    };
                    return StatusCode(401, _resp);
                }
                if (key != API_KEY)
                {
                    //403-Forbidden: The given key is incorrect
                    var _resp = new GenericAPIResponse<string>()
                    {
                        Version = VERSION,
                        Source = SOURCE,
                        Message = "Request Forbidden",
                        Status = 403,
                        Content = "Request Forbidden"
                    };
                    return StatusCode(403, _resp);
                }

                //Fetch record from NBAService
                RestResponse response = _NBAService.FetchRecord(season, teamOneId, teamTwoId);
                var _responseStatus = response.ResponseStatus;
                if (_responseStatus == ResponseStatus.Error)
                {
                    //500-Internal Error: NBA Service responded with an Error
                    var _resp = new GenericAPIResponse<string>()
                    {
                        Version = VERSION,
                        Source = SOURCE,
                        Message = "Failed to Fetch",
                        Status = 500,
                        Content = "Failed to Fetch"
                    };
                    return StatusCode(500, _resp);
                }
                if (_responseStatus == ResponseStatus.TimedOut)
                {
                    //500-Internal Error: NBA Service Timed Out
                    var _resp = new GenericAPIResponse<string>()
                    {
                        Version = VERSION,
                        Source = SOURCE,
                        Message = "External API Timeout",
                        Status = 500,
                        Content = "External API Timeout"
                    };
                    return StatusCode(500, _resp);
                }
                string cache = response.Content ?? "";
                NBAGetResponse resp = new NBAGetResponse();
                try //try-catch to deserialize NBA Service Response
                {
                    resp = Newtonsoft.Json.JsonConvert.DeserializeObject<NBAGetResponse>(cache) ?? new NBAGetResponse();
                }
                catch
                {
                    //500-Internal Error: Response from NBAService in an unexpected format
                    var _resp = new GenericAPIResponse<string>()
                    {
                        Version = VERSION,
                        Source = SOURCE,
                        Message = "Failed to Deserialize Response",
                        Status = 500,                        
                        Content = "Failed to Deserialize Response"
                    };
                    return StatusCode(500, _resp);
                }
                //Use LINQ to project new model from NBA Service Model
                var _data = resp.Response.Select(r => new NBAReturnStats()
                {
                    Date = DateTime.Parse(r.Date.Start),
                    Home = r.Teams.Home.Code,
                    Visitor = r.Teams.Visitors.Code,
                    HomeScore = r.Scores.Home.Points ?? 0,
                    VisitorScore = r.Scores.Visitors.Points ?? 0
                }).ToList();

                //200 - OK
                var output = new GenericAPIResponse<NBAStatPayload>();
                output.Version = VERSION;
                output.Source = SOURCE;
                output.Status = 200;
                output.Content = new NBAStatPayload() { Data = _data };
                //Check the count of games
                if (_data.Count == 0)
                {
                    //For clarity as an empty model will be returned
                    output.Message = "No Games Found";
                }
                else
                {
                    //For messasging consistancy
                    output.Message = $"{_data.Count} Games Found";
                }
                return Ok(output);
            }
            catch(Exception e)
            {
                //500-Internal Error:Something Went Wrong; Something unexpected happened
                var _resp = new GenericAPIResponse<string>()
                {
                    Version = VERSION,
                    Source = SOURCE,
                    Message = "Something went wrong",
                    Status = 500,
                    Content = e.Message
                };
                return StatusCode(500, _resp);
            }
        }

    }
}
