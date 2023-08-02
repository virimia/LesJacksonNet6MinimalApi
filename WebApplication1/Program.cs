using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SixMinApi.Data;
using SixMinApi.Dtos;
using SixMinApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlConnectionBuilder = new SqlConnectionStringBuilder
{
    ConnectionString = builder.Configuration.GetConnectionString("SQLDbConnection"),
    UserID = builder.Configuration["UserId"],
    Password = builder.Configuration["Password"]
};

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(sqlConnectionBuilder.ConnectionString));

builder.Services.AddScoped<ICommandRepository, CommandRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

CreateEndpoints(app);

app.Run();

static void CreateEndpoints(WebApplication app)
{
    app.MapGet("api/v1/commands", async (ICommandRepository repo, IMapper mapper) =>
    {
        var commands = await repo.GetAll();

        return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
    });

    app.MapGet("api/v1/commands/{id}", async (ICommandRepository repo, IMapper mapper, int id) =>
    {
        var command = await repo.GetById(id);

        if (command is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(mapper.Map<CommandReadDto>(command));
    });

    app.MapPost("api/v1/commands", async (ICommandRepository repo, IMapper mapper, CommandCreateDto cmdCreateDto) =>
    {
        var commandModel = mapper.Map<Command>(cmdCreateDto);

        await repo.Create(commandModel);
        await repo.SaveChanges();

        var cmdReadDto = mapper.Map<CommandReadDto>(commandModel);

        return Results.CreatedAtRoute($"api/v1/commands/{cmdReadDto.Id}", cmdReadDto);
    });

    app.MapPut("api/v1/commands", async (ICommandRepository repo, IMapper mapper, int id, CommandUpdateDto cmdUpdateDto) =>
    {
        var cmd = await repo.GetById(id);

        if (cmd is null)
        {
            return Results.NotFound();
        }

        mapper.Map(cmdUpdateDto, cmd);

        await repo.SaveChanges();

        return Results.NoContent();
    });

    app.MapDelete("api/v1/commands/{id}", async (ICommandRepository repo, IMapper mapper, int id) =>
    {
        var cmd = await repo.GetById(id);

        if (cmd is null)
        {
            return Results.NotFound();
        }

        repo.Delete(cmd);
        await repo.SaveChanges();

        return Results.NoContent();
    });
}