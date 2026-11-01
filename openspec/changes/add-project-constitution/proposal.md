# Proposal

## Why

O odonto-manager é um projeto novo, desenvolvido com SDD (Spec-Driven Development) via OpenSpec. Antes de especificar qualquer módulo (agenda, perfil do dentista, pedidos de material, exames), é preciso um documento único e estável — a constituição — que fixe os princípios inegociáveis, a stack, a arquitetura e as convenções. Sem ela, cada change tomaria decisões estruturais isoladas e as specs perderiam coerência entre si.

## What Changes

- Criação de `constitution.md` na raiz do repositório contendo:
  - Missão e escopo do produto (gestão de clínica odontológica).
  - Princípios de engenharia inegociáveis: SOLID, Clean Architecture, testes automatizados, segurança e privacidade de dados de saúde (LGPD), comentários apenas quando estritamente necessários.
  - Stack tecnológica: .NET (C#) com ASP.NET Core Web API, EF Core e PostgreSQL; frontend em Blazor Web App (C#) com Radzen Blazor.
  - Arquitetura de referência em camadas (Domain, Application, Infrastructure, API) e regras de dependência entre elas.
  - Perfis de acesso: Dentista, Recepcionista/Secretária e Administrador da clínica.
  - Convenções de código, Git (Conventional Commits, commits sem assinatura/coautoria) e fluxo SDD com OpenSpec.
  - Roadmap de módulos, cada um a ser especificado em change própria.
  - Processo de emenda da constituição.
- Atualização de `openspec/config.yaml` para que o `context` referencie a constituição, garantindo que toda change futura a respeite.
- Nenhuma alteração de comportamento do sistema: a change é de governança/documentação e declara `skip_specs: true`.

## Capabilities

### New Capabilities
<!-- Nenhuma. Esta change não introduz comportamento do sistema; as capabilities nascerão nas changes do roadmap. -->

### Modified Capabilities
<!-- Nenhuma. -->

## Impact

- **Arquivos novos**: `constitution.md`.
- **Arquivos alterados**: `openspec/config.yaml` (campo `context`).
- **Código/APIs**: nenhum — ainda não há código no repositório.
- **Processo**: todas as próximas changes (agenda, perfil do dentista, pacientes, materiais, exames, autenticação) DEVEM seguir a constituição; divergências exigem emenda formal.
- **Roadmap previsto** (changes futuras, fora deste escopo):
  1. `add-solution-skeleton` — esqueleto da solução .NET (API + Blazor), CI e testes.
  2. `add-identity-access` — autenticação e autorização por perfil.
  3. `add-patient-registry` — cadastro de pacientes.
  4. `add-dentist-profile` — perfil do dentista.
  5. `add-appointment-scheduling` — agenda: marcar, desmarcar e remarcar consultas.
  6. `add-material-requests` — pedidos de material com aprovação do administrador.
  7. `add-exam-files` — upload e visualização de arquivos de exames.
