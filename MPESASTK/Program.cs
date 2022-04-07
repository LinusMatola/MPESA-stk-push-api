using Microsoft.EntityFrameworkCore;
using MpesaLib;
using MPESASTK;
using MPESASTK.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// For Entity Framework  
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//for mpesa creds
builder.Services.Configure<mpesa_cred>(builder.Configuration.GetSection("Mpesa_cred"));


builder.Services.AddHttpClient<IMpesaClient, MpesaClient>(options => options.BaseAddress = RequestEndPoint.SandboxBaseAddress);
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

app.UseAuthorization();

app.MapControllers();

app.Run();
