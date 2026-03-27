using System.Text;
using System.Text.Json.Serialization;
using Lumenform.Infrastructure;
using Lumenform.Infrastructure.Persistence;
using LumenformCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - BEFORE builder.Build()
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // Your Next.js dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(/*...*/)
        .LogTo(Console.WriteLine, LogLevel.Information)  // ← Add this
        .EnableSensitiveDataLogging());  // ← And this for debugging

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://dkjpagysqlhrghfxzapl.supabase.co/auth/v1";
        options.Audience = "authenticated";
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
    });

//builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// Register policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Owner", policy =>
        policy.Requirements.Add(new CohortRoleRequirement(RequiredCohortRole.Owner)));
    
    options.AddPolicy("Coordinator", policy =>
        policy.Requirements.Add(new CohortRoleRequirement(RequiredCohortRole.Coordinator)));
    
    options.AddPolicy("Member", policy =>
        policy.Requirements.Add(new CohortRoleRequirement(RequiredCohortRole.Member)));
});

builder.Services.AddScoped<IAuthorizationHandler, CohortRoleHandler>();


// Now build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();

app.UseCors("AllowFrontend"); 

app.UseAuthentication();  // ← Add this BEFORE authorization
app.UseAuthorization();   // ← Add this

// Map controllers
app.MapControllers();

app.Run();