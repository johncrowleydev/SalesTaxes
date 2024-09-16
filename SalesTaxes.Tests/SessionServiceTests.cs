using Xunit;
using System;
using SalesTaxes.Common.DTOs;
using SalesTaxes.Services;

namespace SalesTaxes.Tests;

public class SessionServiceTests
{
    private readonly SessionService _sessionService;

    public SessionServiceTests()
    {
        _sessionService = new SessionService();
    }

    [Fact]
    public void GetCurrentUser_ShouldThrowException_WhenUserNotSet()
    {
        // Arrange

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _sessionService.GetCurrentUser());
        Assert.Equal("Current user is not set.", exception.Message);
    }

    [Fact]
    public void SetCurrentUser_ShouldStoreUserCorrectly()
    {
        // Arrange
        var user = new UserDTO("Alice", "California");

        // Act
        _sessionService.SetCurrentUser(user);
        var retrievedUser = _sessionService.GetCurrentUser();

        // Assert
        Assert.Equal(user.Id, retrievedUser.Id);
        Assert.Equal(user.Name, retrievedUser.Name);
        Assert.Equal(user.State, retrievedUser.State);
    }

    [Fact]
    public void SetCurrentUser_ShouldOverrideExistingUser()
    {
        // Arrange
        var initialUser = new UserDTO("Alice", "California");
        var newUser = new UserDTO("Bob", "Texas");

        // Act
        _sessionService.SetCurrentUser(initialUser);
        _sessionService.SetCurrentUser(newUser);
        var retrievedUser = _sessionService.GetCurrentUser();

        // Assert
        Assert.Equal(newUser.Id, retrievedUser.Id);
        Assert.Equal(newUser.Name, retrievedUser.Name);
        Assert.Equal(newUser.State, retrievedUser.State);
    }
}
