using Microsoft.EntityFrameworkCore;
using MouseJigglerBackend.BLL.Services;
using MouseJigglerBackend.Core.Interfaces;
using MouseJigglerBackend.DAL.Data;
using MouseJigglerBackend.DAL.Repositories;

namespace MouseJigglerBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();

        // Add Entity Framework
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add repositories
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IActivationKeyRepository, ActivationKeyRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        // Add services
        builder.Services.AddScoped<IActivationKeyService, ActivationKeyService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

        // Add CORS
        builder.Services.AddCors(options => {
            if (builder.Environment.IsDevelopment())
            {
                options.AddPolicy("AllowAll", policy => {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            }
            else
            {
                var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ?? new string[0];
                options.AddPolicy("Production", policy => {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            }
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();
        }
        else
        {
            // In production, only enable Swagger if explicitly configured
            var enableSwagger = builder.Configuration.GetValue<bool>("EnableSwagger", false);
            if (enableSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapOpenApi();
            }
        }
        
        app.UseHttpsRedirection();
        
        // Use appropriate CORS policy based on environment
        if (app.Environment.IsDevelopment())
        {
            app.UseCors("AllowAll");
        }
        else
        {
            app.UseCors("Production");
        }
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}