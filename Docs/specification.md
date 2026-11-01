# Especificação Técnica do Odonto Manager

| Versão | Atualizada em | Norma superior |
|--------|---------------|----------------|
| 1.1.1  | 2026-11-01    | [`constitution.md`](constitution.md) v1.2.1 |

Este documento detalha **como** o Odonto Manager é construído: padrões, bibliotecas, estrutura e convenções técnicas. Ele é normativo (Artigo IV.8 da constituição) e subordinado à constituição: em caso de conflito, prevalece a constituição. As palavras **DEVE**, **NÃO DEVE** e **PODE** têm o mesmo sentido normativo da constituição.

**Sumário**

1. [Visão geral da stack](#1-visão-geral-da-stack)
2. [DDD estratégico](#2-ddd-estratégico)
3. [DDD tático](#3-ddd-tático)
4. [Camada de aplicação](#4-camada-de-aplicação)
5. [Erros](#5-erros)
6. [API](#6-api)
7. [Persistência](#7-persistência)
8. [Aspectos transversais](#8-aspectos-transversais)
9. [Estrutura da solução](#9-estrutura-da-solução)
10. [Testes](#10-testes)
11. [Catálogo de pacotes](#11-catálogo-de-pacotes)
12. [Atualização deste documento](#12-atualização-deste-documento)
13. [Commits semânticos](#13-commits-semânticos)

---

## 1. Visão geral da stack

| Camada | Tecnologia |
|--------|------------|
| Runtime | .NET 10 (LTS), C# 14, `Nullable` habilitado |
| API | ASP.NET Core Minimal APIs, OpenAPI nativo |
| Domínio | DDD com modelo rico, sem dependências externas |
| Casos de uso | CQRS com handlers próprios, sem biblioteca de mediator |
| Persistência | EF Core 10 + Npgsql, PostgreSQL 18 |
| Banco local | PostgreSQL via `docker-compose.yaml` (constituição, Artigo III.3) |
| Frontend | Blazor Web App (Interactive Auto) + Radzen Blazor |
| Testes | xUnit v3, FluentAssertions 7, NSubstitute, Testcontainers, bUnit, Playwright, NetArchTest |

## 2. DDD estratégico

### 2.1 Bounded contexts

Cada módulo do Artigo IV.4 da constituição é um bounded context, com modelo, linguagem ubíqua e schema de banco próprios.

| Bounded context | Responsabilidade | Schema |
|-----------------|------------------|--------|
| Identidade e Acesso | Usuários, perfis, autenticação, autorização | `identidade` |
| Pacientes | Cadastro e dados de contato de pacientes | `pacientes` |
| Profissionais | Dentistas, registro no CRO, especialidades, disponibilidade | `profissionais` |
| Agenda | Consultas: marcação, desmarcação, remarcação | `agenda` |
| Materiais | Pedidos de material e aprovação | `materiais` |
| Exames | Arquivos de exames e seus metadados | `exames` |

### 2.2 Mapa de contextos

```
                 ┌──────────────────────┐
                 │ Identidade e Acesso  │  (upstream de todos: UsuarioId, perfis)
                 └──────────┬───────────┘
          ┌─────────────────┼──────────────────┐
          ▼                 ▼                  ▼
   ┌─────────────┐   ┌──────────────┐   ┌─────────────┐
   │  Pacientes  │   │ Profissionais│   │  Materiais  │◄── DentistaId (solicitante)
   └──────┬──────┘   └──────┬───────┘   └─────────────┘
          │ PacienteId      │ DentistaId, disponibilidade
          ▼                 ▼
        ┌─────────────────────┐
        │        Agenda       │
        └──────────┬──────────┘
                   │ consulta "dentista atende/atendeu paciente?"
                   ▼
            ┌─────────────┐
            │   Exames    │◄── PacienteId
            └─────────────┘
```

- As relações são do tipo **cliente/fornecedor**: o contexto consumidor (abaixo) usa a API pública do fornecedor (acima).
- Referências entre contextos são feitas **apenas por ID**. Os IDs compartilhados (`UsuarioId`, `PacienteId`, `DentistaId`) ficam no **Shared Kernel** (seção 3.8).
- Exames consulta a Agenda para aplicar a autorização por recurso do Artigo VI.1 da constituição.

### 2.3 Glossário da linguagem ubíqua

Os termos abaixo DEVEM ser usados de forma idêntica em specs, código, endpoints e telas. Cada change de módulo DEVE refinar e ampliar o glossário do seu contexto.

| Contexto | Termo | Nome no código | Definição |
|----------|-------|----------------|-----------|
| Identidade e Acesso | Usuário | `Usuario` | Pessoa com acesso ao sistema |
| Identidade e Acesso | Perfil | `Perfil` | Dentista, Recepcionista ou Administrador (constituição, Artigo V) |
| Pacientes | Paciente | `Paciente` | Pessoa atendida pela clínica |
| Pacientes | CPF | `Cpf` | Documento do paciente, validado pelos dígitos verificadores |
| Profissionais | Dentista | `Dentista` | Profissional que realiza atendimentos |
| Profissionais | CRO | `RegistroCro` | Número e UF de inscrição no Conselho Regional de Odontologia |
| Profissionais | Disponibilidade | `Disponibilidade` | Dias e faixas de horário em que o dentista atende |
| Agenda | Consulta | `Consulta` | Atendimento de um paciente por um dentista em um horário |
| Agenda | Horário | `IntervaloDeHorario` | Início e fim de uma consulta |
| Agenda | Marcar | `Agendar` | Criar uma consulta em um horário livre |
| Agenda | Desmarcar | `Cancelar` | Cancelar uma consulta marcada, com motivo |
| Agenda | Remarcar | `Remarcar` | Mover uma consulta para outro horário |
| Agenda | Status da consulta | `StatusDaConsulta` | Agendada, Confirmada, Realizada, Cancelada, Falta |
| Materiais | Pedido de Material | `PedidoDeMaterial` | Solicitação de materiais feita por um dentista |
| Materiais | Item do Pedido | `ItemDoPedido` | Material e quantidade solicitados |
| Materiais | Status do pedido | `StatusDoPedido` | Solicitado, Aprovado, Recusado, Entregue |
| Exames | Exame | `Exame` | Registro de um exame de um paciente (tipo, data, solicitante) |
| Exames | Arquivo de Exame | `ArquivoDeExame` | Arquivo criptografado associado a um exame |

## 3. DDD tático

### 3.1 Agregados

- Um agregado DEVE proteger suas invariantes: todo estado só muda por métodos da **raiz**.
- Objetos de fora NÃO DEVEM manter referência a entidades internas do agregado.
- Agregados DEVEM se referenciar **apenas por ID**, nunca por navegação.
- Uma transação DEVE alterar **um único agregado**. Efeitos em outros agregados acontecem por eventos de domínio (3.5).
- Agregados DEVEM ser pequenos: só inclua no agregado o que precisa mudar junto para manter uma invariante.
- Contextos com pouca regra de negócio (ex.: dados cadastrais) PODEM ter agregados simples, mas a regra de construção e a ausência de setters públicos continuam valendo.

### 3.2 Entidades

- Construtor `private`. A criação é feita por método de fábrica estático com nome da linguagem ubíqua (ex.: `Consulta.Agendar(...)`), que valida as invariantes.
- Propriedades com `private set` ou `init`. Setters públicos NÃO DEVEM existir (verificado por teste de arquitetura).
- Um construtor sem parâmetros `private` PODE existir apenas para o EF Core.
- Coleções expostas como `IReadOnlyCollection<T>`, com a lista mutável privada.

### 3.3 Objetos de valor

- Objetos de valor DEVEM ser `sealed record`, imutáveis, com igualdade por valor. Não há classe base `ValueObject`: o `record` já oferece igualdade estrutural.
- A criação DEVE validar e retornar `Result<T>` quando a entrada vem do usuário (ex.: `Cpf.Criar(string)`).
- Candidatos naturais: `Cpf`, `Email`, `Telefone`, `RegistroCro`, `IntervaloDeHorario`, `Quantidade`.

### 3.4 Identificadores fortemente tipados

- Toda raiz de agregado DEVE ter um ID próprio como `readonly record struct` envolvendo um `Guid`.
- IDs DEVEM ser gerados com `Guid.CreateVersion7()`: são ordenáveis por tempo e reduzem a fragmentação de índices no PostgreSQL.

### 3.5 Eventos de domínio

- Nomeados no passado, como `sealed record` imutáveis implementando `IDomainEvent` (ex.: `ConsultaAgendada`, `ConsultaCancelada`).
- A raiz do agregado registra os eventos com `Raise(...)`; o agregado NÃO DEVE despachá-los.
- Um interceptor do EF Core despacha os eventos **depois** do commit (`SavedChangesAsync`) para os `IDomainEventHandler<TEvento>` registrados.
- Cada `DbContext` tem sua própria transação, então um handler que altera outro contexto roda em transação separada. Por isso os handlers DEVEM ser **idempotentes**, e falhas DEVEM ser registradas em log com o ID do evento.
- Evolução prevista: adotar o padrão Outbox quando houver integração assíncrona ou quando uma falha de handler não puder ser tolerada. Essa adoção DEVE ser feita por uma change `update-specification-outbox`.

### 3.6 Repositórios

- Existe repositório **apenas para raízes de agregado**.
- A interface fica em `Domain`, junto do agregado (constituição, Artigo IV.1); a implementação fica em `Infrastructure`.
- Repositórios DEVEM expor operações de coleção (`ObterPorIdAsync`, `Adicionar`, `Remover`) e NÃO DEVEM expor `IQueryable`.
- Não há repositório genérico. Cada interface tem apenas o que o agregado precisa (ISP).
- A persistência é confirmada por `IUnitOfWork.SalvarAsync`, implementado pelo `DbContext` do módulo.

### 3.7 Serviços de domínio

- Usados quando uma regra não pertence naturalmente a um único agregado (ex.: verificar se há sobreposição de consultas de um dentista antes de agendar).
- Ficam em `Domain`, sem estado, e dependem apenas de abstrações do próprio domínio.
- Regras de unicidade e não sobreposição DEVEM também ser garantidas no banco (índice único ou exclusion constraint), porque a verificação em memória sozinha não resiste a requisições concorrentes.

### 3.8 Shared Kernel

O namespace `OdontoManager.Domain.SharedKernel` contém o mínimo compartilhado entre contextos:

- `Entity<TId>`, `AggregateRoot<TId>`, `IDomainEvent`;
- `Result`, `Result<T>`, `Error`, `ErrorType` (seção 5);
- IDs usados entre contextos: `UsuarioId`, `PacienteId`, `DentistaId`.

O Shared Kernel NÃO DEVE conter regras de negócio de nenhum contexto. Incluir algo nele exige justificativa no `design.md` da change.

### 3.9 Exemplo

```csharp
namespace OdontoManager.Domain.SharedKernel;

public abstract class Entity<TId> where TId : struct
{
    public TId Id { get; protected init; }
}

public abstract class AggregateRoot<TId> : Entity<TId> where TId : struct
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

```csharp
namespace OdontoManager.Domain.Agenda.Consultas;

public readonly record struct ConsultaId(Guid Value)
{
    public static ConsultaId Novo() => new(Guid.CreateVersion7());
}

public sealed record IntervaloDeHorario
{
    public DateTimeOffset Inicio { get; }
    public DateTimeOffset Fim { get; }

    private IntervaloDeHorario(DateTimeOffset inicio, DateTimeOffset fim)
    {
        Inicio = inicio;
        Fim = fim;
    }

    public static Result<IntervaloDeHorario> Criar(DateTimeOffset inicio, DateTimeOffset fim)
    {
        if (fim <= inicio)
        {
            return AgendaErrors.IntervaloInvalido;
        }

        return new IntervaloDeHorario(inicio, fim);
    }

    public bool SobrepoeA(IntervaloDeHorario outro) => Inicio < outro.Fim && outro.Inicio < Fim;
}

public sealed class Consulta : AggregateRoot<ConsultaId>
{
    public PacienteId PacienteId { get; private set; }
    public DentistaId DentistaId { get; private set; }
    public IntervaloDeHorario Horario { get; private set; } = null!;
    public StatusDaConsulta Status { get; private set; }
    public string? MotivoDoCancelamento { get; private set; }

    private Consulta() { }

    public static Consulta Agendar(PacienteId pacienteId, DentistaId dentistaId, IntervaloDeHorario horario)
    {
        var consulta = new Consulta
        {
            Id = ConsultaId.Novo(),
            PacienteId = pacienteId,
            DentistaId = dentistaId,
            Horario = horario,
            Status = StatusDaConsulta.Agendada
        };

        consulta.Raise(new ConsultaAgendada(consulta.Id, dentistaId, horario));
        return consulta;
    }

    public Result Cancelar(string motivo, DateTimeOffset agora)
    {
        if (Status is not (StatusDaConsulta.Agendada or StatusDaConsulta.Confirmada))
        {
            return AgendaErrors.ConsultaNaoPodeSerCancelada;
        }

        if (agora >= Horario.Inicio)
        {
            return AgendaErrors.ConsultaJaIniciada;
        }

        Status = StatusDaConsulta.Cancelada;
        MotivoDoCancelamento = motivo;
        Raise(new ConsultaCancelada(Id, DentistaId, motivo));
        return Result.Success();
    }
}
```

O exemplo é ilustrativo: as regras reais da Agenda serão definidas nas specs da change `add-appointment-scheduling`.

## 4. Camada de aplicação

### 4.1 CQRS com handlers próprios

Nenhuma biblioteca de mediator é usada. As abstrações ficam em `OdontoManager.Application.Abstractions.Messaging`:

```csharp
public interface ICommand<TResponse>;
public interface IQuery<TResponse>;

public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
```

- Comandos que não retornam dados usam `ICommand<Unit>`, com o tipo `Unit` definido em `Application.Abstractions.Messaging`.
- Um caso de uso = um comando ou consulta + um handler + (opcional) um validador, na mesma pasta de feature.
- Os handlers são `internal sealed` e são resolvidos por DI. O registro é feito por varredura de assembly com Scrutor.
- Endpoints dependem da interface `ICommandHandler<,>` / `IQueryHandler<,>`, nunca da classe concreta (DIP).

### 4.2 Decorators

Comportamentos transversais são aplicados com `services.Decorate(...)` do Scrutor, sem alterar os handlers (OCP), nesta ordem (de fora para dentro):

1. **Logging**: registra início, fim, duração e o código do erro, sem dados pessoais.
2. **Validação**: executa os `IValidator<TCommand>` do FluentValidation e devolve um erro do tipo `Validation` (seção 5) sem chamar o handler.

A confirmação da transação é responsabilidade explícita do handler de comando (`IUnitOfWork.SalvarAsync`), para deixar claro qual agregado é persistido.

### 4.3 Consultas (lado de leitura)

- Consultas NÃO DEVEM carregar agregados. Elas DEVEM projetar direto para DTOs de leitura com `AsNoTracking()` e `Select`.
- Como `Application` não depende de EF Core (constituição, Artigo IV.1), o `IQuery<T>` e o DTO de resposta ficam em `Application`, e o **handler de consulta é implementado em `Infrastructure`**, onde está o `DbContext`.
- Handlers de comando ficam em `Application`.

### 4.4 Validação

- **Entrada** (formato, obrigatoriedade, tamanho): FluentValidation, em `Application`, um validador por comando.
- **Invariantes de negócio**: no domínio (fábricas, métodos de agregado, objetos de valor). O validador NÃO DEVE duplicar regras de negócio.

### 4.5 Mapeamento

- O mapeamento é **manual**, sem AutoMapper.
- `Api` converte o request de `Contracts` em comando ou consulta (`request.ToCommand()`) e o resultado em response de `Contracts`.
- `Application` NÃO DEVE referenciar `Contracts` (constituição, Artigo IV.1).

### 4.6 Comunicação entre módulos

- Um módulo expõe operações para outros apenas pelo namespace `OdontoManager.Application.<Modulo>.PublicApi`, com interfaces e DTOs próprios. Essa é a "API pública" do Artigo IV.5 da constituição.
- Qualquer outro tipo de um módulo é `internal` ou não pode ser referenciado por outro módulo (verificado por teste de arquitetura).

## 5. Erros

### 5.1 Result pattern

- Falhas de negócio **esperadas** DEVEM ser retornadas como `Result` / `Result<T>`, sem lançar exceção.
- Exceções ficam reservadas para falhas inesperadas (banco indisponível, bug) e são tratadas pelo handler global.

```csharp
public enum ErrorType { Validation, NotFound, Conflict, BusinessRule }

public record Error(string Code, string Message, ErrorType Type)
{
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    public static Error BusinessRule(string code, string message) => new(code, message, ErrorType.BusinessRule);
}

public sealed record ValidationError(IReadOnlyDictionary<string, string[]> Errors)
    : Error("Validation", "Um ou mais campos são inválidos.", ErrorType.Validation);
```

- `Result<T>` tem conversões implícitas de `T` e de `Error`.
- Erros de cada contexto ficam em uma classe estática `<Contexto>Errors` (ex.: `AgendaErrors.ConsultaNaoPodeSerCancelada`), com código no formato `Contexto.Erro` (ex.: `Agenda.ConsultaNaoPodeSerCancelada`).
- Erros de validação usam `ValidationError`, que leva as mensagens por campo e é convertido em `ValidationProblemDetails`.

### 5.2 Conversão para HTTP

A API converte `Error` em ProblemDetails (RFC 9457) com `type`, `title`, `status`, `detail` e a extensão `code`.

| `ErrorType` / situação | Status HTTP |
|------------------------|-------------|
| `Validation` | 400 Bad Request (`ValidationProblemDetails` com `errors` por campo) |
| Não autenticado | 401 Unauthorized |
| Sem permissão (perfil ou recurso) | 403 Forbidden |
| `NotFound` | 404 Not Found |
| `Conflict` (ex.: horário já ocupado, concorrência otimista) | 409 Conflict |
| `BusinessRule` | 422 Unprocessable Entity |
| Exceção não tratada | 500 Internal Server Error, sem detalhes internos fora de desenvolvimento |

- Um recurso que existe mas não pertence ao usuário DEVE retornar 404, e não 403, para não revelar a existência de dados de pacientes.

## 6. API

- **Minimal APIs.** Cada módulo tem um `MapGroup` com prefixo versionado: `/api/v1/agenda`, `/api/v1/pacientes`, `/api/v1/profissionais`, `/api/v1/materiais`, `/api/v1/exames`, `/api/v1/identidade`.
- Cada endpoint é uma classe estática `internal` por feature, com um método de extensão `Map<Feature>` sobre `RouteGroupBuilder`.
- Retorno com `TypedResults` e tipos de união `Results<...>`, para o OpenAPI refletir todas as respostas.
- Toda rota DEVE declarar autorização (`RequireAuthorization(<política>)`) ou, explicitamente, `AllowAnonymous()`. A política padrão nega acesso anônimo.
- Recursos no plural e em pt-BR, kebab-case (ex.: `/api/v1/materiais/pedidos-de-material`). Ações que não são CRUD usam sub-recursos com verbo (ex.: `POST /api/v1/agenda/consultas/{id}/cancelamento`).
- OpenAPI com `Microsoft.AspNetCore.OpenApi`. Interface Scalar em `/scalar`, **apenas em desenvolvimento**.
- Paginação com `pagina` e `tamanhoDaPagina` (máximo 100) e resposta com `itens`, `pagina`, `tamanhoDaPagina` e `total`.

```csharp
internal static class AgendarConsultaEndpoint
{
    public static RouteGroupBuilder MapAgendarConsulta(this RouteGroupBuilder group)
    {
        group.MapPost("/consultas", async Task<Results<Created<AgendarConsultaResponse>, ProblemHttpResult>> (
                AgendarConsultaRequest request,
                ICommandHandler<AgendarConsultaCommand, ConsultaId> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(), cancellationToken);

                return result.IsSuccess
                    ? TypedResults.Created($"/api/v1/agenda/consultas/{result.Value.Value}", new AgendarConsultaResponse(result.Value.Value))
                    : result.Error.ToProblem();
            })
            .WithName("AgendarConsulta")
            .RequireAuthorization(Politicas.GerenciarAgenda);

        return group;
    }
}
```

## 7. Persistência

### 7.1 DbContext e schema por módulo

- Cada bounded context tem seu próprio `DbContext` (`AgendaDbContext`, `PacientesDbContext`...) e seu próprio schema PostgreSQL (seção 2.1), definido com `HasDefaultSchema`.
- Um `DbContext` NÃO DEVE mapear tabelas de outro schema.
- Joins entre módulos são proibidos (constituição, Artigo IV.5): quem precisa de dados de outro módulo usa a `PublicApi` dele (seção 4.6).
- Todos os `DbContext` usam o mesmo banco e a mesma connection string, `ConnectionStrings:OdontoManager`.

### 7.2 Mapeamento

- Configuração com classes `IEntityTypeConfiguration<T>` (Fluent API), em `Infrastructure/<Modulo>/Configurations`. O domínio NÃO DEVE ter atributos de mapeamento.
- Nomes de tabelas e colunas em snake_case via `UseSnakeCaseNamingConvention()` (EFCore.NamingConventions).
- IDs fortemente tipados mapeados com `HasConversion` para `uuid`.
- Objetos de valor mapeados como **complex types** (`ComplexProperty`) quando de valor único, ou **owned types** quando coleções.
- Datas como `timestamptz`; enums como `text` (`HasConversion<string>()`), para o banco ficar legível e evoluir com segurança.

### 7.3 Concorrência otimista

- Toda raiz de agregado que pode ser alterada em paralelo (no mínimo `Consulta` e `PedidoDeMaterial`) DEVE usar a coluna de sistema `xmin` do PostgreSQL como token de concorrência, via propriedade sombra: `builder.Property<uint>("Version").IsRowVersion()`.
- `DbUpdateConcurrencyException` é convertida em erro `Conflict` (409).

### 7.4 Migrations

- Migrations por `DbContext`, em `Infrastructure/<Modulo>/Migrations`, com tabela de histórico no schema do módulo (`MigrationsHistoryTable("__ef_migrations_history", "<schema>")`).
- Criação:

  ```bash
  dotnet ef migrations add <Nome> --context AgendaDbContext --project src/OdontoManager.Infrastructure --startup-project src/OdontoManager.Api --output-dir Agenda/Migrations
  ```

- Em desenvolvimento, aplicar com `dotnet ef database update --context <Contexto>`. Em outros ambientes, com *migration bundles* (`dotnet ef migrations bundle`), executados de forma explícita no deploy.
- A aplicação NÃO DEVE executar migrations automaticamente na inicialização fora do ambiente de desenvolvimento.
- Migrations já aplicadas em um ambiente compartilhado NÃO DEVEM ser editadas. Correções viram uma nova migration.

### 7.5 Banco local e testes

- O PostgreSQL de desenvolvimento é o serviço `postgres` do `docker-compose.yaml` (constituição, Artigo III.3), em `localhost:${POSTGRES_PORT}`.
- A connection string de desenvolvimento fica em User Secrets do projeto `Api`, nunca no `appsettings.json` versionado (constituição, Artigo VI.4).
- Testes de integração usam Testcontainers com a **mesma imagem** do compose (`postgres:18.6-alpine`), garantindo paridade de versão. Ao atualizar a tag no compose, a constante de imagem dos testes DEVE ser atualizada junto.

## 8. Aspectos transversais

### 8.1 Tempo e fuso horário

- Código de domínio e de aplicação NÃO DEVE usar `DateTime.Now`, `DateTime.UtcNow` ou `DateTimeOffset.Now`. O tempo atual vem de um `TimeProvider` injetado, o que permite controlá-lo nos testes.
- Instantes são persistidos em UTC (`timestamptz`).
- A exibição e as regras de calendário (ex.: "dia da consulta") usam o fuso da clínica, configurado em `Clinica:FusoHorario` (padrão `America/Sao_Paulo`).
- Datas sem horário usam `DateOnly`; horários de disponibilidade usam `TimeOnly`.

### 8.2 Logging e observabilidade

- `ILogger<T>` com mensagens estruturadas, geradas via `[LoggerMessage]` (source generator) nos pontos de log frequentes.
- OpenTelemetry para traces, métricas e logs, com instrumentação de ASP.NET Core, HttpClient e Npgsql, exportando via OTLP (endpoint em `OTEL_EXPORTER_OTLP_ENDPOINT`).
- Logs, traces e tags NÃO DEVEM conter dados pessoais (nome, CPF, telefone, conteúdo de exames), conforme o Artigo VI.7 da constituição. Use IDs.

### 8.3 Health checks

- `/health/live`: o processo está de pé, sem dependências.
- `/health/ready`: inclui o PostgreSQL, via `AspNetCore.HealthChecks.NpgSql`.
- Ambos são anônimos e não expõem detalhes internos fora de desenvolvimento.

### 8.4 Configuração e segredos

- Configurações tipadas com Options pattern: `AddOptions<T>().BindConfiguration(...).ValidateDataAnnotations().ValidateOnStart()`, para que erros de configuração apareçam na inicialização.
- Ordem das fontes: `appsettings.json` → `appsettings.{Ambiente}.json` → User Secrets (desenvolvimento) → variáveis de ambiente.
- Segredos seguem o Artigo VI.4 da constituição.

### 8.5 Segurança na API

- HTTPS obrigatório com HSTS fora de desenvolvimento.
- CORS restrito à origem do frontend.
- Rate limiting nos endpoints de autenticação.
- O mecanismo de autenticação será definido na change `add-identity-access`. Ela DEVE atualizar esta seção por meio de `update-specification-identidade`.

## 9. Estrutura da solução

```
odonto-manager/
├── OdontoManager.slnx
├── global.json                      # fixa o SDK .NET 10
├── Directory.Build.props            # Nullable, TreatWarningsAsErrors, analisadores, LangVersion
├── Directory.Packages.props         # versões centralizadas (Central Package Management)
├── .editorconfig
├── docker-compose.yaml
├── Docs/                          # constitution, specification, plan, tasks, checklist
├── src/
│   ├── OdontoManager.Domain/
│   │   ├── SharedKernel/
│   │   └── Agenda/
│   │       └── Consultas/           # Consulta.cs, ConsultaId.cs, IConsultaRepository.cs, Eventos/
│   ├── OdontoManager.Application/
│   │   ├── Abstractions/            # Messaging/, Decorators/, IUnitOfWork
│   │   └── Agenda/
│   │       ├── AgendarConsulta/     # Command, Handler, Validator
│   │       └── PublicApi/
│   ├── OdontoManager.Infrastructure/
│   │   └── Agenda/                  # AgendaDbContext, Configurations/, Repositories/, Queries/, Migrations/
│   ├── OdontoManager.Contracts/
│   │   └── Agenda/                  # Requests e Responses
│   ├── OdontoManager.Api/
│   │   └── Agenda/                  # AgendaEndpoints.cs e um arquivo por endpoint
│   ├── OdontoManager.Web/
│   └── OdontoManager.Web.Client/
└── tests/
    ├── OdontoManager.Domain.UnitTests/
    ├── OdontoManager.Application.UnitTests/
    ├── OdontoManager.Api.IntegrationTests/
    ├── OdontoManager.Web.ComponentTests/
    ├── OdontoManager.E2ETests/
    └── OdontoManager.ArchitectureTests/
```

- Os projetos de `src/` são exatamente os do Artigo IV.1 da constituição.
- Dentro de cada projeto, a organização é por **contexto → agregado/feature**.
- Namespaces seguem a pasta: `OdontoManager.<Projeto>.<Contexto>.<Feature>`.
- Um arquivo por tipo público; o nome do arquivo é o nome do tipo.
- `Directory.Packages.props` é a única fonte de versões: `PackageReference` nos `.csproj` NÃO DEVE declarar `Version`.
- `InternalsVisibleTo` é permitido apenas para os projetos de teste correspondentes.

## 10. Testes

### 10.1 Pirâmide por camada

| Projeto | Escopo | Ferramentas |
|---------|--------|-------------|
| `Domain.UnitTests` | Agregados, objetos de valor, serviços de domínio | xUnit, FluentAssertions |
| `Application.UnitTests` | Handlers de comando e validadores, com dependências substituídas | xUnit, FluentAssertions, NSubstitute |
| `Api.IntegrationTests` | Endpoints de ponta a ponta no backend, com banco real, autorização e ProblemDetails | `WebApplicationFactory`, Testcontainers, Respawn |
| `Web.ComponentTests` | Componentes Blazor | bUnit |
| `E2ETests` | Fluxos críticos de tela (agenda, pedido de material, exames) | Playwright |
| `ArchitectureTests` | Regras de dependência e de design | NetArchTest.Rules |

- Testes de domínio NÃO DEVEM usar mocks: o domínio não tem dependências.
- Testes de integração compartilham um container por coleção de testes e limpam os dados entre testes com Respawn.
- Todo endpoint DEVE ter teste do caminho de sucesso, de pelo menos um erro de negócio e do acesso negado.
- Nomes e estrutura de testes seguem o Artigo VII.3 da constituição.

### 10.2 Testes de arquitetura

As regras abaixo DEVEM existir em `ArchitectureTests` e rodar na CI.

| Regra | Origem |
|-------|--------|
| `Domain` não depende de `Application`, `Infrastructure`, `Api`, `Contracts`, `Web` | Constituição IV.2 |
| `Domain` não depende de `Microsoft.EntityFrameworkCore` nem de `Microsoft.AspNetCore` | Constituição IV.2 |
| `Application` não depende de `Infrastructure`, `Api`, `Contracts`, `Web`, nem de EF Core | Constituição IV.1 |
| `Contracts` não depende de nenhum outro projeto da solução | Constituição IV.1 |
| `Web` e `Web.Client` dependem apenas de `Contracts` entre os projetos da solução | Constituição IV.3 |
| Tipos de um contexto só referenciam outro contexto pelo namespace `PublicApi` ou pelo Shared Kernel | Constituição IV.5; seção 4.6 |
| Entidades do `Domain` não têm setters públicos | Seção 3.2 |
| Eventos de domínio são `sealed` e implementam `IDomainEvent` | Seção 3.5 |
| Handlers de comando e consulta são `sealed` e não públicos | Seção 4.1 |
| Endpoints não referenciam repositórios nem `DbContext` | Constituição IV.1; seção 4.5 |

## 11. Catálogo de pacotes

Pacotes aprovados. Qualquer pacote fora desta lista exige justificativa no `design.md` da change (licença, manutenção, motivo) e a atualização deste catálogo.

| Pacote | Uso | Licença |
|--------|-----|---------|
| `Microsoft.EntityFrameworkCore` (10.x) | ORM | MIT |
| `Microsoft.EntityFrameworkCore.Design` (10.x) | Ferramentas de migration | MIT |
| `Npgsql.EntityFrameworkCore.PostgreSQL` (10.x) | Provider PostgreSQL | PostgreSQL License |
| `EFCore.NamingConventions` | snake_case | Apache-2.0 |
| `FluentValidation` + `FluentValidation.DependencyInjectionExtensions` | Validação de entrada | Apache-2.0 |
| `Scrutor` | Varredura de assembly e decorators | MIT |
| `Microsoft.AspNetCore.OpenApi` (10.x) | Documento OpenAPI | MIT |
| `Scalar.AspNetCore` | Interface do OpenAPI (desenvolvimento) | MIT |
| `OpenTelemetry.Extensions.Hosting` | Observabilidade | Apache-2.0 |
| `OpenTelemetry.Instrumentation.AspNetCore` / `.Http` | Instrumentação | Apache-2.0 |
| `OpenTelemetry.Exporter.OpenTelemetryProtocol` | Exportação OTLP | Apache-2.0 |
| `Npgsql.OpenTelemetry` | Instrumentação do Npgsql | PostgreSQL License |
| `AspNetCore.HealthChecks.NpgSql` | Health check do PostgreSQL | Apache-2.0 |
| `Radzen.Blazor` | Componentes de UI e Scheduler | MIT |
| `xunit.v3` + `xunit.runner.visualstudio` | Framework de testes | Apache-2.0 |
| `Microsoft.NET.Test.Sdk` | Execução de testes | MIT |
| `FluentAssertions` (**fixado em 7.x**) | Asserções | Apache-2.0 |
| `NSubstitute` | Dublês de teste | BSD-3-Clause |
| `Microsoft.AspNetCore.Mvc.Testing` (10.x) | `WebApplicationFactory` | MIT |
| `Testcontainers.PostgreSql` | PostgreSQL real nos testes | MIT |
| `Respawn` | Limpeza do banco entre testes | Apache-2.0 |
| `bunit` | Testes de componentes Blazor | MIT |
| `Microsoft.Playwright` | Testes ponta a ponta | MIT |
| `NetArchTest.Rules` | Testes de arquitetura | MIT |

**Bibliotecas não usadas, por decisão de projeto:**
- **MediatR e AutoMapper:** licença comercial nas versões atuais, substituídos pelas seções 4.1 e 4.5.
- **FluentAssertions 8+:** licença comercial; por isso a versão fica fixada em 7.x.

## 12. Atualização deste documento

- Alterações DEVEM ser feitas por uma change OpenSpec `update-specification-<tema>` (constituição, Artigo X.5), com justificativa no `proposal.md`.
- Se a alteração contradisser a constituição, ela DEVE ser feita como emenda (`amend-constitution-<tema>`).
- Versionamento semântico:
  - **MAJOR**: remove ou altera de forma incompatível uma regra existente;
  - **MINOR**: adiciona seção, regra ou pacote;
  - **PATCH**: esclarecimento sem mudança de sentido.
- A tabela de versão no topo DEVE ser atualizada a cada alteração, inclusive a versão da constituição de referência.

## 13. Commits semânticos

Detalha o Artigo VII.4 da constituição. Todo commit DEVE seguir esta seção. A validação é feita localmente pelo hook `.githooks/commit-msg` e, no pull request, pela CI, que DEVE executar o mesmo script para cada commit.

### 13.1 Formato

```
<tipo>(<escopo>)!: <descrição>

<corpo>

<rodapé>
```

- `(<escopo>)` é opcional; `!` só é usado em breaking changes (13.5).
- O corpo e o rodapé são opcionais. Quando presentes, DEVEM vir separados do cabeçalho por uma linha em branco.

### 13.2 Tipos

| Tipo | Uso |
|------|-----|
| `feat` | Nova funcionalidade ou nova implementação no código |
| `fix` | Correção de bug |
| `perf` | Alteração de código que melhora o desempenho |
| `refactor` | Mudança no código que não altera o comportamento |
| `style` | Formatação sem efeito no significado do código (espaços, indentação, ponto e vírgula) |
| `test` | Adição ou correção de testes automatizados |
| `docs` | Somente documentação (`*.md`, artefatos do OpenSpec) |
| `build` | Sistema de build e dependências externas |
| `ci` | Arquivos e scripts da integração contínua |
| `env` | Configuração de ambientes e containers |
| `chore` | Ferramentas e manutenção que não chegam à produção |

Os tipos `build`, `ci`, `env` e `chore` se sobrepõem. A fronteira entre eles é definida pelos arquivos afetados:

| Tipo | Arquivos típicos |
|------|------------------|
| `build` | `*.csproj`, `*.slnx`, `Directory.Build.props`, `Directory.Packages.props`, `global.json` |
| `ci` | `.github/workflows/*` |
| `env` | `docker-compose.yaml`, `.env.example`, `Dockerfile`, `appsettings.*.json` |
| `chore` | `.gitignore`, `.editorconfig`, `.githooks/*`, `scripts/*`, `openspec/config.yaml` |

Quando um commit toca arquivos de mais de um tipo, vale o tipo do **objetivo** do commit (ex.: uma funcionalidade que inclui um novo pacote NuGet é `feat`). Se não houver objetivo predominante, o commit DEVE ser dividido.

### 13.3 Escopos

O escopo é opcional. Quando presente, DEVE ser um destes:

| Grupo | Escopos |
|-------|---------|
| Bounded contexts (seção 2.1) | `identidade`, `pacientes`, `profissionais`, `agenda`, `materiais`, `exames` |
| Áreas transversais | `api`, `web`, `infra`, `db`, `docker`, `deps`, `openspec`, `constituicao`, `especificacao` |

Novos escopos exigem uma change `update-specification-commits`.

### 13.4 Redação

- O cabeçalho DEVE ter no máximo **72 caracteres**.
- Tipo e escopo em minúsculas, seguidos de `: ` (dois-pontos e um espaço).
- A descrição é escrita em pt-BR e DEVE:
  - começar com verbo no **presente do indicativo** e em minúscula ("adiciona", "corrige", "remove");
  - não terminar com ponto final;
  - dizer **o que** muda.
- O corpo explica o **porquê** e o contexto, não repete o diff.
- O rodapé usa trailers do Git, um por linha: `BREAKING CHANGE: <descrição>`, `Refs: #123`.

### 13.5 Breaking changes

Uma mudança incompatível (contrato da API, schema sem migração compatível, configuração obrigatória nova) DEVE ter **as duas** marcações:

1. `!` imediatamente antes dos dois-pontos do cabeçalho;
2. o rodapé `BREAKING CHANGE: <o que quebra e como migrar>`.

Uma marcação sem a outra é rejeitada.

### 13.6 Trailers proibidos

Em conformidade com o Artigo VII.4 da constituição, a mensagem NÃO DEVE conter `Co-authored-by:` nem `Signed-off-by:`, em qualquer combinação de maiúsculas e minúsculas.

### 13.7 Mensagens geradas pelo Git

São aceitas sem validação de formato as mensagens que o próprio Git gera: `Merge ...`, `Revert "..."`, `fixup! ...`, `squash! ...` e `amend! ...`. Os trailers proibidos (13.6) continuam valendo para elas.

### 13.8 Exemplos

Válidos:

```
feat(agenda): adiciona remarcação de consultas
fix(pacientes): corrige validação de CPF com dígito zero
env(docker): fixa imagem do pgAdmin na versão 9.19
build(deps): atualiza Npgsql para 10.0.1
docs: descreve fluxo de pedidos de material
refactor(api)!: renomeia rota de cancelamento de consulta

BREAKING CHANGE: POST /api/v1/agenda/consultas/{id}/cancelar passa a ser
POST /api/v1/agenda/consultas/{id}/cancelamento.
```

Inválidos:

| Mensagem | Motivo |
|----------|--------|
| `Adiciona agenda` | Sem tipo |
| `feature: adiciona agenda` | Tipo desconhecido |
| `feat(calendario): adiciona agenda` | Escopo desconhecido |
| `feat:adiciona agenda` | Falta o espaço após `:` |
| `feat: Adiciona agenda` | Descrição com maiúscula inicial |
| `feat: adiciona agenda.` | Ponto final |
| `feat!: remove endpoint legado` (sem rodapé) | `!` sem `BREAKING CHANGE` |

### 13.9 Ativação do hook

```bash
git config core.hooksPath .githooks
```

O comando é executado uma vez por clone. Os testes do hook ficam em `scripts/tests/commit-msg.test.sh` e rodam com `sh scripts/tests/commit-msg.test.sh`.
