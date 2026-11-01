# Tasks

## 1. Estrutura do plan.md

- [ ] 1.1 Criar `plan.md` na raiz com cabeçalho (papel orientativo e relação com constituição e `specification.md`, conforme design.md D1), sumário e a tabela de fases e marcos (D2), destacando o marco MVP; verificar que as quatro fases e o MVP aparecem e que a ordem bate com o Artigo IX
- [ ] 1.2 Adicionar o grafo de dependências entre as changes, indicando que `add-patient-registry` e `add-dentist-profile` podem ser paralelas; verificar que nenhuma dependência contradiz a tabela de fases

## 2. Fichas das changes

- [ ] 2.1 Escrever as fichas da Fase 0 (resumo das quatro changes concluídas) e da Fase 1 (`add-solution-skeleton`) no formato de D3, cobrindo na Fase 1 todas as entregas que D4 atribui a ela; verificar que CI, Dockerfiles, building blocks e testes de arquitetura aparecem nas entregas
- [ ] 2.2 Escrever as fichas da Fase 2 (`add-identity-access`, `add-patient-registry`, `add-dentist-profile`, `add-appointment-scheduling`) no formato de D3, incluindo o painel do dentista e a exportação e correção de dados do paciente (D4); verificar que cada ficha tem todos os campos de D3
- [ ] 2.3 Escrever as fichas da Fase 3 (`add-material-requests`, `add-exam-files`), incluindo auditoria e criptografia em `add-exam-files` e o bloco de cada módulo no painel do dentista (D4); verificar que cada ficha tem todos os campos de D3

## 3. Transversais, pronto e acompanhamento

- [ ] 3.1 Adicionar a tabela de requisitos transversais e suas changes responsáveis (D4); verificar que todo requisito dos Artigos V, VI e VII da constituição e das seções 8, 10 e 13 do `specification.md` tem uma change responsável
- [ ] 3.2 Adicionar a Definição de Pronto (D5) e a seção de manutenção do plano (D1, incluindo `update-plan-<tema>`); verificar consistência com o Artigo VIII da constituição
- [ ] 3.3 Adicionar o quadro de acompanhamento com todas as changes e seus status atuais (Fase 0 concluída, aguardando arquivamento; demais não iniciadas, e esta change em andamento); verificar que as changes do quadro são exatamente as de `openspec list` mais as do roadmap

## 4. Integração

- [ ] 4.1 Adicionar ao `openspec/config.yaml` as regras de `proposal` e `tasks` de D6; verificar com `openspec instructions proposal --change add-implementation-plan --json` que a nova regra aparece em `rules`
- [ ] 4.2 Adicionar ao `README.md` o link para `plan.md` junto aos links da constituição; verificar que o link abre o arquivo
- [ ] 4.3 Marcar esta change como concluída no quadro de acompanhamento do `plan.md` e executar `openspec validate add-implementation-plan`; verificar que a validação passa

## Workflow follow-up

- Arquivar as changes da Fase 0 e esta com `/opsx:archive`.
- Iniciar a Fase 1 com `/opsx:propose add-solution-skeleton`, partindo da ficha do `plan.md`.
