# Proposal

## Why

A Fase 0 definiu regras, arquitetura e planejamento, mas ainda não existe uma linha de código. Os módulos do MVP (identidade, pacientes, profissionais, agenda) dependem de uma base comum: a solução .NET, os building blocks do DDD, o tratamento de erros, a persistência, a observabilidade, o frontend e a infraestrutura de testes. Construí-la antes, uma única vez, faz com que cada módulo comece direto pelo domínio, e permite que os testes de arquitetura garantam as regras da constituição desde o primeiro commit.

## What Changes

- **Solução .NET 10** em `src/` e `tests/` com os projetos do Artigo IV.1 da constituição, mais `global.json`, `Directory.Build.props`, `Directory.Packages.props` e `.editorconfig` (specification 9).
- **Shared Kernel**: `Entity`, `AggregateRoot`, `IDomainEvent`, `Result`, `Error`, `ValidationError` e os IDs compartilhados `UsuarioId`, `PacienteId` e `DentistaId` (specification 3.8 e 5.1).
- **Messaging da aplicação**: `ICommand`/`IQuery`, handlers, registro por varredura e decorators de logging e validação (specification 4.1 e 4.2).
- **API base** em Minimal APIs:
  - conversão de `Error` em ProblemDetails;
  - handler global de exceções;
  - OpenAPI com Scalar apenas em desenvolvimento;
  - HTTPS/HSTS e CORS (specification 5.2, 6 e 8.5).
- **Persistência base**:
  - convenções para `DbContext` por módulo (snake_case, conversão de IDs, schema e histórico de migrations por módulo);
  - `IUnitOfWork`;
  - interceptor que despacha eventos de domínio após o commit (specification 3.5 e 7).
- **Transversais**: `TimeProvider`, Options validadas na inicialização, OpenTelemetry, health checks e User Secrets (specification 8).
- **Web base**: Blazor Web App (Interactive Auto) com Radzen, layout, navegação e cliente HTTP tipado que mostra a disponibilidade da API.
- **Infraestrutura de testes**: unidade, integração com PostgreSQL real (Testcontainers + Respawn + `WebApplicationFactory`), componentes (bUnit) e ponta a ponta (Playwright), cada um com testes de fumaça.
- **Testes de arquitetura** com todas as regras da specification 10.2.
- **Documentação**:
  - README com como rodar a solução;
  - registro da divisão da Fase 1 no `Docs/plan.md`, `Docs/tasks.md` e `Docs/checklist.md`.

**Módulo (Artigo IV da constituição):** nenhum bounded context; base transversal para todos.

**Plano:** [ficha `add-solution-skeleton`](../../../Docs/plan.md#add-solution-skeleton) e etapa [P1.E1](../../../Docs/tasks.md#p1e1--add-solution-skeleton).

**Desvio em relação ao plano:** a etapa tinha 12 tarefas macro, acima do limite de 10 grupos do `Docs/plan.md`, e foi dividida por decisão do responsável:
- **P1.E1.T8 (Docker) e P1.E1.T11 (CI)** saem desta change e formam a nova etapa **P1.E2, `add-ci-pipeline`**. Nela também entra a validação do título do pull request, prevista em `update-specification-branches`.
- **Critérios da ficha:** "`docker compose up` sobe API e Web" e "CI em todo pull request" passam para a ficha de `add-ci-pipeline`.
- **Nesta change:** a API e a Web rodam com `dotnet run`, usando o PostgreSQL do compose.

## Capabilities

### New Capabilities
- `platform/health-checks`: endpoints de vivacidade e prontidão da API, incluindo a verificação do banco.
- `platform/api-errors`: contrato de respostas de erro da API (ProblemDetails, status HTTP por tipo de erro, ausência de detalhes internos fora de desenvolvimento).
- `platform/api-documentation`: documento OpenAPI e interface de documentação disponíveis apenas em desenvolvimento.
- `platform/web-shell`: aplicação Web com layout base, navegação e indicação da disponibilidade da API.

### Modified Capabilities
<!-- Nenhuma: ainda não há specs em openspec/specs/. -->

## Impact

- **Código novo:** toda a solução em `src/` e `tests/`, além de `OdontoManager.slnx`, `global.json`, `Directory.Build.props`, `Directory.Packages.props` e `.editorconfig`.
- **Dependências:** os pacotes do catálogo da specification 11. Não há pacote fora do catálogo; se algum se mostrar necessário durante a implementação, será justificado no design.
- **Arquivos alterados:** `README.md`, `.gitignore` (artefatos de build) e os três documentos de planejamento.
- **Ambiente de desenvolvimento:** exige o .NET SDK 10 e o Docker (para o PostgreSQL e para o Testcontainers). Os testes ponta a ponta exigem os navegadores do Playwright.
- **Changes seguintes:** `add-ci-pipeline` (P1.E2) containeriza e automatiza o que esta change entrega; `add-identity-access` (P2.E1) é a primeira a usar os building blocks.
