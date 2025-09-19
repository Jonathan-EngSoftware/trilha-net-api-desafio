// Importações essenciais para o EF Core, Serialização, etc.
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Context;

// 1. Criação do "construtor" da nossa aplicação web.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 2. AQUI ESTÁ A MÁGICA DO ENTITY FRAMEWORK!
// Esta linha registra o nosso contexto de banco de dados (OrganizadorContext) no sistema de
// injeção de dependência. É por isso que conseguimos recebê-lo no construtor do nosso Controller.
// Ele também configura para usar o SQL Server e pega a string de conexão "ConexaoPadrao"
// do nosso arquivo `appsettings.json`.
// ...
// Obtém a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("ConexaoPadrao");

builder.Services.AddDbContext<OrganizadorContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ConexaoPadrao"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConexaoPadrao"))));

// 3. Configuração dos Controllers e uma melhoria na serialização de Enums.
// AddControllers() registra os serviços que os controllers precisam para funcionar.
// A parte do AddJsonOptions é uma ótima prática: ela faz com que, no JSON de resposta,
// o nosso EnumStatusTarefa apareça como texto ("Pendente", "Finalizado") em vez do número (0, 1),
// o que deixa a API muito mais legível.
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// 4. Configuração do Swagger/OpenAPI.
// Essas duas linhas são responsáveis por gerar aquela página de documentação interativa da API,
// onde podemos ver todos os endpoints e testá-los diretamente pelo navegador. Essencial para o desenvolvimento!
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Constrói a aplicação com todos os serviços que registramos acima.
var app = builder.Build();

// Configure the HTTP request pipeline.

// 5. Ativa o Swagger apenas em ambiente de desenvolvimento.
// É uma boa prática de segurança para não expor a estrutura da sua API em produção.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisições HTTP para HTTPS.
app.UseHttpsRedirection();

// Habilita o uso de autorização (ainda não configuramos nenhuma regra, mas o middleware está pronto).
app.UseAuthorization();

// 6. Mapeia as rotas para os controllers.
// É essa linha que faz a URL "/Tarefa/1" ser direcionada para o método ObterPorId(1) no nosso TarefaController.
app.MapControllers();

// 7. Inicia a aplicação. Agora ela está "ouvindo" as requisições HTTP.
app.Run();