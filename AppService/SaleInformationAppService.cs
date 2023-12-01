using Microsis.CWM.Common;
using Microsis.CWM.DAL;
using Microsis.CWM.Dto.Basic;
using Microsis.CWM.Dto.SalesInformation;
using Microsis.CWM.Dto.Wholesale;
using Microsis.CWM.Model;
using Microsis.CWM.Util;
using Microsoft.EntityFrameworkCore;
using static Microsis.CWM.Util.LoggerProxy;

namespace Microsis.CWM.AppService
{
    public interface ISaleInformationAppService
    {
        Task<ResponseBase<SaleInformationNewResponse>> New(SaleInformationNewRequest obj);
        Task<ResponseBase<SaleInformationModifyResponse>> Modify(SaleInformationModifyRequest obj);
        Task<ResponseBase<SaleInformationDeleteResponse>> Delete(SaleInformationDeleteRequest obj);
        Task<ResponseBase<SaleInformationGetResponse>> Get();
        Task<ResponseBase<SaleInformationModifyActivateResponse>> ModifyActivate(SaleInformationModifyActivateRequest obj);
        Task<ResponseBase<SaleInformationModifyDeActivateResponse>> ModifyDeActivate(SaleInformationModifyDeActivateRequest obj);
        Task<ResponseBase<IList<SaleInformationDeActivatedListResponse>>> DeactivatedList();
        Task<ResponseBase<IList<SaleInformationActivatedListResponse>>> ActivatedList();
    }
    public class SaleInformationAppService : ISaleInformationAppService
    {
        private readonly ICwmCtx _context;
        private readonly IHelperAppService _helperApp;
        private readonly IWholesaleAppService _wholesaleApp;

