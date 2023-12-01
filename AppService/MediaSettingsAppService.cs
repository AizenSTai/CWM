using Microsis.CWM.Common;
using Microsis.CWM.DAL;
using Microsis.CWM.Dto.Basic;
using Microsis.CWM.Dto.MediaSettings;
using Microsis.CWM.Model;
using Microsis.CWM.Util;
using Microsoft.EntityFrameworkCore;
using static Microsis.CWM.Util.LoggerProxy;

namespace Microsis.CWM.AppService
{
    public interface IMediaSettingsAppService
    {
        Task<ResponseBase<MediaSettingsNewResponse>> New(MediaSettingsNewRequest obj);
        Task<ResponseBase<MediaSettingsModifyResponse>> Modify(MediaSettingsModifyRequest obj);
        Task<ResponseBase<MediaSettingsDeleteResponse>> Delete();
        Task<ResponseBase<MediaSettingsGetResponse>> Get();
    }
    public class MediaSettingsAppService : IMediaSettingsAppService
    {
        private readonly ICwmCtx _context;
        private readonly IHelperAppService _helperApp;
        public MediaSettingsAppService(ICwmCtx context, IHelperAppService helperApp)
        {
            _context = context;
            _helperApp = helperApp;
        }
        public async Task<ResponseBase<MediaSettingsNewResponse>> New(MediaSettingsNewRequest obj)
        {
            List<Error> errors = new();
            try
            {
                //var userInfo = _helperApp.GetClaim();
                var UserInfo = _helperApp.GetClaim();
                if (UserInfo == null)
                {
                    Log(LogLevels.Fatal, $"MediaSettingsAppService.New Error {ErrorCode.TokenExpired}.({(int)NErrorCode.TokenExpired})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.TokenExpired, ErrorDesc = ErrorCode.TokenExpired });
                    return new ResponseBase<MediaSettingsNewResponse>(false, ErrorCode.TokenExpired, errors);
                }
                var isAvailble = await _context.mediaSettings.ToListAsync();

                if (isAvailble.Any())
                {
                    Log(LogLevels.Fatal, $"MediaSettingsAppService.New Error {ErrorCode.MaximumRecordsReached}.({(int)NErrorCode.MaximumRecordsReached})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.MaximumRecordsReached, ErrorDesc = ErrorCode.MaximumRecordsReached });
                    return new ResponseBase<MediaSettingsNewResponse>(false, ErrorCode.MaximumRecordsReached, errors);
                }

                MediaSettings mediaAdd = new MediaSettings()
                {
                    PicMaxQuantity = obj.PicMaxQuantity,
                    PicMaxSize = obj.PicMaxSize,
                    RecordDate = DateTimeUtil.MiladiSysStandardDateNow(),
                    RecordTime = DateTimeUtil.SysStandardTimeNow(),
                    RecordUsername = UserInfo.Username
                };

                await _context.mediaSettings.AddAsync(mediaAdd);
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"MediaSettingsAppService.New MediaSettings Added Successfully.");

                return new ResponseBase<MediaSettingsNewResponse>(new MediaSettingsNewResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"MediaSettingsAppService.New\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<MediaSettingsNewResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<MediaSettingsModifyResponse>> Modify(MediaSettingsModifyRequest obj)
        {
            List<Error> errors = new();
            try
            {
                var UserInfo = _helperApp.GetClaim();
                if (UserInfo == null)
                {
                    Log(LogLevels.Fatal, $"MediaSettingsAppService.Modify Error {ErrorCode.TokenExpired}.({(int)NErrorCode.TokenExpired})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.TokenExpired, ErrorDesc = ErrorCode.TokenExpired });
                    return new ResponseBase<MediaSettingsModifyResponse>(false, ErrorCode.TokenExpired, errors);
                }
                var isAvailble = await _context.mediaSettings.ToListAsync();

                if (!isAvailble.Any())
                {
                    Log(LogLevels.Fatal, $"MediaSettingsAppService.Modify Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<MediaSettingsModifyResponse>(false, ErrorCode.RecordNotFound, errors);
                }
                else if(isAvailble.Count > 1)
                {
                    Log(LogLevels.Fatal, $"MediaSettingsAppService.Modify Error {ErrorCode.MultiRecordsFound}.({(int)NErrorCode.MultiRecordsFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.MultiRecordsFound, ErrorDesc = ErrorCode.MultiRecordsFound });
                    return new ResponseBase<MediaSettingsModifyResponse>(false, ErrorCode.MultiRecordsFound, errors);
                }

                isAvailble[0].PicMaxSize = obj.PicMaxSize;
                isAvailble[0].PicMaxQuantity = obj.PicMaxQuantity;
                isAvailble[0].RecordUsername = UserInfo.Username;
                isAvailble[0].RecordTime = DateTimeUtil.SysStandardTimeNow();
                isAvailble[0].RecordDate = DateTimeUtil.MiladiSysStandardDateNow();

                _context.mediaSettings.Update(isAvailble[0]);
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"MediaSettingsAppService.Modify MediaSettings Updated Successfully.");

                return new ResponseBase<MediaSettingsModifyResponse>(new MediaSettingsModifyResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"MediaSettingsAppService.Modify\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<MediaSettingsModifyResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<MediaSettingsDeleteResponse>> Delete()
        {
            List<Error> errors = new();
            try
            {

                var isAvailble = await _context.mediaSettings.ToListAsync();

                if (!isAvailble.Any())
                {
                    Log(LogLevels.Fatal, $"MediaSettingsAppService.Delete Error {ErrorCode.RecordNotFound}.({(int)NErrorCode.RecordNotFound})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.RecordNotFound, ErrorDesc = ErrorCode.RecordNotFound });
                    return new ResponseBase<MediaSettingsDeleteResponse>(false, ErrorCode.RecordNotFound, errors);
                }                
                foreach(var num in isAvailble)
                {
                _context.mediaSettings.Remove(num);
                }
                await _context.SaveChangesAsync();

                Log(LogLevels.Fatal, $"MediaSettingsAppService.Delete MediaSettings Deleted Successfully.");

                return new ResponseBase<MediaSettingsDeleteResponse>(new MediaSettingsDeleteResponse() { });
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"MediaSettingsAppService.Deleted\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<MediaSettingsDeleteResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
        public async Task<ResponseBase<MediaSettingsGetResponse>> Get()
        {
            List<Error> errors = new();

            try
            {
                var list = await _context.mediaSettings.ToListAsync();
                if(list.Count == 0)
                {
                    Log(LogLevels.Fatal, $"MediaSettingsAppService.Get {ErrorCode.EmptyData}.({(int)NErrorCode.EmptyData})");
                    errors.Add(new Error() { ErrorCode = (int)NErrorCode.EmptyData, ErrorDesc = ErrorCode.EmptyData });
                    return new ResponseBase<MediaSettingsGetResponse>(false, ErrorCode.EmptyData, errors);
                }
                //IList<ShopGetResponse> ResponseList = new List<ShopGetResponse>();
                MediaSettingsGetResponse ResponseList = new MediaSettingsGetResponse() { PicMaxQuantity = list[0].PicMaxQuantity, PicMaxSize = list[0].PicMaxSize,RecordDate = list[0].RecordDate,RecordTime = list[0].RecordTime,RecordUsername = list[0].RecordUsername };

                Log(LogLevels.Fatal, $"MediaSettingsAppService.Get MediaSettings Got Successfully.");

                return new ResponseBase<MediaSettingsGetResponse>(ResponseList);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Fatal, $"MediaSettingsAppService.Get\n{ex.Message}", ex);
                errors.Add(new Error() { ErrorCode = (int)NErrorCode.Exc, ErrorDesc = ErrorCode.Exc });
                return new ResponseBase<MediaSettingsGetResponse>(false, ErrorCode.Exc.ToString(), errors);
            }
        }
    }
}
