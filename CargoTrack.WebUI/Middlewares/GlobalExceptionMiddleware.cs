using System.ComponentModel.DataAnnotations;

namespace CargoTrack.WebUI.Middlewares
{
    /// <summary>
    /// Global Exception Handling. Tüm isteklerde oluşan hataları yakalar, loglar ve
    /// kullanıcıya uygun bir hata sayfası/mesajı gösterir.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException vex)
            {
                // İş kuralı / doğrulama hatası: kullanıcıya mesaj gösterilir.
                _logger.LogWarning(vex, "İş kuralı hatası: {Message}", vex.Message);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var returnUrl = context.Request.Headers["Referer"].ToString();

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    context.Response.Redirect($"/Home/HandledError?message={Uri.EscapeDataString(vex.Message)}");
                    return;
                }

                await context.Response.WriteAsync(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Beklenmeyen hata oluştu.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.Redirect("/Home/Error");
            }
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
