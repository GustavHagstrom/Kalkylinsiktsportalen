using Data.EfCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Data;

public static class RegisterExtension
{
    public static WebApplicationBuilder RegisterDataServices(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            if (builder.Configuration["DbType"] == "SqlServer")
            {
                var connectionString = builder.Configuration.GetConnectionString("SqlServer") ?? throw new InvalidOperationException("Connection string 'SqlServer' not found.");
                builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("AppPlatform.Server")));
            }
            else if (builder.Configuration["DbType"] == "MongoDb")
            {
                builder.Services.AddSingleton(sp =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var connectionString = configuration["MongoDbSettings:ConnectionString"];
                    var dbName = configuration["MongoDbSettings:DatabaseName"];
                    if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(connectionString))
                    {
                        throw new InvalidOperationException("MongoDB settings are not configured properly.");
                    }
                    var client = new MongoClient(connectionString);
                    return client.GetDatabase(dbName);
                });

            }
            else
            {
                throw new InvalidOperationException("DbType not found");
            }
        }
        else if (builder.Environment.IsDevelopment())
        {
            if (builder.Configuration["DbType"] == "SqlServer")
            {
                var connectionString = "Data Source=bin/debug/database.db";
                builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlite(connectionString, b => b.MigrationsAssembly("AppPlatform.Server")));
                //builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            }
            else if (builder.Configuration["DbType"] == "MongoDb")
            {
                builder.Services.AddSingleton(sp =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var connectionString = configuration["MongoDbSettings:ConnectionString"];
                    var dbName = configuration["MongoDbSettings:DatabaseName"];
                    if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(connectionString))
                    {
                        throw new InvalidOperationException("MongoDB settings are not configured properly.");
                    }
                    var client = new MongoClient(connectionString);
                    return client.GetDatabase(dbName);
                });

            }
            else
            {
                throw new InvalidOperationException("DbType not found");
            }
        }
            return builder;
    }
}
