using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services.FormatCompressorService;

namespace TeamSpecs.RideAlong.Archiving
{
    public class ArchivingService : IArchivingService
    {
        IFormatCompressorService _compressor;
        IArchivingTarget _aTarget;
        public ArchivingService(IFormatCompressorService compressor, IArchivingTarget aTarget)
        {
            _compressor = compressor;
            _aTarget = aTarget;
        }

        public IResponse Archive(byte[] zipFileBytes)
        {
            List<ILog> newLogs = new List<ILog>();
            #region Extract New Logs
            IResponse compressorResponse = _compressor.Extract<ILog>(zipFileBytes);
            if (compressorResponse.HasError)
            {
                return compressorResponse;
            }
            else if (compressorResponse.ReturnValue is null || compressorResponse.ReturnValue.First() is null)
            {
                IResponse compressorFailedResponse = new Response();
                compressorFailedResponse.HasError = true;
                compressorFailedResponse.ErrorMessage = "No Logs received";
                return compressorFailedResponse;
            }
            foreach (var log in compressorResponse.ReturnValue)
            {
                newLogs.Add((ILog)log);
            }
            #endregion

            #region Set All Logs
            IResponse setLogsResponse = _aTarget.SetLogs(newLogs);
            if (setLogsResponse.HasError)
                return setLogsResponse;
            #endregion

            IResponse successResponse = new Response();
            successResponse.HasError = false;
            return successResponse;
        }

        public IResponse GetArchive(DateTime before, DateTime? butNotBefore = null)
        {
            List<ILog> logs = new List<ILog>();
            #region Get Logs from target
            IResponse targetResponse = _aTarget.GetLogs(before, butNotBefore);
            if (targetResponse.HasError)
            {
                return targetResponse;
            }
            else if (targetResponse.ReturnValue is null || targetResponse.ReturnValue.First() is null)
            {
                IResponse noLogsReturnedResponse = new Response();
                noLogsReturnedResponse.HasError = true;
                noLogsReturnedResponse.ErrorMessage = "No Logs Returned";
                return noLogsReturnedResponse;
            }
            foreach (var log in targetResponse.ReturnValue)
            {
                logs.Add((ILog)log);
            }
            #endregion

            byte[] file;
            #region Get zip From Compressor
            IResponse compressorResponse = _compressor.Compress(logs);
            if (compressorResponse.HasError)
            {
                return compressorResponse;
            }
            else if (compressorResponse.ReturnValue is null || compressorResponse.ReturnValue.First() is null)
            {
                IResponse compressorFailedResponse = new Response();
                compressorFailedResponse.HasError = true;
                compressorFailedResponse.ErrorMessage = "No file bytes received";
                return compressorFailedResponse;
            }
            file = (byte[])compressorResponse.ReturnValue.First();
            #endregion

            IResponse successResponse = new Response();
            successResponse.HasError = false;
            successResponse.ReturnValue = new List<object>() { file };
            return successResponse;
        }

    }
}
