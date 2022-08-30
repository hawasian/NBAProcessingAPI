using RestSharp;

namespace NBAProcessingAPI.Services
{
    public class NBAService
    {
        private readonly string SERVICE_HOST = "api-nba-v1.p.rapidapi.com";
        private readonly string SERVICE_KEY = "0bc50516e8msh6a510fc2b729d03p194455jsn5ebebdca214aXXX";
        
        private RestClient client;
        
        public NBAService()
        {
            client = new RestClient($"https://{SERVICE_HOST}/");
        }

        public RestResponse FetchRecord(int season, int teamOneId, int teamTwoId)
        {
            var request = new RestRequest($"games?season={season}&h2h={teamOneId}-{teamTwoId}");
            request.AddHeader("x-rapidapi-key", SERVICE_KEY);
            request.AddHeader("x-rapidapi-host", SERVICE_HOST);
            RestResponse response = client.Execute(request);
            return response;
        }
    }
}
