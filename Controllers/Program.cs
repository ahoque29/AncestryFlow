using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Controllers;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Register services
        builder.Services.AddDbContext<DatabaseContext>(options => { options.UseSqlite("Data Source=Ancestry.db"); });
        builder.Services.AddScoped<IMemberService, MemberService>();
        builder.Services.AddSingleton<IDatabaseBackup, DatabaseBackup>();

        // Add controllers
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseRouting();
        app.MapControllers();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.Services.GetRequiredService<IDatabaseBackup>().BackupDatabase();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            context.InitializeDatabase().GetAwaiter().GetResult();
        }

        app.Run();
    }
}