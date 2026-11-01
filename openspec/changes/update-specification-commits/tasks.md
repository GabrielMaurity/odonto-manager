# Tasks

## 1. Documentação do padrão

- [x] 1.1 Adicionar a seção 13 "Commits semânticos" ao `specification.md` com formato, os 11 tipos e a tabela de fronteiras `build`/`ci`/`env`/`chore` (design.md D1), escopos (D2), redação (D3), breaking changes (D4), trailers proibidos (D5), exemplos válidos e inválidos e a obrigação de validação na CI; verificar que todos os tipos e escopos do design aparecem na seção
- [x] 1.2 Atualizar o sumário e a tabela de versão do `specification.md` para 1.1.0; verificar o cabeçalho e o link da seção 13 no sumário

## 2. Hook commit-msg

- [x] 2.1 Criar `scripts/tests/commit-msg.test.sh` com casos válidos (cada tipo, com e sem escopo, `!` com `BREAKING CHANGE`, corpo e rodapé, cabeçalho acentuado com 72 caracteres, mensagens `Merge`/`Revert`/`fixup!`, comentários do Git) e inválidos (tipo ou escopo desconhecido, sem espaço após `:`, descrição vazia ou com ponto final, maiúscula inicial, 73 caracteres, sem linha em branco antes do corpo, `!` sem `BREAKING CHANGE` e vice-versa, `Co-authored-by`, `Signed-off-by`); verificar que o script falha antes de o hook existir
- [x] 2.2 Criar `.githooks/commit-msg` em `sh` POSIX, executável, conforme D6; verificar com `sh scripts/tests/commit-msg.test.sh`, com todos os casos passando
- [x] 2.3 Ativar o hook no repositório com `git config core.hooksPath .githooks` e verificar que `git commit --allow-empty -m "Teste inválido"` é rejeitado e que `git commit --allow-empty -m "chore: valida hook"` é aceito (desfazer o commit de teste em seguida)

## 3. Documentação de uso

- [x] 3.1 Adicionar ao `README.md` uma seção "Commits" com o comando de ativação do hook, o formato resumido e o link para a seção 13 do `specification.md`; verificar executando o comando do README em um clone limpo do repositório

## 4. Verificação final

- [x] 4.1 Executar `openspec validate update-specification-commits` e conferir que a seção 13 não contradiz o Artigo VII da constituição

## Workflow follow-up

- Arquivar esta change e as anteriores com `/opsx:archive` após revisão.
- Em `add-solution-skeleton`, adicionar à CI um job que executa `.githooks/commit-msg` para cada commit do pull request.
