# Tasks

## 1. Docs/plan.md

- [x] 1.1 Criar `Docs/plan.md` com cabeçalho (papel orientativo e relação com constituição, `specification.md`, `Docs/tasks.md` e `Docs/checklist.md`, conforme design.md D1), sumário, tabela de fases e marcos com destaque para o MVP (D3) e grafo de dependências; verificar que a ordem bate com o Artigo IX e que `add-patient-registry` e `add-dentist-profile` aparecem como paralelas
- [x] 1.2 Escrever as fichas de todas as changes das Fases 0 a 3 no formato de D4, sem lista de entregas; verificar que cada ficha tem todos os campos de D4 e um link para a etapa correspondente do `Docs/tasks.md`
- [x] 1.3 Adicionar a tabela de requisitos transversais (D5), a Definição de Pronto (D8) e a seção de manutenção dos três documentos (D9); verificar que todo requisito dos Artigos V, VI e VII da constituição e das seções 8, 10 e 13 do `specification.md` tem uma change responsável

## 2. Docs/tasks.md

- [x] 2.1 Criar `Docs/tasks.md` com cabeçalho explicando a relação com o `tasks.md` de cada change (D2) e a numeração `P<fase>.E<etapa>.T<tarefa>` (D6); verificar que o cabeçalho deixa clara a diferença entre os dois arquivos
- [x] 2.2 Escrever a Parte 0 (Fundação) com as etapas concluídas marcadas e com links para o `tasks.md` de cada change existente; verificar que todos os links apontam para arquivos existentes
- [x] 2.3 Escrever a Parte 1 (`add-solution-skeleton`) com as tarefas macro cobrindo todas as entregas que D5 atribui a ela (estrutura da solução, building blocks, API base, persistência base, observabilidade, Web base, Dockerfiles, testes e testes de arquitetura, CI); verificar que cada entrega de D5 tem ao menos uma tarefa macro
- [x] 2.4 Escrever as Partes 2 e 3 com as etapas de cada change e suas tarefas macro, incluindo o painel do dentista e os blocos que cada módulo acrescenta a ele, a exportação e correção de dados do paciente, a auditoria e a criptografia (D5); verificar que cada etapa tem tarefas de testes e de atualização da documentação de planejamento

## 3. Docs/checklist.md

- [x] 3.1 Criar `Docs/checklist.md` com o resumo de progresso por fase, o marco MVP, a legenda (concluído, pendente, 🆕 nova necessidade, riscado) e as regras de atualização de D7; verificar que o resumo soma corretamente os itens de cada fase
- [x] 3.2 Escrever os itens de entrega das Fases 0 a 3, agrupados por módulo ou área, cada um com a change responsável, e marcar como concluídos os itens da Fase 0; verificar que cada etapa do `Docs/tasks.md` tem ao menos um item correspondente no checklist
- [x] 3.3 Adicionar a seção "Pendente de planejamento" (vazia) e a seção de histórico de itens removidos; verificar que as duas seções existem e que suas regras de uso estão descritas

## 4. Integração e verificação

- [x] 4.1 Adicionar ao `openspec/config.yaml` as regras de `proposal` e `tasks` de D9; verificar com `openspec instructions proposal --change add-implementation-plan --json` que as novas regras aparecem em `rules`
- [x] 4.2 Adicionar ao `README.md` os links para `Docs/plan.md`, `Docs/tasks.md` e `Docs/checklist.md`; verificar que os três links apontam para arquivos existentes
- [x] 4.3 Verificar que não há conteúdo duplicado entre os três documentos, conforme a coluna "Não contém" de D1: fichas sem lista de entregas, `Docs/tasks.md` sem status de entregas, `Docs/checklist.md` sem tarefas técnicas
- [x] 4.4 Marcar esta change como concluída no `Docs/tasks.md` e no `Docs/checklist.md` e executar `openspec validate add-implementation-plan`; verificar que a validação passa

## Workflow follow-up

- Arquivar as changes da Fase 0 com `/opsx:archive`.
- Iniciar a Fase 1 com `/opsx:propose add-solution-skeleton`, partindo da ficha do `Docs/plan.md` e da Parte 1 do `Docs/tasks.md`.
