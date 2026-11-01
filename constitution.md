# Constituição do Odonto Manager

| Versão | Ratificada em | Última emenda |
|--------|---------------|---------------|
| 1.0.0  | 2026-11-01    | —             |

Este documento define os princípios, decisões estruturais e convenções que regem o desenvolvimento do Odonto Manager. Toda change, spec, design, tarefa e linha de código DEVE estar em conformidade com ele. Em caso de conflito entre esta constituição e qualquer outro artefato, prevalece a constituição até que seja emendada (Artigo X).

As palavras **DEVE**, **NÃO DEVE** e **PODE** têm sentido normativo: obrigação, proibição e permissão, respectivamente.

---

## Artigo I — Missão e escopo

1. O Odonto Manager é um sistema de gestão de clínicas odontológicas.
2. O produto DEVE cobrir, ao longo do roadmap (Artigo IX):
   - agenda de consultas por profissional, com marcação, desmarcação e remarcação;
   - perfil do dentista, de onde ele acompanha suas consultas, faz pedidos de material e consulta arquivos de exames;
   - cadastro de pacientes;
   - pedidos de material com fluxo de aprovação;
   - armazenamento e visualização de arquivos de exames;
   - controle de acesso por perfil de usuário.
3. Funcionalidades fora deste escopo (financeiro, faturamento de convênios, portal do paciente, entre outras) só PODEM ser incluídas por emenda ao roadmap.

## Artigo II — Princípios inegociáveis

1. **SOLID.** Todo código de produção DEVE respeitar os princípios SOLID:
   - *SRP*: cada classe tem um único motivo para mudar; um caso de uso por handler.
   - *OCP*: comportamento novo entra por extensão (novas implementações), não por modificação de código estável.
   - *LSP*: implementações de uma abstração DEVEM ser substituíveis sem quebrar quem a consome.
   - *ISP*: interfaces DEVEM ser pequenas e específicas para quem as consome.
   - *DIP*: camadas de alto nível DEVEM depender de abstrações; implementações concretas são injetadas via contêiner de DI.
2. **Clean Architecture.** A regra de dependência do Artigo IV NÃO DEVE ser violada.
3. **Testes automatizados.** Toda regra de negócio DEVE ter testes de unidade; todo endpoint DEVE ter testes de integração; todo fluxo crítico de tela (agenda, pedido de material, exames) DEVE ter teste ponta a ponta. Código sem testes NÃO DEVE ser integrado à branch principal.
4. **Segurança e privacidade por padrão.** Dados de pacientes são dados pessoais sensíveis de saúde; os requisitos do Artigo VI são obrigatórios desde a primeira linha de código.
5. **Código autoexplicativo.** Nomes DEVEM expressar intenção. Comentários só DEVEM existir quando explicam um *porquê* não óbvio (regra legal, contorno de bug externo, decisão de desempenho). Comentários que repetem o que o código faz NÃO DEVEM ser escritos.
6. **Simplicidade.** Abstrações NÃO DEVEM ser criadas sem necessidade concreta; uma interface com uma única implementação só é aceita quando existe por inversão de dependência entre camadas ou para testabilidade.

## Artigo III — Stack tecnológica

| Área | Tecnologia |
|------|------------|
| Plataforma | .NET 10 (LTS), C# com `Nullable` habilitado |
| Backend | ASP.NET Core Web API (REST, JSON, OpenAPI) |
| Persistência | Entity Framework Core + PostgreSQL (provider Npgsql), migrations versionadas |
| Frontend | Blazor Web App com render mode Interactive Auto |
| Componentes de UI | Radzen Blazor (licença MIT), incluindo Scheduler para a agenda |
| Contratos | Projeto `Contracts` compartilhado entre `Api` e `Web` (DTOs de request/response) |
| Testes de unidade/integração | xUnit, FluentAssertions, Testcontainers (PostgreSQL real) |
| Testes de componentes | bUnit |
| Testes ponta a ponta | Playwright |

