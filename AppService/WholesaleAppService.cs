using Microsis.CWM.Common;
using Microsis.CWM.DAL;
using Microsis.CWM.Dto.Basic;
using Microsis.CWM.Dto.Wholesale;
using Microsis.CWM.Model;
using Microsis.CWM.Util;
using Microsoft.EntityFrameworkCore;
using static Microsis.CWM.Util.LoggerProxy;

namespace Microsis.CWM.AppService
{
    public interface IWholesaleAppService
    {
        Task<ResponseBase<WholesaleNewResponse>> New(WholesaleNewRequest obj);
        Task<ResponseBase<IList<WholesaleActivatedListResponse>>> ActivatedList();
        Task<ResponseBase<IList<WholesaleDeActivatedListResponse>>> DeActivatedList();
        Task<ResponseBase<WholesaleDeleteResponse>> Delete(WholesaleDeleteRequest obj);
        Task<ResponseBase<WholesaleGetResponse>> Get();
        Task<ResponseBase<WholesaleModifyResponse>> Modify(WholesaleModifyRequest obj);
        Task<ResponseBase<WholesaleModifyActivateResponse>> ModifyActivate(WholesaleModifyActivateRequest obj);
        Task<ResponseBase<WholesaleModifyDeActivateResponse>> ModifyDeActivate(WholesaleModifyDeActivateRequest obj);
        Task<ResponseBase<WholesaleGetWholesaleIdResponse>> GetWholesaleId();
        Task<ResponseBase<WholesaleGetWholesaleIdResponse>> GetWholesaleId(WholesaleGetWholesaleIdRequest obj);
    }
    public class WholesaleAppService : IWholesaleAppService
    {
        private readonly ICwmCtx _context;
        //private readonly IValidationAppService _validationApp;
        private readonly IHelperAppService _helperApp;
        public WholesaleAppService(ICwmCtx context, IHelperAppService helperApp)
        {
            _context = context;
            //_validationApp = validationApp;
            _helperApp = helperApp;
        }
        public async Task<ResponseBase<WholesaleNewResponse>> New(WholesaleNewRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var userInfo = _helperApp.GetClaim();
                if(userInfo == null)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.New Error {ErrorCode.TokenExpired}.({(int)NErrorCode.TokenExpired})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.TokenExpired, ErrorDesc = ErrorCode.TokenExpired });
                    return new ResponseBase<WholesaleNewResponse>(false, ErrorCode.TokenExpired, errors);
                }
                //var valid = _validationApp.ShopNew(obj);
                //if (valid.Any())
                //{
                //    return new ResponseBase<ShopNewResponse>(false, ErrorCode.Nok, valid);
                //}
                Log(LogLevels.Trace, $"Claim Info {userInfo.Username}  {userInfo.UserId}");
                var isAvailble = await _context.Wholesale.Where(x => x.UserId == userInfo.UserId).ToListAsync();
                if (isAvailble.Any())
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.New Error {ErrorCode.ItterateWholesaleGuildNo}.({(int)NErrorCode.ItterateWholesaleGuildNo})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.ItterateWholesaleGuildNo, ErrorDesc = ErrorCode.ItterateWholesaleGuildNo });
                    return new ResponseBase<WholesaleNewResponse>(false, ErrorCode.ItterateWholesaleGuildNo, errors);
                }
                Wholesale wholesale = new Wholesale()
                {
                    GuildNo = obj.GuildNo,
                    NameEn = obj.NameEn,
                    NameFa = obj.NameFa,
                    AdditionalData = obj.AdditionalData,
                    GuildCode = obj.GuildCode,
                    //
                    // todo : After Designing the Guild we'll fill this up
                    GuildId = 0,
                    //
                    IsActive = false,
                    Delete = false,
                    ManagerNameFa = obj.ManagerNameFa,
                    ManagerNationalCode = obj.ManagerNationalCode,
                    Mobile = obj.Mobile,
                    NationalId = obj.NationalId,
                    RegisterDateInt = int.Parse(DateTimeUtil.MiladiSysStandardDateNow(false)),
                    RegisterDateShamsi = DateTimeUtil.ShamsiSysStandardDateTimeNow(),
                    RegisterMiladi = DateTimeUtil.MiladiSysStandardDateTimeNow(),
                    Tel1 = obj.Tel1,
                    Tel2 = obj.Tel2,
                    UserId = userInfo.UserId,
                    UserKey = userInfo.UserKey,
                    WholesaleLogo = obj.WholesaleLogo,
                };
                await _context.Wholesale.AddAsync(wholesale);
                await _context.SaveChangesAsync();
                Log(LogLevels.Fatal, $"WholesaleAppService.New Wholesale {obj.NameFa}({obj.GuildNo}) Added Successfully.");
                return new ResponseBase<WholesaleNewResponse>(new WholesaleNewResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"WholesaleAppService.New\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ex.Message});
                return new ResponseBase<WholesaleNewResponse>(false, ex.Message, errors);
            }
        }
        public async Task<ResponseBase<WholesaleModifyResponse>> Modify(WholesaleModifyRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var wholesaleId = await GetWholesaleId();
                if (wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.Modify Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<WholesaleModifyResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }
                var isAvailble = await _context.Wholesale.Where(x => x.Id == wholesaleId.Data.WholesaleId).ToListAsync();

                if (!isAvailble.Any())
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.Modify Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<WholesaleModifyResponse>(false, ErrorCode.RecordNotFound, errors);
                }

                isAvailble[0].GuildNo =(string.IsNullOrEmpty(obj.GuildNo)) ? isAvailble[0].GuildNo : obj.GuildNo;
                isAvailble[0].NameEn = (string.IsNullOrEmpty(obj.NameEn)) ? isAvailble[0].NameEn : obj.NameEn;
                isAvailble[0].NameFa = (string.IsNullOrEmpty(obj.NameFa)) ? isAvailble[0].NameFa : obj.NameFa;
                isAvailble[0].AdditionalData = (string.IsNullOrEmpty(obj.AdditionalData)) ? isAvailble[0].AdditionalData : obj.AdditionalData;
                isAvailble[0].ManagerNameFa = (string.IsNullOrEmpty(obj.ManagerNameFa)) ? isAvailble[0].ManagerNameFa : obj.ManagerNameFa;
                isAvailble[0].ManagerNationalCode = (string.IsNullOrEmpty(obj.ManagerNationalCode)) ? isAvailble[0].ManagerNationalCode : obj.ManagerNationalCode;
                isAvailble[0].Mobile = (string.IsNullOrEmpty(obj.Mobile)) ? isAvailble[0].Mobile: obj.Mobile;
                isAvailble[0].NationalId = (string.IsNullOrEmpty(obj.NationalId)) ? isAvailble[0].NationalId : obj.NationalId;
                isAvailble[0].Tel1 = (string.IsNullOrEmpty(obj.Tel1)) ? isAvailble[0].Tel1 : obj.Tel1;
                isAvailble[0].Tel2 = (string.IsNullOrEmpty(obj.Tel2)) ? isAvailble[0].Tel2 : obj.Tel2;
                isAvailble[0].WholesaleLogo = (string.IsNullOrEmpty(obj.WholesaleLogo)) ? isAvailble[0].WholesaleLogo : obj.WholesaleLogo;
                isAvailble[0].GuildCode = (string.IsNullOrEmpty(obj.GuildCode)) ? isAvailble[0].GuildCode: obj.GuildCode;
                _context.Wholesale.Update(isAvailble[0]);
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"WholesaleAppService.Modify Wholesale {obj.NameFa}({obj.GuildNo}) Updated Successfully.");

                return new ResponseBase<WholesaleModifyResponse>(new WholesaleModifyResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"WholesaleAppService.Modify\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<WholesaleModifyResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<IList<WholesaleActivatedListResponse>>> ActivatedList()
        {
            List<Error> errors = new();

            try
            {
                var list = await _context.Wholesale.Where(x => x.Delete == false && x.IsActive == true).ToListAsync();
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();
                if (list.Count == 0)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.ActivatedList {ErrorCode.EmptyData}.({(int)NErrorCode.EmptyData})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.EmptyData, ErrorDesc = ErrorCode.EmptyData });
                    return new ResponseBase<IList<WholesaleActivatedListResponse>>(false, ErrorCode.EmptyData, errors);
                }
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();
                IList<WholesaleActivatedListResponse> ResponseList = new List<WholesaleActivatedListResponse>();
                foreach (var num in list)
                {
                    ResponseList.Add(new WholesaleActivatedListResponse()
                    {
                        AdditionalData = num.AdditionalData
                        ,
                        GuildCode = num.GuildCode,
                        GuildId = num.GuildId,
                        GuildNo = num.GuildNo
                        ,
                        IsActive = num.IsActive,
                        ManagerNameFa = num.ManagerNameFa,
                        ManagerNationalCode = num.ManagerNationalCode
                        ,
                        Mobile = num.Mobile,
                        NameEn = num.NameEn,
                        NameFa = num.NameFa,
                        NationalId = num.NationalId
                        ,
                        RegisterDateInt = num.RegisterDateInt,
                        RegisterDateShamsi = num.RegisterDateShamsi
                        ,
                        RegisterMiladi = num.RegisterMiladi,
                        Tel1 = num.Tel1,
                        Tel2 = num.Tel2,
                        WholesaleLogo = num.WholesaleLogo,
                        Id = num.Id,
                        Delete = num.Delete
                        ,UserId = num.UserId
                        ,UserKey=num.UserKey
                    });
                }
                Log(LogLevels.Fatal, $"WholesaleAppService.List Wholesale List Successfully.");
                return new ResponseBase<IList<WholesaleActivatedListResponse>>(ResponseList);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"WholesaleAppService.ActivatedList\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<IList<WholesaleActivatedListResponse>>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<IList<WholesaleDeActivatedListResponse>>> DeActivatedList()
        {
            List<Error> errors = new();

            try
            {
                var list = await _context.Wholesale.Where(x => x.Delete == false && x.IsActive == false).ToListAsync();
                if (list.Count == 0)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.DeActivatedList {ErrorCode.EmptyData}.({(int)NErrorCode.EmptyData})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.EmptyData, ErrorDesc = ErrorCode.EmptyData });
                    return new ResponseBase<IList<WholesaleDeActivatedListResponse>>(false, ErrorCode.EmptyData, errors);
                }
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();
                IList<WholesaleDeActivatedListResponse> ResponseList = new List<WholesaleDeActivatedListResponse>();
                foreach (var num in list)
                {
                    ResponseList.Add(new WholesaleDeActivatedListResponse()
                    {
                        AdditionalData = num.AdditionalData
                        ,
                        GuildCode = num.GuildCode,
                        GuildId = num.GuildId,
                        GuildNo = num.GuildNo
                        ,
                        IsActive = num.IsActive,
                        ManagerNameFa = num.ManagerNameFa,
                        ManagerNationalCode = num.ManagerNationalCode
                        ,
                        Mobile = num.Mobile,
                        NameEn = num.NameEn,
                        NameFa = num.NameFa,
                        NationalId = num.NationalId
                        ,
                        RegisterDateInt = num.RegisterDateInt,
                        RegisterDateShamsi = num.RegisterDateShamsi
                        ,
                        RegisterMiladi = num.RegisterMiladi,
                        Tel1 = num.Tel1,
                        Tel2 = num.Tel2,
                        WholesaleLogo = num.WholesaleLogo,Id = num.Id,
                        Delete=num.Delete,
                        UserId = num.UserId,
                        UserKey = num.UserKey
                    });
                }
                Log(LogLevels.Fatal, $"WholesaleAppService.DeactivatedList Wholesale List Successfully.");

                return new ResponseBase<IList<WholesaleDeActivatedListResponse>>(ResponseList);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"WholesaleAppService.DeactivatedList\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<IList<WholesaleDeActivatedListResponse>>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<WholesaleGetResponse>> Get()
        {
            List<Error> errors = new();
            try
            {
                var wholesaleId = await GetWholesaleId();
                if(wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.Get Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<WholesaleGetResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }
                var list = await _context.Wholesale.Where(x => x.Delete == false && x.Id == wholesaleId.Data.WholesaleId).ToListAsync();
                if(list.Count == 0)
                {
                        Log(LogLevels.Fatal, $"WholesaleAppService.Get {ErrorCode.EmptyData}.({(int)NErrorCode.EmptyData})");
                        errors.Add(new Error() { ErrorCode = (int)NErrorCode.EmptyData, ErrorDesc = ErrorCode.EmptyData });
                        return new ResponseBase<WholesaleGetResponse>(false, ErrorCode.EmptyData, errors);
                }
                Log(LogLevels.Fatal, $"WholesaleAppService.Get Wholesale Got Successfully.");
                var Response = new WholesaleGetResponse() { AdditionalData = list[0].AdditionalData,GuildCode = list[0].GuildCode,GuildId = list[0].GuildId,Id = list[0].Id
                    ,IsActive = list[0].IsActive,GuildNo = list[0].GuildNo,ManagerNameFa = list[0].ManagerNameFa,ManagerNationalCode = list[0].ManagerNationalCode
                    ,Mobile = list[0].Mobile,NameEn = list[0].NameEn,NameFa = list[0].NameFa,NationalId = list[0].NationalId,RegisterDateInt = list[0].RegisterDateInt
                    ,RegisterDateShamsi = list[0].RegisterDateShamsi,RegisterMiladi=list[0].RegisterMiladi,Tel1 = list[0].Tel1,Tel2 = list[0].Tel2,WholesaleLogo = list[0].WholesaleLogo                    
                };
                return new ResponseBase<WholesaleGetResponse>(Response);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"WholesaleAppService.Get\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<WholesaleGetResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<WholesaleDeleteResponse>> Delete(WholesaleDeleteRequest obj)
        {
            List<Error> errors = new();

            try
            {
                var wholesaleId = await GetWholesaleId(new WholesaleGetWholesaleIdRequest() { UserId = obj.UserId });
                if (wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.Delete Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<WholesaleDeleteResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }
                var list = await _context.Wholesale.Where(x => x.Id == wholesaleId.Data.WholesaleId && x.Delete == false).ToListAsync();
                if (list.Count == 1)
                {
                    list[0].Delete = true;
                    _context.Wholesale.Update(list[0]);
                    //_context.Shop.Remove(list[0]);
                }
                else
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.Delete Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<WholesaleDeleteResponse>(false, ErrorCode.RecordNotFound, errors);
                }
                await _context.SaveChangesAsync();
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();

                Log(LogLevels.Fatal, $"WholesaleAppService.Delete Wholesale Deleted Successfully.");

                return new ResponseBase<WholesaleDeleteResponse>(new WholesaleDeleteResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"WholesaleAppService.Delete\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<WholesaleDeleteResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<WholesaleModifyActivateResponse>> ModifyActivate(WholesaleModifyActivateRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var userInfo = _helperApp.GetClaim();
                if (userInfo == null)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.ModifyActivate Error {ErrorCode.TokenExpired}.({(int)NErrorCode.TokenExpired})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.TokenExpired, ErrorDesc = ErrorCode.TokenExpired });
                    return new ResponseBase<WholesaleModifyActivateResponse>(false, ErrorCode.TokenExpired, errors);
                }
                var wholesaleId = await GetWholesaleId(new WholesaleGetWholesaleIdRequest() { UserId = obj.UserId });
                if (wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.WholesaleModifyActivateResponse Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<WholesaleModifyActivateResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }
                var list = await _context.Wholesale.Where(x => x.Id == wholesaleId.Data.WholesaleId && x.IsActive == false && x.Delete == false).ToListAsync();
                if (list.Count == 1)
                {
                    list[0].IsActive = true;
                    list[0].ConfirmUsername = userInfo.Username;

                    _context.Wholesale.Update(list[0]);
                    //_context.Shop.Remove(list[0]);
                }
                else
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.ModifyActivate Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<WholesaleModifyActivateResponse>(false, ErrorCode.RecordNotFound, errors);
                }
                await _context.SaveChangesAsync();
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();

                Log(LogLevels.Fatal, $"WholesaleAppService.ModifyActivate Wholesale Activated Successfully.");

                return new ResponseBase<WholesaleModifyActivateResponse>(new WholesaleModifyActivateResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"WholesaleAppService.ModifyActivate\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<WholesaleModifyActivateResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<WholesaleModifyDeActivateResponse>> ModifyDeActivate(WholesaleModifyDeActivateRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var wholesaleId = await GetWholesaleId(new WholesaleGetWholesaleIdRequest() { UserId = obj.UserId });
                if (wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.WholesaleModifyDeActivateResponse Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<WholesaleModifyDeActivateResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }
                var list = await _context.Wholesale.Where(x => x.Id == wholesaleId.Data.WholesaleId && x.IsActive == true && x.Delete == false).ToListAsync();
                if (list.Count == 1)
                {
                    list[0].IsActive = false;
                    _context.Wholesale.Update(list[0]);
                    //_context.Shop.Remove(list[0]);
                }
                else
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.ModifyDeActivate Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<WholesaleModifyDeActivateResponse>(false, ErrorCode.RecordNotFound, errors);
                }
                await _context.SaveChangesAsync();
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();

                Log(LogLevels.Fatal, $"WholesaleAppService.ModifyDeActivate Wholesale Activated Successfully.");

                return new ResponseBase<WholesaleModifyDeActivateResponse>(new WholesaleModifyDeActivateResponse());
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"WholesaleAppService.ModifyDeActivate\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<WholesaleModifyDeActivateResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        #region ______ Tools ______
        public async Task<ResponseBase<WholesaleGetWholesaleIdResponse>> GetWholesaleId()
        {
            List<Error> errors = new();
            try
            {
                var userInfo = _helperApp.GetClaim();
                if(userInfo == null)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.GetWholesaleId Error {ErrorCode.TokenExpired}.({(int)NErrorCode.TokenExpired})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.TokenExpired, ErrorDesc = ErrorCode.TokenExpired });
                    return new ResponseBase<WholesaleGetWholesaleIdResponse>(false, ErrorCode.TokenExpired, errors);
                }
                var list = await _context.Wholesale.Where(x => x.UserId == userInfo.UserId && x.Delete == false).ToListAsync();
                if (list.Count == 1)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.GetWholesaleId Wholesale WholesaleId Got Successfully.");
                    WholesaleGetWholesaleIdResponse response = new WholesaleGetWholesaleIdResponse() { WholesaleId = list[0].Id };
                    return new ResponseBase<WholesaleGetWholesaleIdResponse>(response);
                }
                else
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.GetWholesaleId Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<WholesaleGetWholesaleIdResponse>(false, ErrorCode.RecordNotFound, errors);
                }
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"WholesaleAppService.GetWholesaleId\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<WholesaleGetWholesaleIdResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<WholesaleGetWholesaleIdResponse>> GetWholesaleId(WholesaleGetWholesaleIdRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var list = await _context.Wholesale.Where(x => x.UserId == obj.UserId && x.Delete == false).ToListAsync();
                if (list.Count == 1)
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.GetWholesaleId Wholesale WholesaleId Got Successfully.");
                    WholesaleGetWholesaleIdResponse response = new WholesaleGetWholesaleIdResponse() { WholesaleId = list[0].Id };
                    return new ResponseBase<WholesaleGetWholesaleIdResponse>(response);
                }
                else
                {
                    Log(LogLevels.Fatal, $"WholesaleAppService.GetWholesaleId Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<WholesaleGetWholesaleIdResponse>(false, ErrorCode.RecordNotFound, errors);
                }
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"WholesaleAppService.GetWholesaleId\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<WholesaleGetWholesaleIdResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        #endregion

    }
}