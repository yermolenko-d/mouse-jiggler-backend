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
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // Replace environment variables in connection string if they exist
            if (!string.IsNullOrEmpty(connectionString))
            {
                connectionString = connectionString
                    .Replace("${DB_HOST}", Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost")
                    .Replace("${DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT") ?? "5432")
                    .Replace("${DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME") ?? "jiggler_prod")
                    .Replace("${DB_USER}", Environment.GetEnvironmentVariable("DB_USER") ?? "dev")
                    .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "");
                Console.WriteLine($"Final connection string: {connectionString}");
            }
            options.UseNpgsql(connectionString);
        });
        // Add repositories
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IActivationKeyRepository, ActivationKeyRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        // Add services
        builder.Services.AddScoped<IActivationKeyService, ActivationKeyService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IPasswordService, PasswordService>();

        // Add CORS
        builder.Services.AddCors(options => {
            if (builder.Environment.IsDevelopment())
            {
                options.AddPolicy("AllowAll", policy => {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            } else {
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
        var enableSwagger = builder.Configuration.GetValue("EnableSwagger", false);
        if (app.Environment.IsDevelopment() || enableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();
        }
        
        app.UseHttpsRedirection();
        
        // Use appropriate CORS policy based on environment
        if (app.Environment.IsDevelopment())
        {
            app.UseCors("AllowAll");
        } else {
            app.UseCors("Production");
        }
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}