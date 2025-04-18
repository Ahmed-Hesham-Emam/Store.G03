using Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Presistence;
using Services;
using Shared.ErrorModels;
using Store.G03.API.Middlewares;
using System.Threading.Tasks;

namespace Store.G03.API.Extentions
    {
    public static class Extentions
        {
        public static IServiceCollection RegisterAllServices(this IServiceCollection services, IConfiguration configuration)
            {
            services.AddBuilInServices();
            services.AddSwaggerServices();


            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices();

            #region BadRequest Response
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(m => m.Value.Errors.Any())
                     .Select(m => new ValidationError
                         {
                         Field = m.Key,
                         Errors = m.Value.Errors.Select(e => e.ErrorMessage)
                         });

                    var response = new ValidationErrorResponse()
                        {
                        Errors = errors
                        };
                    return new BadRequestObjectResult(response);
                };
            });
            #endregion

            return services;
            }

        private static IServiceCollection AddBuilInServices(this IServiceCollection services)
            {
            services.AddControllers();
            return services;
            }


        private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
            {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
            }


        public static async Task<WebApplication> ConfigureMiddlewares(this WebApplication app)
            {

            await app.InitiateDataBaseAsync();
            app.UseGlobalErrorHandling();

            if ( app.Environment.IsDevelopment() )
                {
                app.UseSwagger();
                app.UseSwaggerUI();
                }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            return app;
            }

        private static async Task<WebApplication> InitiateDataBaseAsync(this WebApplication app)
            {

            #region Seeding
            using var scope = app.Services.CreateScope();
            var dbInit = scope.ServiceProvider.GetRequiredService<IDbInitialzer>();
            await dbInit.InitialzeAsync();
            #endregion

            return app;
            }

        private static WebApplication UseGlobalErrorHandling(this WebApplication app)
            {

            app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            return app;
            }


        }
    }
