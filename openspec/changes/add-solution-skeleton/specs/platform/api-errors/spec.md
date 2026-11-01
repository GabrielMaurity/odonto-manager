# Spec Delta

## Purpose

Define o contrato único de respostas de erro da API, para que o frontend e futuros clientes tratem falhas de forma previsível e sem que a API vaze informações internas ou dados de pacientes.

## ADDED Requirements

### Requirement: Erros no formato ProblemDetails
Toda resposta de erro da API (status 4xx ou 5xx) SHALL ter corpo no formato ProblemDetails (RFC 9457), com `Content-Type: application/problem+json` e os campos `type`, `title` e `status`.

#### Scenario: Rota inexistente
- **WHEN** um cliente faz uma requisição para uma rota que não existe
- **THEN** a API responde 404 com corpo `application/problem+json` contendo `status` igual a 404

### Requirement: Código de erro de negócio
Quando o erro vier de uma regra da aplicação, a resposta SHALL incluir a extensão `code`, com o identificador estável do erro no formato `Contexto.Erro`.

#### Scenario: Erro de negócio com código
- **GIVEN** uma operação falha com o erro de código `Agenda.ConsultaNaoPodeSerCancelada`
- **WHEN** a API monta a resposta
- **THEN** o corpo contém `"code": "Agenda.ConsultaNaoPodeSerCancelada"`

### Requirement: Status HTTP por tipo de erro
A API SHALL mapear cada tipo de erro da aplicação para um status HTTP fixo: validação → 400, não encontrado → 404, conflito → 409, regra de negócio → 422.

#### Scenario: Erro de não encontrado
- **WHEN** uma operação falha com erro do tipo não encontrado
- **THEN** a API responde 404

#### Scenario: Erro de conflito
- **WHEN** uma operação falha com erro do tipo conflito
- **THEN** a API responde 409

#### Scenario: Erro de regra de negócio
- **WHEN** uma operação falha com erro do tipo regra de negócio
- **THEN** a API responde 422

### Requirement: Erros de validação por campo
Erros de validação SHALL responder 400 com a extensão `errors`, que associa cada campo inválido à lista de mensagens daquele campo.

#### Scenario: Dois campos inválidos
- **GIVEN** uma requisição com os campos `nome` e `email` inválidos
- **WHEN** a API valida a requisição
- **THEN** a resposta é 400 e `errors` contém as chaves `nome` e `email`, cada uma com ao menos uma mensagem

### Requirement: Falhas inesperadas sem detalhes internos
Uma exceção não tratada SHALL resultar em resposta 500 em ProblemDetails. Fora do ambiente de desenvolvimento, a resposta MUST NOT conter mensagem de exceção, stack trace ou nomes de tipos internos.

#### Scenario: Exceção fora de desenvolvimento
- **GIVEN** a API roda fora do ambiente de desenvolvimento
- **WHEN** o processamento de uma requisição lança uma exceção não tratada
- **THEN** a API responde 500 em ProblemDetails, sem mensagem de exceção nem stack trace no corpo

#### Scenario: Exceção em desenvolvimento
- **GIVEN** a API roda no ambiente de desenvolvimento
- **WHEN** o processamento de uma requisição lança uma exceção não tratada
- **THEN** a API responde 500 em ProblemDetails, e o corpo PODE incluir detalhes da exceção para diagnóstico

### Requirement: Identificador de rastreio nas respostas de erro
Toda resposta de erro SHALL incluir a extensão `traceId`, que permite localizar a requisição nos logs e traces sem expor dados pessoais.

#### Scenario: Erro com traceId
- **WHEN** a API responde com qualquer erro
- **THEN** o corpo contém `traceId` não vazio
