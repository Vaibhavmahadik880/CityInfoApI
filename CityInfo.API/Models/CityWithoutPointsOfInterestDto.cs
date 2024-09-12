namespace CityInfo.API.Models
{
    /// <summary>
    /// Get Only a City without PoI
    /// </summary>
    public class CityWithoutPointsOfInterestDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; }
        
    }
}
