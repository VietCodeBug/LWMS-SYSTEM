using FluentValidation.TestHelper;
using LWMS.Application.Parcels.Commands.Create;
using Xunit;

namespace LWMS.Application.UnitTests.Parcels;

public class CreateParcelCommandValidatorTests
{
    private readonly CreateParcelCommandValidator _validator;

    public CreateParcelCommandValidatorTests()
    {
        _validator = new CreateParcelCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_SenderPhone_Is_Invalid()
    {
        var command = new CreateParcelCommand { SenderPhone = "abc" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SenderPhone);
    }

    [Fact]
    public void Should_Not_Have_Error_When_SenderPhone_Is_Valid()
    {
        var command = new CreateParcelCommand { SenderPhone = "0901234567" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.SenderPhone);
    }

    [Fact]
    public void Should_Have_Error_When_Weight_Is_Zero()
    {
        var command = new CreateParcelCommand { Weight = 0 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Weight);
    }
}
