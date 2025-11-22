using FluentAssertions;
using GloboTicket.API.Middleware;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;

namespace GloboTicket.UnitTests.API;

public class TenantContextTests
{
    [Fact]
    public void CurrentTenantId_WhenHttpContextIsNull_ReturnsNull()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns((HttpContext?)null);
        var tenantContext = new TenantContext(httpContextAccessor);

        // Act
        var result = tenantContext.CurrentTenantId;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CurrentTenantId_WhenUserIsNotAuthenticated_ReturnsNull()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // Not authenticated
        httpContextAccessor.HttpContext.Returns(httpContext);
        var tenantContext = new TenantContext(httpContextAccessor);

        // Act
        var result = tenantContext.CurrentTenantId;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CurrentTenantId_WhenUserIdentityIsNull_ReturnsNull()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(); // User with no identity
        httpContextAccessor.HttpContext.Returns(httpContext);
        var tenantContext = new TenantContext(httpContextAccessor);

        // Act
        var result = tenantContext.CurrentTenantId;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CurrentTenantId_WhenTenantIdClaimIsMissing_ReturnsNull()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "testuser")
        }, "TestAuthentication");
        httpContext.User = new ClaimsPrincipal(identity);
        httpContextAccessor.HttpContext.Returns(httpContext);
        var tenantContext = new TenantContext(httpContextAccessor);

        // Act
        var result = tenantContext.CurrentTenantId;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CurrentTenantId_WhenTenantIdClaimIsPresent_ReturnsCorrectValue()
    {
        // Arrange
        var expectedTenantId = 42;
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim("TenantId", expectedTenantId.ToString())
        }, "TestAuthentication");
        httpContext.User = new ClaimsPrincipal(identity);
        httpContextAccessor.HttpContext.Returns(httpContext);
        var tenantContext = new TenantContext(httpContextAccessor);

        // Act
        var result = tenantContext.CurrentTenantId;

        // Assert
        result.Should().Be(expectedTenantId);
    }

    [Fact]
    public void CurrentTenantId_WhenTenantIdClaimIsNotANumber_ReturnsNull()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim("TenantId", "not-a-number")
        }, "TestAuthentication");
        httpContext.User = new ClaimsPrincipal(identity);
        httpContextAccessor.HttpContext.Returns(httpContext);
        var tenantContext = new TenantContext(httpContextAccessor);

        // Act
        var result = tenantContext.CurrentTenantId;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CurrentTenantId_WhenTenantIdClaimIsEmpty_ReturnsNull()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim("TenantId", "")
        }, "TestAuthentication");
        httpContext.User = new ClaimsPrincipal(identity);
        httpContextAccessor.HttpContext.Returns(httpContext);
        var tenantContext = new TenantContext(httpContextAccessor);

        // Act
        var result = tenantContext.CurrentTenantId;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CurrentTenantId_WithMultipleCalls_ReturnsSameValue()
    {
        // Arrange
        var expectedTenantId = 123;
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("TenantId", expectedTenantId.ToString())
        }, "TestAuthentication");
        httpContext.User = new ClaimsPrincipal(identity);
        httpContextAccessor.HttpContext.Returns(httpContext);
        var tenantContext = new TenantContext(httpContextAccessor);

        // Act
        var result1 = tenantContext.CurrentTenantId;
        var result2 = tenantContext.CurrentTenantId;

        // Assert
        result1.Should().Be(expectedTenantId);
        result2.Should().Be(expectedTenantId);
    }
}