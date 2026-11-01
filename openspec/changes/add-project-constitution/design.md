# Design

## Context

Repositório greenfield: contém apenas a estrutura do OpenSpec (`openspec/config.yaml`, `openspec/specs/` vazio) e as skills/comandos do Claude Code em `.claude/`. A motivação está em proposal.md — Why. Restrições já definidas pelo responsável do projeto: backend em .NET (C#), perfis Dentista, Recepcionista/Secretária e Administrador, uso de SOLID, comentários apenas quando muito necessários e commits sem assinatura.

## Goals / Non-Goals

**Goals:**
- Produzir uma constituição curta, normativa e verificável, que sirva de critério de revisão para toda change futura.
- Tornar a constituição automaticamente visível ao fluxo OpenSpec (via `context` do `config.yaml`).
- Fixar as decisões estruturais que, se adiadas, causariam retrabalho entre módulos (stack, camadas, perfis, convenções).

**Non-Goals:**
- Criar código, solução .NET, projetos de teste ou pipeline de CI (change `add-solution-skeleton`).
- Especificar regras de negócio de agenda, materiais ou exames (changes próprias do roadmap).
- Definir infraestrutura de produção (cloud, hospedagem) — será tratado quando houver deploy.

## Decisions

### D1. Local e formato da constituição
`constitution.md` na raiz do repositório, em pt-BR, com artigos numerados e linguagem normativa (DEVE / NÃO DEVE / PODE).
- *Alternativa*: `openspec/project.md` (convenção antiga do OpenSpec). Rejeitada: o nome “constituição” foi pedido explicitamente e a raiz dá visibilidade a quem não usa OpenSpec.
- *Alternativa*: colar todo o conteúdo em `config.yaml > context`. Rejeitada: o contexto é injetado em toda instrução e tem limite de tamanho; manter um resumo + referência ao arquivo é mais sustentável.

### D2. Integração com o OpenSpec
O `context` do `openspec/config.yaml` passa a conter um resumo dos princípios inegociáveis e a instrução de que todo artefato DEVE respeitar `constitution.md`. Regras por artefato (`rules`) exigem, por exemplo, que `design.md` cite os artigos da constituição afetados e que `tasks.md` inclua tarefas de teste.

### D3. Stack
- **Backend**: .NET 10 (LTS) com ASP.NET Core Web API, EF Core e PostgreSQL. LTS garante suporte longo; PostgreSQL é gratuito, robusto e bem suportado pelo provider Npgsql.
  - *Alternativa*: SQL Server. Rejeitada por custo de licença e menor portabilidade; permanece viável via EF Core se necessário.
- **Frontend**: SPA em React + TypeScript (Vite), consumindo a API REST.
  - *Alternativas*: Angular (mais opinativo, curva maior) e Blazor (mesma linguagem do backend, ecossistema de componentes de calendário menor). React foi escolhido pelo ecossistema maduro de componentes de agenda/calendário, central para o produto. A escolha pode ser trocada por emenda antes de `add-solution-skeleton` sem impacto em outras decisões.
- **Testes**: xUnit + FluentAssertions + Testcontainers (PostgreSQL real em testes de integração); Vitest + Testing Library no frontend.

### D4. Arquitetura: Clean Architecture em camadas
Projetos `Domain` → `Application` → `Infrastructure` / `Api`, com dependências apontando sempre para o domínio. Casos de uso explícitos na camada Application (um handler por caso de uso), interfaces de repositório definidas em Application/Domain e implementadas em Infrastructure. Isso materializa SOLID: SRP por caso de uso, DIP via injeção de dependência nativa do ASP.NET Core, OCP/ISP com interfaces pequenas.
- *Alternativa*: Vertical Slices puras. Rejeitada por ora: menos familiar e com fronteiras menos rígidas; pode ser combinada dentro de Application (organização por feature) sem violar a regra de dependência.
- *Alternativa*: microsserviços. Rejeitada: complexidade operacional injustificada para uma clínica; o monólito modular permite extração futura.

### D5. Monólito modular por contexto de negócio
Módulos: Identidade/Acesso, Pacientes, Profissionais (perfil do dentista), Agenda, Materiais, Exames. Cada módulo só acessa outro por contratos da camada Application, nunca por entidades ou tabelas alheias.

### D6. Segurança e LGPD
Dados de pacientes e exames são dados pessoais sensíveis de saúde. A constituição exige: autorização por perfil e por recurso (dentista vê apenas o que lhe compete), trilha de auditoria de acesso a prontuário/exames, criptografia em trânsito (TLS) e de arquivos em repouso, segredos fora do repositório e retenção/eliminação conforme LGPD.

### D7. Convenções de código e Git
Convenções oficiais do C# (.NET) e ESLint/Prettier no frontend, `.editorconfig` versionado, análise estática com warnings tratados como erros, nullable habilitado. Conventional Commits em pt-BR, sem assinatura ou linhas de coautoria. Comentários apenas para explicar o “porquê” não óbvio.

### D8. Processo de emenda
Alterar a constituição exige uma change OpenSpec própria (`amend-constitution-<tema>`) com justificativa; a constituição registra versão e data da última emenda.

## Risks / Trade-offs

- [Constituição genérica demais e ignorada na prática] → Artigos curtos e verificáveis, referenciados automaticamente pelo `config.yaml` e cobrados nas regras de `design.md`/`tasks.md`.
- [Rigidez excessiva para um projeto inicial] → Processo de emenda leve (D8) e separação entre princípios inegociáveis e diretrizes recomendadas.
- [Escolha de frontend (React) feita sem confirmação explícita] → Registrada como decisão reversível por emenda antes da change `add-solution-skeleton`.
- [Clean Architecture gerar boilerplate] → Permitir organização por feature dentro de Application e evitar abstrações sem segundo uso.
