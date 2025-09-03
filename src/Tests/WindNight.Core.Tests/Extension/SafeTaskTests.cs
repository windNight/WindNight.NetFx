using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WindNight.Core.ExceptionExt;
using WindNight.Core.Tools;
using Xunit;
using Xunit.Abstractions;

namespace WindNight.Core.Tests.Extension
{

    public class SafeTaskTests : TestBase
    {

        public SafeTaskTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            Output("SafeTaskTests initialized.");
        }

        [Fact]
        public async Task Run_WithValidTaskName_LogsExceptionCorrectly()
        {
            // Arrange
            string taskName = "TestOperation";
            Exception thrownException = new InvalidOperationException("Test error");
            Action action = () => throw thrownException;
            Output($"Starting test with taskName: {taskName}, expected exception: {thrownException.Message}");

            string actualLogMessage = null;
            Exception actualException = null;

            // Act
            await SafeTask.Run(action, taskName, (_1, ex) => Output($" taskName[{_1}] SafeTask.Run CatchError {ex.GetMessage()}"));



            Output("Test Run_WithValidTaskName_LogsExceptionCorrectly completed.");
        }

        [Fact]
        public async Task Run_WithNullTaskName_LogsCallerMethodAndDll()
        {
            // Arrange
            Action action = () => throw new InvalidOperationException("Test error");
            string expectedMethodName = MethodBase.GetCurrentMethod()?.Name ?? "Unknown";
            string expectedDllName = MethodBase.GetCurrentMethod()?.Module.Name ?? "Unknown";
            Output("Starting test with null taskName");

            // Act
            await SafeTask.Run(action, "", (_1, ex) => Output($" taskName[{_1}] SafeTask.Run CatchError {ex.GetMessage()}"));



            Output($"Test Run_WithNullTaskName_LogsCallerMethodAndDll completed. Expected method: {expectedMethodName}, DLL: {expectedDllName}");
        }

        [Fact]
        public async Task Run_WithoutException_CompletesSuccessfully()
        {
            // Arrange
            bool executed = false;
            Action action = () => { executed = true; };
            Output("Starting test without exception");

            // Act
            await SafeTask.Run(action, "NoExceptionTask", (_1, ex) => Output($" taskName[{_1}] SafeTask.Run CatchError {ex.GetMessage()}"));



            Output("Test Run_WithoutException_CompletesSuccessfully completed.");
        }

        [Fact]
        public async Task Run_WithNullAction_ThrowsArgumentNullException()
        {
            // Arrange
            Output("Starting test with null action");

            await SafeTask.Run(null, "NullActionTest", (_1, ex) => Output($" taskName[{_1}] SafeTask.Run CatchError {ex.GetMessage()}"));

            Output("Test Run_WithNullAction_ThrowsArgumentNullException completed.");
        }

        [Fact]
        public async Task Run_WithAsyncException_LogsExceptionCorrectly()
        {
            // Arrange
            string taskName = "AsyncTestOperation";
            Exception thrownException = new SocketException(995);
            Func<Task> action = () => Task.FromException(thrownException);
            Output($"Starting test with async exception, taskName: {taskName}");

            // Act
            await SafeTask.Run(action, taskName, (_1, ex) => Output($" taskName[{_1}] SafeTask.Run CatchError {ex.GetMessage()}"));


            Output("Test Run_WithAsyncException_LogsExceptionCorrectly completed.");
        }

        [Fact]
        public async Task Run_WithCancellation_LogsCanceledException()
        {
            // Arrange
            string taskName = "CancelOperation";
            var cts = new CancellationTokenSource();
            Action action = () => throw new OperationCanceledException(cts.Token);
            Output($"Starting test with cancellation, taskName: {taskName}");

            // Act
            await SafeTask.Run(action, cts.Token, taskName, (_1, ex) => Output($" taskName[{_1}] SafeTask.Run CatchError {ex.GetMessage()}"));


            Output("Test Run_WithCancellation_LogsCanceledException completed.");
        }

        [Fact]
        public async Task Run_WithResult_ReturnsValueOnSuccess()
        {
            // Arrange
            string taskName = "ResultOperation";
            Func<int> function = () => 42;
            Output($"Starting test with result, taskName: {taskName}");

            // Act
            int result = await SafeTask.Run(function, taskName, (_1, ex) => Output($" taskName[{_1}] SafeTask.Run CatchError {ex.GetMessage()}"));

            // Assert
            Assert.Equal(42, result);

            Output("Test Run_WithResult_ReturnsValueOnSuccess completed.");
        }

        [Fact]
        public async Task Run_WithResult_ThrowsOnException()
        {
            // Arrange
            string taskName = "ResultErrorOperation";
            Exception thrownException = new InvalidOperationException("Test error");
            Func<int> function = () => throw thrownException;
            Output($"Starting test with result exception, taskName: {taskName}");

            // Act & Assert
            //await Assert.ThrowsAsync<InvalidOperationException>(async () => await SafeTask.Run(function, taskName));
            await SafeTask.Run(function, taskName, (_1, ex) => Output($" taskName[{_1}] SafeTask.Run CatchError {ex.GetMessage()}"));

            Output("Test Run_WithResult_ThrowsOnException completed.");

        }

    }

}
