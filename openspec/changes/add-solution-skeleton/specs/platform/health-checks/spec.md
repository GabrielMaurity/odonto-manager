# Spec Delta

## Purpose

Permite que operadores, orquestradores de containers e a própria aplicação Web saibam se a API está no ar e pronta para atender, incluindo a disponibilidade do banco de dados.

## ADDED Requirements

### Requirement: Endpoint de vivacidade
A API SHALL expor `GET /health/live`, que responde 200 enquanto o processo estiver em execução, sem depender de serviços externos.

#### Scenario: Processo em execução
- **WHEN** um cliente faz `GET /health/live` com a API em execução
- **THEN** a API responde 200

#### Scenario: Banco indisponível não afeta a vivacidade
- **GIVEN** o banco de dados está indisponível
- **WHEN** um cliente faz `GET /health/live`
- **THEN** a API responde 200

### Requirement: Endpoint de prontidão
A API SHALL expor `GET /health/ready`, que responde 200 apenas quando todas as dependências necessárias para atender requisições, incluindo o banco de dados, estão disponíveis, e 503 caso contrário.

#### Scenario: Dependências disponíveis
- **GIVEN** o banco de dados está acessível
- **WHEN** um cliente faz `GET /health/ready`
- **THEN** a API responde 200

#### Scenario: Banco indisponível
- **GIVEN** o banco de dados está inacessível
- **WHEN** um cliente faz `GET /health/ready`
- **THEN** a API responde 503

### Requirement: Health checks anônimos e sem detalhes internos
Os endpoints de health check SHALL ser acessíveis sem autenticação e, fora do ambiente de desenvolvimento, MUST NOT expor detalhes internos como nomes de servidores, strings de conexão ou mensagens de exceção.

#### Scenario: Acesso sem credenciais
- **WHEN** um cliente sem credenciais faz `GET /health/ready`
- **THEN** a API responde com o status de prontidão, e não com 401 ou 403

#### Scenario: Falha fora de desenvolvimento
- **GIVEN** a API roda fora do ambiente de desenvolvimento e o banco está inacessível
- **WHEN** um cliente faz `GET /health/ready`
- **THEN** o corpo da resposta não contém string de conexão, host do banco nem mensagem de exceção
