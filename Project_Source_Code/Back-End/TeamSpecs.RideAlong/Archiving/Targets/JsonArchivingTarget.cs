using System.IO.Compression;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services.FormatCompressorService;

namespace TeamSpecs.RideAlong.Archiving
{
    public class JsonArchivingTarget : IArchivingTarget
    {
        private readonly IFormatCompressorService _formatCompressorService;
        private readonly string _targetPath;
        private readonly string _targetZipFileName;
        
        public JsonArchivingTarget(string targetPath, string targetZipFileName, IFormatCompressorService formatCompressorService)
        {
            _targetPath = targetPath;
            _targetZipFileName = targetZipFileName;
            _formatCompressorService = formatCompressorService; 
        }        
        public IResponse GetLogs(DateTime beforeDate, DateTime? butNotBefore)
        {
            // Get Logs
            List<ILog>? logs = GetFromCompressed<ILog>();
            if (logs is null)
            {
                IResponse failResponse = new Response();
                failResponse.ErrorMessage = "Could not get logs";
                return failResponse;
            }
            
            // Filter Logs
            foreach (var log in logs)
            {
                if (log.LogTime > beforeDate)
                {
                    logs.Remove(log);
                }
                if(butNotBefore is not null && log.LogTime < butNotBefore)
                {
                    logs.Remove(log);
                }
            }

            // Return Logs
            IResponse successResponse = new Response();
            successResponse.ReturnValue = new List<object>();
            foreach (var log in logs)
            {
                successResponse.ReturnValue.Add(log);
            }
            return successResponse;
        }
        public IResponse SetLogs(List<ILog> logs)
        {
            // Get existing logs
            IResponse getLogs = GetLogs(DateTime.Now, null);
            
            if (getLogs.HasError == true || getLogs.ReturnValue is null)
            {
                return new Response();
            }
            List<ILog> existingLogs = (List<ILog>)getLogs.ReturnValue;

            // Append new logs to old logs
            foreach (var log in logs)
            {
                existingLogs.Add(log);
            }

            // Store all
            bool result = SendToCompressed(logs);
            IResponse response = new Response();
            response.HasError = !result;
            if (result == false)
            {
                response.ErrorMessage = "Could Not write to file";
            }
            return response;
        }
        private bool SendToCompressed<T>(List<T> logs)
        {
            byte[] zipFileBytes;
            #region Try Compression
            IResponse tryCompressionResponse = _formatCompressorService.Compress<T>(logs);
            if (tryCompressionResponse.HasError || tryCompressionResponse.ReturnValue is null || tryCompressionResponse.ReturnValue.First() is null)
            {
                return false;
            }
            else
            {
                zipFileBytes = (byte[])tryCompressionResponse.ReturnValue.First();
            }
            #endregion

            #region Try Creating File
            try
            {
                #region Write the zip file bytes to the given filepath
                // Create temp file to store values
                string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tempDir);
                string tempZipFile = Path.Combine(tempDir, "data.zip");
                File.WriteAllBytes(tempZipFile, zipFileBytes);

                // Create Path to target file
                string zipFile = Path.Combine(_targetPath, _targetZipFileName);

                // Replace target file with temp
                File.Copy(tempZipFile, zipFile, true);

                // Delete Temp
                File.Delete(tempDir);
                File.Delete(tempZipFile);
                #endregion

                #region Return Success Outcome
                return true;
                #endregion
            }
            #endregion

            #region Catch Errors
            catch
            {
                return false;
            }
            #endregion
        }
        private List<T>? GetFromCompressed<T>()
        {
            byte[] zipFileBytes;
            #region Get zip file bytes
            try
            {
                string zipFilePath = Path.Combine(_targetPath, _targetZipFileName);
                zipFileBytes = File.ReadAllBytes(zipFilePath);
            } catch {
                return null;
            }
            #endregion

            #region Try Decompressing
            IResponse extractResponse = _formatCompressorService.Extract<T>(zipFileBytes);
            if (extractResponse.HasError || extractResponse.ReturnValue is null)
                return null;
            List<T> objects = new List<T>();
            foreach (T obj in extractResponse.ReturnValue)
            {
                objects.Add(obj!);
            }
            return objects;
            #endregion
        }
    }
}
