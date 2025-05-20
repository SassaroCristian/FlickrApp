using System.Diagnostics;
using FlickrApp.Services;

namespace xUnitTestProject.Services;

public class GlobalErrorHandlerTests : IDisposable
{
    private readonly TestTraceListener _testListener;
    private readonly GlobalErrorHandler _sut;

    private class TestTraceListener : TraceListener
    {
        public List<string> Messages { get; } = new();

        public override void Write(string? message)
        {
        }

        public override void WriteLine(string? message)
        {
            if (message != null) Messages.Add(message);
        }
    }

    public GlobalErrorHandlerTests()
    {
        _testListener = new TestTraceListener();
        Trace.Listeners.Add(_testListener);
        _sut = new GlobalErrorHandler();
    }

    public void Dispose()
    {
        Trace.Listeners.Remove(_testListener);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void HandlerException_WithStandardException_FormatsAndLogsCorrectly()
    {
        const string testExceptionMessage = "Test error";
        const string testPlatformContext = "TestPlatform_iOS";
        const bool testIsTerminating = false;
        Exception? testException;
        try
        {
            throw new InvalidOperationException(testExceptionMessage);
        }
        catch (Exception? ex)
        {
            testException = ex;
        }

        var expectedFormattedMessage =
            $"GLOBAL ERROR CAUGHT\n" +
            $"Platform: {testPlatformContext}\n" +
            $"App is terminating: {testIsTerminating}\n" +
            $"Exception Type: {testException.GetType().FullName}\n" +
            $"Message: {testExceptionMessage}\n" +
            $"Stack Trace:\n{testException.StackTrace}";

        _sut.HandlerException(testException, testPlatformContext, testIsTerminating);

        Assert.Equal(3, _testListener.Messages.Count);
        Assert.Equal("**************************************************", _testListener.Messages[0]);
        Assert.Equal(expectedFormattedMessage, _testListener.Messages[1]);
        Assert.Equal("**************************************************", _testListener.Messages[2]);
    }

    [Fact]
    public void HandlerException_WithIsTerminatingTrue_FormatsAndLogsCorrectly()
    {
        const string testExceptionMessage = "Fatal error";
        const string testPlatformContext = "TestPlatform_Android";
        const bool testIsTerminating = true;
        Exception? testException;
        try
        {
            throw new ApplicationException(testExceptionMessage);
        }
        catch (Exception? ex)
        {
            testException = ex;
        }

        var expectedFormattedMessage =
            $"GLOBAL ERROR CAUGHT\n" +
            $"Platform: {testPlatformContext}\n" +
            $"App is terminating: {testIsTerminating}\n" +
            $"Exception Type: {testException.GetType().FullName}\n" +
            $"Message: {testExceptionMessage}\n" +
            $"Stack Trace:\n{testException.StackTrace}";

        _sut.HandlerException(testException, testPlatformContext, testIsTerminating);

        Assert.Equal(3, _testListener.Messages.Count);
        Assert.Equal(expectedFormattedMessage, _testListener.Messages[1]);
    }

    [Fact]
    public void HandlerException_WithDifferentPlatformContext_FormatsAndLogsCorrectly()
    {
        const string testExceptionMessage = "Platform specific error";
        const string testPlatformContext = "TestPlatform_Windows";
        const bool testIsTerminating = false;
        Exception? testException;
        try
        {
            throw new ArgumentNullException("paramName", testExceptionMessage);
        }
        catch (Exception? ex)
        {
            testException = ex;
        }

        var expectedFormattedMessage =
            $"GLOBAL ERROR CAUGHT\n" +
            $"Platform: {testPlatformContext}\n" +
            $"App is terminating: {testIsTerminating}\n" +
            $"Exception Type: {testException.GetType().FullName}\n" +
            $"Message: {testException.Message}\n" +
            $"Stack Trace:\n{testException.StackTrace}";

        _sut.HandlerException(testException, testPlatformContext, testIsTerminating);

        Assert.Equal(3, _testListener.Messages.Count);
        Assert.Contains(testPlatformContext, _testListener.Messages[1]);
        Assert.Contains(testExceptionMessage, _testListener.Messages[1]);
    }

    private class MockedExceptionWithMessage(string message) : Exception
    {
        public override string Message => message;
        public override string? StackTrace => null;
    }
}