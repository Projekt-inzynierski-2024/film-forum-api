# FilmForumWebAPI

## C# WebAPI to create a film forum. 
It delivers full back-end API that can be combined with any front-end architecture.

## Main technologies and packages
* .NET 7 and C# 11
* BCrypt.Net-Next 4.0.3
* coverlet.collector 6.0.0
* Fluent Assertions 6.12.0
* Fluent Validation.AspNetCore 11.3.0
* MailKit 4.2.0
* Microsoft.AspNetCore.Authentication.JwtBearer 7.0.13
* Microsoft.AspNetCore.Authorization 7.0.13
* Microsoft.AspNetCore.OpenApi 7.0.13
* Microsoft.EntityFrameworkCore
* Microsoft.EntityFrameworkCore.Relational 7.0.13
* Microsoft.EntityFrameworkCore.SqlServer 7.0.13
* Microsoft.Extensions.DependencyInjection 7.0.0
* Microsoft.Extensions.DependencyInjection.Abstractions 7.0.0
* Microsoft.Extensions.Options 7.0.1
* Microsoft.NET.Test.Sdk 17.8.0
* MongoDB.Bson 2.22.0
* MongoDB.Driver 2.22.0
* MongoFramework 0.29.0
* Moq 4.20.69
* Serilog.AspNetCore 7.0.0
* Swashbuckle.AspNetCore 6.5.0
* System.IdentityModel.Tokens.Jwt 7.0.3
* xunit 2.6.1
* xunit.runner.visualstudio 2.6.1

Thanks for all authors of these helpful packages.

## Architecture
The application has been developed using layer architecture.

I have put the whole WebAPI into film-forum-api solution and divided it into src and tests directories.

I decided to put some features into libraries to make code more readable and easy to test.

![obraz](https://github.com/Projekt-inzynierski-2024/film-forum-api/assets/76125047/e69ade12-6ee7-46dd-b227-60f1adc616ec)

###### src
* AuthenticationManager - library responsible for managing and creating JWT to authorize users.
* EmailSender - library responsible for sending e-mails for example to retrieve token necessary to reset password.
* FilmForumModels - library to store Dtos, Entities and Models.
* FilmForumWebAPI - main feature of the application. It uses services for business logic, communicates with databases and deliver controllers.
* PasswordManager - library responsible for hashing and verifying users' passwords. It also delivers token to reset lost password.

###### tests
* AuthenticationManager.UnitTests - testing creating JWT keeping in mind roles of users.
* EmailSender.IntegrationTests - testing sending emails using chosen email provider and possible exceptions while sending the message.
* FilmForumWebAPI.IntegrationTests - testing services of WebAPI. It uses provided MS SQL Server and MongoDB databases. We created copies of original databases to perform integration tests.
* FilmForumWebAPI.UnitTests - testing behaviour of controllers and some helpful features included in WebAPI.
* PasswordManager.UnitTests - testing hashing, verifying of users' password and creating token to reset lost password.










