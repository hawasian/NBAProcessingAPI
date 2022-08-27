namespace NBAProcessingAPI.Models.NBAAPIResponse
{
    public class NBAScores
    {
        public NBAScoreCard Visitors { get; set; }
        public NBAScoreCard Home { get; set; }
    }
    public class NBAScoreCard
    {
        public int? Win { get; set; }
        public int? Loss { get; set; }
        public NBASeries Series { get; set; }
        public int[] LineScore { get; set; }
        public int? Points { get; set; }
    }
    public class NBASeries
    {
        public int? Win { get; set; }
        public int? Loss { get; set; }
    }
}
