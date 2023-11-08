using AuthenticationManager.Interfaces;
using AuthenticationManager.Services;
using FilmForumModels.Entities;
using FilmForumModels.Models.Settings;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationManager.UnitTests.Services;

public class JwtServiceTests
{
    private readonly IJwtService _jwtService = new JwtService();

    public static IEnumerable<object[]> Roles()
    {
        yield return new object[] { null! };
        yield return new object[] { new List<string>() };
        yield return new object[] { new List<string>() { "User" } };
        yield return new object[] { new List<string>() { "User", "Admin" } };
    }

    [Theory]
    [MemberData(nameof(Roles))]
    public void GenerateToken_ForValidData_ReturnsToken(IEnumerable<string>? userRoles)
    {
        //Arrange
        User user = new()
        {
            Id = 1,
            Username = "TestUser",
            Email = "user@email.com",
            Password = "fdfsfsdsajd22321#@#!@#adsad#@#!Dasdaasdas2#@!",
        };
        IEnumerable<string>? roles = userRoles;
        JwtDetails options = new()
        {
            SecretKey = "SuperSecretKey11233233sADSA42432123DASDSAD323123DAD",
            Issuer = "FilmForum",
            Audience = "FilmForum",
            LifetimeInMinutes = 60
        };

        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = options.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Audience,
            RequireExpirationTime = true,
        };

        //Act
        string token = _jwtService.GenerateToken(user, roles, options);

        //Assert
        token.Should().NotBeNullOrEmpty();

        ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

        claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value.Should().Be(user.Id.ToString());
        claimsPrincipal.FindFirst(ClaimTypes.Name)!.Value.Should().Be(user.Username);
        claimsPrincipal.FindFirst(ClaimTypes.Email)!.Value.Should().Be(user.Email);

        //Roles can be null or empty for example when using [Authorize] attribute without roles
        IEnumerable<Claim> rolesFromToken = claimsPrincipal.FindAll(x => x.Type == ClaimTypes.Role);
        if (roles is null || !roles.Any())
        {
            rolesFromToken.Should().BeEmpty();
        }
        else
        {
            rolesFromToken.Select(x => x.Value).Should().BeEquivalentTo(roles);
        }
    }

    [Fact]
    public void GenerateToken_ForNullUser_ThrowsArgumentNullException()
    {
        //Act
        Action action = () => _jwtService.GenerateToken(null!, new List<string>(), new JwtDetails());

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenerateToken_ForNullJwtOptions_ThrowsArgumentNullException()
    {
        //Act
        Action action = () => _jwtService.GenerateToken(new User(), new List<string>(), null!);

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }
}