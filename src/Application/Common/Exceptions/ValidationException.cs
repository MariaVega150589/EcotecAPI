using FluentValidation.Results;

namespace ProjectAPI.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base(failures.First().ErrorMessage)
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public ValidationException(string[] fails, string key)
        : base(fails.First())
    {
        Errors.Add(key, fails);
    }

    public ValidationException(string message) : base(message)
    {
        Errors.Add("error", new[] { message });
    }

    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    public string MessageApp { get; set; } = "";
}