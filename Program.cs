using Microsis.CWM.AppService;
using Microsis.CWM.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

// todo : get app version from configuration
string appVer = "v1";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddCors(
    option => option.AddPolicy("CorsPolicy",
    builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
));

builder.Services.AddMvc(option => option.EnableEndpointRouting = false)
                .AddSessionStateTempDataProvider()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(30); options.Cookie.HttpOnly = true; });

builder.Services.AddSwaggerGen(option =>
        option.SwaggerDoc(appVer, new OpenApiInfo { Title = "Microsis Microservice CWM", Version = appVer })
    );

builder.Services.AddSingleton(builder);

builder.Services.AddControllersWithViews();


#region ______Context______
var connectionString = builder.Configuration.GetConnectionString("MicrosisCwmConn");
builder.Services.AddDbContext<CwmCtx>(x => x.UseSqlServer(connectionString, sqlServerOptionsAction: SqlOption =>
{
    SqlOption.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(5),
        errorNumbersToAdd: null
        );
}));
#endregion

#region ______Cache______
// builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = configuration["RedisCacheUrl"]; });
#endregion

#region ______Services Resolve_______
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICwmCtx, CwmCtx>();
builder.Services.AddScoped<IWholesaleAppService, WholesaleAppService>();
builder.Services.AddScoped<ISaleInformationAppService, SaleInformationAppService>();
builder.Services.AddScoped<IMediaAppService, MediaAppService>();
builder.Services.AddScoped<IMediaSettingsAppService, MediaSettingsAppService>();
builder.Services.AddScoped<IHelperAppService, HelperAppService>();
#endregion

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}
app.UseSwagger();

app.UseSwaggerUI();

app.UseMvc();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
