namespace NBAProcessingAPI.Models.NBAAPIResponse
{
    public class NBATeamMatchup
    {
        public NBATeam? Visitors { get; set; }
        public NBATeam Home { get; set; }
    }
    public class NBATeam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Code { get; set; }
        public string Logo { get; set; }
    }
}
