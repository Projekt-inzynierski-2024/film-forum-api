using FilmForumWebAPI.Models;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Extensions;
using FilmForumWebAPI.Services;
using FilmForumWebAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        #region Database

        builder.Services.AddDbContext<UsersDatabaseContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("UsersDbConnection"));
        });

        builder.Services.Configure<FilmForumMongoDatabaseSettings>(builder.Configuration.GetSection("FilmForumMongoDatabase"));
        builder.Services.AddSingleton<IFilmService, FilmService>();

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
        builder.Services.AddFluentValidationAutoValidation(); //auto validation

        #endregion Validators

        builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.GetValidationErrorsMessagesAsString());
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        WebApplication app = builder.Build();

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
