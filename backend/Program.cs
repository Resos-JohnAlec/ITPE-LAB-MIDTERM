using System.Security.Cryptography;
using System.Text;
using CampusEvents;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5080");
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 16 * 1024);
var connectionString = builder.Configuration.GetConnectionString("CampusEvents")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:CampusEvents.");
var universityDomain = builder.Configuration["UniversityDomain"] ?? "dlsud.edu.ph";
builder.Services.AddSingleton<IUniversityDomainPolicy>(new UniversityDomainPolicy(universityDomain));
builder.Services.AddSingleton<EmailValidator>();
builder.Services.AddScoped(provider => new RegistrationService(connectionString, provider.GetRequiredService<EmailValidator>()));
var app = builder.Build();

if (args.Contains("--init-db"))
{
    await DatabaseBootstrap.InitializeAsync(connectionString, seed: true);
    Console.WriteLine("Application database initialized. Existing records were preserved.");
    return;
}

// Single origin and loopback binding keep classroom setup straightforward.
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self'; connect-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
    if (context.Request.Path.StartsWithSegments("/api"))
        context.Response.Headers.CacheControl = "no-store";
    try { await next(context); }
    catch (RegistrationException exception)
    {
        context.Response.StatusCode = exception.StatusCode;
        await context.Response.WriteAsJsonAsync(new { message = exception.Message });
    }
    catch (SqlException exception)
    {
        app.Logger.LogError("Database operation failed with SQL error {Number}.", exception.Number);
        context.Response.StatusCode = exception.Number is 2601 or 2627 ? 409 : 503;
        await context.Response.WriteAsJsonAsync(new { message = exception.Number is 2601 or 2627
            ? "This registration conflicts with an existing record. Refresh and check your details."
            : "The database is temporarily unavailable. Please try again shortly." });
    }
});

var files = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "wwwroot"));
app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = files });
app.UseStaticFiles(new StaticFileOptions { FileProvider = files });

app.MapGet("/api/config", () => Results.Ok(new { universityDomain }));
app.MapGet("/api/events", async (RegistrationService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetEventsAsync(false, cancellationToken)));
app.MapPost("/api/registrations", async (RegistrationRequest request, RegistrationService service, CancellationToken cancellationToken) =>
    Results.Json(await service.RegisterAsync(request, cancellationToken), statusCode: 201));

var admin = app.MapGroup("/api/admin");
admin.AddEndpointFilter(async (invocation, next) =>
{
    var key = app.Configuration["CAMPUS_ADMIN_KEY"];
    if (string.IsNullOrWhiteSpace(key))
        return Results.Json(new { message = "Administrator access is not configured. Set CAMPUS_ADMIN_KEY on the server." }, statusCode: 503);
    var supplied = invocation.HttpContext.Request.Headers["X-Admin-Key"].ToString();
    var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
    var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(supplied));
    if (!CryptographicOperations.FixedTimeEquals(expectedHash, suppliedHash))
        return Results.Json(new { message = "The administrator access key is incorrect." }, statusCode: 401);
    return await next(invocation);
});
admin.MapGet("/events", async (RegistrationService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetEventsAsync(true, cancellationToken)));
admin.MapGet("/events/{eventId:int}/attendees", async (int eventId, RegistrationService service, CancellationToken cancellationToken) =>
{
    var attendees = await service.GetAttendeesAsync(eventId, cancellationToken);
    return attendees is null ? Results.NotFound(new { message = "Event not found." }) : Results.Ok(attendees);
});

app.Run();
