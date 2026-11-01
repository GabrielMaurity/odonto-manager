# Tasks

## 1. Emenda da constituição

- [ ] 1.1 Adicionar a linha "Ambiente local | Docker + Docker Compose v2" na tabela do Artigo III de `constitution.md` e a cláusula exigindo Docker Compose para a infraestrutura local com imagens em tag fixa (design.md D1); verificar com `grep -n "Docker" constitution.md`
- [ ] 1.2 Atualizar a tabela de versão para 1.1.0, com a data da emenda; verificar que o cabeçalho mostra a nova versão e a data na coluna "Última emenda"

## 2. Arquivos de ambiente e segredos

- [ ] 2.1 Criar `.gitignore` na raiz ignorando `.env` (sem ignorar `.env.example`); verificar com `git check-ignore .env` (ignorado) e `git check-ignore .env.example` (não ignorado)
- [ ] 2.2 Criar `.env.example` com `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_PORT`, `PGADMIN_DEFAULT_EMAIL`, `PGADMIN_DEFAULT_PASSWORD` e `PGADMIN_PORT`, apenas com valores de desenvolvimento; verificar que nenhum valor real foi versionado

## 3. docker-compose.yaml

- [ ] 3.1 Criar `docker-compose.yaml` com `name: odonto-manager`, sem a chave `version:`, e o serviço `postgres` (`postgres:18-alpine`, volume `postgres-data` em `/var/lib/postgresql`, healthcheck `pg_isready`, porta `127.0.0.1:${POSTGRES_PORT}:5432`, variáveis com `${VAR:?}`) conforme D2–D4, D6 e D7; verificar com `docker compose config` sem erros
- [ ] 3.2 Adicionar o serviço `pgadmin` (`dpage/pgadmin4` com tag fixa, profile `tools`, volume `pgadmin-data`, porta em `127.0.0.1`, `depends_on` do `postgres` com `service_healthy`) conforme D5; verificar com `docker compose --profile tools config` e confirmar que `docker compose config --services` sem profile lista apenas `postgres`
- [ ] 3.3 Subir o ambiente com `cp .env.example .env && docker compose up -d` e verificar que `docker compose ps` mostra `postgres` como `healthy` e que `docker compose exec postgres psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" -c 'select version();'` retorna PostgreSQL 18
- [ ] 3.4 Verificar a persistência: criar uma tabela de teste, executar `docker compose down` e `docker compose up -d`, e confirmar que a tabela continua existindo; depois remover a tabela de teste
- [ ] 3.5 Verificar o erro de variável ausente: sem o `.env`, `docker compose config` DEVE falhar com mensagem indicando a variável faltante

## 4. Documentação

- [ ] 4.1 Criar `README.md` com pré-requisitos (Docker Engine 24+, Compose v2), passos para subir (`cp .env.example .env`, `docker compose up -d`), uso do pgAdmin (`--profile tools`) e como derrubar com ou sem remover volumes; verificar executando os comandos do README exatamente como escritos

## Workflow follow-up

- Arquivar esta change e a `add-project-constitution` com `/opsx:archive` após revisão.
- Em `add-solution-skeleton`, adicionar os serviços `api` e `web` a este compose.
