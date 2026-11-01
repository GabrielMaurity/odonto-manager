# Design

## Context

Estado atual: `constitution.md` v1.0.0 (Artigos II e IV definem SOLID, Clean Architecture e os seis módulos), change `amend-constitution-docker` pronta para aplicação (PostgreSQL 18 em Docker, v1.1.0) e nenhum código ainda. A motivação está em proposal.md — Why. Decisões já tomadas pelo responsável: .NET 10, DDD, SOLID, PostgreSQL em Docker, **Minimal APIs** e **handlers próprios sem biblioteca de mediator**. As demais decisões abaixo são recomendações para os pontos em que foi pedida ajuda.

Artigos envolvidos: II (princípios), III (stack), IV (arquitetura), VI (segurança), VII (convenções), VIII (fluxo SDD) e X (emendas).

## Goals / Non-Goals

**Goals:**
- Separar responsabilidades: a constituição diz *o que é inegociável*; o `specification.md` diz *como* construir (padrões, bibliotecas, estrutura, convenções).
- Tornar as regras do DDD verificáveis por testes de arquitetura, e não só por revisão.
- Deixar o `add-solution-skeleton` sem decisões técnicas em aberto.

**Non-Goals:**
- Modelar os agregados de cada módulo em detalhe (cada change do roadmap faz isso nas suas specs).
- Definir o mecanismo de autenticação (change `add-identity-access`).
- Especificação do frontend Blazor (pode ganhar seção própria numa change futura).

## Decisions

### D1. Dois documentos com papéis distintos (Artigos II e X)
`constitution.md` continua curta e normativa; `specification.md` traz o detalhe técnico e DEVE ser seguido. Alterar o `specification.md` exige uma change comum (`update-specification-<tema>`), sem emenda, desde que não contradiga a constituição.
- *Alternativa*: colocar tudo na constituição. Rejeitada: cada troca de biblioteca viraria emenda e o documento perderia legibilidade.

### D2. DDD estratégico: módulo = bounded context (Artigo IV.4–5)
Os seis módulos são bounded contexts com linguagem ubíqua própria. O `specification.md` traz um glossário em pt-BR (Consulta, Paciente, Dentista, Horário, Pedido de Material, Exame...) e um mapa de contextos indicando quem consome quem (ex.: Agenda referencia Paciente e Dentista só por ID e consome eventos de Profissionais).

### D3. DDD tático
- **Agregados** protegem invariantes; só a raiz é acessível de fora; agregados se referenciam apenas por ID; uma transação altera um único agregado. Se for preciso alterar mais de um, o caminho é um evento de domínio.
- **Entidades** com construtores privados e métodos de fábrica que validam invariantes; setters públicos são proibidos.
- **Objetos de valor** como `sealed record` imutáveis, sem classe base `ValueObject`, com validação na criação (ex.: `Cpf`, `Email`, `Telefone`, `IntervaloDeHorario`).
- **IDs fortemente tipados** (`readonly record struct PacienteId(Guid Value)`) gerados com `Guid.CreateVersion7()`, ordenáveis por tempo e melhores para índices no PostgreSQL que o GUID v4.
- **Eventos de domínio** registrados pela raiz do agregado e despachados por um interceptor do EF Core **após o commit** (`SavedChangesAsync`). Como há um `DbContext` por módulo (D8), não existe transação única entre contextos; por isso os handlers DEVEM ser idempotentes. O Outbox fica como evolução prevista.
- **Repositórios** apenas para raízes de agregado, com interface em `Domain` e implementação em `Infrastructure`. A tabela do Artigo IV.1 da constituição, que colocava as interfaces de repositório em `Application`, é ajustada nesta emenda. O `DbContext` do módulo faz o papel de Unit of Work.
- **Building blocks** (`Entity`, `AggregateRoot`, `IDomainEvent`, `Result`, `Error` e os IDs compartilhados entre contextos) ficam em uma pasta `SharedKernel` dentro de `Domain`.

### D4. Camada de aplicação: CQRS com handlers próprios
Interfaces `ICommandHandler<TCommand, TResult>` e `IQueryHandler<TQuery, TResult>` registradas por DI (scan de assembly com Scrutor, licença MIT). Preocupações transversais (validação, logging, transação) são aplicadas como decorators, o que respeita o OCP. Consultas leem com projeções (`AsNoTracking`) sem passar por repositório; como `Application` não depende de EF Core, o handler de consulta é implementado em `Infrastructure`, e a consulta e seu DTO ficam em `Application`.
- *Alternativas*: MediatR (licença comercial desde a v13) e Wolverine (complexidade além do necessário). Decisão do responsável.

