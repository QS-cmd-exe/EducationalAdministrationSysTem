using EducationalAdministrationSystem.API.Common.Helper;
using EducationalAdministrationSystem.CreateTableAPI.Setup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddMvc();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddSqlsugarSetup();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();//�����ʹ�ÿ�������ʱ���Ǳ�Ҫ�ģ���Ȼswagger����ִ�е��������еĽӿ�
app.UseStatusCodePages();


app.Run();
