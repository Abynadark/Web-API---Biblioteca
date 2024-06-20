using Microsoft.EntityFrameworkCore;
using Biblioteca.Models;
using Biblioteca.Repositories;
using Biblioteca.Interfaces;
using Biblioteca.Profiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");// relacionado a conexão com o banco de dados
builder.Services.AddDbContext<Context>(options =>
        options.UseMySql(mySqlConnection,
            ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<ILivroRepository, LivroRepository>();

builder.Services.AddScoped<IExemplarRepository, ExemplarRepository>();


builder.Services.AddScoped<IEmprestimoRepository, EmprestimoRepository>();


builder.Services.AddScoped<IMultaRepository, MultaRepository>();

builder.Services.AddScoped<IRenovacaoRepository, RenovacaoRepository>();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddAutoMapper(typeof(EmprestimoProfile),typeof(UsuarioProfile),typeof(ExemplarProfile),typeof(RenovacaoProfile), typeof(MultaProfile), typeof(LivroProfile));

var app = builder.Build();




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();