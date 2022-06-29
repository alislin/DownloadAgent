using DownloadAgent;
using DownloadAgent.Models;

// 初始化资源
var init = new InitResource();
init.Init();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<DownloadConfig>(builder.Configuration.GetSection(DownloadConfig.Config));
builder.Services.AddSwaggerGen();
var allowOrigins = "MyPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowOrigins, policy =>
    {
        policy
        .AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod();
        //.WithMethods("*");
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();
app.UseDefaultFiles();
app.Urls.Add("http://localhost:36520");
app.UseCors(allowOrigins);
app.Run();
