namespace CityInfo.API.Models
{
    /// <summary>
    /// Get ALL city 
    /// </summary>
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public string? Description { get; set; }

        //public int NumberOfPointsOfInterest
        //{
        //    get
        //    {
        //        return PointsOfInterest.Count;
        //    }
        //}

        public ICollection<PointOfInterestDto>PointsOfInterest { get; set; } = new List<PointOfInterestDto>();

        //public int NumberOfPointsOfInterest { get; set; }
    }
}
