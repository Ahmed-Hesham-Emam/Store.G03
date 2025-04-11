
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Presistence;
using Presistence.Data;
using Services;
using Services.Abstractions;
using System.Threading.Tasks;

namespace Store.G03.API
    {
    public class Program
        {
        public static async Task Main(string[] args)
            {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region DB Service

            builder.Services.AddDbContext<StoreDbContext>(options =>
                    {
                        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                    });

            #endregion

            builder.Services.AddScoped<IDbInitialzer, DbInitialzer>(); // Allowing DI for the DbInitialzer
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Allowing DI for the UnitOfWork
            builder.Services.AddAutoMapper(typeof(ServiceAssemplyReference).Assembly); // Allowing DI for the AutoMapper
            builder.Services.AddScoped<IServiceManager, ServiceManager>(); // Allowing DI for the ServiceManager

            var app = builder.Build();

            #region Seeding

            using var scope = app.Services.CreateScope();
            var dbInit = scope.ServiceProvider.GetRequiredService<IDbInitialzer>();
            await dbInit.InitialzeAsync();

            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
                {
                app.UseSwagger();
                app.UseSwaggerUI();
                }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            }
        }
    }
