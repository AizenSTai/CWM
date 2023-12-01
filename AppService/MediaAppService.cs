using Microsis.CWM.Common;
using Microsis.CWM.DAL;
using Microsis.CWM.Dto.Basic;
using Microsis.CWM.Dto.Media;
using Microsis.CWM.Dto.Wholesale;
using Microsis.CWM.Model;
using Microsis.CWM.Util;
using Microsoft.EntityFrameworkCore;
using static Microsis.CWM.Util.LoggerProxy;

namespace Microsis.CWM.AppService
{
    public interface IMediaAppService
    {
        Task<ResponseBase<IList<MediaGetResponse>>> Get();
        Task<ResponseBase<MediaDeleteResponse>> Delete(MediaDeleteRequest obj);
        Task<int> Modify(MediaModifyRequest obj);
        Task<ResponseBase<MediaNewResponse>> New(MediaNewRequest obj);
        Task<ResponseBase<MediaModifyActivateResponse>> ModifyActivate(MediaModifyActivateRequest obj);
        Task<ResponseBase<MediaModifyDeActivateResponse>> ModifyDeActivate(MediaModifyDeActivateRequest obj);
        Task<ResponseBase<IList<MediaDeActivatedListResponse>>> DeactivatedList();
        Task<ResponseBase<IList<MediaActivatedListResponse>>> ActivatedList();
    }
    public class MediaAppService : IMediaAppService
    {
        private readonly ICwmCtx _context;
        private readonly IHelperAppService _helperApp;
        private readonly IMediaSettingsAppService _mediaSettings;
        private readonly IWholesaleAppService _wholesaleApp;

