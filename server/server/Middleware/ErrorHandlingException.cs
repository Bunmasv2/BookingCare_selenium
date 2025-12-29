namespace server.Middleware
{
    public class ErrorHandlingException : Exception
    {
        public int StatusCode { get; set; } = 500;
        public string ErrorMessage { get; set; }

        public ErrorHandlingException(string message)
        {
            ErrorMessage = message;
        }
        public ErrorHandlingException(int statusCode, string message)
        {
            StatusCode = statusCode;
            ErrorMessage = message;
        }
    }
}
