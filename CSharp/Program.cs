using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CSharpWebApiSample.Domain;
using CSharpWebApiSample.AppConfiguration;
using System.Reflection;
using CSharpWebApiSample.Services;
using Microsoft.OpenApi.Models;
using CSharpWebApiSample;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder();

var services = builder.Services;
var Configuration = builder.Configuration;s

// configure services
services.AddControllers();
services.AddDbContext<PersonsContext>();
services.AddScoped<PersonsRepository>();
services.Configure<DbConfiguration>(Configuration.GetSection("DbConfiguration"));
services.Configure<ApiClientsConfig>(Configuration.GetSection("ApiClients"));
services.AddAutoMapper(Assembly.GetExecutingAssembly());

//client
services.AddHttpClient<PetsApiClient>();

//needed for swagger
services.AddMvc();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

services.AddHostedService<PetsBackgroundJob>();

var env = builder.Environment;


// configure application
var app = builder.Build();

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();s
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", context => context.Response.WriteAsync("Welcome to C#!"));
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.Run();
