# 🚀 Custom API Gateway & Control Center

Este projeto é um **API Gateway de alta performance** desenvolvido em **.NET 8** com **YARP (Yet Another Reverse Proxy)**. Ele atua como ponto único de entrada para múltiplos microsserviços, oferecendo **Rate Limiting distribuído (Redis)**, **telemetria em tempo real** e um **Dashboard administrativo (Blazor Server)** para monitoramento ativo do tráfego.

<img width="1903" height="946" alt="image" src="https://github.com/user-attachments/assets/923c5595-aa13-4a44-b32a-b33f073b65a2" />

---

## 🏗️ Estrutura do Projeto

O ecossistema é dividido em quatro partes principais:

*   **🛡️ Gateway.API**: Porta de entrada que realiza o roteamento dinâmico via **YARP** (encaminhando rotas como `/api/produtos` para microsserviços internos ou atuando como fallback para serviços externos como o `VotoTrack` no Render).
*   **📊 Gateway.Dashboard**: Interface interativa em **Blazor Server** (com design moderno em Glassmorphism) que exibe métricas em tempo real (total de requisições, latência média, taxa de sucesso e bloqueios de segurança) e possibilita filtrar logs detalhados.
*   **📦 Gateway.Shared**: Camada comum que compartilha entidades (`LogTelemetria`) e o contexto do Entity Framework Core (`GatewayDbContext`), otimizado com índices de banco de dados por rota e data de requisição.
*   **🧪 Microservico.TesteAPI**: API de exemplo usada para validar as regras de proxy, roteamento e cabeçalhos de segurança.

---

## ⚡ Recursos Principais

### 🔐 Rate Limiting Inteligente (Redis)
*   **Identificação por Chave**: Validação via cabeçalho `X-Chave-API`. Em rotas públicas, o gateway faz a limitação preventivamente baseada no **IP do cliente**.
*   **Janela Deslizante**: Bloqueios baseados em limite configurável de requisições por minuto através de incrementos atômicos no Redis.
*   **Resiliência (Fail-Open)**: Se o Redis ficar offline, o gateway ignora a limitação e injeta o cabeçalho `X-Limite-Status: Inativo` para não interromper os serviços.

### 📈 Telemetria e Logs Ativos
*   **Medição de Latência**: Middleware de alta precisão que calcula o tempo de resposta de cada requisição.
*   **Detecção de Projetos**: Associa dinamicamente as requisições ao projeto correspondente com base no padrão da rota acessada.
*   **Armazenamento**: Gravação assíncrona de metadados diretamente no **Supabase PostgreSQL**.

---

## 🛠️ Tecnologias Utilizadas

*   **Core**: .NET 8 (Web API & Blazor Interactive Server)
*   **Proxy Reverso**: YARP (Yet Another Reverse Proxy)
*   **Cache / Rate Limit**: Redis (StackExchange.Redis)
*   **Banco de Dados**: PostgreSQL (Hospedado no Supabase) com EF Core
*   **Design UI**: Blazor, Bootstrap 5 + CSS Customizado (Glassmorphism)
