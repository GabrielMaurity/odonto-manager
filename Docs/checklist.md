# Checklist do Odonto Manager

O que já foi entregue e o que falta, visto do projeto como um todo. Cada item é uma **entrega verificável**, não uma tarefa técnica: as tarefas estão no [`tasks.md`](tasks.md), e o porquê de cada etapa está no [`plan.md`](plan.md).

## Resumo

| Fase | Entregues | Total | Situação |
|------|-----------|-------|----------|
| 0. Fundação | 16 | 16 | ✅ Concluída |
| 1. Base técnica | 0 | 9 | ⏳ Próxima |
| 2. MVP | 0 | 18 | ⬜ Não iniciada |
| 3. Expansão | 0 | 11 | ⬜ Não iniciada |
| **Total** | **16** | **54** | |

🎯 **Marco MVP:** ainda não alcançado. Ele é atingido quando todos os itens da Fase 2 estiverem entregues.

**Legenda**
- `- [x]` entregue, indicando a change que entregou;
- `- [ ]` pendente, indicando a change responsável;
- 🆕 nova necessidade, surgida depois do planejamento inicial, com a change ou decisão de origem;
- ~~riscado~~: removido, mantido com o motivo na seção [Histórico de itens removidos](#histórico-de-itens-removidos).

**Como atualizar**
- Toda change, no último grupo de tarefas, marca os itens que entregou e atualiza o resumo.
- Uma nova necessidade entra na fase e no módulo adequados, marcada com 🆕 e com a origem. Se não couber em nenhuma etapa existente, ela também vai para [Pendente de planejamento](#pendente-de-planejamento), até que o `plan.md` e o `tasks.md` a incorporem (change `update-plan-<tema>`).
- Itens não são apagados: um item removido é riscado e movido para o histórico, com o motivo.

---

## Fase 0 — Fundação

### Governança
- [x] Constituição com princípios, stack, arquitetura, perfis, segurança e roadmap — `add-project-constitution`
- [x] Constituição integrada ao fluxo do OpenSpec (`openspec/config.yaml`) — `add-project-constitution`
- [x] Frontend definido como Blazor com Radzen — `add-project-constitution`
- [x] DDD adotado como princípio, com módulos como bounded contexts — `amend-constitution-ddd`
- [x] Especificação técnica do backend (`specification.md`) — `amend-constitution-ddd`
- [x] Documentos do projeto reunidos em `Docs/` — `amend-constitution-docs-folder`

### Ambiente
- [x] PostgreSQL 18 via Docker Compose, com healthcheck e volume persistente — `amend-constitution-docker`
- [x] pgAdmin opcional pelo profile `tools` — `amend-constitution-docker`
- [x] Credenciais em `.env` fora do Git, com `.env.example` versionado — `amend-constitution-docker`

### Convenções
- [x] Padrão de commits semânticos documentado — `update-specification-commits`
- [x] Hook `commit-msg` que valida as mensagens, com testes — `update-specification-commits`
- [x] 🆕 Branch `main` como principal, só alterada por pull request com squash merge — `update-specification-branches`
- [x] 🆕 Uma branch por implementação ou feature, no padrão `<tipo>/<nome>` — `update-specification-branches`
- [x] 🆕 Hook `pre-push` que bloqueia push na `main` e valida nomes de branch, com testes — `update-specification-branches`

### Planejamento
- [x] Plano de implementação com fases, MVP e fichas das etapas — `add-implementation-plan`
- [x] Tarefas macro por parte e etapa, e este checklist — `add-implementation-plan`

## Fase 1 — Base técnica

### Solução e qualidade
- [ ] A solução .NET 10 compila sem warnings, com a estrutura da specification 9 — `add-solution-skeleton`
- [ ] Shared Kernel, Result pattern e messaging prontos para os módulos — `add-solution-skeleton`
- [ ] Testes de arquitetura impedem violações de camadas e de módulos — `add-solution-skeleton`
- [ ] Infraestrutura de testes (integração com banco real, componentes e ponta a ponta) funcionando — `add-solution-skeleton`

### API e Web
- [ ] API responde com ProblemDetails, documentação OpenAPI e health checks — `add-solution-skeleton`
- [ ] Web Blazor abre com o layout base e conversa com a API — `add-solution-skeleton`
- [ ] Logs e traces via OpenTelemetry, sem dados pessoais — `add-solution-skeleton`

### Entrega contínua
- [ ] `docker compose up` sobe banco, API e Web — `add-solution-skeleton`
- [ ] CI roda build, análise, testes e validação de commits em todo pull request — `add-solution-skeleton`

## Fase 2 — MVP

### Identidade e Acesso
- [ ] Usuários entram e saem do sistema com segurança, e a sessão expira por inatividade — `add-identity-access`
- [ ] Cada perfil (Dentista, Recepcionista, Administrador) acessa só o que lhe compete — `add-identity-access`
- [ ] Administrador gerencia usuários e perfis — `add-identity-access`
- [ ] Login protegido contra tentativas repetidas — `add-identity-access`

### Pacientes
- [ ] Recepção cadastra, edita, busca e lista pacientes — `add-patient-registry`
- [ ] CPF validado e sem cadastros duplicados — `add-patient-registry`
- [ ] Paciente pode ter seus dados exportados e corrigidos a pedido (LGPD) — `add-patient-registry`

### Profissionais
- [ ] Administrador cadastra dentistas com CRO e especialidades — `add-dentist-profile`
- [ ] Dentista mantém o próprio perfil e sua disponibilidade — `add-dentist-profile`
- [ ] Dentista tem um painel próprio ("Meu painel") — `add-dentist-profile`

### Agenda
- [ ] Recepção marca consultas para qualquer dentista — `add-appointment-scheduling`
- [ ] Recepção desmarca e remarca consultas — `add-appointment-scheduling`
- [ ] Dentista vê as próprias consultas no painel — `add-appointment-scheduling`
- [ ] Dentista marca e desmarca as próprias consultas — `add-appointment-scheduling`
- [ ] Não há duas consultas no mesmo horário para um dentista, nem com acessos simultâneos — `add-appointment-scheduling`
- [ ] Consultas respeitam a disponibilidade do dentista — `add-appointment-scheduling`
- [ ] Horários exibidos no fuso da clínica — `add-appointment-scheduling`
- [ ] Fluxos de agenda cobertos por testes ponta a ponta — `add-appointment-scheduling`

## Fase 3 — Expansão

### Materiais
- [ ] Dentista faz pedidos de material pelo painel — `add-material-requests`
- [ ] Dentista acompanha o status dos próprios pedidos — `add-material-requests`
- [ ] Administrador aprova, recusa e registra a entrega de pedidos — `add-material-requests`
- [ ] Fluxo de pedido coberto por teste ponta a ponta — `add-material-requests`

### Exames
- [ ] Recepção anexa arquivos de exames ao paciente — `add-exam-files`
- [ ] Dentista vê no painel os exames dos pacientes que atende — `add-exam-files`
- [ ] Dentista não acessa exames de pacientes que não atende — `add-exam-files`
- [ ] Arquivos de exames ficam criptografados e nunca têm URL pública — `add-exam-files`
- [ ] Todo acesso a dados clínicos fica registrado em auditoria — `add-exam-files`
- [ ] Upload limitado em tamanho e tipo de arquivo — `add-exam-files`
- [ ] Fluxo de exames coberto por teste ponta a ponta — `add-exam-files`

---

## Pendente de planejamento

Necessidades identificadas que ainda não pertencem a nenhuma etapa do `plan.md`. Cada item indica a origem e sai daqui quando uma change `update-plan-<tema>` o incorporar.

_Nenhum item no momento._

## Histórico de itens removidos

Itens retirados do escopo, mantidos para registrar a decisão. Formato: ~~item~~ — motivo — change ou decisão que removeu.

_Nenhum item no momento._
