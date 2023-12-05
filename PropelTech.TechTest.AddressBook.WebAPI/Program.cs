using Microsoft.AspNetCore.Http.HttpResults;
using PropelTech.TechTest.AddressBook.DataAccess;
using PropelTech.TechTest.AddressBook.Types;
using PropelTech.TechTest.AddressBook.Types.Options;

namespace PropelTech.TechTest.AddressBook.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.Configure<DataOptions>(builder.Configuration.GetSection(DataOptions.Data));
        builder.Services.AddSingleton<IRepository<AddressItem>, AddressJsonFlatFileRepository>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();            

        //Get All
        app.MapGet("/addressitems", (IRepository<AddressItem> repository) =>
        {
            return repository.GetAll().ToList();
        })
        .WithOpenApi();

        //Get By Id
        app.MapGet("/addressitems/{id}", Results<Ok<AddressItem>, NotFound<string>> (IRepository<AddressItem> repository, Guid id) =>
        {
            try
            {
                var item = repository.GetById(id);
                return TypedResults.Ok(item);
            }
            catch(ArgumentOutOfRangeException ex)
            {
                return TypedResults.NotFound(ex.Message);
            }

        })
        .WithOpenApi();

        //Insert
        app.MapPost("/addressitems/", async (IRepository<AddressItem> repository, AddressItem item) =>
        {
            await repository.Insert(item);
        })
        .WithOpenApi();

        //Update
        app.MapPut("/addressitems/", async Task<Results<Ok, NotFound<string>>> (IRepository<AddressItem> repository, AddressItem item) =>
        {
            try
            {
                await repository.Update(item);
                return TypedResults.Ok();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return TypedResults.NotFound(ex.Message);
            }
        })
        .WithOpenApi();

        //Delete
        app.MapDelete("/addressitems/", async Task<Results<Ok, NotFound<string>>> (IRepository<AddressItem> repository, Guid id) =>
        {
            try
            {
                await repository.Delete(id);
                return TypedResults.Ok();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return TypedResults.NotFound(ex.Message);
            }
        })
        .WithOpenApi();

        //Search
        app.MapGet("/addressitems/search/{searchString}", async (IRepository<AddressItem> repository, string searchString) =>
        {
            return repository.Search(searchString);
        })
        .WithOpenApi();

        app.Run();
    }
}
