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
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        #region Database

        builder.Services.AddDbContext<UsersDatabaseContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("UsersDbConnection"));
        });

        #endregion Database

        #region Services

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPasswordService, PasswordService>();

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

        var app = builder.Build();

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