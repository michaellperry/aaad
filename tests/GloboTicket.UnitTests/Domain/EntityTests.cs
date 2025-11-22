using FluentAssertions;
using GloboTicket.Domain.Entities;

namespace GloboTicket.UnitTests.Domain;

public class EntityTests
{
    // Concrete test implementation of Entity for testing
    private class TestEntity : Entity
    {
    }

    [Fact]
    public void Id_DefaultsToZero()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().Be(0);
    }

    [Fact]
    public void Id_CanBeSet()
    {
        // Arrange
        var entity = new TestEntity();
        var expectedId = 42;

        // Act
        entity.Id = expectedId;

        // Assert
        entity.Id.Should().Be(expectedId);
    }

    [Fact]
    public void CreatedAt_CanBeSet()
    {
        // Arrange
        var entity = new TestEntity();
        var expectedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        entity.CreatedAt = expectedDate;

        // Assert
        entity.CreatedAt.Should().Be(expectedDate);
    }

    [Fact]
    public void UpdatedAt_IsNullableAndDefaultsToNull()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void UpdatedAt_CanBeSet()
    {
        // Arrange
        var entity = new TestEntity();
        var expectedDate = new DateTime(2025, 1, 2, 14, 30, 0, DateTimeKind.Utc);

        // Act
        entity.UpdatedAt = expectedDate;

        // Assert
        entity.UpdatedAt.Should().Be(expectedDate);
    }

    [Fact]
    public void UpdatedAt_CanBeSetToNull()
    {
        // Arrange
        var entity = new TestEntity
        {
            UpdatedAt = new DateTime(2025, 1, 2, 14, 30, 0, DateTimeKind.Utc)
        };

        // Act
        entity.UpdatedAt = null;

        // Assert
        entity.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void InheritanceWorks_TestEntityIsEntity()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Should().BeAssignableTo<Entity>();
    }
}