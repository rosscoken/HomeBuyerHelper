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

    [Fact]
    public void DatabasePath_IsAbsolutePath()
    {
        // Arrange
        var sut = new DatabaseService();

        // Act & Assert
        Path.IsPathRooted(sut.DatabasePath).Should().BeTrue();
    }

    [Fact]
    public void DatabasePath_HasCorrectFilename()
    {
        // Arrange
        var sut = new DatabaseService();

        // Act
        var filename = Path.GetFileName(sut.DatabasePath);

        // Assert
        filename.Should().Be("homebuyerhelper.db");
    }

    [Fact]
    public void DatabasePath_IsInLocalApplicationData()
    {
        // Arrange
        var sut = new DatabaseService();
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // Act & Assert
        sut.DatabasePath.Should().StartWith(localAppData);
    }

    [Fact]
    public void MultipleCalls_ReturnsSamePath()
    {
        // Arrange
        var sut1 = new DatabaseService();
        var sut2 = new DatabaseService();

        // Act & Assert
        sut1.DatabasePath.Should().Be(sut2.DatabasePath);
    }
}
