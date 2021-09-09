namespace app.Errors
{
    public class EpumpError
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public string statusDescription { get; set; }
    }
}