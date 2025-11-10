# Task Management API

API RESTful para gerenciamento de tarefas e projetos, desenvolvida com **ASP.NET Core 8.0**, **Entity Framework Core** e **PostgreSQL**.

---

## 🚀 Execução Rápida com Docker

### Opção 1: Script Automático (Linux/Mac)
```bash
./start.sh
```

### Opção 2: Docker Compose Manual
```bash
docker-compose up -d
```

**Pronto!** A aplicação está rodando:
- 🌐 **API**: http://localhost:5000
- 📚 **Swagger**: http://localhost:5000/swagger
- 🏥 **Health**: http://localhost:5000/health
- 🗄️ **pgAdmin**: http://localhost:5050 (admin@admin.com / admin)

---

## 📋 Pré-requisitos

- [Docker](https://docs.docker.com/get-docker/) 20.10+
- [Docker Compose](https://docs.docker.com/compose/install/) 2.0+

**OU**

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/)

---

## 🏗️ Arquitetura

Projeto desenvolvido seguindo **Clean Architecture**:

```
src/
├── TaskManagement.API/            # Controllers e configuração
├── TaskManagement.Application/    # Services e lógica de negócio
├── TaskManagement.Domain/         # Entidades e regras de domínio
└── TaskManagement.Infrastructure/ # EF Core e repositórios
```

---

## ✨ Funcionalidades

### ✅ Requisitos Funcionais
1. **Listagem de Projetos** - Listar projetos do usuário
2. **Visualização de Tarefas** - Ver tarefas de um projeto
3. **Criação de Projetos** - Criar novo projeto
4. **Criação de Tarefas** - Adicionar tarefa a um projeto
5. **Atualização de Tarefas** - Atualizar status e detalhes
6. **Remoção de Tarefas** - Remover tarefa do projeto

### ✅ Regras de Negócio
1. **Prioridade Imutável** - Não pode alterar prioridade após criação
2. **Restrição de Remoção** - Projeto com tarefas pendentes não pode ser removido
3. **Histórico Automático** - Todas as alterações são registradas
4. **Limite de Tarefas** - Máximo de 20 tarefas por projeto
5. **Relatórios Restritos** - Apenas gerentes podem acessar relatórios
6. **Comentários Rastreados** - Comentários são salvos no histórico

---

## 📡 Endpoints da API

### Projetos
```
GET    /api/projects/user/{userId}     - Listar projetos do usuário
GET    /api/projects/{id}              - Obter projeto específico
POST   /api/projects                   - Criar novo projeto
DELETE /api/projects/{id}              - Remover projeto
```

### Tarefas
```
GET    /api/tasks/project/{projectId}  - Listar tarefas do projeto
GET    /api/tasks/{id}                 - Obter tarefa específica
POST   /api/tasks                      - Criar nova tarefa
PUT    /api/tasks/{id}                 - Atualizar tarefa
DELETE /api/tasks/{id}                 - Remover tarefa
GET    /api/tasks/{id}/history         - Ver histórico da tarefa
```

### Comentários
```
POST   /api/comments                   - Adicionar comentário
GET    /api/comments/task/{taskId}     - Listar comentários da tarefa
```

### Relatórios
```
GET    /api/reports/performance        - Relatório de performance (Manager only)
```

---

## 🗄️ Banco de Dados

### Estrutura
- **Users** - Usuários do sistema
- **Projects** - Projetos criados pelos usuários
- **Tasks** - Tarefas dentro dos projetos
- **TaskHistories** - Histórico de alterações das tarefas
- **TaskComments** - Comentários nas tarefas

### Dados Iniciais (Seed)
```
👤 Usuários:
   • joao.silva@example.com (User)
   • maria.santos@example.com (Manager)
   • carlos.oliveira@example.com (User)

📁 3 Projetos + 3 Tarefas + Comentários e Histórico
```

---

## 🐳 Docker

### Subir aplicação
```bash
docker-compose up -d
```

### Ver logs
```bash
docker-compose logs -f api
```

### Parar aplicação
```bash
docker-compose down
```

### Rebuild após mudanças
```bash
docker-compose up -d --build
```

Ver [DOCKER.md](DOCKER.md) para documentação completa.

---

## 💻 Execução sem Docker

### 1. Configurar Banco
```bash
# Criar banco PostgreSQL
createdb taskmanagement
```

### 2. Configurar Connection String
Edite `src/TaskManagement.API/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=taskmanagement;Username=postgres;Password=postgres"
  }
}
```

### 3. Executar
```bash
cd src/TaskManagement.API
dotnet run
```

---

## 🧪 Testando a API

### Via Swagger UI (Recomendado)
Acesse: http://localhost:5000/swagger

### Via cURL
```bash
# Health check
curl http://localhost:5000/health

# Listar projetos de um usuário
curl http://localhost:5000/api/projects/user/{userId}

# Criar projeto
curl -X POST http://localhost:5000/api/projects \
  -H "Content-Type: application/json" \
  -d '{"name":"Novo Projeto","description":"Descrição","userId":"guid"}'
```

### Via arquivo .http
Abra `src/TaskManagement.API/TaskManagement.http` no VS Code ou Rider.

---

## 📊 Tecnologias Utilizadas

- **ASP.NET Core 8.0** - Framework web
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL 16** - Banco de dados
- **Swagger/OpenAPI** - Documentação da API
- **Docker & Docker Compose** - Containerização
- **Clean Architecture** - Padrão de arquitetura
- **Repository Pattern** - Acesso a dados
- **Unit of Work** - Gerenciamento de transações

---

## 📁 Estrutura de Diretórios

```
TaskManagementAPI/
├── src/
│   ├── TaskManagement.API/            # 🌐 REST API
│   ├── TaskManagement.Application/    # 💼 Lógica de negócio
│   ├── TaskManagement.Domain/         # 📦 Entidades e regras
│   └── TaskManagement.Infrastructure/ # 🗄️ EF Core e repositórios
├── tests/
│   └── TaskManagement.Tests/          # 🧪 Testes unitários
├── docker-compose.yml                  # 🐳 Docker Compose
├── Dockerfile                          # 🐳 Dockerfile da API
├── start.sh                            # 🚀 Script de start
├── stop.sh                             # 🛑 Script de stop
└── README.md                           # 📚 Este arquivo
```

---

## 📚 Documentação Completa

- **[DOCKER.md](DOCKER.md)** - Guia completo do Docker
- **[API-CONTROLLERS.md](API-CONTROLLERS.md)** - Documentação dos endpoints
- **[INFRASTRUCTURE.md](INFRASTRUCTURE.md)** - Arquitetura do banco
- **[SERVICES.md](SERVICES.md)** - Lógica de negócio

---

## 🤝 Como Contribuir

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto é licenciado sob a MIT License.

---

## 👨‍💻 Autor

Desenvolvido como parte de um desafio técnico.

---

## 🔗 Links Úteis

- [Documentação .NET](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [PostgreSQL](https://www.postgresql.org/docs/)
- [Docker](https://docs.docker.com/)
- [Swagger](https://swagger.io/docs/)

---

**⭐ Se este projeto foi útil, considere dar uma estrela!**
