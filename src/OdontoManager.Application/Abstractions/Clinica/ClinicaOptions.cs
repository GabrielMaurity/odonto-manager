using Microsoft.Extensions.Options;

namespace OdontoManager.Application.Abstractions.Clinica;

public sealed class ClinicaOptions
{
    public const string SectionName = "Clinica";

    public string FusoHorario { get; init; } = "America/Sao_Paulo";

    public TimeZoneInfo ObterFusoHorario() => TimeZoneInfo.FindSystemTimeZoneById(FusoHorario);
}

internal sealed class ClinicaOptionsValidator : IValidateOptions<ClinicaOptions>
{
    public ValidateOptionsResult Validate(string? name, ClinicaOptions options) =>
        TimeZoneInfo.TryFindSystemTimeZoneById(options.FusoHorario, out _)
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail($"Clinica:FusoHorario '{options.FusoHorario}' não é um fuso horário válido.");
}
