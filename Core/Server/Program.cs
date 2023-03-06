using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Uamazing.SME.Server.Config;
using Uamazing.SME.Server.Utils.DotNetCoreSetup;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// ��� signalR
services.AddSignalR();

// ���� hyphen-case ·��
services.SetupSlugifyCaseRoute();

// ������
builder.MapConfiguration(new ConfigurationMapper());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
