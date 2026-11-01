namespace OdontoManager.Api.IntegrationTests.Infrastructure;

public static class PostgresImage
{
    // Mantida igual à imagem do serviço postgres em docker-compose.yaml (specification 7.5).
    public const string Name = "postgres:18.6-alpine";
}