        public MediaAppService(ICwmCtx context, IHelperAppService helperApp, IMediaSettingsAppService mediaSettings, IWholesaleAppService wholesaleApp)
        {
            _context = context;
            _helperApp = helperApp;
            _mediaSettings = mediaSettings;
            _wholesaleApp = wholesaleApp;
        }
        public async Task<ResponseBase<MediaNewResponse>> New(MediaNewRequest obj)
        {
            List<Error> errors = new();
            try
            {
                if (obj.OrderId == null || string.IsNullOrEmpty(obj.WholesaleImage))
                {
                    Log(LogLevels.Fatal, $"MediaAppService.New Error {ErrorCode.EmptyData}.({(int)NErrorCode.EmptyData})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.EmptyData, ErrorDesc = ErrorCode.EmptyData });
                    return new ResponseBase<MediaNewResponse>(false, ErrorCode.EmptyData, errors);
                }
                var UserInfo = _helperApp.GetClaim();
                if (UserInfo == null)
                {
                    Log(LogLevels.Fatal, $"MediaAppService.New Error {ErrorCode.TokenExpired}.({(int)NErrorCode.TokenExpired})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.TokenExpired, ErrorDesc = ErrorCode.TokenExpired });
                    return new ResponseBase<MediaNewResponse>(false, ErrorCode.TokenExpired, errors);
                }
                var wholesaleId = await _wholesaleApp.GetWholesaleId(new WholesaleGetWholesaleIdRequest() { UserId = UserInfo.UserId });
                if (wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"MediaAppService.New Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<MediaNewResponse>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }
                var picCount = _mediaSettings.Get();
                var ImagesCount = await _context.Media.Where(x => x.WholesaleId == wholesaleId.Data.WholesaleId).ToListAsync();
                var isUpdated = await Modify(new MediaModifyRequest() { OrderId = obj.OrderId, WholesaleImage = obj.WholesaleImage,WholeSaleId = wholesaleId.Data.WholesaleId });
                if(isUpdated == 200)
                {
                    return new ResponseBase<MediaNewResponse>(new MediaNewResponse() { });
                }
                else if(isUpdated == 404)
                {
                    if (picCount.Result.Data != null)
                    {
                        if (ImagesCount.Count >= picCount.Result.Data.PicMaxQuantity)
                        {
                            Log(LogLevels.Fatal, $"MediaAppService.New Error {ErrorCode.MaximumRecordsReached}.({(int)NErrorCode.MaximumRecordsReached})");
                            errors.Add(new Error() { ErrorCode = (int)NErrorCode.MaximumRecordsReached, ErrorDesc = ErrorCode.MaximumRecordsReached });
                            return new ResponseBase<MediaNewResponse>(false, ErrorCode.MaximumRecordsReached, errors);
                        }
                        else
                        {
                            Media media = new()
                            {
                                OrderId = (int)obj.OrderId,
                                WholesaleId = wholesaleId.Data.WholesaleId,
                                WholesaleImage = obj.WholesaleImage,
                                IsActive = false,
                                RecordDate = DateTimeUtil.MiladiSysStandardDateNow(),
                                RecordTime = DateTimeUtil.SysStandardTimeNow(),
                                RecordDateint = int.Parse(DateTimeUtil.MiladiSysStandardDateNow(false)),
                                RecordTimeint = int.Parse(DateTimeUtil.SysStandardTimeNow(false)),
                                RecordUsername = UserInfo.Username
                            };

                            await _context.Media.AddAsync(media);
                            await _context.SaveChangesAsync();

                            Log(LogLevels.Fatal, $"MediaAppService.New Media Added Successfully.");
                            return new ResponseBase<MediaNewResponse>(new MediaNewResponse() { });
                        }
                    }
                    else
                    {
                        Media media = new()
                        {
                            OrderId = (int)obj.OrderId,
                            WholesaleId = wholesaleId.Data.WholesaleId,
                            WholesaleImage = obj.WholesaleImage,
                            IsActive = false,
                            RecordDate = DateTimeUtil.MiladiSysStandardDateNow(),
                            RecordTime = DateTimeUtil.SysStandardTimeNow(),
                            RecordDateint = int.Parse(DateTimeUtil.MiladiSysStandardDateNow(false)),
                            RecordTimeint = int.Parse(DateTimeUtil.SysStandardTimeNow(false)),
                            RecordUsername = UserInfo.Username
                        };

                        await _context.Media.AddAsync(media);
                        await _context.SaveChangesAsync();

                        Log(LogLevels.Fatal, $"MediaAppService.New Media Added Successfully.");
                        return new ResponseBase<MediaNewResponse>(new MediaNewResponse() { });
                    }
                }                
                Log(LogLevels.Fatal, $"MediaAppService.New Media Added Successfully.");
                return new ResponseBase<MediaNewResponse>(new MediaNewResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"MediaAppService.New\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<MediaNewResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<int> Modify(MediaModifyRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var isAvailable = await _context.Media.Where(x => x.WholesaleId == obj.WholeSaleId && x.OrderId == obj.OrderId).ToListAsync();
                if (!isAvailable.Any())
                {
                    Log(LogLevels.Fatal, $"MediaAppService.Modify Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return 404;
                }                
                isAvailable[0].WholesaleImage = (obj.WholesaleImage == null) ? isAvailable[0].WholesaleImage : obj.WholesaleImage;
                _context.Media.Update(isAvailable[0]);
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"MediaAppService.Modify Media Updated Successfully.");

                return 200;
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"MediaAppService.Modify\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return 500;
            }
        }
        public async Task<ResponseBase<MediaDeleteResponse>> Delete(MediaDeleteRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var isAvailble = await _context.Media.Where(x => x.Id == obj.Id).ToListAsync();

                if (!isAvailble.Any())
                {
                    Log(LogLevels.Fatal, $"MediaAppService.Delete Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<MediaDeleteResponse>(false, ErrorCode.RecordNotFound, errors);
                }
                foreach (var num in isAvailble)
                {
                    _context.Media.Remove(num);
                }
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"MediaAppService.Delete Media Deleted Successfully.");

                return new ResponseBase<MediaDeleteResponse>(new MediaDeleteResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"MediaAppService.Deleted\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<MediaDeleteResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<IList<MediaGetResponse>>> Get()
        {
            List<Error> errors = new();

            try
            {
                var wholesaleId = await _wholesaleApp.GetWholesaleId();
                if (wholesaleId.Data == null)
                {
                    Log(LogLevels.Fatal, $"MediaAppService.Get Error {wholesaleId.Error[0].ErrorDesc}.({wholesaleId.Error[0].ErrorCode})");
                    errors.Add(new Error() { ErrorCode = wholesaleId.Error[0].ErrorCode, ErrorDesc = wholesaleId.Error[0].ErrorDesc });
                    return new ResponseBase<IList<MediaGetResponse>>(false, wholesaleId.Error[0].ErrorDesc, errors);
                }
                var list = await _context.Media.Where(x => x.WholesaleId == wholesaleId.Data.WholesaleId).ToListAsync();
                if (list.Count == 0)
                {
                    Log(LogLevels.Fatal, $"MediaAppService.Get {ErrorCode.EmptyData}.({(int)NErrorCode.EmptyData})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.EmptyData, ErrorDesc = ErrorCode.EmptyData });
                    return new ResponseBase<IList<MediaGetResponse>>(false, ErrorCode.EmptyData, errors);
                }
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();
                IList<MediaGetResponse> ResponseList = new List<MediaGetResponse>();
                foreach (var num in list)
                {
                    ResponseList.Add(new MediaGetResponse()
                    {
                        Id = num.Id,
                        OrderId = num.OrderId,
                        WholesaleImage = num.WholesaleImage,
                    });
                }

                Log(LogLevels.Fatal, $"MediaAppService.Get Media Got Successfully.");

                return new ResponseBase<IList<MediaGetResponse>>(ResponseList);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"MediaAppService.Get\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<IList<MediaGetResponse>>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<MediaModifyActivateResponse>> ModifyActivate(MediaModifyActivateRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var UserInfo = _helperApp.GetClaim();
                if (UserInfo == null)
                {
                    Log(LogLevels.Fatal, $"MediaAppService.ModifyActivate Error {ErrorCode.TokenExpired}.({(int)NErrorCode.TokenExpired})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.TokenExpired, ErrorDesc = ErrorCode.TokenExpired });
                    return new ResponseBase<MediaModifyActivateResponse>(false, ErrorCode.TokenExpired, errors);
                }
                var list = await _context.Media.Where(x => x.Id == obj.Id && x.IsActive == false).ToListAsync();
                if (list.Count == 1)
                {
                    list[0].IsActive = true;
                    list[0].ConfirmDate = DateTimeUtil.MiladiSysStandardDateNow();
                    list[0].ConfirmTime = DateTimeUtil.SysStandardTimeNow();
                    list[0].ConfirmDateint = int.Parse(DateTimeUtil.MiladiSysStandardDateNow(false));
                    list[0].ConfirmTimeint = int.Parse(DateTimeUtil.SysStandardTimeNow(false));
                    list[0].ConfirmUsername = UserInfo.Username;
                    _context.Media.Update(list[0]);
                }
                else
                {
                    Log(LogLevels.Fatal, $"MediaAppService.ModifyActivate Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<MediaModifyActivateResponse>(false, ErrorCode.RecordNotFound, errors);
                }
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"MediaAppService.ModifyActivate Media Activated Successfully.");
                return new ResponseBase<MediaModifyActivateResponse>(new MediaModifyActivateResponse());
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"MediaAppService.ModifyActivate\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<MediaModifyActivateResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<MediaModifyDeActivateResponse>> ModifyDeActivate(MediaModifyDeActivateRequest obj)
        {
            List<Error> errors = new();
            try
            {

                var list = await _context.Media.Where(x => x.Id == obj.Id && x.IsActive == true).ToListAsync();
                if (list.Count == 1)
                {
                    list[0].IsActive = false;
                    _context.Media.Update(list[0]);
                }
                else
                {
                    Log(LogLevels.Fatal, $"MediaAppService.ModifyDeActivate Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<MediaModifyDeActivateResponse>(false, ErrorCode.RecordNotFound, errors);
                }
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"MediaAppService.ModifyDeActivate Media DeActivated Successfully.");
                return new ResponseBase<MediaModifyDeActivateResponse>(new MediaModifyDeActivateResponse());
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"MediaAppService.ModifyDeActivate\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<MediaModifyDeActivateResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<IList<MediaDeActivatedListResponse>>> DeactivatedList()
        {
            List<Error> errors = new();

            try
            {
                var list = await _context.Media.Where(x => x.IsActive == false).ToListAsync();
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();
                IList<MediaDeActivatedListResponse> ResponseList = new List<MediaDeActivatedListResponse>();
                foreach (var num in list)
                {
                    ResponseList.Add(new MediaDeActivatedListResponse()
                    {
                        WholesaleId = num.Id,
                        ConfirmDate = num.ConfirmDate,
                        ConfirmDateint = num.ConfirmDateint,
                        ConfirmTime = num.ConfirmTime,
                        ConfirmTimeint = num.ConfirmTimeint,
                        ConfirmUsername = num.ConfirmUsername,
                        Id = num.Id,
                        IsActive = num.IsActive,
                        RecordDateint = num.RecordDateint,
                        RecordDate = num.RecordDate,
                        RecordTime = num.RecordTime,
                        RecordTimeint = num.RecordTimeint,
                        RecordUsername = num.RecordUsername,
                        OrderId = num.OrderId,
                        WholesaleImage = num.WholesaleImage
                    });
                }
                Log(LogLevels.Fatal, $"SaleInformationAppService.DeactivateList Media List Successfully.");
                return new ResponseBase<IList<MediaDeActivatedListResponse>>(ResponseList);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"SaleInformationAppService.DeactivateList\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<IList<MediaDeActivatedListResponse>>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<IList<MediaActivatedListResponse>>> ActivatedList()
        {
            List<Error> errors = new();

            try
            {
                var list = await _context.Media.Where(x => x.IsActive == true).ToListAsync();
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();
                IList<MediaActivatedListResponse> ResponseList = new List<MediaActivatedListResponse>();
                foreach (var num in list)
                {
                    ResponseList.Add(new MediaActivatedListResponse()
                    {
                        WholesaleId = num.Id,
                        ConfirmDate = num.ConfirmDate,
                        ConfirmDateint = num.ConfirmDateint,
                        ConfirmTime = num.ConfirmTime,
                        ConfirmTimeint = num.ConfirmTimeint,
                        ConfirmUsername = num.ConfirmUsername,
                        Id = num.Id,
                        IsActive = num.IsActive,
                        RecordDateint = num.RecordDateint,
                        RecordDate = num.RecordDate,
                        RecordTime = num.RecordTime,
                        RecordTimeint = num.RecordTimeint,
                        RecordUsername = num.RecordUsername,
                        OrderId = num.OrderId,
                        WholesaleImage = num.WholesaleImage
                    });
                }

                Log(LogLevels.Fatal, $"SaleInformationAppService.ActivateList Media List Successfully.");
                return new ResponseBase<IList<MediaActivatedListResponse>>(ResponseList);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"SaleInformationAppService.ActivateList\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<IList<MediaActivatedListResponse>>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
    }
}
