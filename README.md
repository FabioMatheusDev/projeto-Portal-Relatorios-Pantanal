# Portal de Relatórios (SAP B1 + AD + Angular)

Solução em camadas (.NET 10 / DDD) com API ASP.NET Core, persistência em **SAP HANA** (EF Core), autenticação **LDAP/Active Directory** + **JWT**, integração com **SAP Business One Service Layer**, e SPA **Angular 19** com **Tailwind CSS**.

## Estrutura

- `src/PortalRelatorios.API` — Controllers, Swagger, JWT, middleware global, Serilog (JSON compacto no console).
- `src/PortalRelatorios.Application` — DTOs, casos de uso (`AuthAppService`, `PermissionAppService`), contratos.
- `src/PortalRelatorios.Domain` — Entidades e interfaces de repositório.
- `src/PortalRelatorios.Infrastructure` — EF Core (HANA ou InMemory), repositórios, LDAP, JWT, cliente SAP.
- `src/PortalRelatorios.CrossCutting` — Claims e utilitários transversais.
- `src/PortalRelatorios.Web` — Frontend Angular (login, dashboard, relatórios por setor, permissões admin).

## Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) (10.x no ambiente em que o projeto foi gerado).
- Node.js LTS + npm (para o Angular).
- SAP HANA Client / drivers no servidor que hospedar a API (quando `Database:UseInMemoryDatabase` for `false`).

## Configuração da API (`appsettings` / User Secrets)

| Seção | Descrição |
| --- | --- |
| `Database:UseInMemoryDatabase` | `true` para desenvolvimento sem HANA. |
| `Database:HanaConnectionString` | Obrigatório quando InMemory está desligado. Ex.: `Server=host:port;UserID=...;Password=...;Current Schema=PORTAL` (ajuste conforme documentação SAP). |
| `Jwt:*` | `Issuer`, `Audience`, `SigningKey` (chave longa e secreta), `ExpireMinutes`. |
| `Ldap:*` | Servidor, `BaseDn`, filtros, `BindUserDn` / `BindPassword` para buscas. `UseMock: true` simula AD. |
| `Sap:*` | `BaseUrl` do Service Layer, `CompanyDB`, `UserName`, `Password`, `UseMockResponses`. |
| `ReportEndpoints:Routes` | Mapa chave → caminho relativo ao Service Layer (ex.: OData exposto a partir de Calculation Views). |
| `Cors:Origins` | Origens do Angular (ex.: `http://localhost:4200`). |

**Variável de ambiente (migrations):** `PORTAL_HANA_DESIGN_CS` pode apontar para uma connection string válida ao gerar migrations com o provedor SAP HANA (veja `AppDbContextFactory`).

## Executar a API

```bash
cd src/PortalRelatorios.API
dotnet run
```

Por padrão (perfil `http`): `http://localhost:5248`. Swagger em `/swagger` (ambiente Development).

## Executar o Angular

```bash
cd src/PortalRelatorios.Web
npm install
npx ng serve
```

Ajuste `src/environments/environment.development.ts` se a API não estiver em `http://localhost:5248/api`.

## Desenvolvimento sem AD / SAP / HANA

- `Ldap:UseMock: true` — login mock: usuário **`admin`** / senha **`admin`** (administrador; criado pelo seed). Outros nomes aceitam qualquer senha.
- `Sap:UseMockResponses: true` — evita chamadas reais ao Service Layer.
- `Database:UseInMemoryDatabase: true` — dados de permissões em memória (reinicia ao parar o processo).

## Identidade visual (login)

Substitua `src/PortalRelatorios.Web/src/assets/brand/logo.svg` pela logomarca oficial da Pantanal Agrícola. Opcionalmente, adicione `src/assets/images/login-bg.gif` (ou vídeo) e referencie no CSS do componente de login.

## Migrations (HANA)

Com `UseInMemoryDatabase: false`, aplique as migrations no SAP HANA:

```bash
dotnet ef database update --project src/PortalRelatorios.Infrastructure --startup-project src/PortalRelatorios.API
```

## Licenças de terceiros

O pacote `Sap.EntityFrameworkCore.Hana.*` está sujeito à licença SAP indicada no pacote NuGet.
