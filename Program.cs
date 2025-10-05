using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MovieVotesApi.Data;
using MovieVotesApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddProblemDetails();        
builder.Services.AddExceptionHandler();     

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();


app.UseExceptionHandler(); 


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!await db.Movies.AnyAsync())
    {
        db.Movies.AddRange(
            new Movie { Title = "The Matrix" },
            new Movie { Title = "Inception" },
            new Movie { Title = "Interstellar" }
        );
        await db.SaveChangesAsync();
    }
}



// GET /movies 
app.MapGet("/movies", async (AppDbContext db) =>
{
    var movies = await db.Movies
        .OrderBy(m => m.Id)
        .Select(m => new MovieDto(m.Id, m.Title, m.Votes))
        .ToListAsync();

    return TypedResults.Ok(movies);
})
.WithName("GetMovies")
.WithSummary("Получить список фильмов с текущими голосами");

// POST /vote/{movieId:int} 
app.MapPost("/vote/{movieId:int}", async Task<Results<Ok<MovieDto>, NotFound>> (int movieId, AppDbContext db) =>
{
    var movie = await db.Movies.FindAsync(movieId);
    if (movie is null) return TypedResults.NotFound();

    movie.Votes += 1;
    await db.SaveChangesAsync();

    var updated = new MovieDto(movie.Id, movie.Title, movie.Votes);
    return TypedResults.Ok(updated);
})
.WithName("VoteForMovie")
.WithSummary("Проголосовать за фильм по идентификатору");


app.MapGet("/", () => Results.Text("MovieVotesApi is up"));

app.Run();


public record MovieDto(int Id, string Title, int Votes);
