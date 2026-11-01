# Odonto Manager

Sistema de gestão de clínicas odontológicas. Os princípios e decisões do projeto estão em [`Docs/constitution.md`](Docs/constitution.md) e a especificação técnica em [`Docs/specification.md`](Docs/specification.md). O planejamento da implementação está em [`Docs/plan.md`](Docs/plan.md), as tarefas em [`Docs/tasks.md`](Docs/tasks.md) e o andamento em [`Docs/checklist.md`](Docs/checklist.md); o desenvolvimento segue SDD com [OpenSpec](https://github.com/Fission-AI/OpenSpec) (changes em `openspec/changes/`).

## Ambiente de desenvolvimento

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) (`dotnet --version`)
- Docker Engine 24+ com o plugin Docker Compose v2 (`docker compose version`), usado pelo banco local e pelos testes de integração
- PowerShell (`pwsh`), apenas para instalar o navegador dos testes ponta a ponta

### Subir o banco de dados

```bash
cp .env.example .env
```

```bash
docker compose up -d
```

O PostgreSQL fica disponível em `localhost:5432` (porta configurável por `POSTGRES_PORT` no `.env`). Confira se está saudável:

```bash
docker compose ps
```

### pgAdmin (opcional)

```bash
docker compose --profile tools up -d
```

Acesse `http://localhost:5050` com `PGADMIN_DEFAULT_EMAIL` e `PGADMIN_DEFAULT_PASSWORD` do `.env`. Ao registrar o servidor no pgAdmin, use o host `postgres` e a porta `5432`.

### Derrubar o ambiente

Mantendo os dados:

```bash
docker compose --profile tools down
```

Removendo também os volumes (apaga o banco):

```bash
docker compose --profile tools down -v
```

O arquivo `.env` contém credenciais e não é versionado; altere as senhas de exemplo se o ambiente não for apenas local.

## Executar a solução

A solução fica em `OdontoManager.slnx`: projetos de produção em `src/` e testes em `tests/` (detalhes na [seção 9 do `Docs/specification.md`](Docs/specification.md#9-estrutura-da-solução)).

### Configurar a conexão com o banco

A connection string fica nos User Secrets da API, nunca nos arquivos versionados. Com os valores do `.env.example`:

```bash
dotnet user-secrets set "ConnectionStrings:OdontoManager" "Host=localhost;Port=5432;Database=odonto_manager;Username=odonto;Password=odonto_dev_password" --project src/OdontoManager.Api
```

### Subir a API e a Web

Com o banco no ar (`docker compose up -d`), em dois terminais:

```bash
dotnet run --project src/OdontoManager.Api
```

```bash
dotnet run --project src/OdontoManager.Web
```

| Aplicação | Endereço |
|-----------|----------|
| Web | http://localhost:5090 |
| API | http://localhost:5080 |
| Documentação da API (só em desenvolvimento) | http://localhost:5080/scalar |
| Saúde da API | http://localhost:5080/health/live e http://localhost:5080/health/ready |

A página inicial da Web indica se o sistema está disponível, consultando a prontidão da API.

## Testes

Compilar sem warnings (eles são tratados como erros):

```bash
dotnet build
```

Rodar os testes de unidade, integração, componentes e arquitetura (os de integração sobem um PostgreSQL próprio via Docker):

```bash
dotnet test -- --filter-not-trait "Category=E2E"
```

### Testes ponta a ponta

Os testes E2E usam o Playwright. Instale o Chromium uma vez, depois de compilar:

```bash
pwsh tests/OdontoManager.E2ETests/bin/Debug/net10.0/playwright.ps1 install chromium
```

```bash
dotnet test --project tests/OdontoManager.E2ETests -- --filter-trait "Category=E2E"
```

Para usar um Chromium já instalado na máquina, defina `PLAYWRIGHT_CHROMIUM_EXECUTABLE_PATH` com o caminho do executável.

## Commits

O projeto usa commits semânticos (Conventional Commits em pt-BR). Ative os hooks que validam mensagens de commit e pushes, uma vez por clone:

```bash
git config core.hooksPath .githooks
```

Formato: `<tipo>(<escopo>)!: <descrição>`, por exemplo `feat(agenda): adiciona remarcação de consultas`. Tipos, escopos e regras completas estão na [seção 13 do `Docs/specification.md`](Docs/specification.md#13-commits-semânticos).

## Fluxo de trabalho

A `main` é a branch principal e só recebe alterações por pull request. Cada implementação, feature ou correção começa em uma branch nova a partir da `main`:

```bash
git switch main
```

```bash
git pull
```

```bash
git switch -c feat/add-solution-skeleton
```

As branches seguem o padrão `<tipo>/<nome-da-change>`, com os mesmos tipos dos commits. O pull request tem título no formato de commit semântico e entra na `main` por squash merge. Detalhes na [seção 14 do `Docs/specification.md`](Docs/specification.md#14-branches-e-pull-requests).
