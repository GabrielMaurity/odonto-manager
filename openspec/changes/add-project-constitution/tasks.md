# Tasks

## 1. Redação da constituição

- [ ] 1.1 Criar `constitution.md` na raiz com cabeçalho (versão 1.0.0, data de ratificação) e o Artigo I — Missão e escopo do produto; verificar que o arquivo existe e descreve gestão de clínica odontológica com agenda, perfil do dentista, materiais e exames
- [ ] 1.2 Redigir o Artigo II — Princípios inegociáveis (SOLID, Clean Architecture, testes automatizados obrigatórios, segurança/LGPD, comentários apenas quando estritamente necessários) usando linguagem DEVE/NÃO DEVE; verificar que cada princípio é verificável em revisão de código
- [ ] 1.3 Redigir o Artigo III — Stack tecnológica conforme design.md D3 (.NET 10 LTS, ASP.NET Core Web API, EF Core, PostgreSQL, React + TypeScript/Vite, xUnit/Testcontainers, Vitest); verificar que versões e ferramentas batem com o design
- [ ] 1.4 Redigir o Artigo IV — Arquitetura (camadas Domain/Application/Infrastructure/Api, regra de dependência, monólito modular e módulos do D5, comunicação entre módulos só por contratos); verificar que a regra de dependência está explícita
- [ ] 1.5 Redigir o Artigo V — Perfis de acesso (Dentista, Recepcionista/Secretária, Administrador) com as responsabilidades de cada um e o princípio do menor privilégio; verificar que os três perfis estão descritos
- [ ] 1.6 Redigir o Artigo VI — Segurança e privacidade de dados de saúde conforme D6 (autorização por recurso, auditoria, criptografia, segredos fora do repositório, LGPD); verificar cobertura de todos os itens do D6
- [ ] 1.7 Redigir o Artigo VII — Convenções de código, testes e Git conforme D7 (Conventional Commits em pt-BR, commits sem assinatura/coautoria, warnings como erros, nullable habilitado); verificar consistência com as preferências do responsável
- [ ] 1.8 Redigir o Artigo VIII — Fluxo SDD com OpenSpec (toda funcionalidade nasce de uma change com proposal/specs/design/tasks; specs são fonte da verdade) e o Artigo IX — Roadmap de módulos listado em proposal.md; verificar que as sete changes do roadmap aparecem em ordem
- [ ] 1.9 Redigir o Artigo X — Processo de emenda conforme D8 (change `amend-constitution-<tema>`, versionamento semântico da constituição); verificar que o artigo define quem aprova e como versionar

## 2. Integração com o OpenSpec

- [ ] 2.1 Atualizar `context` em `openspec/config.yaml` com resumo dos princípios inegociáveis e a obrigação de respeitar `constitution.md`, mantendo as regras de idioma existentes; verificar com `openspec context --json` que o comando executa sem erro
- [ ] 2.2 Adicionar `rules` em `openspec/config.yaml` (design DEVE citar artigos da constituição afetados; tasks DEVE incluir tarefas de teste por grupo); verificar com `openspec instructions design --change add-project-constitution --json` que as regras aparecem no campo `rules`

## 3. Verificação final

- [ ] 3.1 Executar `openspec validate add-project-constitution` e verificar que a change é válida
- [ ] 3.2 Revisar `constitution.md` contra proposal.md e design.md e verificar que todas as decisões D1–D8 estão refletidas sem contradições

## Workflow follow-up

- Arquivar a change com `/opsx:archive` após revisão do responsável.
- Iniciar a próxima change do roadmap: `add-solution-skeleton`.
