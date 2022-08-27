namespace NBAProcessingAPI.Models.NBAAPIResponse
{
    public class NBAStatus
    {
        public object Clock { get; set; }
        public bool? Halftime { get; set; }
        public int? Short { get; set; }
        public string Long { get; set; }
    }
}
