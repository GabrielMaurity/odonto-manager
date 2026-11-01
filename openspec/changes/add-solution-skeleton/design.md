# Design

## Context

O repositório contém só documentação, o `docker-compose.yaml` (PostgreSQL 18.6) e os hooks. Não há código .NET. Esta change materializa o que a constituição (Artigos III, IV e VI) e o `Docs/specification.md` (seções 3 a 10) definem, sem nenhum bounded context. O comportamento observável está nas specs `platform/*`; a motivação, em proposal.md — Why.

**Ambiente de implementação:** nesta sessão, o .NET SDK não pode ser instalado diretamente, porque os servidores de download da Microsoft estão bloqueados. A implementação usa a imagem `mcr.microsoft.com/dotnet/sdk:10.0` (SDK 10.0.403), com o repositório montado e o socket do Docker compartilhado para o Testcontainers. O NuGet está acessível. Nada disso entra no repositório: quem desenvolve localmente usa o SDK instalado.

## Goals / Non-Goals

**Goals:**
- `dotnet build` sem warnings e `dotnet test` verde na raiz.
- Building blocks prontos para o primeiro módulo (`add-identity-access`) começar pelo domínio.
- Regras de arquitetura verificadas por teste desde já.
- API e Web executáveis localmente com `dotnet run`.

**Non-Goals:**
- Dockerfiles, serviços `api`/`web` no compose e CI (etapa P1.E2, `add-ci-pipeline`).
- Autenticação e autorização (`add-identity-access`). Nesta change, os endpoints de plataforma são todos anônimos.
- Qualquer `DbContext` de módulo ou migration: não há módulo ainda.

## Decisions

### D1. Projetos e referências (constituição IV.1; specification 9)
`OdontoManager.slnx` com:

| Projeto | Tipo | Referencia |
|---------|------|------------|
| `src/OdontoManager.Domain` | classlib | — |
| `src/OdontoManager.Application` | classlib | Domain |
| `src/OdontoManager.Infrastructure` | classlib | Application, Domain |
| `src/OdontoManager.Contracts` | classlib | — |
| `src/OdontoManager.Api` | web | Application, Infrastructure, Contracts |
| `src/OdontoManager.Web` | Blazor Web App (servidor) | Web.Client, Contracts |
| `src/OdontoManager.Web.Client` | Blazor WebAssembly | Contracts |
| `tests/OdontoManager.Domain.UnitTests` | xUnit v3 | Domain |
| `tests/OdontoManager.Application.UnitTests` | xUnit v3 | Application |
| `tests/OdontoManager.Api.IntegrationTests` | xUnit v3 | Api, Infrastructure |
| `tests/OdontoManager.Web.ComponentTests` | xUnit v3 + bUnit | Web.Client |
| `tests/OdontoManager.E2ETests` | xUnit v3 + Playwright | Web |
| `tests/OdontoManager.ArchitectureTests` | xUnit v3 + NetArchTest | todos os de `src/` |

- `Directory.Build.props`: `net10.0`, `Nullable` e `ImplicitUsings` habilitados, `TreatWarningsAsErrors`, `AnalysisLevel=latest-recommended`, `EnforceCodeStyleInBuild` e `InvariantGlobalization=false` (pt-BR).
- `Directory.Packages.props` com `ManagePackageVersionsCentrally`.
- `global.json` fixa o SDK 10.0 com `rollForward: latestFeature`.
- `.editorconfig` com as convenções C# do .NET e severidades que o build verifica.

### D2. Shared Kernel (specification 3.8, 3.9 e 5.1)
Segue o código de exemplo da specification:
- `Result` e `Result<T>`: `IsSuccess`, `IsFailure`, `Error` e `Value`. Acessar `Value` de um resultado com falha lança `InvalidOperationException`, porque é erro de programação, não de negócio. Há conversões implícitas de `T` e de `Error`.
- `Error` é um `record`, com o `Error.None` interno; `ValidationError` herda de `Error`.
- `UsuarioId`, `PacienteId` e `DentistaId` são `readonly record struct` com `Novo()` via `Guid.CreateVersion7()`.

### D3. Messaging e decorators (specification 4.1 e 4.2)
- As abstrações ficam em `Application/Abstractions/Messaging`, e `Unit` também.
- `AddApplication()`:
  - registra os handlers por varredura do assembly com Scrutor;
  - aplica os decorators `LoggingCommandHandler`/`LoggingQueryHandler` (externo) e `ValidationCommandHandler` (interno), com `services.Decorate`.
