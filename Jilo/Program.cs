using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Jilo.Models;
using System.Net.Http.Headers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
                context.Token = context.Request.Cookies["jwt"];
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7136/"); // Базовый адрес вашего API
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddControllersWithViews();
var app = builder.Build();
app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<JiloContext>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
