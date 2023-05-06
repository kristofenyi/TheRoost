using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TheRoost.API.AppDbContext;
using TheRoost.Services;
using TheRoost.API.Middleware;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using TheRoost.API.Services;
using TheRoost.API.Configuration;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace TheRoost.API
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(o => 
                o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Host.UseSerilog();
            builder.Services.AddScoped<IAccommodationService, AccommodationService>();
            builder.Services.AddAutoMapper(typeof(MapperConfig));
            builder.Services.AddScoped<IUserService,UserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IManagerActionService, ManagerActionService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITranslateService, TranslateService>();
            builder.Services.AddScoped<AuthorizeFilter>();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<RoomService>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt_Issuer"],
                        ValidAudience = builder.Configuration["Jwt_Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt_Key"]))
                    };
                });

            builder.Services.AddSwaggerGen(c =>
            {
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "The Roost API",
                    Description = "Next generation booking of vacation stays",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Roost John",
                        Email = "john@theroost.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under OpenApiLicense",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
                });
            });

            builder.Services.AddDbContext<MainDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();
            var supportedCultures = new[] { "en-US", "cs", "sk" };
            var localizationOptions =
                new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                //Logging config - development - to logging file
                Log.Logger = new LoggerConfiguration()
                    .WriteTo
                    .File("../logs/log-.txt",
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }
            else
            {   // Logging config - production - to DB
                var co = new ColumnOptions { };
                co.Store.Remove(StandardColumn.MessageTemplate);
                Log.Logger = new LoggerConfiguration()
                    .WriteTo
                    .MSSqlServer(
                        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
                        sinkOptions: new MSSqlServerSinkOptions
                        {
                            TableName = "Logs",
                            AutoCreateSqlTable = true
                        },
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        formatProvider: null,
                        columnOptions: co,
                        logEventFormatter: null)
                    .CreateLogger();
            }
            app.UseSerilogRequestLogging();
            app.UseGlobalExceptionHandlerMiddleware();
            app.UseHttpsRedirection();
            app.UseMiddleware<ResponseFormatterMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
