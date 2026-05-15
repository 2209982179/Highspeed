using highspeed.api;
using highspeed.api.Filter;
using highspeed.api.WebsocketHandler;
using highspeed.business._01.Models;
using highspeed.framework.Common;
using Microsoft.AspNetCore.Http.Features;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var settingJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
var settingJson_env = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings.{builder.Environment.EnvironmentName}.json");
// 加载自定义的 appsettings.json 文件
if (File.Exists(settingJson)) builder.Configuration.AddJsonFile(settingJson, optional: false, reloadOnChange: true);
// 加载环境特定的配置文件（如果需要）
if (File.Exists(settingJson_env)) builder.Configuration.AddJsonFile(settingJson_env, optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});
AppConfiguration.FaultTreeBaseUrl = builder.Configuration["FaultTreeBaseUrl"];
AppConfiguration.EndPoints = builder.Configuration["EndPoints"];
AppConfiguration.AuthMultiEndPoint = builder.Configuration.GetValue<bool?>("AuthMultiEndPoint") == true;
AppConfiguration.AuthMultiUser = builder.Configuration.GetValue<bool?>("AuthMultiUser") == true;

// 文件上传的最大限制
builder.Services.Configure<FormOptions>(x =>
{
    x.MultipartBodyLengthLimit = 209715200; // 200MB限制
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 209715200; // 200MB限制
});

#region 跨域设置

builder.Services.AddCors(options =>
{
    // 允许所有跨域访问
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "myCors", x =>
        //x.AllowAnyOrigin()
        x.SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
    );
});

#endregion 跨域设置

#region 注册过滤器，设置Json转换

builder.Services
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddControllers(options =>
    {
        options.Filters.Add<ApiResourceFilter>();
        options.Filters.Add<ApiActionFilter>();
        options.Filters.Add<ApiExceptionFilter>();
        options.Filters.Add<ApiResultFilter>();
    })
    .AddNewtonsoftJson(options =>
    {
        //数据格式首字母小写 不使用驼峰   小驼峰firstName  大驼峰 FirstName
        //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        //使用默认方式，不更改元数据的key的大小写
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        // 忽略循环引用
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        // 设置时间格式
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
        //忽略空值 不包含属性的null序列化
        options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
        //忽略默认值，不包含属性默认值
        options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
    });

#endregion 注册过滤器，设置Json转换
 
GlobalContext.BizConfigurations.Clear();
GlobalContext.BizConfigurations.Add("MongoDBConnName", builder.Configuration.GetRequiredSection("MongoDB:ConnName").Value);
GlobalContext.BizConfigurations.Add("MongoDBDatabaseName", builder.Configuration.GetRequiredSection("MongoDB:DatabaseName").Value);
GlobalContext.InitialMongoDB();
GlobalContext.ConnectionStrings.Clear();
var conns = builder.Configuration.GetRequiredSection("ConnectionStrings").GetChildren();
foreach (var conn in conns)
{
    GlobalContext.ConnectionStrings.Add(conn.Key, conn.Value);
}
GlobalContext.Setup();

#region APP 创建
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    // 静态资源访问
    app.UseStaticFiles();
}

// 应用跨域设置
app.UseCors("myCors");

// 开启认证
app.UseAuthentication();
// 开启授权
app.UseAuthorization();

// 加载Controller
app.MapControllers();
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "highspeed.api v1"));
}

// WebSocket 
app.Map("/Common/GetTestData", WebsocketHandler.Map);

// 启动用户认证
app.UseAuthorization();

app.Run();
#endregion