using Microsoft.Extensions.Logging;

namespace OdontoManager.Application.Abstractions.Decorators;

internal static partial class ApplicationLog
{
    [LoggerMessage(Level = LogLevel.Information, Message = "{Operacao} concluída em {DuracaoMs} ms")]
    public static partial void OperacaoConcluida(ILogger logger, string operacao, double duracaoMs);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Operacao} falhou com o erro {CodigoDoErro} em {DuracaoMs} ms")]
    public static partial void OperacaoFalhou(ILogger logger, string operacao, string codigoDoErro, double duracaoMs);
}
