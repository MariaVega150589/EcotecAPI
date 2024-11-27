using ProjectAPI.Application;
using ProjectAPI.Infrastructure;
using ProjectAPI.Infrastructure.Persistence;
using ProjectAPI.WebUI;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).WriteTo.Console().CreateLogger();
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddApplicationServices();

    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddWebUIServices(builder.Configuration);

    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    app.UseMigrationsEndPoint();

    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
    app.UseCors("AllowCors");
    app.UseHealthChecks("/health");
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseSwaggerUi3(settings =>
    {
        settings.Path = "/api";
        settings.DocumentPath = "/api/specification.json";


    });



    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");

    app.MapRazorPages();
    app.MapFallbackToFile("index.html"); ;

    app.Run();
}
catch (Exception ex)
{
    string filePath = "logInitError.txt"; // Path to the text file

    // Create a new text file or overwrite the existing one
    using (StreamWriter writer = new(filePath))
    {
        writer.WriteLine(ex.Message); // Write content to the file
        writer.WriteLine(ex.ToString()); // Write content to the file
    }
    throw;
}

// Make the implicit Program class public so test projects can access it
public partial class Program
{ }