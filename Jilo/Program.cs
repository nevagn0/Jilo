using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Jilo.Models;
using Serilog;
using System.Net.Http.Headers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<JiloContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if(context.Request.Cookies.TryGetValue("jwt", out var token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };

    });

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Минимальный уровень логирования
    .WriteTo.File(
        path: "Logs/audit.log", // Путь к файлу логов
        rollingInterval: RollingInterval.Day, // Новый файл каждый день
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}" // Формат записи
    )
    .WriteTo.Console() // Дублирование в консоль (для отладки)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7136/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<JiloContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Games.Any())
    {
        var games = new List<Game>
        {
            new Game { Name = "Dota 2", Discrip = "MOBA-игра", Avatar = "/uploads/dota2" },
            new Game { Name = "CS:GO", Discrip = "Тактический шутер", Avatar = "/uploads/cs2" },
            new Game { Name = "Valorant", Discrip = "Командный шутер", Avatar = "/uploads/govnishe" },
            new Game { Name = "League of Legends", Discrip = "MOBA", Avatar = "/uploads/RareGovnishe" }
        };

        await dbContext.Games.AddRangeAsync(games);
        await dbContext.SaveChangesAsync();

        Console.WriteLine("Игры успешно добавлены в БД!");
    }
    else
    {
        Console.WriteLine("Игры уже есть в БД, пропускаем инициализацию.");
    }
}

app.UseStaticFiles();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
