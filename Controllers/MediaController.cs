using Microsis.CWM.AppService;
using Microsis.CWM.Dto.Media;
using Microsoft.AspNetCore.Mvc;

namespace Microsis.CWM.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMediaAppService _mediaApp;
        public MediaController(IConfiguration configuration, IMediaAppService mediaApp)
        {
            _configuration = configuration;
            _mediaApp = mediaApp;
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> New([FromBody] MediaNewRequest obj)
        {
            try
            {
                var res = await _mediaApp.New(obj);
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
                var res = await _mediaApp.Get();
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
        //[HttpPost]
        //[Route("[Action]")]
        //public async Task<IActionResult> Delete([FromBody] MediaDeleteRequest obj)
        //{
        //    try
        //    {
        //        var res = await _mediaApp.Delete(obj);
        //        if (res.OperationResult == true)
        //            return Ok(res);
        //        else
        //            return StatusCode(406, res);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500);
        //    }
        //}
        //[HttpPost]
        //[Route("[Action]")]
        //public async Task<IActionResult> Modify([FromBody] MediaModifyRequest obj)
        //{
        //    try
        //    {
        //        var res = await _mediaApp.Modify(obj);
        //        if (res.OperationResult == true)
        //            return Ok(res);
        //        else
        //            return StatusCode(406, res);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500);
        //    }
        //}
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> ModifyActivate([FromBody] MediaModifyActivateRequest obj)
        {
            try
            {
                var res = await _mediaApp.ModifyActivate(obj);
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
        public async Task<IActionResult> ModifyDeActivate([FromBody] MediaModifyDeActivateRequest obj)
        {
            try
            {
                var res = await _mediaApp.ModifyDeActivate(obj);
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
        public async Task<IActionResult> DeActivatedList()
        {
            try
            {
                var res = await _mediaApp.DeactivatedList();
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
        public async Task<IActionResult> ActivatedList()
        {
            try
            {
                var res = await _mediaApp.ActivatedList();
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