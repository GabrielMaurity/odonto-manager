# Spec Delta

## Purpose

Disponibiliza a documentação navegável dos contratos da API para quem desenvolve o sistema, sem expor a superfície da API em ambientes compartilhados ou de produção.

## ADDED Requirements

### Requirement: Documento OpenAPI em desenvolvimento
No ambiente de desenvolvimento, a API SHALL publicar o documento OpenAPI em `GET /openapi/v1.json`.

#### Scenario: Documento disponível em desenvolvimento
- **GIVEN** a API roda no ambiente de desenvolvimento
- **WHEN** um cliente faz `GET /openapi/v1.json`
- **THEN** a API responde 200 com um documento OpenAPI válido

### Requirement: Interface de documentação em desenvolvimento
No ambiente de desenvolvimento, a API SHALL servir uma interface web de documentação em `GET /scalar`, baseada no documento OpenAPI.

#### Scenario: Interface disponível em desenvolvimento
- **GIVEN** a API roda no ambiente de desenvolvimento
- **WHEN** um cliente faz `GET /scalar`
- **THEN** a API responde 200 com a página da documentação

### Requirement: Documentação indisponível fora de desenvolvimento
Fora do ambiente de desenvolvimento, a API MUST NOT publicar o documento OpenAPI nem a interface de documentação.

#### Scenario: Documento fora de desenvolvimento
- **GIVEN** a API roda fora do ambiente de desenvolvimento
- **WHEN** um cliente faz `GET /openapi/v1.json` ou `GET /scalar`
- **THEN** a API responde 404
