namespace NBAProcessingAPI.Models.Shared
{ 
    public class GenericAPIResponse<T>
    {
        public string Version { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public T Content { get; set; }
    }
}
