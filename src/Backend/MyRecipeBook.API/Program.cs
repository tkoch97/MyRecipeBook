using MyRecipeBook.API.Filters;
using MyRecipeBook.API.Middleware;
using MyRecipeBook.Application;
using MyRecipeBook.Infrastructure;
using MyRecipeBook.Infrastructure.Extensions;
using MyRecipeBook.Infrastructure.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Configuration.GetConnectionString("");
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

MigrateDataBase();

app.Run();

void MigrateDataBase()
{
    if (builder.Configuration.IsInMemoryTestEnvironment()) 
    {
        return;
    }
    var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    var connectionString = builder.Configuration.ConnectionString();

    DatabaseMigration.Migrate(connectionString, serviceScope.ServiceProvider);

    // O service Scope é um escopo temporário de injeção de dependência
    // que é criado para realizar a migração do banco de dados. Pq o DbCOntext e o IMigrationRunner são scoped e
    // Precisam de um escopo para serem resolvidos corretamente.
    // O usamos sempre que precisamos usar serviços com tempo de vida scoped fora do contexto de uma requisição HTTP.

}

public partial class Program { }

