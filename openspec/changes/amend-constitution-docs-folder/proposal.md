# Proposal

## Why

O responsável pelo projeto pediu que os documentos do projeto fiquem reunidos em uma pasta `Docs/` na raiz, em vez de espalhados pela raiz junto com arquivos de configuração. A constituição (Artigo IV.8) diz que o `specification.md` fica "na raiz do repositório", então a mudança exige uma emenda (Artigo X.1).

## What Changes

- Move `constitution.md` e `specification.md` para `Docs/`, preservando o histórico do Git (`git mv`).
- **Emenda PATCH na constituição (1.2.0 → 1.2.1):**
  - Artigo IV.8 passa a localizar o `specification.md` em `Docs/`;
  - novo item I.3 define que os documentos do projeto (constituição, `specification.md`, `plan.md`, `tasks.md` e `checklist.md`) ficam em `Docs/` e que o `README.md` permanece na raiz.
- **`specification.md` 1.1.0 → 1.1.1 (PATCH):** a árvore da seção 9 inclui `Docs/`, e a referência à constituição é atualizada para 1.2.1.
- Atualiza as referências em `README.md`, `openspec/config.yaml` e `.githooks/commit-msg`.
- Ajusta a change pendente `add-implementation-plan` para criar `Docs/plan.md`, `Docs/tasks.md` e `Docs/checklist.md`, em minúsculas.

**Módulo (Artigo IV da constituição):** transversal, apenas documentação.

## Capabilities

### New Capabilities
<!-- Nenhuma (skip_specs: true). -->

### Modified Capabilities
<!-- Nenhuma. -->

## Impact

- **Arquivos movidos:** `constitution.md` → `Docs/constitution.md` e `specification.md` → `Docs/specification.md`.
- **Arquivos alterados:** `Docs/constitution.md`, `Docs/specification.md`, `README.md`, `openspec/config.yaml`, `.githooks/commit-msg` e os artefatos da change `add-implementation-plan`.
- **Changes concluídas** (`add-project-constitution`, `amend-constitution-docker`, `amend-constitution-ddd`, `update-specification-commits`): os artefatos não são alterados, pois registram o estado da época em que foram aplicadas.
