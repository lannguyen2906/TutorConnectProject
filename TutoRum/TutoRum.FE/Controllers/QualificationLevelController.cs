using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TutoRum.FE.Common;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.FE.Controllers
{

    public class QualificationLevelController : ApiControllerBase
    {
        private readonly IQualificationLevelService _qualificationLevelService;

        public QualificationLevelController(IQualificationLevelService qualificationLevelService)
        {
            _qualificationLevelService = qualificationLevelService;
        }
   
        [HttpGet]
        [Route(Common.Url.User.QualificationLevel.GetAllQualificationLevels)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<QualificationLevelDto>>), 200)]
        public async Task<IActionResult> GetAllQualificationLevels()
        {
            try
            {
                var qualificationLevels = await _qualificationLevelService.GetAllQualificationLevelsAsync();
                var response = ApiResponseFactory.Success(qualificationLevels);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }
    }
}
  
