# Tarefas da Implementação do Odonto Manager

Este documento quebra o [plano](plan.md) em **partes** (as fases), **etapas** (as changes do OpenSpec) e **tarefas macro**. Ele é o ponto de partida de cada `/opsx:propose`. O andamento das entregas fica no [`checklist.md`](checklist.md).

> **`Docs/tasks.md` × `tasks.md` das changes.** Este arquivo lista as tarefas *macro* do projeto inteiro. Cada change do OpenSpec tem o seu próprio `openspec/changes/<change>/tasks.md`, com as tarefas *detalhadas* daquela etapa, e esse é o documento oficial durante a implementação. Quando a change é criada, a etapa daqui ganha o link para ela; as tarefas macro não são reescritas com o detalhe.

**Numeração.** `P<parte>.E<etapa>.T<tarefa>`: por exemplo, `P1.E1.T3` é a terceira tarefa macro da primeira etapa da Parte 1. Os números são estáveis. Uma tarefa removida mantém o número, riscada, e tarefas novas recebem o próximo número livre.

**Sumário**

- [Parte 0 — Fundação](#parte-0--fundação)
- [Parte 1 — Base técnica](#parte-1--base-técnica)
- [Parte 2 — MVP](#parte-2--mvp)
- [Parte 3 — Expansão](#parte-3--expansão)

---

## Parte 0 — Fundação

Todas as etapas desta parte estão concluídas.

| Etapa | Change | Detalhamento |
|-------|--------|--------------|
| P0.E1 | `add-project-constitution` | [tasks.md](../openspec/changes/add-project-constitution/tasks.md) |
| P0.E2 | `amend-constitution-docker` | [tasks.md](../openspec/changes/amend-constitution-docker/tasks.md) |
| P0.E3 | `amend-constitution-ddd` | [tasks.md](../openspec/changes/amend-constitution-ddd/tasks.md) |
| P0.E4 | `update-specification-commits` | [tasks.md](../openspec/changes/update-specification-commits/tasks.md) |
| P0.E5 | `amend-constitution-docs-folder` | [tasks.md](../openspec/changes/amend-constitution-docs-folder/tasks.md) |
| P0.E6 | `add-implementation-plan` | [tasks.md](../openspec/changes/add-implementation-plan/tasks.md) |
| P0.E7 | `update-specification-branches` | [tasks.md](../openspec/changes/update-specification-branches/tasks.md) |

## Parte 1 — Base técnica

### P1.E1 — `add-solution-skeleton`

Ficha: [plano](plan.md#add-solution-skeleton). Detalhamento: a criar no `/opsx:propose`.

| Tarefa | Entrega |
|--------|---------|
| P1.E1.T1 | Solução `OdontoManager.slnx`, `global.json`, `Directory.Build.props`, `Directory.Packages.props` e `.editorconfig`, com os projetos de `src/` e `tests/` (specification 9) |
| P1.E1.T2 | Shared Kernel: `Entity`, `AggregateRoot`, `IDomainEvent`, `Result`, `Error`, `ValidationError` e IDs compartilhados, com testes de unidade (specification 3.8, 5.1) |
| P1.E1.T3 | Messaging: `ICommand`/`IQuery`, handlers, registro com Scrutor e decorators de logging e validação, com testes (specification 4.1, 4.2) |
| P1.E1.T4 | API base: Minimal APIs, conversão de `Error` em ProblemDetails, handler global de exceções, OpenAPI e Scalar em desenvolvimento, HTTPS, HSTS e CORS (specification 5.2, 6, 8.5) |
| P1.E1.T5 | Persistência base: convenções de `DbContext` por módulo, snake_case, conversão de IDs, interceptor de eventos de domínio pós-commit e `IUnitOfWork` (specification 3.5, 7) |
| P1.E1.T6 | Transversais: `TimeProvider`, Options validadas na inicialização, OpenTelemetry, health checks `/health/live` e `/health/ready`, User Secrets (specification 8) |
| P1.E1.T7 | Web base: Blazor Web App com Interactive Auto, Radzen, layout, navegação por perfil (estrutura) e cliente HTTP tipado para a API (constituição III, IV.3) |
| P1.E1.T8 | Docker: Dockerfiles de `api` e `web` e os serviços no `docker-compose.yaml`, com `depends_on` saudável do `postgres` (constituição III.3) |
| P1.E1.T9 | Infraestrutura de testes: `WebApplicationFactory` com Testcontainers e Respawn, projeto bUnit e projeto Playwright, cada um com um teste de fumaça (specification 10.1) |
| P1.E1.T10 | Testes de arquitetura com todas as regras da specification 10.2 |
| P1.E1.T11 | CI no GitHub Actions: build, analisadores, todos os testes e validação das mensagens de commit com `.githooks/commit-msg` (constituição VII.6; specification 13) |
| P1.E1.T12 | Documentação: README com como rodar a solução; atualização de `tasks.md` e `checklist.md` |

## Parte 2 — MVP

### P2.E1 — `add-identity-access`

Ficha: [plano](plan.md#add-identity-access). Detalhamento: a criar no `/opsx:propose`.

| Tarefa | Entrega |
|--------|---------|
| P2.E1.T1 | Decisão do mecanismo de autenticação para Blazor Interactive Auto + API (cookie ou token) no design da change e atualização da specification 8.5 |
| P2.E1.T2 | Domínio de Identidade: `Usuario`, `Perfil`, regras de ativação e desativação, com testes de unidade |
| P2.E1.T3 | ASP.NET Core Identity com schema `identidade`, hash de senha e expiração de sessão por inatividade |
| P2.E1.T4 | Políticas de autorização por perfil e política padrão que nega acesso anônimo, com teste de acesso negado para cada política |
| P2.E1.T5 | Endpoints de login, logout e usuário atual, com rate limiting no login e testes de integração |
| P2.E1.T6 | Gestão de usuários e perfis pelo Administrador (API e telas) |
| P2.E1.T7 | Telas de login e navegação por perfil na Web, com testes bUnit e teste E2E de login |
| P2.E1.T8 | Glossário de Identidade e Acesso; atualização de `tasks.md` e `checklist.md` |

### P2.E2 — `add-patient-registry`

Ficha: [plano](plan.md#add-patient-registry). Detalhamento: a criar no `/opsx:propose`.

| Tarefa | Entrega |
|--------|---------|
| P2.E2.T1 | Domínio de Pacientes: agregado `Paciente` e objetos de valor `Cpf`, `Email` e `Telefone`, com testes de unidade |
| P2.E2.T2 | Persistência no schema `pacientes`, CPF único no banco e migration |
| P2.E2.T3 | Cadastrar, editar, buscar e listar pacientes (comandos, consultas e endpoints), com autorização e testes de integração |
| P2.E2.T4 | Exportação e correção dos dados do paciente (constituição VI.6), com testes |
| P2.E2.T5 | `PublicApi` de Pacientes para outros módulos (existência e dados resumidos) |
| P2.E2.T6 | Telas de pacientes na Web, com testes bUnit |
| P2.E2.T7 | Glossário de Pacientes; atualização de `tasks.md` e `checklist.md` |

### P2.E3 — `add-dentist-profile`

Ficha: [plano](plan.md#add-dentist-profile). Detalhamento: a criar no `/opsx:propose`.

| Tarefa | Entrega |
|--------|---------|
| P2.E3.T1 | Domínio de Profissionais: agregado `Dentista`, `RegistroCro`, especialidades e `Disponibilidade`, com testes de unidade |
| P2.E3.T2 | Persistência no schema `profissionais` e migration |
| P2.E3.T3 | Cadastro de dentistas pelo Administrador, vinculado a um usuário, com autorização e testes de integração |
| P2.E3.T4 | O dentista mantém o próprio perfil e a disponibilidade de atendimento, com testes |
| P2.E3.T5 | `PublicApi` de Profissionais (dados do dentista e disponibilidade) e evento de alteração de disponibilidade |
| P2.E3.T6 | Painel do dentista ("Meu painel") com estrutura de blocos e o bloco de perfil, com testes bUnit |
| P2.E3.T7 | Glossário de Profissionais; atualização de `tasks.md` e `checklist.md` |

### P2.E4 — `add-appointment-scheduling`

Ficha: [plano](plan.md#add-appointment-scheduling). Detalhamento: a criar no `/opsx:propose`.

| Tarefa | Entrega |
|--------|---------|
| P2.E4.T1 | Domínio da Agenda: agregado `Consulta`, `IntervaloDeHorario`, `StatusDaConsulta` e regras de agendar, cancelar, remarcar, confirmar, realizar e falta, com testes de unidade |
| P2.E4.T2 | Serviço de domínio de não sobreposição e respeito à disponibilidade do dentista, via `PublicApi` de Profissionais |
| P2.E4.T3 | Persistência no schema `agenda`, exclusion constraint contra sobreposição, concorrência por `xmin` e migration, com teste de concorrência |
| P2.E4.T4 | Comandos, consultas e endpoints de marcar, desmarcar, remarcar e listar agenda por dentista e período, com autorização e testes de integração |
| P2.E4.T5 | Regras de calendário no fuso da clínica (dia, semana e horário de funcionamento) |
| P2.E4.T6 | Agenda da recepção na Web: Scheduler do Radzen encapsulado em componente próprio, para todos os dentistas, com testes bUnit |
| P2.E4.T7 | Bloco "Minhas consultas" no painel do dentista, com marcar e desmarcar |
| P2.E4.T8 | Testes E2E dos fluxos de marcar, desmarcar e remarcar |
| P2.E4.T9 | Glossário da Agenda; atualização de `tasks.md` e `checklist.md` (marco MVP) |

## Parte 3 — Expansão

### P3.E1 — `add-material-requests`

Ficha: [plano](plan.md#add-material-requests). Detalhamento: a criar no `/opsx:propose`.

| Tarefa | Entrega |
|--------|---------|
| P3.E1.T1 | Domínio de Materiais: agregado `PedidoDeMaterial`, `ItemDoPedido`, `Quantidade` e transições de `StatusDoPedido`, com testes de unidade |
| P3.E1.T2 | Persistência no schema `materiais`, concorrência por `xmin` e migration |
| P3.E1.T3 | Comandos, consultas e endpoints de criar, acompanhar, aprovar, recusar e entregar pedidos, com autorização e testes de integração |
| P3.E1.T4 | Bloco "Meus pedidos de material" no painel do dentista e tela de aprovação do Administrador, com testes bUnit |
| P3.E1.T5 | Teste E2E do fluxo de pedido e aprovação |
| P3.E1.T6 | Glossário de Materiais; atualização de `tasks.md` e `checklist.md` |

### P3.E2 — `add-exam-files`

Ficha: [plano](plan.md#add-exam-files). Detalhamento: a criar no `/opsx:propose`.

| Tarefa | Entrega |
|--------|---------|
| P3.E2.T1 | Decisão do armazenamento de arquivos e da gestão de chaves de criptografia no design da change, com atualização do `specification.md` |
| P3.E2.T2 | Domínio de Exames: agregado `Exame`, `ArquivoDeExame` e tipos de exame, com testes de unidade |
| P3.E2.T3 | Armazenamento criptografado em repouso, limite de tamanho e validação de tipo de arquivo, com testes |
| P3.E2.T4 | Trilha de auditoria imutável de acesso a dados clínicos (constituição VI.2), reutilizável por outros módulos, com testes |
| P3.E2.T5 | Autorização por recurso: o dentista acessa só exames de pacientes que atende ou atendeu, via `PublicApi` da Agenda, com testes de acesso negado |
| P3.E2.T6 | Endpoints de upload, listagem e download autenticado (sem URL pública), com testes de integração |
| P3.E2.T7 | Upload de exames pela recepção e bloco "Exames dos meus pacientes" no painel do dentista, com testes bUnit |
| P3.E2.T8 | Teste E2E de upload e visualização, incluindo acesso negado |
| P3.E2.T9 | Glossário de Exames; atualização de `tasks.md` e `checklist.md` (escopo completo) |
