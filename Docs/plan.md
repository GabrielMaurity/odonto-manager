# Plano de Implementação do Odonto Manager

Este é o planejamento da implementação: **o quê** será construído, **em que ordem** e **por quê**. É um documento orientativo e vivo, subordinado à [constituição](constitution.md) e ao [`specification.md`](specification.md), que são normativos. Ele trabalha junto com outros dois documentos:

| Documento | Responde | Conteúdo |
|-----------|----------|----------|
| `plan.md` (este) | O quê e por quê? | Fases, marcos, dependências, fichas das etapas, transversais, Definição de Pronto |
| [`tasks.md`](tasks.md) | Como quebrar? | Tarefas macro de cada etapa, numeradas por parte e etapa |
| [`checklist.md`](checklist.md) | Onde estamos? | Status das entregas do projeto |

Cada informação fica em um só desses documentos: as fichas abaixo não listam entregas (estão no `tasks.md`) nem status (está no `checklist.md`).

**Sumário**

1. [Fases e marcos](#1-fases-e-marcos)
2. [Dependências entre etapas](#2-dependências-entre-etapas)
3. [Fichas das etapas](#3-fichas-das-etapas)
4. [Requisitos transversais](#4-requisitos-transversais)
5. [Definição de Pronto](#5-definição-de-pronto)
6. [Manutenção dos documentos de planejamento](#6-manutenção-dos-documentos-de-planejamento)

---

## 1. Fases e marcos

As fases não têm datas; o esforço de cada etapa é indicado pelo tamanho relativo da ficha (P, M ou G).

| Fase | Etapas (changes) | Marco ao final |
|------|------------------|----------------|
| **0. Fundação** | `add-project-constitution`, `amend-constitution-docker`, `amend-constitution-ddd`, `update-specification-commits`, `amend-constitution-docs-folder`, `add-implementation-plan`, `update-specification-branches` | Regras do projeto e planejamento definidos |
| **1. Base técnica** | `add-solution-skeleton`, `add-ci-pipeline` | A solução compila, testes e CI ficam verdes, API e Web sobem no Docker |
| **2. MVP** | `add-identity-access`, `add-patient-registry`, `add-dentist-profile`, `add-appointment-scheduling` | 🎯 **MVP**: a recepção agenda consultas, e o dentista vê e gerencia a própria agenda |
| **3. Expansão** | `add-material-requests`, `add-exam-files` | Escopo completo do Artigo I da constituição |

**MVP.** É a primeira versão utilizável numa clínica. Com ele é possível:
- usuários entram com seu perfil;
- a recepção cadastra pacientes e marca, desmarca e remarca consultas de qualquer dentista;
- o dentista vê seu painel com a própria agenda e gerencia suas consultas.

Pedidos de material e exames ficam para a Fase 3.

A ordem das etapas segue o Artigo IX da constituição.

## 2. Dependências entre etapas

```
add-solution-skeleton
        │
        ▼
add-ci-pipeline
        │
        ▼
add-identity-access
        │
        ├──────────────────────┐
        ▼                      ▼
add-patient-registry    add-dentist-profile      ← podem ser feitas em paralelo
        │                      │
        └──────────┬───────────┘
                   ▼
       add-appointment-scheduling                 🎯 MVP
                   │
        ┌──────────┴───────────┐
        ▼                      ▼
add-material-requests    add-exam-files
```

- `add-material-requests` depende de `add-dentist-profile` (o dentista é o solicitante) e acrescenta um bloco ao painel. Ela vem depois da agenda pela ordem do Artigo IX, não por dependência técnica.
- `add-exam-files` depende de `add-patient-registry` e de `add-appointment-scheduling`: a autorização por recurso (constituição, Artigo VI.1) verifica se o dentista tem ou teve consulta com o paciente.

## 3. Fichas das etapas

Tamanho: **P** = até 3 grupos de tarefas; **M** = 4 a 6; **G** = 7 ou mais. Se uma etapa passar de 10 grupos no propose, ela deve ser dividida, e a divisão registrada nos três documentos.

### Fase 0 — Fundação

| Change | Objetivo | Tamanho |
|--------|----------|---------|
| `add-project-constitution` | Constituição com princípios, stack, arquitetura, perfis, segurança e roadmap | M |
| `amend-constitution-docker` | Docker Compose com PostgreSQL e pgAdmin para o ambiente local | M |
| `amend-constitution-ddd` | DDD na constituição e criação do `specification.md` | G |
| `update-specification-commits` | Commits semânticos e hook `commit-msg` | M |
| `amend-constitution-docs-folder` | Documentos do projeto reunidos em `Docs/` | P |
| `add-implementation-plan` | Este plano, o `tasks.md` e o `checklist.md` | M |
| `update-specification-branches` | Branch `main` e fluxo de uma branch por trabalho, com hook `pre-push` | M |

Tarefas: [Parte 0 do `tasks.md`](tasks.md#parte-0--fundação).

### Fase 1 — Base técnica

#### `add-solution-skeleton`
- **Objetivo:** criar a solução .NET e toda a infraestrutura comum, para que os módulos de negócio comecem só pelo domínio.
- **Bounded context:** nenhum (transversal).
- **Depende de:** Fase 0.
- **Critérios de pronto:**
  - `dotnet build` sem warnings;
  - todos os testes passam, inclusive os de arquitetura;
  - `/health/ready` responde 200;
  - a Web abre o layout base com o Radzen;
  - API e Web rodam localmente com `dotnet run`.
- **Riscos:**
  - a escolha de render mode Interactive Auto pode exigir ajustes (constituição III; design da `add-project-constitution`).
- **Tamanho:** G.
- **Divisão:** a etapa original tinha 12 tarefas macro, acima do limite de 10 grupos; Docker e CI foram para `add-ci-pipeline` (proposal de `add-solution-skeleton`).
- **Tarefas:** [P1.E1](tasks.md#p1e1--add-solution-skeleton).

#### `add-ci-pipeline`
- **Objetivo:** containerizar a API e a Web e automatizar a verificação de todo pull request.
- **Bounded context:** nenhum (transversal).
- **Depende de:** `add-solution-skeleton`.
- **Critérios de pronto:**
  - `docker compose up` sobe PostgreSQL, API e Web;
  - a CI roda em todo pull request: build, analisadores, todos os testes (inclusive os E2E, com instalação do navegador) e validação das mensagens de commit e do título do PR;
  - a proteção da `main` exige os checks da CI (specification 14.5).
- **Riscos:** tempo da CI com Testcontainers e Playwright; cache de pacotes NuGet e de navegadores.
- **Tamanho:** M.
- **Tarefas:** [P1.E2](tasks.md#p1e2--add-ci-pipeline).

### Fase 2 — MVP

#### `add-identity-access`
- **Objetivo:** autenticação e autorização pelos perfis Dentista, Recepcionista/Secretária e Administrador.
- **Bounded context:** Identidade e Acesso.
- **Depende de:** `add-solution-skeleton`.
- **Critérios de pronto:**
  - login e logout funcionando na Web e na API;
  - cada perfil acessa só o que lhe compete, com teste de acesso negado em cada política;
  - sessão expira por inatividade;
  - rate limiting no login;
  - o Administrador gerencia usuários e perfis;
  - a seção 8.5 do `specification.md` foi atualizada com o mecanismo escolhido.
- **Riscos:** autenticação entre Blazor Interactive Auto (servidor e WebAssembly) e a API, que precisa funcionar nos dois modos; decidir cookie versus token no design da change.
- **Tamanho:** G.
- **Tarefas:** [P2.E1](tasks.md#p2e1--add-identity-access).

#### `add-patient-registry`
- **Objetivo:** cadastro de pacientes com os dados necessários ao atendimento.
- **Bounded context:** Pacientes.
- **Depende de:** `add-identity-access`.
- **Critérios de pronto:**
  - a recepção cadastra, edita, busca e lista pacientes;
  - CPF validado e único;
  - exportação e correção dos dados do paciente a pedido dele (constituição VI.6);
  - nenhum dado pessoal aparece em logs.
- **Riscos:** coletar mais dados que o necessário (LGPD); duplicidade de cadastros.
- **Tamanho:** M.
- **Tarefas:** [P2.E2](tasks.md#p2e2--add-patient-registry).

#### `add-dentist-profile`
- **Objetivo:** cadastro do dentista e o **painel do dentista**, ponto de entrada do profissional no sistema.
- **Bounded context:** Profissionais.
- **Depende de:** `add-identity-access`.
- **Critérios de pronto:**
  - o Administrador cadastra dentistas (CRO, especialidades) vinculados a usuários;
  - o dentista mantém o próprio perfil e sua disponibilidade de atendimento;
  - o painel "Meu painel" existe, com estrutura em blocos para os módulos seguintes.
- **Riscos:** o painel ser desenhado sem os blocos futuros; mitigado com a estrutura em blocos independentes (seção 4).
- **Tamanho:** M.
- **Tarefas:** [P2.E3](tasks.md#p2e3--add-dentist-profile).

#### `add-appointment-scheduling`
- **Objetivo:** agenda de consultas: marcar, desmarcar e remarcar.
- **Bounded context:** Agenda.
- **Depende de:** `add-patient-registry` e `add-dentist-profile`.
- **Critérios de pronto:**
  - a recepção marca, desmarca e remarca consultas de qualquer dentista;
  - o dentista vê e gerencia a própria agenda no painel;
  - não há sobreposição de consultas, nem com requisições simultâneas (teste de concorrência);
  - a disponibilidade do dentista é respeitada;
  - horários exibidos no fuso da clínica.
- **Riscos:**
  - regras de agenda subestimadas (encaixes, faltas, duração variável por procedimento);
  - limitações do Scheduler do Radzen: validar cedo e encapsular (constituição IV.7).
- **Tamanho:** G.
- **Tarefas:** [P2.E4](tasks.md#p2e4--add-appointment-scheduling).

### Fase 3 — Expansão

#### `add-material-requests`
- **Objetivo:** pedidos de material feitos pelo dentista, com aprovação do Administrador.
- **Bounded context:** Materiais.
- **Depende de:** `add-dentist-profile`.
- **Critérios de pronto:**
  - o dentista cria e acompanha pedidos pelo painel;
  - o Administrador aprova, recusa e marca como entregue;
  - transições de status inválidas são rejeitadas;
  - concorrência na aprovação é tratada.
- **Riscos:** confusão entre pedido de material e controle de estoque; estoque está fora do escopo (constituição I.4) até uma emenda.
- **Tamanho:** M.
- **Tarefas:** [P3.E1](tasks.md#p3e1--add-material-requests).

#### `add-exam-files`
- **Objetivo:** upload, armazenamento seguro e visualização de arquivos de exames.
- **Bounded context:** Exames.
- **Depende de:** `add-patient-registry` e `add-appointment-scheduling`.
- **Critérios de pronto:**
  - a recepção anexa exames ao paciente;
  - o dentista vê no painel apenas os exames de pacientes que atende ou atendeu;
  - arquivos ficam criptografados em repouso e são servidos só por endpoint autenticado;
  - todo acesso gera registro de auditoria imutável;
  - existe limite de tamanho e de tipos de arquivo.
- **Riscos:** escolha do armazenamento (disco, volume ou objeto compatível com S3) e da gestão de chaves; arquivos grandes no Blazor WebAssembly.
- **Tamanho:** G.
- **Tarefas:** [P3.E2](tasks.md#p3e2--add-exam-files).

## 4. Requisitos transversais

Requisitos da constituição e do `specification.md` que não pertencem a um único módulo, cada um com a etapa responsável:

| Requisito | Origem | Etapa responsável |
|-----------|--------|-------------------|
| Pipeline de CI (build, analisadores, testes, validação de commits) | Constituição VII.6; specification 13 | `add-ci-pipeline` |
| Dockerfiles e serviços `api`/`web` no compose | Constituição III.3; specification 7.5 | `add-ci-pipeline` |
| Building blocks (Shared Kernel, messaging, decorators, ProblemDetails) | specification 3.8, 4.1, 4.2, 5 | `add-solution-skeleton` |
| Observabilidade, health checks e configuração validada | specification 8.2 a 8.4 | `add-solution-skeleton` |
| Testes de arquitetura | specification 10.2 | `add-solution-skeleton` |
| Infraestrutura de testes (Testcontainers, Respawn, `WebApplicationFactory`, bUnit, Playwright) | Constituição II.3, VII.3; specification 10.1 | `add-solution-skeleton` |
| Convenções de código (`.editorconfig`, `Nullable`, warnings como erros, analisadores) | Constituição VII.1; specification 9 | `add-solution-skeleton` |
| Segredos fora do repositório (User Secrets, variáveis de ambiente) | Constituição VI.4; specification 8.4 | `add-solution-skeleton` |
| Logs sem dados pessoais | Constituição VI.7; specification 8.2 | `add-solution-skeleton` (decorator de logging) e cada módulo |
| Commits semânticos | Constituição VII.4; specification 13 | `update-specification-commits` (hook) e `add-ci-pipeline` (CI) |
| Autenticação, perfis, políticas, sessão e rate limiting | Constituição V, VI.5; specification 8.5 | `add-identity-access` |
| HTTPS, HSTS e CORS | Constituição VI.3; specification 8.5 | `add-solution-skeleton` (base) e `add-identity-access` (ajuste com o frontend) |
| Autorização por recurso | Constituição VI.1 | Cada módulo, a partir de `add-patient-registry` |
| Exportação e correção de dados do paciente | Constituição VI.6 | `add-patient-registry` |
| Painel do dentista | Pedido do responsável; constituição V | Estrutura em `add-dentist-profile`; um bloco em cada um de `add-appointment-scheduling`, `add-material-requests` e `add-exam-files` |
| Concorrência e não sobreposição de horários | specification 3.7, 7.3 | `add-appointment-scheduling` |
| Tempo e fuso horário da clínica | specification 8.1 | `add-solution-skeleton` (`TimeProvider`) e `add-appointment-scheduling` (regras de calendário) |
| Trilha de auditoria de dados clínicos | Constituição VI.2 | `add-exam-files`, primeiro módulo com dado clínico |
| Criptografia de arquivos em repouso | Constituição VI.3 | `add-exam-files` |
| Testes ponta a ponta dos fluxos críticos | Constituição II.3; specification 10.1 | `add-appointment-scheduling`, `add-material-requests` e `add-exam-files` |
| Glossário da linguagem ubíqua | Constituição II.7; specification 2.3 | Cada módulo, para os próprios termos |

Pacientes e Agenda têm dados pessoais, mas não dados clínicos; por isso a auditoria do Artigo VI.2 começa em `add-exam-files`. Nomenclatura em pt-BR (constituição VII.2) e fluxo de branches e pull requests (VII.5) valem para todas as etapas, sem uma responsável específica.

## 5. Definição de Pronto

Uma etapa só é considerada concluída quando:

1. todas as tarefas do `tasks.md` da change estão marcadas e `openspec validate <change>` passa;
2. build, analisadores e todos os testes passam localmente e, a partir da Fase 1, na CI;
3. os testes de arquitetura passam;
4. os critérios de pronto da ficha (seção 3) foram atendidos;
5. o glossário do `specification.md` (2.3) foi atualizado, se a etapa introduz termos novos;
6. o [`tasks.md`](tasks.md) e o [`checklist.md`](checklist.md) foram atualizados;
7. a change foi arquivada com `/opsx:archive`, com as specs sincronizadas em `openspec/specs/`.

## 6. Manutenção dos documentos de planejamento

- **Atualização de rotina**, feita dentro de cada change, sem change própria. No último grupo de tarefas, toda change:
  - marca suas entregas e acrescenta novas necessidades no `checklist.md`;
  - atualiza os links da sua etapa no `tasks.md`, inclusive para o caminho em `openspec/changes/archive/` depois do arquivamento.
- **Mudança de planejamento** exige uma change `update-plan-<tema>`. Isso vale para:
  - incluir, remover ou reordenar etapas;
  - mudar o escopo de uma fase ou do MVP;
  - alterar uma ficha.
- **Desvios no propose:** uma change pode refinar sua ficha e suas tarefas macro, desde que registre o desvio no `proposal.md`.
- **Conflito com a constituição:** reordenar as etapas do Artigo IX segue o próprio Artigo IX. Incluir funcionalidade fora do Artigo I exige emenda.
