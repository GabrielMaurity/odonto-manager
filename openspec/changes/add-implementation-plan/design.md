# Design

## Context

Estado atual: as quatro changes de fundação estão concluídas (aguardando arquivamento). Ainda não há código .NET, pipeline de CI nem specs em `openspec/specs/`. A ordem das entregas está no Artigo IX da constituição; a arquitetura e os padrões estão no Artigo IV e no `specification.md` (seções 2 e 9). Decisões do responsável: fases **sem datas**, MVP = **agenda funcionando** e três documentos de planejamento em `Docs/` (`plan.md`, `tasks.md`, `checklist.md`). A motivação está em proposal.md — Why.

## Goals / Non-Goals

**Goals:**
- Cada change do roadmap ter escopo inicial, tarefas macro, dependências e critérios de pronto antes do `/opsx:propose`.
- Nenhum requisito transversal da constituição ficar sem change responsável.
- Um único lugar para consultar o andamento do projeto, que cresce junto com novas necessidades.
- Nenhuma informação duplicada entre os três documentos.

**Non-Goals:**
- Datas, sprints e estimativas em horas.
- Requisitos detalhados de cada módulo: ficam nas specs e no `tasks.md` da própria change.
- Funcionalidades fora do Artigo I (financeiro, convênios, portal do paciente).

## Decisions

### D1. Três documentos, uma informação em cada lugar
| Documento | Pergunta | Conteúdo | Não contém |
|-----------|----------|----------|------------|
| `Docs/plan.md` | O quê e por quê? | Fases, marcos, dependências, fichas, transversais, Definição de Pronto, manutenção | Lista de tarefas, status |
| `Docs/tasks.md` | Como quebrar? | Partes (fases) → etapas (changes) → tarefas macro numeradas | Critérios, riscos, status de entregas |
| `Docs/checklist.md` | Onde estamos? | Entregas do projeto como checkboxes, por fase e módulo, com a change de origem | Tarefas de implementação, justificativas |

Os três são **orientativos**: não usam DEVE/NÃO DEVE para regras de produto, ao contrário da constituição e do `specification.md`. Cada documento referencia os outros por link em vez de repetir conteúdo.
- *Alternativa*: um único `Docs/plan.md` com tudo. Rejeitada a pedido do responsável e porque o status muda com muito mais frequência que o planejamento.

### D2. Local e nomes dos arquivos
Os três documentos ficam em `Docs/`, junto com a constituição e o `specification.md` (constituição, Artigo I.3, emenda 1.2.1), com nomes em minúsculas, como os demais documentos. O `Docs/tasks.md` tem o mesmo nome do `tasks.md` de cada change do OpenSpec (`openspec/changes/<change>/tasks.md`), mas não há conflito: estão em pastas diferentes e têm papéis diferentes. O cabeçalho do `Docs/tasks.md` explica a relação entre os dois, e os textos sempre citam o caminho completo `Docs/tasks.md`.

### D3. Fases e MVP (`Docs/plan.md`)
| Fase | Changes | Marco ao final |
|------|---------|----------------|
| 0. Fundação | `add-project-constitution`, `amend-constitution-docker`, `amend-constitution-ddd`, `update-specification-commits`, `amend-constitution-docs-folder`, `add-implementation-plan` | Regras e planejamento definidos |
| 1. Base técnica | `add-solution-skeleton` | Solução compila, testes e CI verdes, API e Web sobem no Docker |
| 2. MVP | `add-identity-access`, `add-patient-registry`, `add-dentist-profile`, `add-appointment-scheduling` | **MVP**: a recepção agenda, e o dentista vê e gerencia sua agenda |
| 3. Expansão | `add-material-requests`, `add-exam-files` | Escopo completo do Artigo I |

A ordem do Artigo IX é mantida. `add-patient-registry` e `add-dentist-profile` só dependem de `add-identity-access` e PODEM ser desenvolvidas em paralelo.

### D4. Ficha padrão por change (`Docs/plan.md`)
Cada change tem: **objetivo**, **bounded context**, **depende de**, **critérios de pronto** específicos, **riscos** e **tamanho** (P = até 3 grupos de tarefas, M = 4 a 6, G = 7 ou mais). As entregas não ficam na ficha, e sim na etapa correspondente do `Docs/tasks.md` (D6).

