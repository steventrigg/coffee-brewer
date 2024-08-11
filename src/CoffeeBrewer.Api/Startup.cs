using CoffeeBrewer.Adaptors.Data;
using CoffeeBrewer.App;
using CoffeeBrewer.App.Coffee.Queries;
using CoffeeBrewer.App.Coffee.Validators;
using MediatR;

namespace CoffeeBrewer.Api;

public class Startup
{
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BrewCoffeeQuery).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IValidator<>), typeof(AprilFoolsValidator<>));
        services.AddTransient(typeof(IValidator<>), typeof(HopperEmptyValidator<>));

        services.AddControllers();

        //if (_env.IsDevelopment())
        //{
        services.AddTransient(typeof(IHopperLevelRepository), typeof(LocalHopperLevelRepository));
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