- O decorator de validação roda todos os `IValidator<TCommand>` e devolve `ValidationError` agrupando as mensagens por propriedade, sem chamar o handler.
- O decorator de logging registra nome da operação, duração e código do erro. Nunca registra o conteúdo do comando (constituição VI.7).
- Como ainda não há casos de uso reais, os testes de unidade usam comandos e handlers declarados no próprio projeto de teste.

### D4. API base (specification 5.2, 6 e 8.5; specs `platform/api-errors` e `platform/api-documentation`)
- **ProblemDetails:**
  - `AddProblemDetails` com `CustomizeProblemDetails`, que acrescenta `traceId` (de `Activity.Current` ou `HttpContext.TraceIdentifier`);
  - `UseStatusCodePages` para que 404 de rota inexistente também saia em ProblemDetails;
  - `IExceptionHandler` global, que responde 500 e inclui detalhes da exceção apenas em `Development`.
- **Conversão de erros:** `Error.ToProblem()` em `Api/Infrastructure`, com a tabela da specification 5.2. `ValidationError` vira `ValidationProblemDetails` com `errors`, e todo erro de negócio leva a extensão `code`.
- **Registro de módulos:** cada módulo implementa `IApiModule.MapEndpoints` e o registra no container por meio de `Add<Modulo>Module(IServiceCollection, IConfiguration)`. O `Program.cs` mapeia todos os `IApiModule` registrados dentro do grupo `/api/v1`, então incluir um módulo novo não altera o mapeamento (OCP). Nesta change, nenhum módulo está registrado. *Ajuste feito durante a implementação: a versão anterior previa um `Map<Modulo>Endpoints` chamado explicitamente no `Program.cs`.*
- **Testes do mapeamento de erros:** os testes de integração registram um `IApiModule` próprio, só do projeto de teste (via `ConfigureTestServices`), com endpoints que devolvem cada tipo de `Error` ou lançam exceção. Nenhum endpoint de teste é publicado na API.
- **OpenAPI:** `AddOpenApi()` + `MapOpenApi()` e `MapScalarApiReference()`, ambos só em `Development`.
- **Transporte e CORS:** `UseHsts` fora de `Development` e `UseHttpsRedirection`. CORS com política nomeada `Web`, cujas origens vêm de `Cors:AllowedOrigins`; lista vazia por padrão.

### D5. Persistência base (specification 3.5, 3.6 e 7)
- `ModuleDbContext` (abstrato, em `Infrastructure/Persistence`), base dos `DbContext` de módulo:
  - recebe o nome do schema;
  - aplica `HasDefaultSchema`;
  - implementa `IUnitOfWork.SalvarAsync`;
  - registra as conversões dos IDs do Shared Kernel em `ConfigureConventions`.
- `AddModuleDbContext<TContext>(schema)` configura `UseNpgsql` com a connection string `OdontoManager`, `MigrationsHistoryTable("__ef_migrations_history", schema)`, `UseSnakeCaseNamingConvention()` e o interceptor de eventos.
- `DomainEventsInterceptor`:
  - no `SavedChangesAsync`, coleta os eventos das raízes rastreadas, limpa as listas e despacha cada evento para os `IDomainEventHandler<T>` resolvidos do container;
  - falhas são registradas em log com o tipo do evento e não interrompem os demais handlers (specification 3.5).
- **Testes:** um `DbContext` existente apenas no projeto de integração verifica, contra o PostgreSQL real:
  - schema, snake_case e conversão de IDs;
  - despacho após o commit;
  - ausência de despacho quando o `SaveChanges` falha.
- `IUnitOfWork` fica em `Application/Abstractions`; `IDomainEventHandler<T>` também.

### D6. Transversais (specification 8; spec `platform/health-checks`)
- **Tempo:** `TimeProvider.System` como singleton.
- **Options:** `ClinicaOptions` (seção `Clinica`, com `FusoHorario` padrão `America/Sao_Paulo`), com validação customizada de que o fuso existe (`TimeZoneInfo.TryFindSystemTimeZoneById`) e `ValidateOnStart`. É a primeira Options real e serve de modelo para as próximas.
- **OpenTelemetry:**
  - instrumentação de ASP.NET Core, HttpClient e Npgsql;
  - exportador OTLP habilitado só quando `OTEL_EXPORTER_OTLP_ENDPOINT` está definido, para não gerar erros no ambiente local sem coletor;
  - os logs também passam pelo OpenTelemetry.
