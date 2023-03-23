using ETicaretAPI.API.Configurations.ColumsWriters;
using ETicaretAPI.API.Extensions;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Insfrastructure;
using ETicaretAPI.Insfrastructure.Filters;
using ETicaretAPI.Insfrastructure.Services.Storage.Azure;
using ETicaretAPI.Insfrastructure.Services.Storage.Local;
using ETicaretAPI.Persistence;
using ETicaretAPI.SignalR;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddAplicationServices();
builder.Services.AddSignalRServices();
//builder.Services.AddStorage<LocalStorage>();
builder.Services.AddStorage<AzureStorage>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
	policy.WithOrigins("http://localhost:4200","https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()
));
// Log Ayarlarý
Logger log = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("logs/log.txt")
	.WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"),"logs",needAutoCreateTable:true,
		columnOptions: new Dictionary<string, ColumnWriterBase>
		{
			{"message", new RenderedMessageColumnWriter() },
			{"message_template", new MessageTemplateColumnWriter() },
			{"level", new LevelColumnWriter() },
			{"time_stamp", new TimestampColumnWriter() },
			{"exception", new ExceptionColumnWriter() },
			{"log_event", new LogEventSerializedColumnWriter() },
			{"user_name", new UsernameColumnWriter() },
		})
	.WriteTo.Seq(builder.Configuration["Seq:ServerURL"])
	.Enrich.FromLogContext()
	.MinimumLevel.Information()
	.CreateLogger();
builder.Host.UseSerilog(log);

builder.Services.AddHttpLogging(logging =>
{
	logging.LoggingFields = HttpLoggingFields.All;
	logging.RequestHeaders.Add("sec-ch-ua");
	logging.MediaTypeOptions.AddText("application/javascript");
	logging.RequestBodyLogLimit = 4096;
	logging.ResponseBodyLogLimit = 4096;

});

builder.Services.AddControllers(options=>options.Filters.Add<ValidationFilter>())
	.AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>
	()).ConfigureApiBehaviorOptions(options=>options.SuppressModelStateInvalidFilter=true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer("Admin",options =>
	{
		options.TokenValidationParameters = new()
		{
			ValidateAudience=true,                //Oluþturalacak token deðerini kimlerin/hangi originlerin/sitelerin kullanýcý belirlediðimiz deðerdir.
			ValidateIssuer=true,                     //Oluþturalacak token deðerini kimin daðýttýðýný ifade edeceðimiz alandýr.
			ValidateLifetime=true,                 //Oluþturulan token deðerinin süresini kontrol edecek olan doðrulamadýr.
			ValidateIssuerSigningKey=true,  //Üretilecek token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden suciry key verisinin doðrulanmasýdýr.

			ValidAudience = builder.Configuration["Token:Audience"],
			ValidIssuer= builder.Configuration["Token:Issuer"],
			IssuerSigningKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
			LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

			NameClaimType = ClaimTypes.Name // JWT üzerinde Name claim'e karþýlýk gelen user elde ediyoruz. Identity.Name propertysinden elde edebiliriz.
		};
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

app.UseStaticFiles();

app.UseSerilogRequestLogging();
app.UseHttpLogging();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
	var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
	LogContext.PushProperty("user_name",username);
	await next();
});

app.MapControllers();
app.MapHubs();

app.Run();
