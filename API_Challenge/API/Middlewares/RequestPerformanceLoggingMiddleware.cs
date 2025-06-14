using System.Diagnostics;

namespace API.Middlewares;

public class RequestPerformanceLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestPerformanceLoggingMiddleware> _logger;

    public RequestPerformanceLoggingMiddleware(RequestDelegate next, ILogger<RequestPerformanceLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var traceId = context.TraceIdentifier;
        var method = context.Request.Method;
        var path = context.Request.Path;

        await _next(context);

        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;
        var statusCode = context.Response.StatusCode;

        _logger.LogInformation(
            "Request {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms (TraceId={TraceId})",
            method, path, statusCode, elapsedMs, traceId
        );
    }
}