### D5. Responsável por cada requisito transversal (`Docs/plan.md`)
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

### D6. Estrutura do `Docs/tasks.md`
- **Parte** = fase do `Docs/plan.md`; **etapa** = change; **tarefa macro** = bloco de trabalho que vira um ou mais grupos no `tasks.md` da change.
- Numeração estável `P<fase>.E<etapa>.T<tarefa>` (ex.: `P1.E1.T3`), para que proposals e o `Docs/checklist.md` possam referenciar tarefas sem ambiguidade.
- Cada etapa traz: a change correspondente, o link para o `tasks.md` dela (quando existir) e as tarefas macro, cada uma com uma linha dizendo o que entrega.
- Quando uma change é proposta, as tarefas macro da etapa são detalhadas no `tasks.md` da change; o `Docs/tasks.md` não é reescrito com o detalhe, apenas ganha o link.

### D7. Estrutura do `Docs/checklist.md`
- Agrupado por fase e, dentro de cada fase, por módulo ou área.
- Cada item é uma **entrega verificável** do ponto de vista do projeto (ex.: "Recepção marca consulta para qualquer dentista"), e não uma tarefa técnica. Cada item indica a change responsável e, quando concluído, a change que o entregou.
- Um resumo no topo mostra o progresso por fase (itens concluídos / total) e o marco MVP.
- **Novas necessidades**: um item novo entra na fase adequada com a marcação 🆕 e a change ou decisão que o originou. Se o item não couber em nenhuma change existente, ele também vai para a seção "Pendente de planejamento", até que `Docs/plan.md` e `Docs/tasks.md` o incorporem.
- **Itens removidos** não são apagados: são riscados com o motivo, para preservar o histórico de decisões.

### D8. Definição de Pronto comum (`Docs/plan.md`)
Uma change só é concluída quando:
- todas as tarefas estão marcadas e o `openspec validate` passa;
- build, analisadores e todos os testes passam (localmente e, a partir da Fase 1, na CI);
- os testes de arquitetura passam;
- os critérios de pronto da ficha foram atendidos;
- o glossário (specification 2.3) foi atualizado, quando a change introduz termos novos;
- o `Docs/tasks.md` e o `Docs/checklist.md` foram atualizados;
- a change foi arquivada (`/opsx:archive`), com as specs sincronizadas em `openspec/specs/`.

### D9. Manutenção e integração com o OpenSpec
- Atualizar status (`Docs/checklist.md`) e links (`Docs/tasks.md`) faz parte de cada change e não exige change própria.
- Mudar o escopo de uma fase, o MVP, a ordem das etapas ou as fichas exige uma change `update-plan-<tema>`.
- Regras novas no `openspec/config.yaml`:
  - `proposal`: citar a ficha do `Docs/plan.md` e a etapa do `Docs/tasks.md`, e justificar desvios de escopo;
  - `tasks`: incluir, no último grupo, a tarefa de atualizar o `Docs/tasks.md` (link e ajustes) e o `Docs/checklist.md` (itens entregues e novas necessidades).

## Risks / Trade-offs

- [Documentos desatualizarem em relação às changes] → A regra de `tasks` (D9) e a Definição de Pronto (D8) obrigam a atualização a cada change.
- [Duplicação entre os três documentos] → A tabela de D1 define o que cada um *não* contém; a tarefa final de verificação procura conteúdo repetido.
- [Confusão entre `Docs/tasks.md` (projeto) e `tasks.md` (change)] → Cabeçalho explicativo (D2) e links diretos entre eles (D6).
- [Tarefas macro detalhadas demais engessarem o propose] → São ponto de partida: o propose pode refiná-las, desde que registre o desvio na proposal.
- [`add-solution-skeleton` ficar grande demais (G)] → Se o propose identificar mais de 10 grupos de tarefas, a change DEVE ser dividida (ex.: `add-ci-pipeline`), registrando a divisão nos três documentos.
- [Auditoria só chegar na Fase 3] → Pacientes e Agenda têm dados pessoais, mas não clínicos (o Artigo VI.2 cobre dados clínicos e exames). Os logs técnicos sem dados pessoais (specification 8.2) valem desde o início.
