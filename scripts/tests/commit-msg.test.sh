#!/bin/sh
set -u

root=$(cd "$(dirname "$0")/../.." && pwd)
hook="$root/.githooks/commit-msg"
tmp=$(mktemp)
trap 'rm -f "$tmp"' EXIT

passed=0
failed=0

run() {
    expected=$1
    description=$2
    printf '%b' "$3" > "$tmp"

    if sh "$hook" "$tmp" > /dev/null 2>&1; then
        actual=valid
    else
        actual=invalid
    fi

    if [ "$actual" = "$expected" ]; then
        passed=$((passed + 1))
    else
        failed=$((failed + 1))
        printf 'FALHOU: %s (esperado %s, obtido %s)\n' "$description" "$expected" "$actual"
    fi
}

valid() { run valid "$@"; }
invalid() { run invalid "$@"; }

for type in build ci docs feat fix perf refactor style test chore env; do
    valid "tipo $type sem escopo" "$type: adiciona algo\n"
done

for scope in identidade pacientes profissionais agenda materiais exames \
    api web infra db docker deps openspec constituicao especificacao; do
    valid "escopo $scope" "feat($scope): adiciona algo\n"
done

valid "breaking change com ! e rodapé" "refactor(api)!: renomeia rota de cancelamento\n\nBREAKING CHANGE: a rota antiga deixa de existir.\n"
valid "corpo e rodapé" "fix(pacientes): corrige validação de CPF\n\nO dígito zero era tratado como inválido.\n\nRefs: #12\n"
valid "cabeçalho acentuado com 72 caracteres" "feat(agenda): adiciona remarcação de consultas com aviso ao paciente já!\n"
valid "comentários do git ignorados" "docs: descreve fluxo de pedidos\n# Please enter the commit message for your changes.\n# On branch main\n"
valid "merge gerado pelo git" "Merge branch 'main' into feature\n"
valid "revert gerado pelo git" "Revert \"feat: adiciona algo\"\n\nThis reverts commit abc123.\n"
valid "fixup gerado pelo git" "fixup! feat: adiciona algo\n"
valid "squash gerado pelo git" "squash! feat: adiciona algo\n"
valid "amend gerado pelo git" "amend! feat: adiciona algo\n"

invalid "sem tipo" "Adiciona agenda\n"
invalid "tipo desconhecido" "feature: adiciona agenda\n"
invalid "tipo em maiúsculas" "Feat: adiciona agenda\n"
invalid "escopo desconhecido" "feat(calendario): adiciona agenda\n"
invalid "sem espaço após dois-pontos" "feat:adiciona agenda\n"
invalid "descrição vazia" "feat: \n"
invalid "descrição com ponto final" "feat: adiciona agenda.\n"
invalid "descrição com maiúscula inicial" "feat: Adiciona agenda\n"
invalid "descrição com maiúscula acentuada" "feat: Édita agenda\n"
invalid "cabeçalho com 73 caracteres" "feat(agenda): adiciona remarcação de consultas com aviso ao paciente já!!\n"
invalid "sem linha em branco antes do corpo" "feat: adiciona agenda\ncorpo colado no cabeçalho\n"
invalid "! sem BREAKING CHANGE" "feat!: remove endpoint legado\n"
invalid "BREAKING CHANGE sem !" "feat: remove endpoint legado\n\nBREAKING CHANGE: endpoint removido.\n"
invalid "co-authored-by" "feat: adiciona agenda\n\nCo-authored-by: Fulano <fulano@example.com>\n"
invalid "signed-off-by" "feat: adiciona agenda\n\nSigned-off-by: Fulano <fulano@example.com>\n"
invalid "co-authored-by em minúsculas" "feat: adiciona agenda\n\nco-authored-by: Fulano <fulano@example.com>\n"
invalid "merge com trailer proibido" "Merge branch 'main'\n\nCo-authored-by: Fulano <fulano@example.com>\n"
invalid "mensagem vazia" ""

printf '%d passaram, %d falharam\n' "$passed" "$failed"
[ "$failed" -eq 0 ]
