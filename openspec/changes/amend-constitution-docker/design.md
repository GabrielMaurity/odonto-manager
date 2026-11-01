# Design

## Context

O repositório tem apenas `constitution.md` (v1.0.0), a configuração do OpenSpec e as skills em `.claude/`; ainda não há solução .NET, `.gitignore` nem README. A motivação está em proposal.md — Why. Artigos envolvidos: III (stack), VI.4 (segredos fora do repositório), VII.3 (testes de integração com PostgreSQL real) e X (emendas).

## Goals / Non-Goals

**Goals:**
- Subir o banco de desenvolvimento com `docker compose up -d`, sem nenhuma instalação além do Docker.
- Deixar o compose pronto para receber os serviços `api` e `web` sem precisar ser reestruturado.
- Formalizar o Docker na constituição (Artigo X).

**Non-Goals:**
- Dockerfiles da API e do Blazor (change `add-solution-skeleton`).
- Ambiente de produção: orquestração, TLS, backups e cofre de segredos.
- Serviços de observabilidade (Seq, Grafana) ou de armazenamento de arquivos de exames (avaliar em `add-exam-files`).

## Decisions

### D1. Emenda MINOR em vez de PATCH (Artigo X.3)
Adicionar um item de stack é MINOR pela regra de versionamento: 1.0.0 → 1.1.0. A emenda acrescenta a linha "Ambiente local | Docker + Docker Compose v2" na tabela do Artigo III e uma cláusula exigindo Compose para a infraestrutura local e imagens com tag fixa.

### D2. Nome e formato do arquivo
`docker-compose.yaml` na raiz, como pedido. O Compose v2 também aceita `compose.yaml`, mas o nome pedido é igualmente suportado e mais reconhecível. O arquivo NÃO usa a chave `version:`, que está obsoleta no Compose v2, e define `name: odonto-manager` para o nome do projeto não depender do diretório.

### D3. PostgreSQL 18 em imagem Alpine, com tag fixa
Imagem `postgres:18-alpine` (Artigo III: PostgreSQL; nova cláusula de tag fixa). A versão 18 é a estável atual e tem suporte do Npgsql/EF Core 10. A variante Alpine reduz o tamanho da imagem.
- A partir da imagem 18, os dados ficam em `/var/lib/postgresql/18/docker`. Por isso o volume nomeado é montado em `/var/lib/postgresql`, e não no caminho antigo `/var/lib/postgresql/data`; isso também facilita upgrades de versão maior.
- *Alternativa*: PostgreSQL 17. Rejeitada: não há motivo para começar um projeto novo em versão anterior.

### D4. Healthcheck com `pg_isready`
O serviço `postgres` tem healthcheck com `pg_isready`. Assim, quando `api` entrar no compose, ela poderá usar `depends_on: condition: service_healthy` e só subir com o banco pronto.

### D5. pgAdmin opcional, via profile `tools`
`dpage/pgadmin4` com tag fixa, ativado só com `docker compose --profile tools up -d`. Quem usa outro cliente (DBeaver, Rider) não paga o custo de memória.
- *Alternativa*: não incluir ferramenta visual. Rejeitada: facilita a inspeção do banco no início do projeto, com custo zero quando desligada.

### D6. Variáveis em `.env`, com `.env.example` versionado (Artigo VI.4)
Usuário, senha, nome do banco e portas vêm de variáveis de ambiente com interpolação `${VAR:?erro}`: o compose falha com uma mensagem clara se uma variável obrigatória faltar, em vez de subir com valor vazio. O `.env` fica no `.gitignore`; o `.env.example` traz apenas valores de exemplo para desenvolvimento.
- *Alternativa*: Docker secrets. Rejeitada para o ambiente local: complexidade sem ganho real; pode ser reavaliada para produção.

### D7. Portas expostas só em `127.0.0.1`
As portas são publicadas como `127.0.0.1:${POSTGRES_PORT}:5432`, para que o banco com dados de teste não fique acessível pela rede local (Artigo VI). A porta é configurável para evitar conflito com um PostgreSQL já instalado na máquina.

### D8. Rede e volumes nomeados
Volumes nomeados (`postgres-data`, `pgadmin-data`) e a rede padrão do projeto. Uma rede explícita só será criada se surgir a necessidade de isolar serviços.

## Risks / Trade-offs

- [Desenvolvedor esquece de criar o `.env`] → Interpolação com `:?` gera erro explícito; o README manda copiar o `.env.example`.
- [Conflito com PostgreSQL local na porta 5432] → Porta configurável via `POSTGRES_PORT`.
- [Tag fixa ficar desatualizada] → Atualizar a tag é uma mudança explícita e revisada; o Dependabot poderá ser configurado em `add-solution-skeleton`.
- [Volume montado no caminho errado e perda de dados entre recriações] → D3 define o caminho correto da imagem 18; a tarefa de verificação recria o container e confere que os dados persistem.