1. Novas dependências DEVEM ter licença compatível com uso comercial e manutenção ativa, e DEVEM ser justificadas no `design.md` da change que as introduz.
2. A troca de qualquer item desta tabela exige emenda (Artigo X).

## Artigo IV — Arquitetura

1. A solução DEVE ser organizada nos projetos:

   | Projeto | Responsabilidade | Pode depender de |
   |---------|------------------|------------------|
   | `Domain` | Entidades, objetos de valor, regras de negócio invariantes, eventos de domínio | nada |
   | `Application` | Casos de uso (handlers), interfaces de repositório e serviços externos, validações de entrada | `Domain` |
   | `Infrastructure` | EF Core, repositórios, armazenamento de arquivos, integrações externas | `Application`, `Domain` |
   | `Contracts` | DTOs de request/response da API | nada |
   | `Api` | Endpoints HTTP, autenticação, composição de DI | `Application`, `Infrastructure`, `Contracts` |
   | `Web` / `Web.Client` | Interface Blazor | `Contracts` |

2. **Regra de dependência.** As dependências DEVEM apontar para o `Domain`. `Domain` NÃO DEVE referenciar EF Core, ASP.NET Core ou qualquer biblioteca de infraestrutura.
3. **Frontend desacoplado.** `Web` e `Web.Client` NÃO DEVEM referenciar `Application`, `Infrastructure` ou `Domain`; a comunicação com o backend DEVE ocorrer exclusivamente por HTTP, usando clientes tipados. Componentes de `Web.Client` NÃO DEVEM depender de APIs exclusivas do servidor.
4. **Monólito modular.** O sistema é um único deploy, dividido nos módulos: **Identidade e Acesso**, **Pacientes**, **Profissionais**, **Agenda**, **Materiais** e **Exames**.
5. Um módulo NÃO DEVE acessar entidades, repositórios ou tabelas de outro módulo; a interação entre módulos DEVE ocorrer por contratos públicos da camada `Application` ou por eventos de domínio.
6. Dentro de `Application`, o código DEVE ser organizado por módulo e por funcionalidade (feature folders).
7. Componentes de terceiros (ex.: Scheduler do Radzen) DEVEM ser encapsulados em componentes próprios do projeto, permitindo sua substituição sem afetar as telas.

## Artigo V — Perfis de acesso

1. O sistema DEVE suportar os perfis:

   | Perfil | Responsabilidades |
   |--------|-------------------|
   | **Dentista** | Consultar a própria agenda; marcar, desmarcar e remarcar as próprias consultas; solicitar material; visualizar exames dos pacientes que atende; manter o próprio perfil. |
   | **Recepcionista/Secretária** | Gerenciar a agenda de todos os dentistas; cadastrar e atualizar pacientes; anexar arquivos de exames. |
   | **Administrador da clínica** | Gerenciar usuários, perfis e dentistas; aprovar ou recusar pedidos de material; configurar a clínica (horários, salas, procedimentos). |

2. **Menor privilégio.** Cada perfil DEVE ter acesso apenas ao necessário para suas responsabilidades. Toda permissão DEVE ser verificada no backend; ocultar elementos na interface não é controle de acesso.
3. Um usuário PODE acumular perfis; as permissões resultantes são a união das permissões de cada perfil.
4. Novos perfis exigem emenda.

## Artigo VI — Segurança e privacidade de dados de saúde

1. **Autorização por recurso.** Além do perfil, o backend DEVE verificar a relação do usuário com o recurso (ex.: um dentista só acessa exames de pacientes com os quais tem ou teve consulta).
2. **Auditoria.** Todo acesso de leitura ou escrita a dados clínicos e arquivos de exames DEVE gerar registro de auditoria imutável contendo usuário, ação, recurso e data/hora.
3. **Criptografia.** Toda comunicação DEVE usar TLS. Arquivos de exames DEVEM ser armazenados criptografados em repouso e servidos apenas por endpoints autenticados, nunca por URL pública.
4. **Segredos.** Senhas, chaves e strings de conexão NÃO DEVEM ser versionados; DEVEM vir de variáveis de ambiente, User Secrets (desenvolvimento) ou cofre de segredos (produção).
5. **Autenticação.** Senhas DEVEM ser armazenadas com hash adaptativo (via ASP.NET Core Identity); sessões DEVEM expirar por inatividade.
6. **LGPD.** O sistema DEVE:
   - coletar apenas os dados necessários à finalidade do atendimento;
   - permitir exportar e corrigir os dados de um paciente mediante solicitação;
   - respeitar os prazos legais de guarda de prontuário antes de qualquer eliminação ou anonimização.
