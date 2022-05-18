
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EducationalAdministrationSystem.API.Common.Helper;
using EducationalAdministrationSysTem.API.IRepository.Base;
using EducationalAdministrationSysTem.API.IServices.Base;
using EducationalAdministrationSysTem.API.Model.Context;
using EducationalAdministrationSysTem.API.Services.Base;
using EducationalAdministrationSysTem.API.Setup;
using SqlSugar;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
// Add services to the container.
//�������ļ�ע����з������ʹ��
builder.Services.AddSingleton(new EducationalAdministrationSystem.API.Common.Helper.AppSettings(builder.Configuration));

//ע��autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());//ע��autofac
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    //    builder.Register(c => new CallLogger()).Named<IInterceptor>("demotest");

    //builder.RegisterType<CallLogger>().AsSelf().InstancePerLifetimeScope();//ע��������

    var basePath = AppContext.BaseDirectory;
    var servicesDllFile = Path.Combine(basePath, "EducationalAdministrationSysTem.API.Services.dll");//��ȡע����Ŀ����·��

    var repositoryDllFile = Path.Combine(basePath, "EducationalAdministrationSysTem.API.IRepository.dll");
    containerBuilder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();//ע��ִ�
    var assemblysServices = Assembly.LoadFrom(servicesDllFile);//ֱ�Ӳ��ü����ļ��ķ���
    var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);//ֱ�Ӳ��ü����ļ��ķ���
    containerBuilder.RegisterAssemblyTypes(assemblysServices)
              .AsImplementedInterfaces()
              .InstancePerLifetimeScope()
              .PropertiesAutowired();
    //.EnableInterfaceInterceptors();//����������

    containerBuilder.RegisterAssemblyTypes(assemblysRepository)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .PropertiesAutowired();
    // .EnableInterfaceInterceptors();//��������;.InterceptedBy(typeof(CallLogger))


});




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

//ע��sqlsugar�����ļ�
builder.Services.AddSqlsugarSetup();


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