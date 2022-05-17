
using EducationalAdministrationSysTem.API.Model.Context;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
// Add services to the container.
//ע��swagger
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "�������ϵͳ�ӿ��ĵ�",
        Version = "v1",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
        {
            Name = "EducationSystem",
            Email = "442534979@qq.com"
        }

    });
    s.OrderActionsBy(o => o.RelativePath);
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "EducationalAdministrationSysTem.API.xml");
    s.IncludeXmlComments(xmlPath);//����ע��
});

//ע��sqlsugar
builder.Services.AddSingleton(sp => new SqlSugarContext(
    new SqlSugarClient(new ConnectionConfig()
    {
        ConnectionString = builder.Configuration.GetConnectionString("DbConnectionString"),
        DbType = DbType.SqlServer,//���ݿ�����
        IsAutoCloseConnection = true,//�Զ��ͷ�
    })
    ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "EducationSystem v1"); });
app.UseAuthorization();//Ȩ����֤

app.MapControllers();//�����ʹ�ÿ�������ʱ���Ǳ�Ҫ�ģ���Ȼswagger����ִ�е��������еĽӿ�
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseRouting();//ʹ��·��
app.Run();