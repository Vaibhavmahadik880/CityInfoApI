
using CityInfo.API.Models; // Import the models namespace
using Microsoft.AspNetCore.Http; // Import ASP.NET Core HTTP functionality
using Microsoft.AspNetCore.Mvc; // Import ASP.NET Core MVC functionality
using Microsoft.AspNetCore.JsonPatch; // Import JSON Patch functionality
using Serilog; // Import Serilog for logging
using CityInfo.API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning; // Import the services namespace

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")] // Define the route for           
    [Authorize(Policy = "MustBeFromMumabai")]
    [ApiController]
    [ApiVersion(2)]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ??
                throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(
            int cityId)
        {

            //var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            //if (!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
            //{
            //    return Forbid();
            //}

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointsOfInterestForCity = await _cityInfoRepository
                .GetPointsOfInterestForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(
            int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
           int cityId,
           PointOfInterestForCreationDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(
                cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn =
                _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                 new
                 {
                     cityId = cityId,
                     pointOfInterestId = createdPointOfInterestToReturn.Id
                 },
                 createdPointOfInterestToReturn);
        }

        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(
                pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(
            int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send(
                "Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }

    }
}
//Indicate that this is an API controller
//                public class PointsOfInterestController : ControllerBase
//{
//    // In the JSON, Information and Warning log levels are specified
//    // Trace = 0, Debug = 1, Information = 2, Warning = 3, Error = 4, Critical = 5, and None = 6.

//    private readonly ILogger<PointsOfInterestController> _logger; // Logger instance for logging information
//    private readonly IMailService _mailService; // Mail service for sending emails
//    private readonly CitiesDataStore _citiesDataStore; // Data store for city information
//    private readonly IEmailNotificationService _emailNotificationService;
//    private readonly ICityInfoRepository _cityInfoRepository;
//    private readonly IMapper _mapper;


//    // Dependency injection to initialize the controller with required services
//    //public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, IEmailNotificationService emailNotificationService, CitiesDataStore citiesDataStore)
//    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, IEmailNotificationService emailNotificationService, ICityInfoRepository cityInfoRepository, IMapper mapper)
//    {
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Ensure logger is not null
//        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService)); // Ensure mail service is not null
//        _emailNotificationService = emailNotificationService ?? throw new ArgumentNullException(nameof(emailNotificationService));
//        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
//        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
//        //_citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore)); // Ensure data store is not null
//    }

//    // Get all points of interest for a city
//    //[HttpGet]
//    //public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
//    //{
//    //    // Throw an exception to test global exception handling
//    //    // throw new Exception("Exception sample");

//    //    try
//    //    {
//    //        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId); // Find city by ID
//    //        if (city == null)
//    //        {
//    //            _logger.LogInformation($"City with id {cityId} wasn't found when accessing"); // Log information if city not found
//    //            return NotFound(); // Return 404 Not Found
//    //        }
//    //        if (city.PointsOfInterest == null || !city.PointsOfInterest.Any())
//    //        {
//    //            return NoContent(); // Return 204 No Content if there's no data
//    //        }
//    //        return Ok(city.PointsOfInterest); // Return 200 OK with points of interest
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex); // Log critical exception
//    //        return StatusCode(500, "A problem happened while handling request"); // Return 500 Internal Server Error
//    //    }
//    //}
//    [HttpGet]
//    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
//    {
//        if (!await _cityInfoRepository.CityExistsAsync(cityId))
//        {
//            _logger.LogInformation($"city with id {cityId} wasn't found when accessing points of interest ");
//            return NotFound();
//        }
//        var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

//        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));

//    }
//    // Get a specific point of interest for a city
//    [HttpGet("{pointofinterestId}", Name = "GetPointOfInterest")]
//    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(
//        int cityId, int pointOfInterestId)
//    {
//        if (!await _cityInfoRepository.CityExistsAsync(cityId))
//        {
//            return NotFound();
//        }

//        var pointOfInterest = await _cityInfoRepository
//            .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

//        if (pointOfInterest == null)
//        {
//            return NotFound();
//        }

//        return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
//    }
//    //    return Ok(pointOfInterest);
//    //    // Return 200 OK with the point of interest
//    //}



//    [HttpPost]       //create operation
//    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForCreationDto pointOfInterest)
//    {
//        if (!await _cityInfoRepository.CityExistsAsync(cityId))
//        {
//            return NotFound();
//        }


//        var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

//        await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

//        await _cityInfoRepository.SaveChangesAsync();

//        var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

//        // Return a CreatedAtRoute response with the location of the new resource // return the response with a 201 Created status 
//        return CreatedAtRoute("GetPointOfinterest",
//            new
//            {
//                cityId = cityId,
//                pointOfInterestId = finalPointOfInterest.Id
//            },
//          createdPointOfInterestToReturn);
//    }
//    // Add a new point of interest for a city
//    //[HttpPost]
//    //public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
//    //{
//    //    if (!ModelState.IsValid) // Check if the model state is valid
//    //    {
//    //        return BadRequest(); // Return 400 Bad Request if model state is invalid
//    //    }

//    //    // Find the city by ID
//    //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
//    //    if (city == null)
//    //    {
//    //        return NotFound(); // Return 404 Not Found if city not found
//    //    }

//    //    // Generate a new ID for the point of interest
//    //    var maxPointOfInterestId = _citiesDataStore.Cities
//    //        .SelectMany(c => c.PointsOfInterest)
//    //        .Max(p => p.Id);

//    //    // Create the new point of interest
//    //    var finalPointOfInterest = new PointOfInterestDto()
//    //    {
//    //        Id = ++maxPointOfInterestId,
//    //        Name = pointOfInterest.Name,
//    //        Description = pointOfInterest.Description,
//    //    };

//    //    // Add the new point of interest to the city
//    //    city.PointsOfInterest.Add(finalPointOfInterest);

//    //    // Return a CreatedAtRoute response with the location of the new resource
//    //    return CreatedAtRoute("GetPointOfInterest",
//    //        new
//    //        { cityId = cityId, pointOfInterestId = finalPointOfInterest.Id },
//    //        finalPointOfInterest);
//    //}

//    // Update an existing point of interest for a city
//    //[HttpPut("{pointOfInterestId}")]
//    //public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
//    //{
//    //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId); // Find city by ID
//    //    if (city == null)
//    //    {
//    //        return NotFound(); // Return 404 Not Found if city not found
//    //    }

//    //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId); // Find point of interest by ID
//    //    if (pointOfInterestFromStore == null)
//    //    {
//    //        return NotFound(); // Return 404 Not Found if point of interest not found
//    //    }

//    [HttpPut("{pointOfInterestId}")]
//    public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
//    {
//        if (!await _cityInfoRepository.CityExistsAsync(cityId))
//        {
//            return NotFound();
//        }

//        var pointOfInterestFromEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);// Find point of interest by ID
//        if (pointOfInterestFromEntity == null)
//        {
//            return NotFound(); // Return 404 Not Found if point of interest not found
//        }
//        _mapper.Map(pointOfInterest, pointOfInterestFromEntity);
//        // Update the point of interest
//        await _cityInfoRepository.SaveChangesAsync();

//        return NoContent(); // Return 204 No Content
//    }

//    // Partially update an existing point of interest for a city
//    [HttpPatch("{pointOfInterestId}")]
//    public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
//    {
//        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId); // Find city by ID
//        if (city == null)
//        {
//            return NotFound(); // Return 404 Not Found if city not found
//        }

//        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId); // Find point of interest by ID
//        if (pointOfInterestFromStore == null)
//        {
//            return NotFound(); // Return 404 Not Found if point of interest not found
//        }

//        // Create a DTO for patching
//        var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
//        {
//            Name = pointOfInterestFromStore.Name,
//            Description = pointOfInterestFromStore.Description,
//        };

//        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState); // Apply the patch document to the DTO

//        if (!ModelState.IsValid)
//        {
//            return BadRequest(ModelState); // Return 400 Bad Request if the model state is invalid
//        }

//        // Update the point of interest with patched values
//        pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
//        pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

//        return NoContent(); // Return 204 No Content
//    }

//    // Delete an existing point of interest for a city
//    [HttpDelete("{pointOfInterestId}")]
//    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
//    {
//        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId); // Find city by ID
//        if (city == null)
//        {
//            return NotFound(); // Return 404 Not Found if city not found
//        }

//        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId); // Find point of interest by ID
//        if (pointOfInterestFromStore == null)
//        {
//            return NotFound(); // Return 404 Not Found if point of interest not found
//        }

//        // Remove the point of interest from the city
//        city.PointsOfInterest.Remove(pointOfInterestFromStore);

//        // Send a notification email about the deletion
//        _mailService.Send("Point of interest deleted", $"Point of interest '{pointOfInterestFromStore.Name}' with ID {pointOfInterestFromStore.Id} was deleted");
//        _emailNotificationService.Notify("Point of interest deleted", $"Point of interest {pointOfInterestFromStore.Name} with ID {pointOfInterestFromStore.Id} was deleted");
//        return NoContent(); // Return 204 No Content
//    }
//}