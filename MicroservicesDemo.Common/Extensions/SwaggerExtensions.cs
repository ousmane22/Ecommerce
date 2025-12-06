using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Ecommerce.Common.Extensions;

public static class SwaggerExtensions
{
    /// <summary>
    /// Configure Swagger de manière centralisée
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services,
        string title,
        string version = "v1",
        string description = "")
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = description,
                Contact = new OpenApiContact
                {
                    Name = "Microservices Demo",
                    Email = "support@microservicesdemo.com"
                }
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    /// <summary>
    /// Configure Swagger UI de manière centralisée
    /// </summary>
    public static WebApplication UseSwaggerDocumentation(
        this WebApplication app,
        string title,
        string version = "v1",
        bool routePrefixEmpty = false)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{title} {version}");
                if (routePrefixEmpty)
                {
                    c.RoutePrefix = string.Empty;
                }
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableFilter();
            });
        }

        return app;
    }
}

