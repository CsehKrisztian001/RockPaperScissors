using Swashbuckle.AspNetCore.Annotations;

namespace RockPaperScissors_Dll.Enums
{
    [SwaggerSchema(Description = "Kő–papír–olló játék lehetséges végeredményei.")]
    public enum GameResult
    {
        Won = 0,
        Lost = 1,
        Draw = 2,
    }
}
