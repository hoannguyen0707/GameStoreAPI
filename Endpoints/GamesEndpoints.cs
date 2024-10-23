using GameStore.Dtos;

namespace GameStore.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndPointName = "GetGame";


    private static readonly List<GameDto> games = [
        new (1, "Street Figter II", "Fighting", 59.99M, new DateOnly(2021, 9, 30)),
        new (2, "Final Fantasy XIV", "Roleplaying", 69.99M, new DateOnly(2012, 9, 30)),
        new (3, "FIFA 2024", "Sports", 79.99M, new DateOnly(2024, 08, 27)),
        new (4, "Chesse", "Sports MM", 10.99M, new DateOnly(2016, 04, 25))
    ];

    public static RouteGroupBuilder MapGamesEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        //GET /games
        group.MapGet("/", () => games);

        //GET /games/1
        group.MapGet("/{id}", (int id) =>
        {
            GameDto? game = games.Find(game => game.Id == id);
            return game is null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetGameEndPointName);

        //POST /games
        group.MapPost("/", (CreateGameDto newGame) =>
        {
            GameDto game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            );
            games.Add(game);

            return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
        }).WithParameterValidation();

        //PUT /games/1
        group.MapPut("/{id}", (int id, UpdateGameDto updateGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);
            if (index == -1)
            {
                return Results.NotFound();
            }
            games[index] = new GameDto(
                id,
                updateGame.Name,
                updateGame.Genre,
                updateGame.Price,
                updateGame.ReleaseDate
            );

            return Results.NoContent();
        });

        //DELETE /games/1
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(games => games.Id == id);
            return Results.NoContent();
        });

        return group;
    }
}
