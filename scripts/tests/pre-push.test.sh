#!/bin/sh
set -u

root=$(cd "$(dirname "$0")/../.." && pwd)
hook="$root/.githooks/pre-push"
sha=1111111111111111111111111111111111111111
zero=0000000000000000000000000000000000000000

passed=0
failed=0

run() {
    expected=$1
    description=$2

    if printf '%b' "$3" | sh "$hook" origin https://example.com/repo.git > /dev/null 2>&1; then
        actual=accepted
    else
        actual=rejected
    fi

    if [ "$actual" = "$expected" ]; then
        passed=$((passed + 1))
    else
        failed=$((failed + 1))
        printf 'FALHOU: %s (esperado %s, obtido %s)\n' "$description" "$expected" "$actual"
    fi
}

push() { printf 'refs/heads/%s %s refs/heads/%s %s\n' "$1" "$sha" "$1" "$zero"; }

accepted() { run accepted "$@"; }
rejected() { run rejected "$@"; }

for type in build ci docs feat fix perf refactor style test chore env; do
    accepted "tipo $type" "$(push "$type/add-solution-skeleton")"
done

accepted "descrição com números" "$(push fix/cpf-digito-0)"
accepted "branch de sessão do Claude Code" "$(push claude/relaxed-fermi-w2ie4h)"
accepted "tag" "refs/tags/v1.0.0 $sha refs/tags/v1.0.0 $zero\n"
accepted "várias refs válidas" "$(push feat/a)\n$(push fix/b)"
accepted "nada a enviar" ""

rejected "push direto para main" "$(push main)"
rejected "branch local enviada para main" "refs/heads/feat/x $sha refs/heads/main $sha\n"
rejected "exclusão da main" "(delete) $zero refs/heads/main $sha\n"
rejected "tipo desconhecido" "$(push feature/add-agenda)"
rejected "sem tipo" "$(push add-agenda)"
rejected "maiúsculas" "$(push feat/Add-Agenda)"
rejected "underscore" "$(push feat/add_agenda)"
rejected "descrição vazia" "$(push feat/)"
rejected "hífen no fim" "$(push feat/add-agenda-)"
rejected "hífen duplo" "$(push feat/add--agenda)"
rejected "acento" "$(push feat/remarcação)"
rejected "subnível extra" "$(push feat/agenda/remarcar)"
rejected "uma válida e uma inválida" "$(push feat/a)\n$(push main)"

printf '%d passaram, %d falharam\n' "$passed" "$failed"
[ "$failed" -eq 0 ]
