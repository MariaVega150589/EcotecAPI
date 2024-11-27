namespace ProjectAPI.Application.Common.Exceptions;

public class ConflictException : Exception
{
    public ConflictException()
        : base()
    {
    }

    public ConflictException(string traceMessage, string? appMessage = default)
        : base(traceMessage)
    {
        Detail = appMessage ?? traceMessage;
    }

    public string Detail { get; } = default!;
}