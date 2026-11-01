# Tasks

## 1. Estrutura da solução

- [x] 1.1 Criar `global.json`, `Directory.Build.props`, `Directory.Packages.props` e `.editorconfig` (design.md D1) e acrescentar ao `.gitignore` os artefatos de build (`bin/`, `obj/`, `TestResults/`, `*.user`); verificar que `git check-ignore bin/x` ignora o caminho
- [x] 1.2 Criar os 7 projetos de `src/` e os 6 de `tests/` com as referências da tabela de D1 e montar o `OdontoManager.slnx`; verificar com `dotnet build` sem warnings
- [x] 1.3 Verificar que uma violação proposital de estilo (variável não usada) quebra o build e depois remover a violação; verificar com `dotnet build` falhando e depois passando

## 2. Infraestrutura de testes e testes de arquitetura

- [x] 2.1 Adicionar os pacotes de teste do catálogo (specification 11) e um teste de fumaça em cada projeto de teste de unidade; verificar com `dotnet test`
- [x] 2.2 Criar a `ApiFactory` com Testcontainers (`postgres:18.6-alpine` em constante única), Respawn e assembly fixture (D8), com um teste de fumaça que sobe a API e recebe 404 em rota inexistente; verificar com `dotnet test tests/OdontoManager.Api.IntegrationTests`
- [x] 2.3 Escrever em `ArchitectureTests` as regras de camadas, de módulos e de endpoints da specification 10.2, de forma genérica por convenção de namespace (D9). As regras de entidades, eventos e handlers entram nas tarefas 3.1 e 4.1, junto com os tipos que verificam. Verificar que passam e que uma referência proibida temporária (`Domain` → EF Core) faz o teste falhar, removendo-a em seguida

## 3. Shared Kernel

- [x] 3.1 Implementar `Entity<TId>`, `AggregateRoot<TId>` e `IDomainEvent` (D2), com testes de unidade de registro e limpeza de eventos e as regras de arquitetura "entidades sem setter público" e "eventos `sealed`"; verificar com `dotnet test tests/OdontoManager.Domain.UnitTests`
- [x] 3.2 Implementar `Result`, `Result<T>`, `Error`, `ErrorType` e `ValidationError` com conversões implícitas (D2), com testes de sucesso, falha, conversões e `Value` em falha lançando exceção; verificar com `dotnet test`
- [x] 3.3 Implementar `UsuarioId`, `PacienteId` e `DentistaId` com `Novo()` via GUID v7, com testes de igualdade e de ordenação temporal; verificar com `dotnet test`

## 4. Messaging da aplicação

- [x] 4.1 Implementar `ICommand`, `IQuery`, `ICommandHandler`, `IQueryHandler`, `Unit`, `IUnitOfWork` e `IDomainEventHandler` em `Application/Abstractions` e `AddApplication()` com varredura via Scrutor (D3), com a regra de arquitetura "handlers `sealed` e não públicos"; verificar com teste que resolve um handler de teste pela interface
- [x] 4.2 Implementar os decorators de validação e de logging (D3), com testes de que a validação devolve `ValidationError` agrupado por campo sem chamar o handler e de que o log não contém o conteúdo do comando; verificar com `dotnet test tests/OdontoManager.Application.UnitTests`

## 5. API base

- [ ] 5.1 Configurar ProblemDetails com `traceId`, `UseStatusCodePages` e `IExceptionHandler` global (D4); verificar com testes de integração dos cenários de `platform/api-errors` para 404 de rota, `traceId` e 500 com e sem detalhes conforme o ambiente
- [ ] 5.2 Implementar `Error.ToProblem()` com a tabela da specification 5.2 (D4); verificar com testes de integração, usando endpoints só de teste, para 400 com `errors` por campo, 404, 409, 422 e a extensão `code`
- [ ] 5.3 Configurar o grupo `/api/v1`, a convenção `Add<Modulo>Module`/`Map<Modulo>Endpoints`, HSTS, redirecionamento HTTPS e a política CORS `Web` (D4); verificar com teste de integração de preflight CORS para uma origem permitida e uma negada
- [ ] 5.4 Configurar OpenAPI e Scalar apenas em `Development` (D4); verificar com testes de integração dos cenários de `platform/api-documentation` nos ambientes `Development` e `Production`

