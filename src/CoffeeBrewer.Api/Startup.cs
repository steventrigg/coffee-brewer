using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using CoffeeBrewer.Adaptors.Data;
using CoffeeBrewer.Adaptors.Weather;
using CoffeeBrewer.App;
using CoffeeBrewer.App.Coffee.Models;
using CoffeeBrewer.App.Coffee.Policies;
using CoffeeBrewer.App.Coffee.Queries;
using CoffeeBrewer.App.Coffee.Validators;
using MediatR;

namespace CoffeeBrewer.Api;

public class Startup
{
    private readonly IWebHostEnvironment _env;
    private readonly string _hopperLevelTableName;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;

        _hopperLevelTableName = Environment.GetEnvironmentVariable("HopperLevelTableName") ?? string.Empty;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BrewCoffeeQuery).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IValidator<>), typeof(AprilFoolsValidator<>));
        services.AddTransient(typeof(IValidator<>), typeof(HopperEmptyValidator<>));
        services.AddTransient(typeof(ITempPolicy<>), typeof(TempPolicy<>));

        services.AddTransient(typeof(IOpenWeatherService), typeof(OpenWeatherService));

        services.AddControllers();
        services.AddHttpClient();

        services.AddDefaultAWSOptions(new AWSOptions{ Region = Amazon.RegionEndpoint.USWest2 });
        services.AddAWSService<IAmazonDynamoDB>();

        // Quick and dirty alternative to running docker
        //if (_env.IsDevelopment())
        //{
            // I could not get the lambda to connect to dynamo db, so restorting to just using this in-memory store.
            services.AddTransient(typeof(IHopperLevelRepository), typeof(LocalHopperLevelRepository));
        //}
        //else
        //{
        //    services.AddTransient<IHopperLevelRepository>(p => new DynDbHopperLevelRepository
        //    (
        //        p.GetRequiredService<IAmazonDynamoDB>(),
        //        _hopperLevelTableName,
        //        p.GetRequiredService<ILogger<DynDbHopperLevelRepository>>()
        //    ));
        //}
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("OK");
            });
        });
    }
}