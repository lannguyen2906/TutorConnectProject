using Microsoft.AspNetCore.Mvc;
using TutoRum.FE.Common;
namespace TutoRum.FE.Controllers
{
    [ProducesResponseTypeFilter]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    { }

}
