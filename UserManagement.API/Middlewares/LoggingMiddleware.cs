namespace UserManagement.API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "log.txt");

            if (!Directory.Exists(Path.GetDirectoryName(logFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            }

            var requestLog = $"{DateTime.UtcNow}: Request: {context.Request.Method} {context.Request.Path} {context.Request.Body}{Environment.NewLine}";
            await File.AppendAllTextAsync(logFilePath, requestLog);

            await _next(context);

            var responseLog = $"{DateTime.UtcNow}: Response: {context.Response.StatusCode}{context.Response.Body}{Environment.NewLine}";
            await File.AppendAllTextAsync(logFilePath, responseLog);
        }

    }
}
