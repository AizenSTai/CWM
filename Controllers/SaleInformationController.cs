using Microsis.CWM.AppService;
using Microsis.CWM.Dto.SalesInformation;
using Microsoft.AspNetCore.Mvc;

namespace Microsis.CWM.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SaleInformationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISaleInformationAppService _saleApp;
        public SaleInformationController(IConfiguration configuration, ISaleInformationAppService saleApp)
        {

            _configuration = configuration;
            _saleApp = saleApp;
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> New([FromBody] SaleInformationNewRequest obj)
        {
            try
            {
                var res = await _saleApp.New(obj);
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
        public async Task<IActionResult> Delete([FromBody] SaleInformationDeleteRequest obj)
        {
            try
            {
                var res = await _saleApp.Delete(obj);
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
                var res = await _saleApp.Get();
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
        public async Task<IActionResult> Modify([FromBody] SaleInformationModifyRequest obj)
        {
            try
            {
                var res = await _saleApp.Modify(obj);
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
        public async Task<IActionResult> ModifyActivate([FromBody] SaleInformationModifyActivateRequest obj)
        {
            try
            {
                var res = await _saleApp.ModifyActivate(obj);
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
        public async Task<IActionResult> ModifyDeActivate([FromBody] SaleInformationModifyDeActivateRequest obj)
        {
            try
            {
                var res = await _saleApp.ModifyDeActivate(obj);
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
                var res = await _saleApp.DeactivatedList();
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
                var res = await _saleApp.ActivatedList();
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