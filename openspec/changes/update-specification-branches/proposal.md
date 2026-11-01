# Proposal

## Why

Antes de começar o código, o responsável quer um fluxo de branches definido: uma `main` como branch principal e uma branch nova para cada implementação ou feature. O Artigo VII.5 da constituição já exige trabalho em branch própria e entrada na principal por pull request, mas não nomeia a principal, não define o padrão de nomes das branches nem como o merge é feito. Além disso, a `main` ainda não existe no repositório.

## What Changes

- **Nova seção 14, "Branches e pull requests", no `Docs/specification.md`** (1.1.1 → 1.2.0, MINOR):
  - a `main` é a branch principal, sempre estável, e nunca recebe push direto;
  - uma branch por change do OpenSpec, nomeada `<tipo>/<nome-da-change>`, com os tipos dos commits semânticos (seção 13);
  - branches para correções e ajustes fora de uma change, no padrão `<tipo>/<descricao-curta>`;
  - ciclo de vida: criar a partir da `main` atualizada, abrir pull request, CI verde, merge e exclusão da branch;
  - merge por **squash**, com o título do pull request no formato de commit semântico;
  - proteção da `main` no GitHub (pull request obrigatório, CI obrigatória, sem force push).
- **Hook `.githooks/pre-push`**: bloqueia push direto para a `main` e rejeita branches fora do padrão de nomes, com testes em `scripts/tests/pre-push.test.sh`.
- **Criação da branch `main` no GitHub** a partir do estado atual, que contém toda a Fase 0.
- **README:** seção de fluxo de trabalho.
- **Documentos de planejamento:** nova etapa P0.E7 no `Docs/tasks.md`, ficha na Fase 0 do `Docs/plan.md` e itens no `Docs/checklist.md`.

**Módulo (Artigo IV da constituição):** nenhum; convenção transversal de desenvolvimento. **Plano:** etapa nova da Fase 0, P0.E7, que surgiu depois do planejamento inicial.

## Capabilities

### New Capabilities
<!-- Nenhuma: convenção e ferramenta de desenvolvimento (skip_specs: true). -->

### Modified Capabilities
<!-- Nenhuma. -->

## Impact

- **Arquivos novos:** `.githooks/pre-push`, `scripts/tests/pre-push.test.sh`.
- **Arquivos alterados:** `Docs/specification.md`, `README.md`, `Docs/plan.md`, `Docs/tasks.md`, `Docs/checklist.md`.
- **Repositório:** nova branch `main`. A proteção da branch e a definição de `main` como branch padrão são feitas pelo responsável nas configurações do GitHub, porque as ferramentas desta sessão não alteram as configurações do repositório.
- **Constituição:** sem alteração. A seção 14 detalha o Artigo VII.5 sem contradizê-lo (Artigo X.5).
- **Changes futuras:** `add-solution-skeleton` DEVE validar na CI o título do pull request, além dos commits.
