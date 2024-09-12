using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class PointOfInterestForCreationDto
    {
        [Required(ErrorMessage = "You should provide a name value")]    //message is displayed if we enter blank value

        [MaxLength(50)]          //these are data anotation the string name shouldbe maximum 50
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]            //maximum length  should be 200
        public string? Description { get; set; }

    }
}
