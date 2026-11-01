# Proposal

## Why

O Artigo VII.4 da constituição exige Conventional Commits em pt-BR, mas só cita alguns tipos como exemplo. Não define escopos, formato da descrição, tratamento de breaking changes nem como o padrão é verificado. O responsável pelo projeto quer adotar commits semânticos com os tipos do *Angular Commit Message Guidelines*, mais o tipo `env`. Sem uma regra precisa e validada automaticamente, o histórico perde consistência, e fica inviável gerar changelog e versões a partir dos commits.

## What Changes

- **Nova seção 13, "Commits semânticos", no `specification.md`** (versão 1.0.0 → 1.1.0, MINOR), com:
  - formato `<tipo>(<escopo>)!: <descrição>`, corpo e rodapé;
  - os 11 tipos permitidos: `build`, `ci`, `docs`, `feat`, `fix`, `perf`, `refactor`, `style`, `test`, `chore`, `env`, com a fronteira entre os que se sobrepõem (`build` × `chore` × `ci` × `env`);
  - escopos permitidos: os bounded contexts e as áreas transversais;
  - regras de redação em pt-BR (presente do indicativo, minúsculas, sem ponto final, até 72 caracteres no cabeçalho);
  - breaking changes com `!` e rodapé `BREAKING CHANGE:`;
  - proibição de `Co-authored-by` e `Signed-off-by` (Artigo VII.4);
  - exemplos válidos e inválidos.
- **Hook `commit-msg` versionado em `.githooks/`**: script POSIX `sh`, sem dependência de Node.js, que rejeita mensagens fora do padrão. Mensagens geradas pelo Git (`Merge ...`, `Revert ...`, `fixup!`, `squash!`) são aceitas.
- **Testes do hook** em `scripts/tests/commit-msg.test.sh`, com casos válidos e inválidos.
- **README**: instrução para ativar o hook (`git config core.hooksPath .githooks`) e referência à seção 13.
- **Validação na CI**: registrada no `specification.md` como obrigatória. O job será criado na change `add-solution-skeleton`, que cria a pipeline.

**Módulo (Artigo IV da constituição):** nenhum módulo de negócio; é convenção transversal de desenvolvimento.

## Capabilities

### New Capabilities
<!-- Nenhuma: convenção e ferramenta de desenvolvimento, sem mudança de comportamento do sistema (skip_specs: true). -->

### Modified Capabilities
<!-- Nenhuma. -->

## Impact

- **Arquivos novos:** `.githooks/commit-msg`, `scripts/tests/commit-msg.test.sh`.
- **Arquivos alterados:** `specification.md` (seção 13, sumário e tabela de versão), `README.md`.
- **Constituição:** sem alteração. O Artigo VII.4 lista tipos apenas como exemplo, e a seção 13 o detalha sem contradizê-lo (Artigo X.5).
- **Histórico:** commits já existentes não são reescritos. O padrão vale a partir da ativação do hook.
- **Changes futuras:** `add-solution-skeleton` DEVE incluir na CI a validação das mensagens de commit do pull request, reutilizando o mesmo script.
