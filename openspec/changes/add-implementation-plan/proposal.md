# Proposal

## Why

A fundação do projeto está pronta: constituição v1.2.0, `specification.md` v1.1.0, Docker e commits semânticos. O Artigo IX da constituição, porém, só lista a ordem das changes, em uma linha por módulo. Falta um planejamento que diga o que cada change entrega, do que depende, quando está pronta e onde termina o MVP. Sem isso, cada `/opsx:propose` futuro redefiniria o escopo do zero, e lacunas transversais (auditoria, CI, Dockerfiles) ficariam sem dono.

## What Changes

- **Novo `plan.md` na raiz**: o planejamento vivo da implementação, contendo:
  - **fases**, sem datas: Fase 0 (Fundação, concluída), Fase 1 (Base técnica), Fase 2 (MVP: agenda funcionando) e Fase 3 (Materiais e Exames);
  - uma **ficha por change** do roadmap, com objetivo, entregas, dependências, critérios de pronto, riscos e tamanho relativo (P/M/G);
  - **marcos**, com o MVP definido como esqueleto + identidade + pacientes + perfil do dentista + agenda;
  - **grafo de dependências** entre as changes;
  - **responsável por cada requisito transversal** da constituição (auditoria, LGPD, CI, Dockerfiles, painel do dentista);
  - **Definição de Pronto** comum a todas as changes;
  - **quadro de acompanhamento** com o status de cada change;
  - regras de manutenção do próprio plano.
- **`openspec/config.yaml`**: novas regras para que toda proposal cite o item correspondente do `plan.md` e toda change inclua uma tarefa de atualizar o quadro de acompanhamento.
- **`README.md`**: link para o `plan.md`.

**Módulo (Artigo IV da constituição):** transversal, pois o planejamento cobre todos os módulos.

## Capabilities

### New Capabilities
<!-- Nenhuma: documento de planejamento, sem mudança de comportamento do sistema (skip_specs: true). -->

### Modified Capabilities
<!-- Nenhuma. -->

## Impact

- **Arquivos novos:** `plan.md`.
- **Arquivos alterados:** `openspec/config.yaml` (regras de `proposal` e `tasks`) e `README.md`.
- **Constituição:** sem alteração. O `plan.md` mantém a ordem do Artigo IX e só a detalha; mudanças de ordem seguem o próprio Artigo IX.
- **Changes futuras:** cada `/opsx:propose` do roadmap parte da ficha correspondente no `plan.md` e a atualiza ao ser arquivada.
