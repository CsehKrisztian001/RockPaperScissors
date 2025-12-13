using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors_Bll.Models
{
    [SwaggerSchema(Description = "Játékos statisztika, nyert, vesztetett és döntetlen játékok száma.")]
    public class UserStatsModel
    {
        public int Won { get; set; }
        public int Lost { get; set; }
        public int Draw { get; set; }
    }
}
