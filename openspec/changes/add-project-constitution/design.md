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
- **Frontend**: Blazor Web App (.NET 10) com render mode Interactive Auto, consumindo a API REST por clientes HTTP tipados. Contratos (DTOs de request/response) ficam em um projeto `Contracts` compartilhado entre `Api` e `Web`. Biblioteca de componentes: Radzen Blazor (licença MIT), que inclui Scheduler para a agenda, DataGrid e formulários com validação.
  - *Alternativas*: React + TypeScript (ecossistema de calendário maior, mas exige uma segunda linguagem e duplica os contratos) e Angular (mais opinativo, curva maior). Blazor foi escolhido para manter uma única linguagem (C#) em toda a solução, reaproveitar validações e contratos e testar a maturidade atual do framework. Para componentes, MudBlazor foi considerado, mas não traz scheduler nativo.
  - *Por que manter a API mesmo com Blazor*: o `Web` não acessa Application nem Infrastructure diretamente. Isso preserva a regra de dependência (D4), permite clientes futuros (app mobile, integrações) e faz o Interactive Auto funcionar igual no servidor e no WebAssembly.
- **Testes**: xUnit + FluentAssertions + Testcontainers (PostgreSQL real em testes de integração); bUnit para componentes Blazor; Playwright para testes ponta a ponta.

### D4. Arquitetura: Clean Architecture em camadas
Projetos `Domain` → `Application` → `Infrastructure` / `Api`, com dependências apontando sempre para o domínio. O frontend `Web` (Blazor) depende apenas de `Contracts` e fala com o backend exclusivamente via HTTP. Casos de uso explícitos na camada Application (um handler por caso de uso), interfaces de repositório definidas em Application/Domain e implementadas em Infrastructure. Isso materializa SOLID: SRP por caso de uso, DIP via injeção de dependência nativa do ASP.NET Core, OCP/ISP com interfaces pequenas.
- *Alternativa*: Vertical Slices puras. Rejeitada por ora: menos familiar e com fronteiras menos rígidas; pode ser combinada dentro de Application (organização por feature) sem violar a regra de dependência.
- *Alternativa*: microsserviços. Rejeitada: complexidade operacional injustificada para uma clínica; o monólito modular permite extração futura.

### D5. Monólito modular por contexto de negócio
Módulos: Identidade/Acesso, Pacientes, Profissionais (perfil do dentista), Agenda, Materiais, Exames. Cada módulo só acessa outro por contratos da camada Application, nunca por entidades ou tabelas alheias.

### D6. Segurança e LGPD
Dados de pacientes e exames são dados pessoais sensíveis de saúde. A constituição exige: autorização por perfil e por recurso (dentista vê apenas o que lhe compete), trilha de auditoria de acesso a prontuário/exames, criptografia em trânsito (TLS) e de arquivos em repouso, segredos fora do repositório e retenção/eliminação conforme LGPD.

### D7. Convenções de código e Git
Convenções oficiais do C# (.NET) em toda a solução, inclusive no frontend Blazor, `.editorconfig` versionado, análise estática com warnings tratados como erros, nullable habilitado. Conventional Commits em pt-BR, sem assinatura ou linhas de coautoria. Comentários apenas para explicar o “porquê” não óbvio.

### D8. Processo de emenda
Alterar a constituição exige uma change OpenSpec própria (`amend-constitution-<tema>`) com justificativa; a constituição registra versão e data da última emenda.

## Risks / Trade-offs

- [Constituição genérica demais e ignorada na prática] → Artigos curtos e verificáveis, referenciados automaticamente pelo `config.yaml` e cobrados nas regras de `design.md`/`tasks.md`.
- [Rigidez excessiva para um projeto inicial] → Processo de emenda leve (D8) e separação entre princípios inegociáveis e diretrizes recomendadas.
- [Interactive Auto adiciona complexidade (componentes rodam no servidor e no WebAssembly)] → Componentes interativos ficam em um projeto `Web.Client` sem dependência de servidor e só usam clientes HTTP; se a complexidade não compensar, a troca para Interactive Server é uma emenda local que não afeta os demais artigos.
- [Scheduler do Radzen não cobrir alguma necessidade da agenda] → Validar o componente logo na change `add-appointment-scheduling`; isolar o uso dele atrás de componentes próprios do projeto para permitir a troca.
- [Clean Architecture gerar boilerplate] → Permitir organização por feature dentro de Application e evitar abstrações sem segundo uso.
