using Microsoft.AspNetCore.Mvc;
using System.Data;
using Npgsql;
using TaskTeamBackend.Services;
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

// CORS ayarlaması
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// PostgreSQL connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// IDbConnection servisini DI sistemine ekle
builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(connectionString));
builder.Services.AddScoped<PersonelService>();
builder.Services.AddScoped<ProjectService>();

// Controller’ları ekle
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS middleware (UseHttpsRedirection öncesi)
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();