7. Dados pessoais NÃO DEVEM aparecer em logs técnicos.

## Artigo VII — Convenções de código, testes e Git

1. **Código C#.** Seguir as convenções oficiais do .NET; `.editorconfig` versionado na raiz; `Nullable` habilitado; `TreatWarningsAsErrors` habilitado; analisadores .NET ativos.
2. **Nomenclatura.** Nomes do domínio em português (ex.: `Consulta`, `Paciente`, `PedidoDeMaterial`); termos técnicos consagrados PODEM permanecer em inglês (ex.: `Repository`, `Handler`, `Dto`).
3. **Testes.** Nome no padrão `Metodo_Cenario_ResultadoEsperado`; estrutura Arrange/Act/Assert; testes de integração usam PostgreSQL real via Testcontainers, não bancos em memória.
4. **Commits.** Conventional Commits em pt-BR (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`). Commits NÃO DEVEM conter assinatura nem linhas de coautoria.
5. **Branches e revisão.** Todo trabalho ocorre em branch própria e entra na principal por pull request com a build e os testes passando.
6. **Integração contínua.** A pipeline DEVE executar build, análise estática e todos os testes a cada pull request.

## Artigo VIII — Fluxo SDD com OpenSpec

1. Toda funcionalidade ou mudança de comportamento DEVE nascer de uma change OpenSpec com `proposal.md`, specs delta, `design.md` (quando aplicável) e `tasks.md`.
2. As specs em `openspec/specs/` são a fonte da verdade do comportamento do sistema; o código DEVE refletir as specs, e não o contrário.
3. O `design.md` de cada change DEVE citar os artigos desta constituição que afeta ou nos quais se apoia.
4. Cada grupo de tarefas DEVE entregar seus próprios testes; testes NÃO DEVEM ser adiados para um grupo final.
5. Uma change só PODE ser arquivada quando todas as tarefas estiverem concluídas e os testes passando.
6. Mudanças puramente técnicas (refatoração, ferramentas, documentação) PODEM declarar `skip_specs: true`.

## Artigo IX — Roadmap

Os módulos DEVEM ser entregues como changes separadas, nesta ordem:

1. `add-solution-skeleton` — esqueleto da solução .NET (API + Blazor), CI e infraestrutura de testes.
2. `add-identity-access` — autenticação e autorização pelos perfis do Artigo V.
3. `add-patient-registry` — cadastro de pacientes.
4. `add-dentist-profile` — perfil do dentista.
5. `add-appointment-scheduling` — agenda: marcar, desmarcar e remarcar consultas.
6. `add-material-requests` — pedidos de material com aprovação do administrador.
7. `add-exam-files` — upload e visualização de arquivos de exames.

A ordem PODE ser alterada quando a dependência entre módulos permitir, desde que registrada na proposal da change afetada.

## Artigo X — Emendas

1. Toda alteração desta constituição DEVE ser feita por uma change OpenSpec dedicada, nomeada `amend-constitution-<tema>`, com justificativa e impacto descritos na proposal.
2. A emenda DEVE ser aprovada pelo mantenedor do repositório por meio do merge do pull request correspondente.
3. A versão segue versionamento semântico:
   - **MAJOR**: remoção ou redefinição incompatível de um princípio ou artigo;
   - **MINOR**: novo artigo, princípio ou item de stack;
   - **PATCH**: esclarecimentos e correções de redação sem mudança de sentido.
4. A tabela de versão no topo deste documento DEVE ser atualizada a cada emenda.
