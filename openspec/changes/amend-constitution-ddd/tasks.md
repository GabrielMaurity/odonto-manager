# Tasks

## 1. Pré-condição

- [ ] 1.1 Confirmar que a `amend-constitution-docker` já foi aplicada (constituição em 1.1.0); verificar com `head -6 constitution.md`. Se não tiver sido, pausar e aplicá-la primeiro

## 2. Emenda da constituição (→ 1.2.0)

- [ ] 2.1 Adicionar ao Artigo II o princípio DDD (modelo rico, linguagem ubíqua, padrões táticos obrigatórios conforme `specification.md`); verificar com `grep -n "DDD" constitution.md`
- [ ] 2.2 Ajustar o Artigo IV para declarar cada módulo como bounded context (design.md D2) e adicionar a cláusula que torna o `specification.md` normativo, subordinado à constituição (D1); verificar que o Artigo IV cita "bounded context" e "specification.md"
- [ ] 2.3 Ajustar o Artigo VIII.3 para que o `design.md` cite também as seções do `specification.md` e o Artigo X para registrar que alterações no `specification.md` usam changes `update-specification-<tema>`; verificar com `grep -n "specification.md" constitution.md`
- [ ] 2.4 Atualizar a tabela de versão para 1.2.0 com a data da emenda; verificar o cabeçalho

## 3. specification.md — fundamentos e DDD

- [ ] 3.1 Criar `specification.md` com cabeçalho (versão 1.0.0, relação com a constituição), seção de visão geral da stack e seção de processo de atualização (D1); verificar que o arquivo referencia `constitution.md` como norma superior
- [ ] 3.2 Escrever a seção de DDD estratégico: bounded contexts, mapa de contextos e glossário da linguagem ubíqua em pt-BR com pelo menos os termos dos seis módulos (D2); verificar que todos os módulos do Artigo IV aparecem no mapa
- [ ] 3.3 Escrever a seção de DDD tático: agregados, entidades, objetos de valor, IDs fortemente tipados com GUID v7, eventos de domínio, repositórios, serviços de domínio e SharedKernel, com um exemplo curto de código C# para agregado e objeto de valor (D3); verificar que cada regra é verificável em revisão ou por teste de arquitetura

## 4. specification.md — aplicação, API e persistência

- [ ] 4.1 Escrever a seção da camada de aplicação: CQRS com `ICommandHandler`/`IQueryHandler`, decorators, consultas por projeção, FluentValidation e mapeamento manual (D4, D6); verificar que não há referência a MediatR ou AutoMapper como dependência
- [ ] 4.2 Escrever a seção de erros: Result pattern, tipos de `Error` e tabela de conversão para status HTTP com ProblemDetails (D5); verificar que a tabela cobre 400, 404, 409 e 422
- [ ] 4.3 Escrever a seção de API: Minimal APIs com `MapGroup` por módulo, versionamento na URL, `TypedResults`, OpenAPI e Scalar só em desenvolvimento (D7); verificar com um exemplo de endpoint consistente com as convenções
- [ ] 4.4 Escrever a seção de persistência: `DbContext` e schema por módulo, Fluent API, snake_case, objetos de valor, migrations, concorrência com `xmin` e uso do PostgreSQL do `docker-compose.yaml` (D8); verificar que a seção remete ao Docker Compose e ao Testcontainers

## 5. specification.md — transversais, estrutura e testes

- [ ] 5.1 Escrever a seção de aspectos transversais: `TimeProvider` e fuso horário, logging e OpenTelemetry sem dados pessoais, health checks, Options pattern e segredos (D9); verificar coerência com o Artigo VI
- [ ] 5.2 Escrever a seção de estrutura da solução com a árvore de diretórios (`.slnx`, `src/`, `tests/`, `Directory.Build.props`, `Directory.Packages.props`) e convenções de nomenclatura de pastas e namespaces (D10); verificar que os projetos batem com o Artigo IV.1
- [ ] 5.3 Escrever a seção de testes: pirâmide por camada, Testcontainers, testes de arquitetura com NetArchTest e a lista de regras de D11; verificar que cada regra do Artigo IV tem um teste de arquitetura correspondente
- [ ] 5.4 Escrever o catálogo de pacotes NuGet aprovados com finalidade e licença (D12); verificar que todo pacote citado no documento aparece no catálogo

## 6. Integração e verificação

- [ ] 6.1 Atualizar o `context` do `openspec/config.yaml` para citar `specification.md` e DDD; verificar com `openspec context --json` sem erro
- [ ] 6.2 Executar `openspec validate amend-constitution-ddd` e revisar `constitution.md` e `specification.md` contra este design, verificando que D1–D12 estão refletidos e que não há contradição entre os dois documentos

## Workflow follow-up

- Arquivar as changes concluídas com `/opsx:archive` após revisão.
- Seguir para `add-solution-skeleton`, que materializa a estrutura e as abstrações descritas no `specification.md`.
