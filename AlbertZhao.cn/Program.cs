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

//app.UseResponseCaching(); //��UseCors֮����MapControllers֮ǰ

app.MapControllers();

app.Run();
