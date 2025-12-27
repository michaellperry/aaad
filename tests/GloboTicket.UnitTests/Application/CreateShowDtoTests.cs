using FluentAssertions;
using GloboTicket.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace GloboTicket.UnitTests.Application;

public class CreateShowDtoTests
{
    [Fact]
    public void GivenCreateShowDto_WhenShowGuidIsEmpty_ThenValidationFails()
    {
        // Arrange
        var dto = new CreateShowDto
        {
            ShowGuid = Guid.Empty,
            VenueGuid = Guid.NewGuid(),
            TicketCount = 100,
            StartTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(CreateShowDto.ShowGuid)));
    }

    [Fact]
    public void GivenCreateShowDto_WhenVenueGuidIsEmpty_ThenValidationFails()
    {
        // Arrange
        var dto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = Guid.Empty,
            TicketCount = 100,
            StartTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(CreateShowDto.VenueGuid)));
    }

    [Fact]
    public void GivenCreateShowDto_WhenTicketCountIsZero_ThenValidationFails()
    {
        // Arrange
        var dto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = Guid.NewGuid(),
            TicketCount = 0,
            StartTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle(vr =>
            vr.MemberNames.Contains(nameof(CreateShowDto.TicketCount)) &&
            vr.ErrorMessage!.Contains("must be at least 1"));
    }

    [Fact]
    public void GivenCreateShowDto_WhenTicketCountIsNegative_ThenValidationFails()
    {
        // Arrange
        var dto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = Guid.NewGuid(),
            TicketCount = -100,
            StartTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle(vr =>
            vr.MemberNames.Contains(nameof(CreateShowDto.TicketCount)) &&
            vr.ErrorMessage!.Contains("must be at least 1"));
    }

    [Fact]
    public void GivenCreateShowDto_WhenStartTimeIsDefault_ThenValidationFails()
    {
        // Arrange
        var dto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = Guid.NewGuid(),
            TicketCount = 100,
            StartTime = default
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(CreateShowDto.StartTime)));
    }

    [Fact]
    public void GivenCreateShowDto_WhenAllFieldsValid_ThenValidationPasses()
    {
        // Arrange
        var dto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = Guid.NewGuid(),
            TicketCount = 100,
            StartTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    private static IList<ValidationResult> ValidateDto(object dto)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto, null, null);
        Validator.TryValidateObject(dto, validationContext, validationResults, true);
        return validationResults;
    }
}
