using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CityInfo.API.Controllers
{
    [ApiController]      //strictly necessary
    [Authorize]
   [Route("api/v{version:apiVersion}/cities")]
   [ApiVersion(1)]
    [ApiVersion(2)]

    public class CitiesController : ControllerBase
    {

        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;
        //public CitiesController(CitiesDataStore citiesDataStore)
        //{
        //    _citiesDataStore = citiesDataStore?? throw new ArgumentNullException(nameof(citiesDataStore));
        //}

        public CitiesController(ICityInfoRepository cityInfoRepository , IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        //[HttpGet]
        //public ActionResult<IEnumerable<CityDto>> GetCities()
        //{
        //     //var temp =new JsonResult(CitiesDataStore.current.Cities);
        //     //temp.StatusCode = 200;
        //    return Ok(_citiesDataStore.Cities);
        //}

        [HttpGet]
        public  async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(string? name, string? searchQuery ,int pageNumber=1, int pageSize=5)
        {
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }

            var (cityEntities, paginationMetadata) = await _cityInfoRepository
                .GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            //var results = new List<CityWithoutPointsOfInterestDto>();
            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto
            //    {
            //        Id = cityEntity.Id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name,
            //    });
            //}

            Response.Headers.Append("X-Pagination",
             JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }

        //[HttpGet("{id}")]
        ////ActionResult<T> allows returning different types of responses(e.g., data, errors, status codes)
        //public ActionResult<CityDto> GetCity(int id) {
        //    //find city
        //    //var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);

        //    //if (cityToReturn == null)                
        //    //{
        //    //    return NotFound();
        //    //}
        //    // return Ok(cityToReturn);   //ok method is provided by controllerBase class //which represents a response with a status code of 200 (OK)
        //    return Ok();

        //}
        /// <summary>
        /// Get A city by Id
        /// </summary>
        /// 
        /// <param name="id"></param>
        /// <param name="includePointsOfInterest"></param>
        /// <returns></returns>
        [HttpGet("{cityId}")]
        //ActionResult<T> allows returning different types of responses(e.g., data, errors, status codes)
     
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCity(int cityId, bool includePointsOfInterest =false)
        {
            var city = await _cityInfoRepository.GetCityAsync(cityId,
                includePointsOfInterest);
            if (city == null)
            {
                return NotFound();
            }
            //Console.WriteLine(includePointsOfInterest ? city.PointOfInterests.Count : "No PointsOfInterest");
            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));   
        }
    }
}
