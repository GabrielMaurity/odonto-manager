# Design

## Context

O repositório tem uma única branch, `claude/relaxed-fermi-w2ie4h`, criada pela sessão do Claude Code, com toda a Fase 0. A constituição VII.5 exige branch própria e pull request, e a VII.6 exige CI em todo pull request (a CI virá em `add-solution-skeleton`). A seção 13 do `specification.md` define os tipos de commit e o hook `commit-msg`. Seções envolvidas: 13 e a nova seção 14.

## Goals / Non-Goals

**Goals:**
- `main` existente e definida como a principal.
- Nome da branch indicando o tipo de trabalho e a change do OpenSpec.
- Proteção local (hook) e remota (regras do GitHub) contra push direto na `main`.

**Non-Goals:**
- Git Flow com `develop` e `release`: complexidade desnecessária para um time pequeno com CI.
- Versionamento e releases (tags), que podem vir depois, com changelog.

## Decisions

### D1. Trunk-based simples, com a `main` sempre estável
Uma única branch longa, a `main`. Todo trabalho acontece em branches curtas que saem da `main` e voltam por pull request.
- *Alternativa*: Git Flow. Rejeitada: a `develop` duplica a `main` sem ganho real quando há CI em todo pull request.

### D2. Nomes `<tipo>/<nome>` alinhados ao OpenSpec e aos commits
- Branch de change: `<tipo>/<nome-da-change>`, por exemplo `feat/add-solution-skeleton` e `docs/update-plan-mvp`.
- Branch fora de change (correção pontual, atualização de dependência): `<tipo>/<descricao-curta>`, por exemplo `fix/cpf-digito-zero`.
- O tipo é um dos 11 da seção 13. O nome usa kebab-case, em minúsculas, sem acentos.
- **Exceção:** branches `claude/<nome>`, criadas automaticamente por sessões do Claude Code na nuvem, que não permitem escolher o nome. O conteúdo delas segue as mesmas regras, e o pull request segue o padrão normal.

Usar o nome da change no nome da branch liga código, branch, pull request e change sem tabela extra.

### D3. Merge por squash, com título semântico
O pull request entra na `main` como **um único commit**, cuja mensagem é o título do pull request. Por isso, o título DEVE seguir a seção 13. O histórico da `main` fica com um commit por change ou correção, o que facilita reverter e gerar changelog. Os commits da branch continuam validados pelo hook.
- *Alternativa*: merge commit. Rejeitada: polui o histórico da `main` com commits intermediários.
- *Alternativa*: rebase. Rejeitada: preserva commits intermediários e exige mais cuidado de quem faz o merge.

### D4. Hook `pre-push` (local)
Script `sh` POSIX em `.githooks/`, ativado pelo mesmo `core.hooksPath` do `commit-msg`. Para cada ref enviada:
- rejeita push para `refs/heads/main`, incluindo criação e exclusão;
- rejeita branches fora do padrão de D2;
- ignora tags.

### D5. Proteção remota no GitHub
O hook local pode ser ignorado com `--no-verify` ou não estar ativado. A garantia real vem de uma regra de proteção da `main` no GitHub:
- pull request obrigatório antes do merge;
- checks da CI obrigatórios, assim que existirem;
- branch atualizada antes do merge;
- sem force push e sem exclusão;
- apenas squash merge habilitado no repositório, com exclusão automática da branch após o merge.

Esses itens são configurados pelo responsável, e a seção 14 traz o passo a passo.

### D6. Criação da `main`
A `main` é criada no GitHub (via API) a partir do commit `ec3ee22`, o último da Fase 0, já revisada pelo responsável. Esta própria change não entra direto: ela vai para a `main` pelo primeiro pull request, inaugurando o fluxo. A criação via API não passa pelo hook `pre-push`, então não é preciso ignorá-lo. Depois disso, o trabalho da Fase 1 começa em `feat/add-solution-skeleton`.

## Risks / Trade-offs

- [Hook local não ativado] → A proteção do GitHub (D5) é a garantia; o README orienta a ativação.
- [Branch `claude/*` fora do padrão `<tipo>/`] → Exceção explícita e restrita a sessões do Claude Code (D2).
- [Squash esconder commits intermediários] → Os commits continuam no pull request, que fica no histórico do GitHub.
