using Swashbuckle.AspNetCore.Annotations;

namespace RockPaperScissors_Bll.Enums
{
    [SwaggerSchema(Description = "Kő–papír–olló játék lehetséges választásai.")]
    public enum GameChoice
    {
        Rock,
        Paper,
        Scissors
    }
}
