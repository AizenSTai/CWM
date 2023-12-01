using Microsis.CWM.AppService;
using Microsis.CWM.Dto.MediaSettings;
using Microsoft.AspNetCore.Mvc;
using static Microsis.CWM.Util.LoggerProxy;

namespace Microsis.CWM.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MediaSettingsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMediaSettingsAppService _mediaSettingsApp;
        public MediaSettingsController(IConfiguration configuration, IMediaSettingsAppService mediaSettingsApp)
        {
            _configuration = configuration;
            _mediaSettingsApp = mediaSettingsApp;
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> New([FromBody] MediaSettingsNewRequest obj)
        {
            try
            {
                var res = await _mediaSettingsApp.New(obj);

                if (res.OperationResult == true)
                    return Ok(res);
                else
                    return StatusCode(406, res);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> Delete()
        {
            try
            {
                var res = await _mediaSettingsApp.Delete();
                if (res.OperationResult == true)
                    return Ok(res);
                else
                    return StatusCode(406, res);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var res = await _mediaSettingsApp.Get();
                if (res.OperationResult == true)
                    return Ok(res);
                else
                    return StatusCode(406, res);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> Modify([FromBody] MediaSettingsModifyRequest obj)
        {
            try
            {
                var res = await _mediaSettingsApp.Modify(obj);
                if (res.OperationResult == true)
                    return Ok(res);
                else
                    return StatusCode(406, res);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}