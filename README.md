# API Projeto Final – Gestão de Produtos

**Autores:** Hudson Peres & Tassina Nascimento  
**Curso:** Técnico/a Especialista em Tecnologias de Programação de Sistemas de Informação  

---

## Visão geral

Este projeto é uma **API REST** desenvolvida em **.NET 8** que permite gerir utilizadores e produtos. A API inclui:

- **Autenticação segura** com tokens JWT.
- **Cache híbrido** (memória local + Redis) para acelerar as respostas.
- **Resiliência** (retries e circuit breaker) para chamadas a serviços externos.
- **Mock de serviços externos** (inventário e pagamentos) usando Mountebank.
- **Frontend simples** (HTML/CSS/JS) para demonstrar o consumo da API.
- **Documentação interativa** com Swagger.
- **Orquestração completa com Docker Compose** (API + PostgreSQL + Redis + Mountebank).

---

## Tecnologias utilizadas

| Camada | Tecnologia |
|--------|------------|
| Backend | .NET 8 (C#), ASP.NET Core Web API |
| Base de dados | PostgreSQL (com Entity Framework Core) |
| Cache distribuído | Redis |
| Cache local e resiliência | Polly (Retry, Circuit Breaker) |
| Mock externo | Mountebank |
| Autenticação | JWT (JSON Web Tokens) |
| Documentação | Swagger / Swashbuckle |
| Frontend | HTML5, CSS3, JavaScript (fetch API) |
| Containerização | Docker + Docker Compose |

---

## Arquitetura do sistema

```text
[Website/Frontend]  →  [API .NET 8]  →  [PostgreSQL]
                           ↓
                        [Redis]
                           ↓
                    [Mountebank (mock)]
