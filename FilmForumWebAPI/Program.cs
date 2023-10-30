using FilmForumWebAPI.Database;
using FilmForumWebAPI.Extensions;
using FilmForumWebAPI.Middlewares;
using FilmForumWebAPI.Models;
using FilmForumWebAPI.Services;
using FilmForumWebAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        builder.Services.AddScoped<IActorService, ActorService>();
        builder.Services.AddScoped<IDirectorService, DirectorService>();
        builder.Services.AddScoped<IEpisodeService, EpisodeService>();
        builder.Services.AddScoped<IFilmService, FilmService>();
        builder.Services.AddScoped<IPasswordService, PasswordService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IUserService, UserService>();

        #endregion Services

        #region Validators

        builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // register validators

        #endregion Validators

        builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.GetValidationErrorsMessagesAsString()));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        WebApplication app = builder.Build();

        app.UseMiddleware<RequestExceptionMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}