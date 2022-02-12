using System;

using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;

using Infrastructure.Shared.OperationResult;

using Xunit;

namespace Shared.InfrastructureTests
{
    public class OperationResultTests
    {
        private const int TestResultValue = 10;
        private const int TestErrorValue = 20;
        private const string TestErrorMessage = "TestErrorMessage";

        [Fact]
        public void OperationResultOk_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            OperationResult operationResult = OperationResult.Ok();

            operationResult.Success.Should().BeTrue();
            operationResult.Failure.Should().BeFalse();
            operationResult.Error.Should().BeNull();
        }

        [Fact]
        public void OperationResultFailWithMessage_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            var operationError = new OperationError(TestErrorMessage);
            OperationResult operationResult = OperationResult.Fail(operationError);

            operationResult.Success.Should().BeFalse();
            operationResult.Failure.Should().BeTrue();
            operationResult.Error.Should().NotBeNull();
            operationResult.Error.Message.Should().NotBeNullOrEmpty();
            operationResult.Error.Message.Should().BeEquivalentTo(TestErrorMessage);
            operationResult.Error.Exception.Should().BeNull();
        }

        [Fact]
        public void OperationResultFailWithException_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            var exception = new Exception(TestErrorMessage);
            var operationError = new OperationError(exception);
            OperationResult operationResult = OperationResult.Fail(operationError);

            operationResult.Success.Should().BeFalse();
            operationResult.Failure.Should().BeTrue();
            operationResult.Error.Should().NotBeNull();
            operationResult.Error.IsExceptionalError.Should().BeTrue();
            operationResult.Error.Message.Should().NotBeNullOrEmpty();
            operationResult.Error.Message.Should().BeEquivalentTo(TestErrorMessage);
            operationResult.Error.Exception.Should().NotBeNull();
            operationResult.Error.Exception.Should().BeEquivalentTo(exception);
        }

        [Fact]
        public void OperationResultWithValueOk_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            OperationResult<int> operationResult = OperationResult<int>.Ok(TestResultValue);

            operationResult.Success.Should().BeTrue();
            operationResult.Failure.Should().BeFalse();
            operationResult.Error.Should().BeNull();
            operationResult.Value.Should().Be(TestResultValue);
        }

        [Fact]
        public void OperationResultWithValueFailWithMessage_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            var operationError = new OperationError(TestErrorMessage);
            OperationResult<int> operationResult = OperationResult<int>.Fail(operationError);

            operationResult.Success.Should().BeFalse();
            operationResult.Failure.Should().BeTrue();
            operationResult.Value.Should().Be(expected: default);
            operationResult.Error.Should().NotBeNull();
            operationResult.Error.Message.Should().NotBeNullOrEmpty();
            operationResult.Error.Message.Should().BeEquivalentTo(TestErrorMessage);
            operationResult.Error.Exception.Should().BeNull();
        }

        [Fact]
        public void OperationResultWithValueFailWithException_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            var exception = new Exception(TestErrorMessage);

            var operationError = new OperationError(exception);
            OperationResult<int> operationResult = OperationResult<int>.Fail(operationError);

            operationResult.Success.Should().BeFalse();
            operationResult.Value.Should().Be(expected: default);
            operationResult.Failure.Should().BeTrue();
            operationResult.Error.Should().NotBeNull();
            operationResult.Error.IsExceptionalError.Should().BeTrue();
            operationResult.Error.Message.Should().NotBeNullOrEmpty();
            operationResult.Error.Message.Should().BeEquivalentTo(TestErrorMessage);
            operationResult.Error.Exception.Should().NotBeNull();
            operationResult.Error.Exception.Should().BeEquivalentTo(exception);
        }

        [Fact]
        public void OperationResultWithValuesOk_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            OperationResult<int, int> operationResult = OperationResult<int, int>.Ok(TestResultValue);

            operationResult.Success.Should().BeTrue();
            operationResult.Value.Should().Be(TestResultValue);
            operationResult.Failure.Should().BeFalse();
            operationResult.Error.Should().BeNull();
        }

        [Fact]
        public void OperationResultWithValuesFailWithMessage_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            var operationError = new OperationError<int>(TestErrorMessage, TestErrorValue);
            OperationResult<int, int> operationResult = OperationResult<int, int>.Fail(operationError);

            operationResult.Success.Should().BeFalse();
            operationResult.Value.Should().Be(expected: default);
            operationResult.Failure.Should().BeTrue();
            operationResult.Error.Should().NotBeNull();
            operationResult.Error.Value.Should().Be(TestErrorValue);
            operationResult.Error.Message.Should().NotBeNullOrEmpty();
            operationResult.Error.Message.Should().BeEquivalentTo(TestErrorMessage);
            operationResult.Error.Exception.Should().BeNull();
        }

        [Fact]
        public void OperationResultWithValuesFailWithException_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            var exception = new Exception(TestErrorMessage);
            var operationError = new OperationError<int>(exception, TestErrorValue);
            OperationResult<int, int> operationResult = OperationResult<int, int>.Fail(operationError);

            operationResult.Success.Should().BeFalse();
            operationResult.Value.Should().Be(expected: default);
            operationResult.Failure.Should().BeTrue();
            operationResult.Error.Should().NotBeNull();
            operationResult.Error.Value.Should().Be(TestErrorValue);
            operationResult.Error.IsExceptionalError.Should().BeTrue();
            operationResult.Error.Message.Should().NotBeNullOrEmpty();
            operationResult.Error.Message.Should().BeEquivalentTo(TestErrorMessage);
            operationResult.Error.Exception.Should().NotBeNull();
            operationResult.Error.Exception.Should().BeEquivalentTo(exception);
        }
    }
}
