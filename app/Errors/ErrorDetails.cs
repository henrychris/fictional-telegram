namespace app.Errors
{
    public class ErrorDetails
    {
        public ErrorDetails(int statusCode, string message = null, string details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
        private int StatusCode { get; set; }
        private string Message { get; set; }
        private string Details { get; set; }
    }
}