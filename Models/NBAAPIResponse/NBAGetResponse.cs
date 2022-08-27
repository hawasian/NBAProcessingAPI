namespace NBAProcessingAPI.Models.NBAAPIResponse
{
    public class NBAGetResponse
    {
        public string Get { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public object[] Errors { get; set; }
        public int? Results { get; set; }
        public List<NBAGameRecord> Response { get; set; }
    }
    public class NBAGameRecord
    {
        public int? Id { get; set; }
        public string League { get; set; }
        public int? Season { get; set; }
        public NBAGameDate Date { get; set; }
        public int? Stage { get; set; }
        public NBAStatus Status { get; set; }
        public NBAPeriods Periods { get; set; }
        public NBAArena Arena { get; set; }
        public NBATeamMatchup Teams { get; set; }
        public NBAScores Scores { get; set; }
        public string[] Officials { get; set; }
        public int? TimesTied { get; set; }
        public int? LeadChanges { get; set; }
        public object Nugget { get; set; }
    }

}
