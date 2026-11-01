using System.Diagnostics.CodeAnalysis;

namespace OdontoManager.Domain.SharedKernel;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    BusinessRule
}

[SuppressMessage("Naming", "CA1716", Justification = "Nome definido na specification 5.1; a solução é exclusivamente C#.")]
public record Error(string Code, string Message, ErrorType Type)
{
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    public static Error BusinessRule(string code, string message) => new(code, message, ErrorType.BusinessRule);
}

public sealed record ValidationError(IReadOnlyDictionary<string, string[]> Errors)
    : Error("Validation", "Um ou mais campos são inválidos.", ErrorType.Validation);
