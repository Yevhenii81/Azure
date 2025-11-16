using Microsoft.EntityFrameworkCore;
using MyAzureApiDbHW.Data;
using MyAzureApiDbHw.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Подключаем БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSql")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // теперь Swagger будет на /
    });

}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
