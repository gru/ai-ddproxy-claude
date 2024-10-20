using AI.DaDataProxy.Http.Contracts;
using AI.DaDataProxy.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace AI.DaDataProxy.Tests.Validators
{
    public class CreateCommandValidatorTests
    {
        private readonly CreateCommandValidator _validator = new();

        [Fact]
        public void Should_have_error_when_Name_is_null()
        {
            var command = new CreateDaDataCommand { Name = null! };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Should_have_error_when_Name_is_empty()
        {
            var command = new CreateDaDataCommand { Name = string.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Should_have_error_when_Name_is_too_short()
        {
            var command = new CreateDaDataCommand { Name = "A" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name must be between 5 and 150 characters.");
        }

        [Fact]
        public void Should_have_error_when_Name_is_too_long()
        {
            var command = new CreateDaDataCommand { Name = new string('A', 151) };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name must be between 5 and 150 characters.");
        }

        [Fact]
        public void Should_not_have_error_when_Name_is_valid()
        {
            var command = new CreateDaDataCommand { Name = "Valid Name" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}