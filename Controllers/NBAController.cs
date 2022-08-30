using Microsoft.AspNetCore.Mvc;
using NBAProcessingAPI.Models.NBAAPIResponse;
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
        private readonly NBAService _NBAService = new NBAService();
        private readonly string API_KEY = "1234";
        private readonly string VERSION = "0.0.1";

        [HttpGet]
        public IActionResult Get(int season, int teamOneId, int teamTwoId, string key)
        {
            try{
                if (string.IsNullOrEmpty(key))
                {
                    //401-Unauthorized
                    var _resp = new GenericAPIResponse<string>()
                    {
                        Version = VERSION,
                        Message = "Unauthorized Request",
                        Status = 401,
                        Content = "Unauthorized Request"
                    };
                    return StatusCode(401, _resp);
                }
                if (key != API_KEY)
                {
                    //403-Forbidden
                    var _resp = new GenericAPIResponse<string>()
                    {
                        Version = VERSION,
                        Message = "Request Forbidden",
                        Status = 403,
                        Content = "Request Forbidden"
                    };
                    return StatusCode(403, _resp);
                }
                RestResponse response = _NBAService.FetchRecord(season, teamOneId, teamTwoId);
                var _responseStatus = response.ResponseStatus;
                if (_responseStatus == ResponseStatus.Error)
                {
                    var _resp = new GenericAPIResponse<string>()
                    {
                        Version = VERSION,
                        Message = "Failed to Fetch",
                        Status = 500,
                        Content = "Failed to Fetch"
                    };
                    return StatusCode(500, _resp);
                }
                if (_responseStatus == ResponseStatus.TimedOut)
                {
                    var _resp = new GenericAPIResponse<string>()
                    {
                        Version = VERSION,
                        Message = "External API Timeout",
                        Status = 500,
                        Content = "External API Timeout"
                    };
                    return StatusCode(500, _resp);
                }
                string cache = response.Content ?? "";
                NBAGetResponse resp = new NBAGetResponse();
                try
                {
                    resp = Newtonsoft.Json.JsonConvert.DeserializeObject<NBAGetResponse>(cache) ?? new NBAGetResponse();
                }
                catch
                {
                    var _resp = new GenericAPIResponse<string>()
                    {
                        Version = VERSION,
                        Message = "Failed to Deserialize Response",
                        Status = 500,
                        Content = "Failed to Deserialize Response"
                    };
                    return StatusCode(500, _resp);
                }
                var _data = resp.Response.Select(r => new NBAReturnStats()
                {
                    Date = DateTime.Parse(r.Date.Start),
                    Home = r.Teams.Home.Code,
                    Visitor = r.Teams.Visitors.Code,
                    HomeScore = r.Scores.Home.Points ?? 0,
                    VisitorScore = r.Scores.Visitors.Points ?? 0
                }).ToList();
                var output = new GenericAPIResponse<NBAStatPayload>();
                output.Version = VERSION;
                output.Status = 200;
                output.Content = new NBAStatPayload() { Data = _data };
                if (_data.Count == 0)
                {
                    output.Message = "No Games Found";
                }
                else
                {
                    output.Message = $"{_data.Count} Games Found";
                }
                return Ok(output);
            }
            catch(Exception e)
            {
                var _resp = new GenericAPIResponse<string>()
                {
                    Version = VERSION,
                    Message = "Something went wrong",
                    Status = 500,
                    Content = e.Message
                };
                return StatusCode(500, _resp);
            }
        }


        public class NBAStatPayload
        {
            public List<NBAReturnStats> Data { get; set; }
        }

        public class NBAReturnStats
        {
            public DateTime Date { get; set; }
            public string Home { get; set; }
            public int HomeScore { get; set; }
            public string Visitor { get; set; }
            public int VisitorScore { get; set; }
        }

    }
}
