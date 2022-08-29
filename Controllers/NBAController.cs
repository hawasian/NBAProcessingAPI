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
        public IActionResult Get(int season, int teamOneId, int teamTwoId, string? key)
        {
            //TODO IMP API KEY
            /*
                Codes to Consider
                400-Bad Request
                429-Too Many Requests
                500-Internal Server Error
             */
            if(string.IsNullOrEmpty(key))
            {
                //401-Unauthorized
                return Unauthorized();
            }
            if (key != API_KEY)
            {
                //403-Forbidden
                return StatusCode(403);
            }
            RestResponse response = _NBAService.FetchRecord(season, teamOneId, teamTwoId);
            var x = response.StatusCode;
            string cache = response.Content ?? "";
            NBAGetResponse resp = Newtonsoft.Json.JsonConvert.DeserializeObject<NBAGetResponse>(cache) ?? new NBAGetResponse();
            var _data = resp.Response.Select(r => new NBAReturnStats() { 
                Date = DateTime.Parse(r.Date.Start),
                Home = r.Teams.Home.Code,
                Visitor = r.Teams.Visitors.Code,
                HomeScore = r.Scores.Home.Points ?? 0,
                VisitorScore = r.Scores.Visitors.Points ?? 0
            }).ToList();
            var output = new GenericAPIResponse<NBAStatPayload>();
            output.Version = VERSION;
            output.Status = 200;
            output.Content = new NBAStatPayload() {Data = _data};
            if(_data.Count == 0)
            {
                output.Message = "No Games Found";
            }else
            {
                output.Message = $"{_data.Count} Games Found";
            }
            return Ok(output);
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
