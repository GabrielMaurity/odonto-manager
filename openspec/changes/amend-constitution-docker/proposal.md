# Proposal

## Why

O projeto vai usar Docker para padronizar o ambiente de desenvolvimento: todo desenvolvedor sobe o mesmo PostgreSQL, na mesma versão, com um único comando e sem instalar nada além do Docker. O Testcontainers, já previsto no Artigo III, também depende de Docker. Como a constituição não cita Docker, o Artigo X exige uma emenda antes de adotá-lo.

## What Changes

- **Emenda MINOR na constituição (1.0.0 → 1.1.0):**
  - Artigo III passa a incluir Docker e Docker Compose como ferramentas oficiais do ambiente local.
  - Novo item no Artigo III: dependências de infraestrutura do ambiente local DEVEM subir via Docker Compose, com imagens em tag fixa (nunca `latest`).
  - Atualização da tabela de versão no topo da constituição.
- **`docker-compose.yaml` inicial na raiz**, apenas com infraestrutura:
  - `postgres`: PostgreSQL com volume persistente, healthcheck e porta exposta só em `localhost`.
  - `pgadmin`: pgAdmin opcional, ativado só pelo profile `tools`.
- **`.env.example`** versionado com as variáveis do compose; o `.env` real fica fora do Git.
- **`.gitignore`** inicial, ignorando o `.env`.
- **README** curto explicando como subir e derrubar o ambiente.
- Os serviços `api` e `web` (e seus Dockerfiles) ficam **fora** desta change: entram em `add-solution-skeleton`, quando houver código para buildar.

**Módulo (Artigo IV):** nenhum módulo de negócio; é infraestrutura transversal de desenvolvimento.

## Capabilities

### New Capabilities
<!-- Nenhuma: ferramentas de ambiente e documentação, sem mudança de comportamento do sistema (skip_specs: true). -->

### Modified Capabilities
<!-- Nenhuma. -->

## Impact

- **Arquivos novos:** `docker-compose.yaml`, `.env.example`, `.gitignore`, `README.md`.
- **Arquivos alterados:** `constitution.md` (Artigo III e tabela de versão).
- **Dependências:** Docker Engine 24+ com o plugin Docker Compose v2 na máquina de quem desenvolve.
- **Changes futuras:** `add-solution-skeleton` DEVE adicionar os serviços `api` e `web` a este compose e apontar a connection string de desenvolvimento para o `postgres` daqui.
