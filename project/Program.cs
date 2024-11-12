using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserApi.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {});



builder.Services.AddControllers();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

var credentials = configuration["Database:Credentials"];

builder.Services.AddDbContext<UserDbContext>(options =>
options.UseMySql(credentials,
        new MySqlServerVersion(new Version(9, 1, 0))));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    options.DefaultChallengeScheme = IdentityConstants.ExternalScheme;
});

services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
    });
}



app.MapGet("/", async (HttpContext httpContext) =>
{
    httpContext.Response.ContentType = "text/html";

    if (httpContext.User.Identity?.IsAuthenticated == true)
    {
        var username = httpContext.User.Identity.Name;
        var logoutForm = "<form method='post' action='/api/account/logout'><button type='submit'>Wyloguj sie</button></form>";

        await httpContext.Response.WriteAsync($"Witaj, {username}! <br> {logoutForm}");
    }
    else
    {
        var RegisterLink = "<a href='/api/account/register'>Zarejestruj sie</a>";
        var LoginLink = "<a href='/api/account/login'>Zaloguj sie</a>";
        var GoogleLoginLink = "<a href='/api/account/ExternalLogin?provider=Google'>Zaloguj sie przez Google</a>";

        await httpContext.Response.WriteAsync($"Witaj! <br>");
        await httpContext.Response.WriteAsync($"{RegisterLink} <br>");
        await httpContext.Response.WriteAsync($"{LoginLink} <br>");
        await httpContext.Response.WriteAsync($"{GoogleLoginLink}");
    }
});


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
