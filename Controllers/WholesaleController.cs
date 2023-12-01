using Microsis.CWM.AppService;
using Microsis.CWM.Dto.Wholesale;
using Microsoft.AspNetCore.Mvc;
using static Microsis.CWM.Util.LoggerProxy;

namespace Microsis.CWM.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WholesaleController : ControllerBase
    {
        private readonly IWholesaleAppService _wholesaleApp;
        public WholesaleController(IWholesaleAppService wholesaleApp)
        {
            _wholesaleApp = wholesaleApp;
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> New([FromBody] WholesaleNewRequest obj)
        {
            try
            {
                var res = await _wholesaleApp.New(obj);
                if (res.OperationResult == true)
                    return Ok(res);
                else
                    return StatusCode(406, res);
            }
            catch (Exception ex)
            {
                return StatusCode(500 , ex.Message);
            }
        }
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> Delete([FromBody] WholesaleDeleteRequest obj)
        {
            try
            {
                var res = await _wholesaleApp.Delete(obj);
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
                var res = await _wholesaleApp.Get();
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
                var res = await _wholesaleApp.ActivatedList();
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
                var res = await _wholesaleApp.DeActivatedList();
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
        public async Task<IActionResult> Modify([FromBody] WholesaleModifyRequest obj)
        {
            try
            {
                var res = await _wholesaleApp.Modify(obj);
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
        public async Task<IActionResult> ModifyActivate([FromBody] WholesaleModifyActivateRequest obj)
        {
            try
            {
                var res = await _wholesaleApp.ModifyActivate(obj);
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
        public async Task<IActionResult> ModifyDeActivate([FromBody] WholesaleModifyDeActivateRequest obj)
        {
            try
            {
                var res = await _wholesaleApp.ModifyDeActivate(obj);
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