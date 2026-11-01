# Tasks

## 1. Mudança de pasta

- [x] 1.1 Mover `constitution.md` e `specification.md` para `Docs/` com `git mv`; verificar com `git status` que aparecem como renomeados

## 2. Emenda e atualização de referências

- [x] 2.1 Atualizar a constituição para 1.2.1 (Artigo IV.8 e novo item I.3); verificar com `grep -n "Docs/" Docs/constitution.md`
- [x] 2.2 Atualizar o `specification.md` para 1.1.1 (árvore da seção 9 e referência à constituição); verificar o cabeçalho e a árvore
- [x] 2.3 Atualizar as referências em `README.md`, `openspec/config.yaml` e `.githooks/commit-msg`; verificar com `sh scripts/tests/commit-msg.test.sh` e `openspec context --json`
- [x] 2.4 Ajustar a change `add-implementation-plan` para `Docs/plan.md`, `Docs/tasks.md` e `Docs/checklist.md`; verificar que não restam `Tasks.md` ou `Checklist.md` com maiúscula

## 3. Verificação

- [x] 3.1 Verificar que nenhuma referência fora das changes concluídas aponta para os caminhos antigos e executar `openspec validate amend-constitution-docs-folder`
