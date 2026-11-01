# Proposal

## Why

A fundação do projeto está pronta: constituição v1.2.0, `specification.md` v1.1.0, Docker e commits semânticos. O Artigo IX da constituição, porém, só lista a ordem das changes, em uma linha por módulo. Faltam três coisas: um planejamento que diga o que cada change entrega, do que depende e onde termina o MVP; uma quebra das etapas em tarefas, que sirva de ponto de partida para cada `/opsx:propose`; e um lugar único para ver o que já foi feito e o que falta. Sem isso, cada change futura redefiniria o escopo do zero, e lacunas transversais (auditoria, CI, Dockerfiles) ficariam sem dono.

## What Changes

Três documentos na raiz, com papéis distintos e sem conteúdo duplicado:

- **`plan.md`**: o *o quê e por quê*. Contém:
  - fases sem datas: Fase 0 (Fundação, concluída), Fase 1 (Base técnica), Fase 2 (MVP: agenda funcionando) e Fase 3 (Materiais e Exames);
  - marcos e grafo de dependências entre as changes;
  - uma ficha por change, com objetivo, bounded context, dependências, critérios de pronto, riscos e tamanho relativo (P/M/G);
  - a change responsável por cada requisito transversal da constituição;
  - a Definição de Pronto comum;
  - as regras de manutenção dos três documentos.
- **`Tasks.md`**: o *como quebrar*. Lista as tarefas da implementação por **partes** (fases) e **etapas** (changes), com as tarefas macro de cada etapa. É o backlog que alimenta o `/opsx:propose` de cada etapa; quando a change é criada, o `tasks.md` dela passa a ser o detalhamento oficial, e o `Tasks.md` aponta para ele.
- **`Checklist.md`**: o *onde estamos*. Um checklist das entregas do projeto, agrupado por fase e módulo, marcado conforme as changes são concluídas. Ele é ampliado sempre que surgir uma nova necessidade de implementação e registra a change de origem de cada item novo.
- **`openspec/config.yaml`**: novas regras para que toda proposal cite a ficha do `plan.md` e a etapa do `Tasks.md`, e para que toda change atualize o `Tasks.md` e o `Checklist.md` no último grupo de tarefas.
- **`README.md`**: links para os três documentos.

**Módulo (Artigo IV da constituição):** transversal, pois o planejamento cobre todos os módulos.

## Capabilities

### New Capabilities
<!-- Nenhuma: documentos de planejamento, sem mudança de comportamento do sistema (skip_specs: true). -->

### Modified Capabilities
<!-- Nenhuma. -->

## Impact

- **Arquivos novos:** `plan.md`, `Tasks.md` e `Checklist.md`.
- **Arquivos alterados:** `openspec/config.yaml` (regras de `proposal` e `tasks`) e `README.md`.
- **Constituição:** sem alteração. Os documentos mantêm a ordem do Artigo IX e só a detalham; mudanças de ordem seguem o próprio Artigo IX.
- **Changes futuras:** cada `/opsx:propose` parte da ficha do `plan.md` e da etapa do `Tasks.md`. Ao ser concluída, a change atualiza o `Tasks.md` e o `Checklist.md`.
