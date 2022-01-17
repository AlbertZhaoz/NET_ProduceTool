using AlbertZhao.cn.DbContextExtension;
using AlbertZhao.cn.Models;
using Albert.Commons;
using Albert.Commons.Interfaces;
using Albert.Commons.Helpers;

var builder = WebApplication.CreateBuilder(args);
var urls = new[] { "http://localhost:3000" }; //urls最后不要加/
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//依赖注入
builder.Services.AddFileService();
builder.Services.AddScanDir();

//内存缓存
builder.Services.AddMemoryCache();

//启用分布式缓存Redis
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost";
    options.InstanceName = "albertzhaoz_";
});


//启用分布式缓存Albert帮助类，更加方便的使用GetOrCreateAsync方法和类型限制
builder.Services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();

// CORS策略
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

//启用中间件 CORS策略：跨域策略
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

//app.UseResponseCaching(); //启用中间件 CORS策略：跨域策略

app.MapControllers();

app.Run();
