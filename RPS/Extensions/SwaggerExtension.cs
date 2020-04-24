using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace RPS.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddApiDoc(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "RPS.BACKEND",
                        Version = "v1",
                        Description = "Backend RPS",
                        Contact = new OpenApiContact
                        {
                            Name = "Yan Pitangui",
                            Url = new System.Uri("https://github.com/yanpitangui")
                        }
                    });
                c.DescribeAllParametersInCamelCase();
                c.OrderActionsBy(x => x.RelativePath);
                var schema = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", schema);


                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(schema, new[] { "Bearer" });
                c.AddSecurityRequirement(securityRequirement);

                var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlfile);
                //c.IncludeXmlComments(xmlPath);

            });
            return services;
        }

        public static IApplicationBuilder UseApiDoc(this IApplicationBuilder app)
        {
            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.RoutePrefix = "api-docs";
                   c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"v1");
                   c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
               });
            return app;
        }
    }
}