## 6. Persistência base

- [ ] 6.1 Implementar `ModuleDbContext`, `AddModuleDbContext<TContext>` e as conversões dos IDs do Shared Kernel (D5); verificar com teste de integração, usando um `DbContext` só de teste, que a tabela é criada no schema do módulo, em snake_case, com IDs `uuid`
- [ ] 6.2 Implementar o `DomainEventsInterceptor` (D5); verificar com testes de integração que os eventos são despachados após o commit, que não são despachados quando o `SaveChanges` falha e que a falha de um handler não impede os demais

## 7. Transversais e health checks

- [ ] 7.1 Registrar `TimeProvider` e `ClinicaOptions` com validação do fuso e `ValidateOnStart` (D6); verificar com teste de integração de que a API não inicia com um fuso inválido
- [ ] 7.2 Configurar OpenTelemetry com exportador OTLP condicionado à variável de ambiente (D6); verificar com teste de que a API inicia sem `OTEL_EXPORTER_OTLP_ENDPOINT` definido
- [ ] 7.3 Configurar `/health/live` e `/health/ready` com o check do PostgreSQL (D6); verificar com testes de integração de todos os cenários de `platform/health-checks`, inclusive banco parado (container pausado) e corpo sem detalhes fora de `Development`
- [ ] 7.4 Configurar User Secrets no `Api` e executar a API com `dotnet run` contra o PostgreSQL do `docker-compose.yaml`; verificar que `curl http://localhost:5080/health/ready` responde 200 e, com o `postgres` parado, 503

## 8. Web base

- [ ] 8.1 Criar a Web (template Blazor com Interactive Auto) com Radzen, `MainLayout`, `NavMenu` e página `NotFound`, e a cultura pt-BR no servidor e no cliente (D7); verificar com testes bUnit do layout, da página não encontrada e da formatação pt-BR
- [ ] 8.2 Implementar `ISystemStatusClient` e a indicação de disponibilidade na página inicial (D7); verificar com testes bUnit dos cenários "API disponível" e "API indisponível" de `platform/web-shell`, sem detalhes técnicos na mensagem
- [ ] 8.3 Criar o teste E2E com Playwright, sobre a Web em Kestrel real (D8), que abre a página inicial e verifica layout, nome do sistema e "sistema indisponível"; verificar com `dotnet test --filter Category=E2E`. Se os navegadores não estiverem disponíveis no ambiente, reportar e manter a tarefa pendente
- [ ] 8.4 Executar API e Web com `dotnet run` e verificar no navegador que a página inicial mostra "sistema disponível" com o PostgreSQL ativo

## 9. Documentação e planejamento

- [ ] 9.1 Atualizar o `README.md` com pré-requisitos (.NET SDK 10, Docker), configuração dos User Secrets, comandos para rodar API e Web, `dotnet test` e o filtro dos testes E2E; verificar executando os comandos como escritos
- [ ] 9.2 Registrar a divisão da Fase 1 no `Docs/plan.md` (nova ficha `add-ci-pipeline` e critérios movidos), no `Docs/tasks.md` (P1.E1.T8 e P1.E1.T11 riscadas com remissão para a nova etapa P1.E2 e link para este `tasks.md`) e no `Docs/checklist.md` (itens de Docker e CI atribuídos a `add-ci-pipeline` e entregas desta change marcadas, com o resumo atualizado); verificar que âncoras e contagens batem
- [ ] 9.3 Executar `dotnet build` (sem warnings), `dotnet test` (todos os testes) e `openspec validate add-solution-skeleton --strict`; verificar que tudo passa

## Workflow follow-up

- Abrir o pull request desta change para a `main`, com título semântico e squash merge (specification 14).
- Arquivar a change com `/opsx:archive` após o merge.
- Iniciar a etapa P1.E2 com `/opsx:propose add-ci-pipeline`.
