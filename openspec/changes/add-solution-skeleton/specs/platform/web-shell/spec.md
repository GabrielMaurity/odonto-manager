# Spec Delta

## Purpose

Fornece a estrutura visual comum da aplicação Web, onde os módulos de negócio vão inserir suas telas, e mostra ao usuário se o sistema está disponível para uso.

## ADDED Requirements

### Requirement: Layout base
A aplicação Web SHALL exibir em todas as páginas um layout comum, com o nome do sistema, um menu de navegação lateral e uma área de conteúdo.

#### Scenario: Página inicial
- **WHEN** um usuário abre a página inicial da aplicação Web
- **THEN** a página exibe o nome "Odonto Manager", o menu de navegação e a área de conteúdo

### Requirement: Página não encontrada
A aplicação Web SHALL exibir, dentro do layout base, uma página de "página não encontrada" para endereços que não existem.

#### Scenario: Endereço inexistente
- **WHEN** um usuário abre um endereço da aplicação Web que não existe
- **THEN** a aplicação exibe a mensagem de página não encontrada dentro do layout, com um link para a página inicial

### Requirement: Indicação de disponibilidade do sistema
A página inicial SHALL indicar se a API está pronta para atender, com base no endpoint de prontidão da API, e informar o usuário de forma clara quando ela estiver indisponível.

#### Scenario: API disponível
- **GIVEN** a API responde 200 em `/health/ready`
- **WHEN** um usuário abre a página inicial
- **THEN** a página indica que o sistema está disponível

#### Scenario: API indisponível
- **GIVEN** a API não responde ou responde 503 em `/health/ready`
- **WHEN** um usuário abre a página inicial
- **THEN** a página indica que o sistema está indisponível, sem exibir detalhes técnicos do erro

### Requirement: Interface em português
Todos os textos da aplicação Web SHALL estar em pt-BR, e datas e números SHALL usar a formatação da cultura pt-BR.

#### Scenario: Textos e formatação
- **WHEN** um usuário abre qualquer página da aplicação Web
- **THEN** os textos estão em português e datas e números seguem o formato brasileiro