- **Health checks:**
  - `/health/live` sem checks (predicate vazio);
  - `/health/ready` com o check do PostgreSQL (`AddNpgSql`), com tag `ready`;
  - resposta só com o status agregado, sem descrições, fora de `Development`.
- **Connection string:** User Secrets do projeto `Api` (`UserSecretsId` no `.csproj`); o README traz o comando para defini-la a partir do `.env`.

### D7. Web base (constituição III e IV.3, IV.7; spec `platform/web-shell`)
- **Projetos:** criados a partir do template `blazor` com `--interactivity Auto --all-interactive`. Os componentes ficam em `Web.Client`, para funcionarem tanto no servidor quanto no WebAssembly.
- **Radzen:** `RadzenTheme`, `RadzenLayout`, `RadzenSidebar` e `RadzenPanelMenu`, encapsulados em `MainLayout` e `NavMenu` próprios.
- **Cliente HTTP:**
  - `ISystemStatusClient`, com implementação tipada que chama `GET /health/ready` e devolve `Disponivel` ou `Indisponivel`, tratando timeout e exceção como indisponível;
  - base address em `Api:BaseUrl`, lida do `appsettings.json` do servidor e do `wwwroot/appsettings.json` do cliente;
  - o WebAssembly chama a API diretamente, por isso a origem da Web entra em `Cors:AllowedOrigins` da API em desenvolvimento.
- **Cultura pt-BR:** definida em `DefaultThreadCurrentCulture`/`UICulture` no servidor e no cliente, com `BlazorWebAssemblyLoadAllGlobalizationData`.
- **`NotFound`:** página dentro do layout, com link para a página inicial.

### D8. Infraestrutura de testes (specification 10.1)
- **Integração:**
  - `ApiFactory : WebApplicationFactory<Program>` com Testcontainers;
  - imagem `postgres:18.6-alpine` em uma constante única, a mesma do compose;
  - Respawn limpa o banco entre testes;
  - um container por execução, via assembly fixture do xUnit v3.
- **Componentes:** bUnit com `BunitContext`, serviços do Radzen registrados e `ISystemStatusClient` substituído por NSubstitute.
- **Ponta a ponta:** Playwright sobre a Web iniciada com `WebApplicationFactory` em Kestrel real (`UseKestrel`, disponível no .NET 10), em porta livre. A API não é iniciada, o que exercita o cenário "API indisponível". Os testes E2E têm o trait `Category=E2E`, para poderem ser filtrados.
- **Pacotes:** as versões estáveis mais recentes no momento da implementação, registradas no `Directory.Packages.props`. FluentAssertions fica fixado em `7.*` (specification 11).

### D9. Testes de arquitetura (specification 10.2)
- Todas as regras da tabela 10.2, com NetArchTest.
- As regras de camadas já têm código real para verificar.
- As regras de módulos (`PublicApi`), de entidades sem setter público e de handlers `sealed` passam sem verificar nada por enquanto, porque ainda não há módulos. Ficam escritas de forma genérica, por convenção de namespace, para valerem automaticamente a partir de `add-identity-access`.

### D10. Portas locais
API em `http://localhost:5080` e `https://localhost:7080`; Web em `http://localhost:5090` e `https://localhost:7090`, definidas no `launchSettings.json`.

## Risks / Trade-offs

- [Interactive Auto duplica a execução (servidor e WebAssembly)] → Todos os componentes ficam em `Web.Client` e usam só o cliente HTTP (constituição IV.3); o teste E2E roda no navegador real.
- [Navegadores do Playwright indisponíveis no ambiente] → Os testes E2E têm o trait `Category=E2E`. Se não puderem rodar nesta sessão, a tarefa é marcada como pendente e reportada, e não como concluída. A execução passa a ser garantida pela CI em `add-ci-pipeline`.
- [Regras de arquitetura de módulos sem código para verificar] → Escritas por convenção, valem a partir do primeiro módulo; a revisão de `add-identity-access` confirma que elas detectam violações.
- [Warnings como erros travarem o uso dos templates] → Ajustar o código gerado pelos templates até compilar sem warnings, em vez de suprimir regras globalmente; supressões pontuais precisam de justificativa.
- [Versões de pacotes recentes com mudanças incompatíveis] → Versões fixadas no `Directory.Packages.props`; atualização apenas por commits `build(deps)`.
