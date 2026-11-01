# Design

## Context

O Artigo VII.4 da constituição define Conventional Commits em pt-BR, sem assinatura nem coautoria, e o Artigo VII.6 exige CI em todo pull request. Os commits atuais já usam `tipo: descrição` em pt-BR, no presente do indicativo ("adiciona", "propõe"). Ainda não há pipeline de CI nem nenhum outro hook. A motivação está em proposal.md — Why. Seções do `specification.md` envolvidas: nova seção 13 e a seção 12 (versionamento do documento).

## Goals / Non-Goals

**Goals:**
- Regra de commit precisa o bastante para ser validada por script.
- Validação local sem instalar nada além do Git, mantendo o projeto livre de Node.js.
- Fronteiras claras entre os tipos que se sobrepõem.

**Non-Goals:**
- Geração automática de changelog e versão (pode vir depois, com uma ferramenta compatível com Conventional Commits).
- Reescrever o histórico existente.
- Criar a pipeline de CI (change `add-solution-skeleton`).

## Decisions

### D1. Tipos: lista do Angular mais `env`
Os 11 tipos pedidos pelo responsável. Os tipos `build`, `chore`, `ci` e `env` se sobrepõem, então a seção 13 define fronteiras objetivas, por arquivo afetado:

| Tipo | Quando usar | Exemplos de arquivos |
|------|-------------|----------------------|
| `build` | Sistema de build e dependências de produção | `*.csproj`, `Directory.Build.props`, `Directory.Packages.props`, `global.json` |
| `ci` | Pipeline de integração contínua | `.github/workflows/*` |
| `env` | Configuração de ambientes e containers | `docker-compose.yaml`, `.env.example`, Dockerfiles, `appsettings.*.json` |
| `chore` | Ferramentas e manutenção que não chegam à produção | `.gitignore`, `.editorconfig`, `.githooks/*`, `openspec/config.yaml` |

Quando um commit toca arquivos de mais de um tipo, vale o tipo que representa o **objetivo** do commit. Se não houver objetivo predominante, o commit DEVE ser dividido.
- *Alternativa*: descartar `env` e usar `ci`/`chore`. Rejeitada: o tipo foi pedido explicitamente e a tabela elimina a ambiguidade.

### D2. Escopos fechados
O escopo é opcional. Quando presente, DEVE ser um destes:
- **Bounded contexts** (`specification.md` 2.1): `identidade`, `pacientes`, `profissionais`, `agenda`, `materiais`, `exames`.
- **Áreas transversais:** `api`, `web`, `infra`, `db`, `docker`, `deps`, `openspec`, `constituicao`, `especificacao`.

Uma lista fechada permite validar no script e mantém o vocabulário alinhado à linguagem ubíqua. Novos escopos exigem `update-specification-commits`.

### D3. Redação
- Cabeçalho com no máximo 72 caracteres, tipo e escopo em minúsculas.
- Descrição em pt-BR, iniciando por verbo no **presente do indicativo** ("adiciona", "corrige", "remove"), em minúscula e sem ponto final. Isso segue o padrão já usado no histórico.
- O corpo é opcional, separado por linha em branco, e explica o porquê.
- O rodapé segue o formato de trailers do Git (`BREAKING CHANGE: ...`, `Refs: #123`).
- *Alternativa*: imperativo ("adicionar", "adicione"). Rejeitada: o histórico já usa o presente do indicativo.

### D4. Breaking changes
Uma mudança incompatível DEVE ter `!` antes dos dois-pontos **e** o rodapé `BREAKING CHANGE: <descrição>`. Exigir os dois deixa a quebra visível no cabeçalho e documentada no rodapé, e o script verifica a coerência entre eles.

### D5. Proibição de assinatura e coautoria (constituição VII.4)
O hook rejeita mensagens com os trailers `Co-authored-by:` ou `Signed-off-by:`, sem diferenciar maiúsculas de minúsculas. A preferência do responsável deixa de depender só de disciplina.

### D6. Hook em shell POSIX, versionado
`.githooks/commit-msg` em `sh` POSIX, ativado com `git config core.hooksPath .githooks`. O hook:
- ignora linhas de comentário (`#`) que o Git adiciona ao editor;
- aceita mensagens geradas pelo Git: `Merge ...`, `Revert "..."`, `fixup! ...`, `squash! ...`, `amend! ...`;
- valida cabeçalho, linha em branco após o cabeçalho, tamanho, tipo, escopo, `!` × `BREAKING CHANGE` e trailers proibidos;
- em caso de erro, imprime a regra violada e um exemplo válido.

O mesmo script poderá ser chamado pela CI para cada commit do pull request.
- *Alternativa*: commitlint + husky. Rejeitada: exigem Node.js e `package.json` num projeto .NET.
- *Alternativa*: Husky.Net (ferramenta .NET local). Rejeitada por agora: depende da solução .NET, que ainda não existe; pode substituir o `core.hooksPath` no futuro sem mudar a regra.

### D7. Testes do hook
`scripts/tests/commit-msg.test.sh` executa o hook contra mensagens válidas e inválidas, gravadas em arquivos temporários, e falha se algum resultado divergir do esperado. Isso cumpre a regra de que cada grupo de tarefas entrega os próprios testes, e o script poderá rodar na CI.

## Risks / Trade-offs

- [Hook local é opcional: quem não ativa o `core.hooksPath` não é validado] → README orienta a ativação, e a validação na CI (em `add-solution-skeleton`) é a garantia final.
- [Lista fechada de escopos frustrar um commit legítimo] → O escopo é opcional: o commit pode omiti-lo, e novos escopos entram por `update-specification-commits`.
- [Diferenças entre implementações de `sh` (dash, bash, busybox, Git for Windows)] → Script restrito a POSIX (`grep -E`, `sed`, `case`), e os testes são executados com `sh`.
- [Contagem de caracteres com acentos] → Usar contagem de caracteres (`wc -m`, com locale UTF-8) em vez de bytes; os testes cobrem um cabeçalho acentuado no limite.
