using AoL_HCI_Backend.Config;
using AoL_HCI_Backend.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

//CORS Policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

//Configurations
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

//MongoDB Connection
builder.Services.AddSingleton<MongoDBServices>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoDBServices(settings.ConnectionString, settings.DatabaseName);
});

//Firebase Authentication
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("firebase.json")
});

//Authentication Service
builder.Services.AddSingleton<IAuthenticationServices, AuthenticationServices>();

//JWT Provider Service
builder.Services.AddHttpClient<IJwtProviderServices, JwtProviderServices>((serviceProvider, httpClient) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<TokenSettings>>().Value;
    httpClient.BaseAddress = new Uri(settings.TokenUri);
});
builder.Services.AddHttpClient<IJwtRefreshTokenServices, JwtRefreshTokenServices>((serviceProvider, httpClient) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<TokenSettings>>().Value;
    httpClient.BaseAddress = new Uri(settings.RefreshTokenUri);
});
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions => JwtValidator.ConfigureJwtOptions(jwtOptions, builder.Configuration));

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors();
// app.UseHttpsRedirection();

app.Run();