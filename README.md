# Odonto Manager

Sistema de gestão de clínicas odontológicas. Os princípios e decisões do projeto estão em [`constitution.md`](constitution.md); o desenvolvimento segue SDD com [OpenSpec](https://github.com/Fission-AI/OpenSpec) (changes em `openspec/changes/`).

## Ambiente de desenvolvimento

### Pré-requisitos

- Docker Engine 24+ com o plugin Docker Compose v2 (`docker compose version`)

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
