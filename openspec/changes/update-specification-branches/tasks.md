# Tasks

## 1. Documentação do fluxo

- [x] 1.1 Adicionar a seção 14 "Branches e pull requests" ao `Docs/specification.md` (D1 a D5), atualizar o sumário e a versão para 1.2.0; verificar o cabeçalho, o link no sumário e que os tipos de branch são os mesmos da seção 13

## 2. Hook pre-push

- [x] 2.1 Criar `scripts/tests/pre-push.test.sh` com casos aceitos (cada tipo, `claude/*`, tags) e rejeitados (push para `main`, exclusão da `main`, tipo desconhecido, maiúsculas, espaços, sem tipo); verificar que falha antes de o hook existir
- [x] 2.2 Criar `.githooks/pre-push` em `sh` POSIX, executável (D4); verificar com `sh scripts/tests/pre-push.test.sh` em `sh` e `bash`

## 3. Branch main

- [x] 3.1 Criar a branch `main` no GitHub a partir do commit atual (D6); verificar com `git ls-remote --heads origin main` que ela aponta para `ec3ee22`, o último commit da Fase 0, para que esta change entre na `main` pelo primeiro pull request
- [x] 3.2 Verificar que `git push origin HEAD:main` é rejeitado pelo hook

## 4. Documentação de uso e planejamento

- [x] 4.1 Adicionar ao `README.md` a seção de fluxo de trabalho (criar branch, pull request, squash merge) e o link para a seção 14; verificar os links
- [x] 4.2 Registrar a etapa P0.E7 no `Docs/tasks.md`, a ficha na Fase 0 do `Docs/plan.md` e os itens 🆕 no `Docs/checklist.md`, atualizando o resumo; verificar que as contagens batem
- [x] 4.3 Executar `openspec validate update-specification-branches`
