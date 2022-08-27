using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NBAProcessingAPI.Models.NBAAPIResponse;
using RestSharp;

namespace NBAProcessingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NBAController : ControllerBase
    {
        [HttpGet]
        public void Get(int season, int teamOneId, int teamTwoId)
        {
            var client = new RestClient($"https://api-nba-v1.p.rapidapi.com/games?season={season}&h2h={teamOneId}-{teamTwoId}");
            var request = new RestRequest();
            request.AddHeader("x-rapidapi-key", "0bc50516e8msh6a510fc2b729d03p194455jsn5ebebdca214a");
            request.AddHeader("x-rapidapi-host", "api-nba-v1.p.rapidapi.com");
            RestResponse response = client.Execute(request);
            if(Response.StatusCode != 200)
            {
                //No Response from NBA
            }
            var cache = response.Content;
            var resp = Newtonsoft.Json.JsonConvert.DeserializeObject<NBAGetResponse>(cache);
            //var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NBAGameRecord>>(resp.Response.ToString());
            Console.WriteLine(response.Content);
        }
        
    }
}
