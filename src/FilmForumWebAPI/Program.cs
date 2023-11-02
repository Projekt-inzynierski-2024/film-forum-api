using AuthenticationManager.Extensions;
using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Extensions;
using FilmForumWebAPI.Middlewares;
using FilmForumWebAPI.Services;
using FilmForumWebAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PasswordManager.Extensions;
using EmailSender.Extensions;
using Serilog;
using System.Runtime.InteropServices;

namespace FilmForumWebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        #region Configuration

        builder.Services.Configure<FilmForumMongoDatabaseSettings>(builder.Configuration.GetSection("FilmForumMongoDatabase"));
        builder.Services.Configure<JwtDetails>(builder.Configuration.GetSection("JwtDetails"));
        builder.Services.Configure<EmailSenderDetails>(builder.Configuration.GetSection("EmailSenderDetails"));

        #endregion Configuration

        #region Logging

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            string filePath = $"{Environment.GetEnvironmentVariable("HOME")}/Documents/projinz/logs/film-forum.log";
            builder.Host.UseSerilog((hostContext, services, configuration) =>
            {
                configuration.WriteTo.Console();
                configuration.WriteTo.File(filePath);
            });
        }
        else
        {
            builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
        }

        #endregion Logging

        #region Database

        builder.Services.AddDbContext<UsersDatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UsersDbConnection")));
        builder.Services.AddScoped<FilmsDatabaseContext>();

        #endregion Database

        #region Services

        builder.Services.AddPasswordManager();
        builder.Services.AddScoped<IActorService, ActorService>();
        builder.Services.AddScoped<IDirectorService, DirectorService>();
        builder.Services.AddScoped<IEpisodeService, EpisodeService>();
        builder.Services.AddScoped<IFilmService, FilmService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddEmailSender();
        #endregion Services

        #region Validators

        builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // register validators

        #endregion Validators

        builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.GetValidationErrorsMessagesAsString()));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "FilmForumWebAPI", Version = "v1" });
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme,
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme,
                        },
                    },
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddAuthorization();
        builder.Services.AddAuthenticationManager(builder.Configuration.GetSection("JwtDetails").Get<JwtDetails>()!);

        WebApplication app = builder.Build();

        app.UseMiddleware<RequestExceptionMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}