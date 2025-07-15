# Projeto: Cliente HTTP Resiliente com .NET e Polly

##  Visão Geral

Este projeto é uma demonstração prática de como construir um cliente HTTP robusto e resiliente em um ecossistema de microsserviços usando .NET. O objetivo é aplicar padrões de resiliência e observabilidade para garantir que a comunicação entre serviços possa lidar com falhas transitórias e catastróficas de forma elegante.

A aplicação consiste em uma API principal (`ResilientClient.Api`) que consome uma segunda API (`Provider.Mock`), a qual simula diferentes cenários de falha (instabilidade, lentidão, indisponibilidade total).

## Principais Funcionalidades e Conceitos Aplicados

- **Políticas de Resiliência com Polly:**
  - **Retry:** Nova tentativa automática com backoff exponencial para lidar com falhas transitórias.
  - **Circuit Breaker:** Bloqueio automático de chamadas para serviços indisponíveis, prevenindo falhas em cascata.
  - **Timeout:** Definição de um tempo limite para cada requisição HTTP.
  - **Fallback (Opcional, se implementado):** Definição de um caminho alternativo em caso de falha total.

- **Observabilidade e Rastreabilidade:**
  - **Logging Estruturado com Serilog:** Todos os logs são gerados em formato JSON para fácil processamento.
  - **Stack ELK (Elasticsearch & Kibana):** Os logs são centralizados no Elasticsearch e podem ser visualizados e pesquisados na interface do Kibana.
  - **Correlation ID:** Todas as requisições e suas tentativas são rastreáveis através de um ID de correlação único, permitindo a visualização do fluxo completo de uma operação.

- **Containerização:**
  - **Docker & Docker Compose:** Todo o ambiente (APIs, Elasticsearch, Kibana) é orquestrado com Docker Compose, permitindo que qualquer pessoa suba o projeto com um único comando.
  - **Hot-Reload:** Configurado para desenvolvimento, permitindo que alterações no código C# sejam refletidas em tempo real nos containers sem a necessidade de um novo build.

## Tecnologias Utilizadas

- .NET 8 (ou 9)
- ASP.NET Core
- Polly (para políticas de resiliência)
- Serilog (para logging estruturado)
- Docker & Docker Compose
- Elasticsearch & Kibana

## Como Executar o Projeto

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução.
- .NET SDK 8 (ou 9) (opcional, para desenvolvimento fora do Docker).

### Passos para Execução

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/LeoMorbeck/dotnet-resilient-api-client
   cd dotnet-resilient-api-client
   ```

2. **Suba o ambiente com Docker Compose:**
   No terminal, na raiz do projeto, execute o comando:
   ```bash
   docker-compose up --build
   ```
   Aguarde alguns minutos para todos os containers (APIs, Elasticsearch e Kibana) iniciarem.

3. **Acesse os serviços:**
   - **API Principal (Swagger):** `http://localhost:8081`
   - **API de Mock (Swagger):** `http://localhost:8082/swagger`
   - **Kibana (Logs):** `http://localhost:5601`

### Testando os Cenários de Resiliência

Acesse a interface do Swagger da API Principal (`http://localhost:8081`) e execute os endpoints na seção `TestResilience` para observar os diferentes comportamentos.

- **`/test/success`**: A chamada funciona na primeira tentativa.
- **`/test/unstable`**: A chamada falha 2 vezes e tem sucesso na terceira. **Observe os logs no Kibana** filtrando pelo `CorrelationId` para ver as novas tentativas da política de Retry.
- **`/test/slow`**: A chamada falha por timeout após 10 segundos.
- **`/test/down`**: Após 5 chamadas com falha, o Circuit Breaker abrirá. A 6ª chamada falhará instantaneamente, sem nem mesmo tentar a conexão.

---
**Criado por Leonardo Graciano Morbeck Gomes** | [GitHub](https://github.com/LeoMorbeck) | [LinkedIn](https://www.linkedin.com/in/leonardo-morbeck-5a86b7206/)
