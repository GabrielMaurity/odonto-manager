# Proposal

## Why

A constituição fixa os princípios (SOLID, Clean Architecture, monólito modular), mas não define *como* modelar o domínio nem quais padrões e bibliotecas o backend usa. O responsável pelo projeto quer adotar DDD (Domain-Driven Design) no backend .NET 10, com PostgreSQL em Docker. Sem um documento técnico detalhado, cada módulo do roadmap decidiria sozinho sobre agregados, tratamento de erros, persistência e organização de pastas, o que gera inconsistência logo no esqueleto da solução.

## What Changes

- **Emenda MINOR na constituição (→ 1.2.0)**, aplicada depois da `amend-constitution-docker` (→ 1.1.0):
  - Artigo II ganha o princípio **DDD**: modelo de domínio rico, linguagem ubíqua e padrões táticos obrigatórios.
  - Artigo IV passa a tratar cada módulo como um **bounded context**.
  - Nova cláusula: `specification.md` é o documento técnico normativo e DEVE ser seguido; ele detalha a constituição e não pode contradizê-la.
  - Artigo VIII passa a exigir que o `design.md` cite também as seções do `specification.md` usadas.
- **Novo `specification.md` na raiz**, com a especificação técnica do backend e do banco:
  - Visão geral da stack (.NET 10, ASP.NET Core Minimal APIs, EF Core 10, PostgreSQL 18 em Docker).
  - DDD estratégico: bounded contexts, mapa de contextos e glossário da linguagem ubíqua em pt-BR.
  - DDD tático: agregados, entidades, objetos de valor, IDs fortemente tipados, eventos de domínio, repositórios e serviços de domínio.
  - Camada de aplicação: CQRS com handlers próprios (sem MediatR), validação, Result pattern e mapeamento manual.
  - API: Minimal APIs agrupadas por módulo, versionamento, ProblemDetails, OpenAPI.
  - Persistência: um `DbContext` e um schema PostgreSQL por módulo, configurações via Fluent API, migrations, concorrência otimista.
  - Aspectos transversais: tempo e fuso horário, logging e observabilidade, health checks, configuração e segredos.
  - Estrutura da solução e dos projetos, gestão central de pacotes e testes (incluindo testes de arquitetura).
  - Catálogo de pacotes NuGet aprovados, com licença.
  - Processo de atualização do `specification.md`.
- **`openspec/config.yaml`**: o `context` passa a citar o `specification.md` e o DDD.

**Módulo (Artigo IV):** transversal; afeta todos os módulos.

## Capabilities

### New Capabilities
<!-- Nenhuma: documentação técnica e de governança, sem mudança de comportamento do sistema (skip_specs: true). -->

### Modified Capabilities
<!-- Nenhuma. -->

## Impact

- **Arquivos novos:** `specification.md`.
- **Arquivos alterados:** `constitution.md` (Artigos II, IV, VIII, X e tabela de versão), `openspec/config.yaml`.
- **Dependência entre changes:** DEVE ser aplicada depois de `amend-constitution-docker`, para manter a sequência de versões 1.1.0 → 1.2.0. O `specification.md` usa o PostgreSQL do `docker-compose.yaml` definido naquela change.
- **Changes futuras:** `add-solution-skeleton` DEVE criar a estrutura de projetos, as abstrações do DDD tático e os testes de arquitetura descritos aqui.