        public SaleInformationAppService(ICwmCtx context, IHelperAppService helperApp, IWholesaleAppService wholesaleApp)
        {
            _context = context;
            _helperApp = helperApp;
            _wholesaleApp = wholesaleApp;
        }
        public async Task<ResponseBase<SaleInformationNewResponse>> New(SaleInformationNewRequest obj)
        {
            List<Error> errors = new();
            try
            {
                if (string.IsNullOrEmpty(obj.SaleCondition) || string.IsNullOrEmpty(obj.OpenningDescription))
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.New Error {ErrorCode.EmptyData}.({(int)NErrorCode.EmptyData})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.EmptyData, ErrorDesc = ErrorCode.EmptyData });
                    return new ResponseBase<SaleInformationNewResponse>(false, ErrorCode.EmptyData, errors);
                }
                var UserInfo = _helperApp.GetClaim();
                if (UserInfo == null)
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.New Error {ErrorCode.TokenExpired}.({(int)NErrorCode.TokenExpired})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.TokenExpired, ErrorDesc = ErrorCode.TokenExpired });
                    return new ResponseBase<SaleInformationNewResponse>(false, ErrorCode.TokenExpired, errors);
                }
                var wholesaleId = await _wholesaleApp.GetWholesaleId();
                if (wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.New Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<SaleInformationNewResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }
                var isAvailble = await _context.salesInformation.Where(x => x.WholesaleId == wholesaleId.Data.WholesaleId).ToListAsync();
                if (isAvailble.Any())
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.New Error {ErrorCode.ItterateWholesaleGuildNo}.({(int)NErrorCode.ItterateWholesaleGuildNo})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.ItterateWholesaleGuildNo, ErrorDesc = ErrorCode.ItterateWholesaleGuildNo });
                    return new ResponseBase<SaleInformationNewResponse>(false, ErrorCode.ItterateWholesaleGuildNo, errors);
                }

                SaleInformation saleInf = new SaleInformation()
                {
                    OpenningDescription = obj.OpenningDescription,
                    WholesaleId = wholesaleId.Data.WholesaleId,
                    SaleCondition = obj.SaleCondition,
                    IsActive = false,
                    RecordDate = DateTimeUtil.MiladiSysStandardDateNow(),
                    RecordTime = DateTimeUtil.SysStandardTimeNow(),
                    RecordDateint = int.Parse(DateTimeUtil.MiladiSysStandardDateNow(false)),
                    RecordTimeint = int.Parse(DateTimeUtil.SysStandardTimeNow(false)),
                    RecordUsername = UserInfo.Username
                };

                await _context.salesInformation.AddAsync(saleInf);
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"SaleInformationAppService.New SaleInformation ({wholesaleId.Data.WholesaleId}) Added Successfully.");

                return new ResponseBase<SaleInformationNewResponse>(new SaleInformationNewResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"SaleInformationAppService.New\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<SaleInformationNewResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<SaleInformationModifyResponse>> Modify(SaleInformationModifyRequest obj)
        {
            List<Error> errors = new();
            try
            {

                var wholesaleId = await _wholesaleApp.GetWholesaleId();
                if (wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.Modify Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<SaleInformationModifyResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }
                var isAvailble = await _context.salesInformation.Where(x => x.WholesaleId == wholesaleId.Data.WholesaleId).ToListAsync();

                if (!isAvailble.Any())
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.Modify Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<SaleInformationModifyResponse>(false, ErrorCode.RecordNotFound, errors);
                }

                isAvailble[0].SaleCondition = (string.IsNullOrEmpty(obj.SaleCondition)) ? isAvailble[0].SaleCondition: obj.SaleCondition;
                isAvailble[0].OpenningDescription = (string.IsNullOrEmpty(obj.OpenningDescription)) ? isAvailble[0].OpenningDescription : obj.OpenningDescription;

                _context.salesInformation.Update(isAvailble[0]);
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"SaleInformationAppService.Modify SaleInformation ({wholesaleId.Data.WholesaleId}) Updated Successfully.");

                return new ResponseBase<SaleInformationModifyResponse>(new SaleInformationModifyResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"SaleInformationAppService.Modify\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<SaleInformationModifyResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<SaleInformationDeleteResponse>> Delete(SaleInformationDeleteRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var wholesaleId = await _wholesaleApp.GetWholesaleId();
                if (wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.Delete Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<SaleInformationDeleteResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }

                var isAvailble = await _context.salesInformation.Where(x => x.WholesaleId == wholesaleId.Data.WholesaleId).ToListAsync();

                if (!isAvailble.Any())
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.Delete Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<SaleInformationDeleteResponse>(false, ErrorCode.RecordNotFound, errors);
                }

                _context.salesInformation.Remove(isAvailble[0]);
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"SaleInformationAppService.Delete SaleInformation ({wholesaleId.Data.WholesaleId}) Deleted Successfully.");

                return new ResponseBase<SaleInformationDeleteResponse>(new SaleInformationDeleteResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"SaleInformationAppService.Deleted\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<SaleInformationDeleteResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<SaleInformationGetResponse>> Get()
        {
            List<Error> errors = new();

            try
            {
                var wholesaleId = await _wholesaleApp.GetWholesaleId();
                if (wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.Get Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<SaleInformationGetResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }
                var list = await _context.salesInformation.Where(x => x.WholesaleId == wholesaleId.Data.WholesaleId).ToListAsync();
                if (list.Count == 0)
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.Get Error {ErrorCode.EmptyData}.({(int)NErrorCode.EmptyData})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.EmptyData, ErrorDesc = ErrorCode.EmptyData });
                    return new ResponseBase<SaleInformationGetResponse>(false, ErrorCode.EmptyData, errors);
                }
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();

                Log(LogLevels.Fatal, $"SaleInformationAppService.Get SaleInformation Got Successfully.");
                var Response = new SaleInformationGetResponse() { OpenningDescription = list[0].OpenningDescription, SaleCondition = list[0].SaleCondition };
                return new ResponseBase<SaleInformationGetResponse>(Response);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"SaleInformationAppService.Get\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<SaleInformationGetResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<SaleInformationModifyActivateResponse>> ModifyActivate(SaleInformationModifyActivateRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var UserInfo = _helperApp.GetClaim();
                if (UserInfo == null)
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.ModifyActivate Error {ErrorCode.TokenExpired}.({(int)NErrorCode.TokenExpired})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.TokenExpired, ErrorDesc = ErrorCode.TokenExpired });
                    return new ResponseBase<SaleInformationModifyActivateResponse>(false, ErrorCode.TokenExpired, errors);
                }
                //var wholesaleId = await _shopApp.GetWholesaleId(new ShopGetWholesaleIdRequest() { UserId = obj.UserId });
                //if (wholesaleId.Data == null)
                //{
                //    Log(LogLevels.Fatal, $"SaleInformationAppService.ModifyActivate Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                //    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                //    return new ResponseBase<SaleInformationModifyActivateResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                //}
                var list = await _context.salesInformation.Where(x => x.Id == obj.WholesaleId && x.IsActive == false).ToListAsync();
                if (list.Count == 1)
                {
                    list[0].IsActive = true;
                    list[0].ConfirmDate = DateTimeUtil.MiladiSysStandardDateNow();
                    list[0].ConfirmTime = DateTimeUtil.SysStandardTimeNow();
                    list[0].ConfirmDateint = int.Parse(DateTimeUtil.MiladiSysStandardDateNow(false));
                    list[0].ConfirmTimeint = int.Parse(DateTimeUtil.SysStandardTimeNow(false));
                    list[0].ConfirmUsername = UserInfo.Username;
                    _context.salesInformation.Update(list[0]);
                }
                else
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.ModifyActivate Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<SaleInformationModifyActivateResponse>(false, ErrorCode.RecordNotFound, errors);
                }
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"SaleInformationAppService.ModifyActivate SaleInformation Activated Successfully.");
                return new ResponseBase<SaleInformationModifyActivateResponse>(new SaleInformationModifyActivateResponse());
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"SaleInformationAppService.ModifyActivate\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<SaleInformationModifyActivateResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<SaleInformationModifyDeActivateResponse>> ModifyDeActivate(SaleInformationModifyDeActivateRequest obj)
        {
            List<Error> errors = new();
            try
            {
                //var wholesaleId = await _shopApp.GetWholesaleId(new ShopGetWholesaleIdRequest() { UserId = obj.UserId });
                //if (wholesaleId.Data == null)
                //{
                //    Log(LogLevels.Fatal, $"SaleInformationAppService.ModifyDeActivate Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                //    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                //    return new ResponseBase<SaleInformationModifyDeActivateResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                //}
                var list = await _context.salesInformation.Where(x => x.Id == obj.WholesaleId && x.IsActive == true).ToListAsync();
                if (list.Count == 1)
                {
                    list[0].IsActive = false;
                    _context.salesInformation.Update(list[0]);
                }
                else
                {
                    Log(LogLevels.Fatal, $"SaleInformationAppService.ModifyDeActivate Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<SaleInformationModifyDeActivateResponse>(false, ErrorCode.RecordNotFound, errors);
                }
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"SaleInformationAppService.ModifyDeActivate SaleInformation DeActivated Successfully.");
                return new ResponseBase<SaleInformationModifyDeActivateResponse>(new SaleInformationModifyDeActivateResponse());
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"SaleInformationAppService.ModifyDeActivate\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<SaleInformationModifyDeActivateResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<IList<SaleInformationDeActivatedListResponse>>> DeactivatedList()
        {
            List<Error> errors = new();

            try
            {
                var list = await _context.salesInformation.Where(x => x.IsActive == false).ToListAsync();
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();
                IList<SaleInformationDeActivatedListResponse> ResponseList = new List<SaleInformationDeActivatedListResponse>();
                foreach(var num in list)
                {
                    ResponseList.Add(new SaleInformationDeActivatedListResponse()
                    {
                        WholesaleId = num.Id,
                        ConfirmDate = num.ConfirmDate,
                        ConfirmDateint = num.ConfirmDateint,
                        ConfirmTime = num.ConfirmTime,
                        ConfirmTimeint = num.ConfirmTimeint,
                        ConfirmUsername = num.ConfirmUsername,
                        Id = num.Id,
                        IsActive = num.IsActive,
                        OpenningDescription = num.OpenningDescription,
                        RecordDateint = num.RecordDateint,
                        RecordDate = num.RecordDate,
                        RecordTime = num.RecordTime,
                        RecordTimeint = num.RecordTimeint,
                        RecordUsername = num.RecordUsername,
                        SaleCondition = num.SaleCondition,
                    });
                }
                Log(LogLevels.Fatal, $"SaleInformationAppService.DeactivateList SaleInformation List Successfully.");
                return new ResponseBase<IList<SaleInformationDeActivatedListResponse>>(ResponseList);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"SaleInformationAppService.DeactivateList\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<IList<SaleInformationDeActivatedListResponse>>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<IList<SaleInformationActivatedListResponse>>> ActivatedList()
        {
            List<Error> errors = new();
            try
            {
                var list = await _context.salesInformation.Where(x => x.IsActive == true).ToListAsync();
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();
                IList<SaleInformationActivatedListResponse> ResponseList = new List<SaleInformationActivatedListResponse>();
                foreach (var num in list)
                {
                    ResponseList.Add(new SaleInformationActivatedListResponse()
                    {
                        WholesaleId = num.Id,
                        ConfirmDate = num.ConfirmDate,
                        ConfirmDateint = num.ConfirmDateint,
                        ConfirmTime = num.ConfirmTime,
                        ConfirmTimeint = num.ConfirmTimeint,
                        ConfirmUsername = num.ConfirmUsername,
                        Id = num.Id,
                        IsActive = num.IsActive,
                        OpenningDescription = num.OpenningDescription,
                        RecordDateint = num.RecordDateint,
                        RecordDate = num.RecordDate,
                        RecordTime = num.RecordTime,
                        RecordTimeint = num.RecordTimeint,
                        RecordUsername = num.RecordUsername,
                        SaleCondition = num.SaleCondition,
                    });
                }
                Log(LogLevels.Fatal, $"SaleInformationAppService.ActivateList SaleInformation List Successfully.");
                return new ResponseBase<IList<SaleInformationActivatedListResponse>>(ResponseList);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"SaleInformationAppService.ActivateList\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<IList<SaleInformationActivatedListResponse>>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
    }
}
