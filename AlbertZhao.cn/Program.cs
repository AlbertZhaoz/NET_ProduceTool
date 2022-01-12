using AlbertZhao.cn.DbContextExtension;
using AlbertZhao.cn.Models;

var builder = WebApplication.CreateBuilder(args);
var urls = new[] { "http://localhost:3000" }; //urls���Ҫ��/
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//����ע��
builder.Services.AddFileService();
builder.Services.AddScanDir();

//启用分布式缓存Redis
builder.Services.AddStackExchangeRedisCache(options=>{
    options.Configuration = "localhost";
    options.InstanceName="albertzhaoz_";
});

//�ڴ滺��
builder.Services.AddMemoryCache();

// CORS����
builder.Services.AddCors(options =>
   options.AddDefaultPolicy(builder =>
       builder.WithOrigins(urls).AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//�����м�� CORS���ԣ��������
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

//app.UseResponseCaching(); //��������Ӧ���棬��UseCors֮����MapControllers֮ǰ

app.MapControllers();

app.Run();
