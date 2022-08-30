namespace NBAProcessingAPI.Models.ReturnModels
{
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
