using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>  
{
    options.AddPolicy("AllowAll", policy => 
    {
        policy.AllowAnyOrigin() 
            .AllowAnyMethod()  
            .AllowAnyHeader(); 
    });
});

builder.Services.AddControllers();  

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.MapControllers();
app.Run();