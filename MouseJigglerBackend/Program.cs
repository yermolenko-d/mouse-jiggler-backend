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
            options.AddPolicy("AllowAll", policy => {
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            
        }
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapOpenApi();
        
        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}