using FluentAssertions;

namespace HomeBuyerHelper.Data.Tests;

/// <summary>
/// Tests for the DatabaseService.
/// </summary>
public class DatabaseServiceTests
{
    [Fact]
    public void DatabasePath_IsNotEmpty()
    {
        // Arrange
        var sut = new DatabaseService();

        // Act & Assert
        sut.DatabasePath.Should().NotBeNullOrEmpty();
        sut.DatabasePath.Should().EndWith(".db");
    }

    [Fact]
    public void DatabasePath_ContainsAppName()
    {
        // Arrange
        var sut = new DatabaseService();

        // Act & Assert
        sut.DatabasePath.Should().Contain("HomeBuyerHelper");
    }
}