### D5. Erros: Result pattern + ProblemDetails
Falhas de negócio esperadas retornam `Result<T>` com um `Error` tipado (código + mensagem); exceções só para situações realmente excepcionais. A API converte `Error` em respostas ProblemDetails (RFC 9457) com o status HTTP adequado (400, 404, 409, 422).
- *Alternativa*: exceções de domínio + middleware. Rejeitada: torna o fluxo de controle implícito e caro.

### D6. Validação e mapeamento
Validação de entrada com FluentValidation (Apache-2.0) na camada Application; invariantes ficam no Domain. Mapeamento DTO ↔ domínio é manual, com métodos de extensão.
- *Alternativa*: AutoMapper. Rejeitada: licença comercial nas versões atuais e esconde erros de mapeamento.

### D7. API com Minimal APIs
Um `MapGroup` por módulo (`/api/v1/agenda`, `/api/v1/pacientes`...), endpoints em classes estáticas por feature, `TypedResults`, OpenAPI nativo (`Microsoft.AspNetCore.OpenApi`) e Scalar como interface de documentação só em desenvolvimento. Versão da API na URL.

### D8. Persistência
- Um `DbContext` e um **schema PostgreSQL por módulo** (`agenda`, `pacientes`...). Isso reforça no banco a fronteira entre contextos do Artigo IV.5.
- Configurações com `IEntityTypeConfiguration` (Fluent API), sem atributos no Domain.
- Nomes em snake_case com `EFCore.NamingConventions` (Apache-2.0).
- Objetos de valor mapeados como complex types ou owned types.
- Migrations por `DbContext`, versionadas e aplicadas por um comando explícito, nunca automaticamente na inicialização em produção.
- Concorrência otimista com a coluna de sistema `xmin` do PostgreSQL, importante para evitar dois agendamentos simultâneos no mesmo horário.
- *Alternativa*: um único `DbContext`. Rejeitada: facilita joins entre módulos, violando o Artigo IV.5.

### D9. Aspectos transversais
- **Tempo**: `TimeProvider` injetado (testável), armazenamento em UTC (`timestamptz`) e exibição no fuso da clínica (padrão `America/Sao_Paulo`, configurável).
- **Logging e observabilidade**: `ILogger` com logs estruturados e OpenTelemetry (traces, métricas, logs via OTLP), sem dados pessoais (Artigo VI.7).
- **Health checks**: `/health/live` e `/health/ready` (este verifica o PostgreSQL com `AspNetCore.HealthChecks.NpgSql`).
- **Configuração**: Options pattern com validação na inicialização (`ValidateOnStart`); segredos conforme o Artigo VI.4.

### D10. Estrutura da solução
Arquivo de solução `.slnx`, com `src/` (projetos do Artigo IV.1, organizados internamente por módulo) e `tests/` (`Domain.UnitTests`, `Application.UnitTests`, `Api.IntegrationTests`, `ArchitectureTests`). `Directory.Build.props` centraliza `Nullable`, `TreatWarningsAsErrors` e analisadores; `Directory.Packages.props` faz a gestão central de versões dos pacotes.
- *Alternativa*: projetos separados por módulo (`Agenda.Domain`, `Agenda.Application`...). Rejeitada por agora: seriam 24 ou mais projetos. As fronteiras entre módulos são garantidas por testes de arquitetura, e a separação física continua possível no futuro.

### D11. Testes de arquitetura
NetArchTest.Rules (MIT) para verificar automaticamente (o FluentAssertions do Artigo III fica fixado em 7.x, última versão Apache-2.0): a regra de dependência entre camadas, `Domain` sem referência a EF Core/ASP.NET Core, ausência de referências cruzadas entre namespaces de módulos, e entidades sem setters públicos.

### D12. Catálogo de pacotes
O `specification.md` lista os pacotes aprovados com licença e finalidade. Pacotes fora do catálogo exigem justificativa no `design.md` da change que os introduz (regra já existente) e a atualização do catálogo.

## Risks / Trade-offs

- [Excesso de cerimônia do DDD em partes simples (CRUD de cadastro)] → O `specification.md` permite agregados simples e consultas diretas por projeção; DDD rico é obrigatório onde existem invariantes (Agenda, Materiais).
- [Despacho de eventos na mesma transação acoplar módulos] → Handlers de eventos entre módulos ficam restritos a efeitos idempotentes; a migração para outbox fica registrada como evolução prevista.
- [Fronteiras de módulo garantidas só por convenção em projetos compartilhados] → Testes de arquitetura (D11) rodam na CI e quebram a build.
- [Ordem de aplicação com a `amend-constitution-docker`] → As tarefas verificam a versão atual da constituição antes de emendar.
- [Bibliotecas mudarem de licença (como MediatR e AutoMapper)] → O catálogo registra a licença; a revisão de dependências faz parte de cada change.
