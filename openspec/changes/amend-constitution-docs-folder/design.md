# Design

## Context

Pedido direto do responsável. Artigos envolvidos: I (escopo), IV.8 (local do `specification.md`) e X (emendas); seção 9 (estrutura) e seção 12 (atualização) do `specification.md`.

## Goals / Non-Goals

**Goals:**
- Reunir os documentos do projeto em `Docs/`, sem quebrar nenhum link nem referência.

**Non-Goals:**
- Mover o `README.md` (o GitHub o exibe a partir da raiz), o `docker-compose.yaml` (o Artigo III.3 o mantém na raiz) ou os artefatos do OpenSpec.

## Decisions

### D1. Emenda PATCH
Só muda o local dos arquivos, não o sentido de nenhuma regra: é PATCH pelo Artigo X.3. A mesma change atualiza o `specification.md` (também PATCH), em vez de abrir uma `update-specification-<tema>` separada, porque as duas alterações são a mesma mudança.

### D2. Nomes em minúsculas
Os documentos novos (`plan.md`, `tasks.md`, `checklist.md`) seguem o padrão em minúsculas dos documentos existentes, como pedido pelo responsável.

### D3. `git mv` e histórico
A mudança de pasta é feita com `git mv`, para que `git log --follow` continue mostrando o histórico dos arquivos.

## Risks / Trade-offs

- [Links quebrados] → A verificação procura todas as referências aos documentos fora das changes concluídas.
- [Confusão entre `Docs/tasks.md` e o `tasks.md` das changes] → Tratado em `add-implementation-plan` (D2): caminho completo nos textos e cabeçalho explicativo.
