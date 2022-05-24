
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EducationalAdministrationSystem.API.Common.ConvertHelper;
using EducationalAdministrationSystem.API.Common.Helper;
using EducationalAdministrationSystem.API.Common.redis;
using EducationalAdministrationSysTem.API.IRepository.Base;
using EducationalAdministrationSysTem.API.IServices.Base;
using EducationalAdministrationSysTem.API.Model.Context;
using EducationalAdministrationSysTem.API.Model.ViewModel;
using EducationalAdministrationSysTem.API.Services.Base;
using EducationalAdministrationSysTem.API.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

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
#region jwt Bearer��֤������֤

var _tokenParameter = AppSettings.app<tokenParameter>(new string[] { "JwtConfig" }).FirstOrDefault();//��ȡ��appsettings�����õ���Ϣ��ת��Ϊmodel


var key = Encoding.ASCII.GetBytes(_tokenParameter.Secret);//��ȡ��JWT���ܵ�Key,���ֵ�ĳ��Ȳ���̫�̣��������ִ���

//��װ������֤����
var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    RequireExpirationTime = false,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidationParams);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParams;
});
#endregion

#region redis����
var section = builder.Configuration.GetSection("Redis:Default");
//�����ַ���
string _connectionString = section.GetSection("Connection").Value;
//ʵ������
string _instanceName = section.GetSection("InstanceName").Value;
//Ĭ�����ݿ� 
int _defaultDB = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
builder.Services.AddSingleton(new RedisHelper(_connectionString, _instanceName, _defaultDB).GetDatabase());//����ģʽע��redis������
#endregion
 
#region ����������� 
string anyAllowSpecificOrigins = "any";//������� ���·���Ҫ����ʹ��
//�������
builder.Services.AddCors(options =>
{
    options.AddPolicy(anyAllowSpecificOrigins, corsbuilder =>
    {
        var corsPath = builder.Configuration.GetSection("CorsPaths").GetChildren().Select(p => p.Value).ToArray();
        corsbuilder.WithOrigins(corsPath)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();//ָ������cookie
    });
});
#endregion

builder.Services.AddHttpClient();

builder.Services.AddControllers(options =>
{
    //options.Filters.Add<ExtentionTool.LoginAuthorzation>();
    //options.Filters.Add<Filter.TokenAuthorizeAttribute>(); // ��������֤������ -- �˵�����Ȩ��
})  //ȫ������Json���л�����
.AddJsonOptions(options =>
{
    //��ʽ������ʱ���ʽ
    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());//�Զ���������ʽ��yyyy-MM-dd HH:mm:ss
                                                                              //���ݸ�ʽ����ĸСд
                                                                              //options.JsonSerializerOptions.PropertyNamingPolicy =JsonNamingPolicy.CamelCase;
                                                                               //���ݸ�ʽԭ�����
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    //ȡ��Unicode����
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    //���Կ�ֵ
    //options.JsonSerializerOptions.IgnoreNullValues = true;
    //����������
    options.JsonSerializerOptions.AllowTrailingCommas = true;
    //�����л����������������Ƿ�ʹ�ò����ִ�Сд�ıȽ�
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
}
);


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

app.UseCors(anyAllowSpecificOrigins);//֧�ֿ��������ض���Դ����������

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "EducationSystem v1"); });
app.UseAuthentication();//JWT
app.UseAuthorization();//Ȩ����֤

app.MapControllers();//�����ʹ�ÿ�������ʱ���Ǳ�Ҫ�ģ���Ȼswagger����ִ�е��������еĽӿ�
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseRouting();//ʹ��·��
app.Run();