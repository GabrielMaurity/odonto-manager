# Design

## Context

Estado atual: as quatro changes de fundação estão concluídas (aguardando arquivamento). Ainda não há código .NET, pipeline de CI nem specs em `openspec/specs/`. A ordem das entregas está no Artigo IX da constituição; a arquitetura e os padrões estão no Artigo IV e no `specification.md` (seções 2 e 9). Decisões do responsável: fases **sem datas** e MVP = **agenda funcionando**. A motivação está em proposal.md — Why.

## Goals / Non-Goals

**Goals:**
- Cada change do roadmap ter escopo inicial, dependências e critérios de pronto antes do `/opsx:propose`.
- Nenhum requisito transversal da constituição ficar sem change responsável.
- Ter um único lugar para consultar o andamento do projeto.

**Non-Goals:**
- Datas, sprints e estimativas em horas.
- Requisitos detalhados de cada módulo: ficam nas specs da própria change. As fichas do plano são escopo inicial, refinado no propose.
- Funcionalidades fora do Artigo I (financeiro, convênios, portal do paciente).

## Decisions

### D1. Papel do `plan.md`: planejamento vivo, não normativo
A constituição e o `specification.md` são normativos; o `plan.md` é **orientativo** e muda com frequência (o status, por exemplo). Por isso ele não usa DEVE/NÃO DEVE para regras de produto, e alterá-lo não exige change dedicada: cada change atualiza sua própria ficha e o quadro de acompanhamento. Mudanças de escopo de fase ou de MVP, porém, exigem uma change `update-plan-<tema>`.
- *Alternativa*: incorporar o plano ao Artigo IX. Rejeitada: toda atualização de status viraria emenda.

### D2. Fases e MVP
| Fase | Changes | Marco ao final |
|------|---------|----------------|
| 0. Fundação | `add-project-constitution`, `amend-constitution-docker`, `amend-constitution-ddd`, `update-specification-commits` | Regras do projeto definidas (concluída) |
| 1. Base técnica | `add-solution-skeleton` | Solução compila, testes e CI verdes, API e Web sobem no Docker |
| 2. MVP | `add-identity-access`, `add-patient-registry`, `add-dentist-profile`, `add-appointment-scheduling` | **MVP**: a recepção agenda, e o dentista vê e gerencia sua agenda |
| 3. Expansão | `add-material-requests`, `add-exam-files` | Escopo completo do Artigo I |

A ordem do Artigo IX é mantida. `add-patient-registry` e `add-dentist-profile` só dependem de `add-identity-access` e PODEM ser desenvolvidas em paralelo.

### D3. Ficha padrão por change
Cada change tem: **objetivo**, **bounded context**, **entregas iniciais**, **depende de**, **critérios de pronto** específicos, **riscos** e **tamanho** (P = até 3 grupos de tarefas, M = 4 a 6, G = 7 ou mais, medidos pelo `tasks.md` previsto). O tamanho relativo dispensa datas e ainda permite comparar esforço.

### D4. Responsável por cada requisito transversal
Requisitos da constituição que não pertencem a um único módulo ganham uma change responsável explícita:

| Requisito | Origem | Change responsável |
|-----------|--------|--------------------|
| Pipeline de CI (build, analisadores, testes, validação de commits) | Constituição VII.6; specification 13 | `add-solution-skeleton` |
| Dockerfiles e serviços `api`/`web` no compose | specification 7.5; `amend-constitution-docker` | `add-solution-skeleton` |
| Building blocks (SharedKernel, messaging, decorators, ProblemDetails) | specification 3.8, 4.1, 5 | `add-solution-skeleton` |
| Testes de arquitetura | specification 10.2 | `add-solution-skeleton` |
| Autenticação, perfis, políticas, rate limiting | Constituição V, VI.5; specification 8.5 | `add-identity-access` |
| Exportação e correção de dados do paciente | Constituição VI.6 | `add-patient-registry` |
| Painel do dentista ("Meu painel") | Pedido do responsável; constituição V | Estrutura em `add-dentist-profile`; cada módulo seguinte acrescenta seu bloco |
| Concorrência e não sobreposição de horários | specification 3.7, 7.3 | `add-appointment-scheduling` |
| Trilha de auditoria de dados clínicos | Constituição VI.2 | `add-exam-files`, o primeiro módulo com dado clínico |
| Criptografia de arquivos em repouso | Constituição VI.3 | `add-exam-files` |

O painel do dentista é a dependência mais sutil: o perfil (Profissionais) vem antes da agenda, mas o pedido inicial era um painel onde o dentista vê consultas, pede material e acessa exames. A solução é `add-dentist-profile` criar o painel com os dados do próprio dentista, e cada módulo posterior acrescentar seu bloco nele.

### D5. Definição de Pronto comum
Uma change só é concluída quando:
- todas as tarefas estão marcadas e o `openspec validate` passa;
- build, analisadores e todos os testes passam (localmente e, a partir da Fase 1, na CI);
- os testes de arquitetura passam;
- os critérios de pronto da ficha foram atendidos;
- o glossário (specification 2.3) foi atualizado, quando a change introduz termos novos;
- o quadro de acompanhamento do `plan.md` foi atualizado;
- a change foi arquivada (`/opsx:archive`), com as specs sincronizadas em `openspec/specs/`.

### D6. Integração com o fluxo OpenSpec
Duas regras novas no `openspec/config.yaml`:
- `proposal`: citar a ficha do `plan.md` correspondente e justificar qualquer desvio de escopo em relação a ela;
- `tasks`: incluir, no último grupo, a tarefa de atualizar o quadro de acompanhamento do `plan.md`.

Assim o plano se mantém atualizado sem depender de memória.

## Risks / Trade-offs

- [Plano desatualizar em relação às changes] → A regra de `tasks` (D6) e a Definição de Pronto (D5) obrigam a atualização a cada change.
- [Fichas detalhadas demais engessarem o propose] → As fichas são escopo **inicial**: o propose pode refiná-las, desde que registre o desvio na proposal.
- [`add-solution-skeleton` ficar grande demais (G)] → A ficha lista as entregas em blocos independentes; se o propose identificar mais de 10 grupos de tarefas, a change DEVE ser dividida (ex.: `add-ci-pipeline` separado), registrando a divisão no `plan.md`.
- [Auditoria só chegar na Fase 3] → Pacientes e Agenda têm dados pessoais, mas não clínicos (Artigo VI.2 cobre dados clínicos e exames). Os logs técnicos sem dados pessoais (specification 8.2) valem desde o início.
