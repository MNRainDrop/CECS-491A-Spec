using System.IO.Compression;
using System.Text.Json;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services.FormatCompressorService
{
    public class JsonCompressorService : IFormatCompressorService
    {
        public IResponse Compress<T>(List<T> objects)
        {
            IResponse response = new Response();
            try
            {
                string json = JsonSerializer.Serialize(objects);

                #region Create a temporary directory to hold the JSON file
                string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tempDir);
                #endregion

                #region Write the JSON to a file
                string jsonFilePath = Path.Combine(tempDir, "data.json");
                File.WriteAllText(jsonFilePath, json);
                #endregion

                #region Create a zip file
                string zipFilePath = Path.Combine(Path.GetTempPath(), "data.zip");
                ZipFile.CreateFromDirectory(tempDir, zipFilePath);
                #endregion

                #region Read the zip file as bytes
                byte[] zipFileBytes = File.ReadAllBytes(zipFilePath);
                #endregion

                #region Clean up temporary files
                File.Delete(jsonFilePath);
                File.Delete(zipFilePath);
                Directory.Delete(tempDir);
                #endregion

                #region Return the zip file bytes in response
                response.HasError = false;
                response.ReturnValue = new List<object>() { zipFileBytes };
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Catch Errors
                response.ErrorMessage = ex.Message;
                return response;
                #endregion
            }
        }

        public IResponse Extract<T>(byte[] zipFileBytes)
        {
            IResponse response = new Response();
            try
            {
                #region Create a temporary directory to extract the zip file
                string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tempDir);
                #endregion

                #region Write the zip file bytes to a temporary file
                string zipFilePath = Path.Combine(tempDir, "data.zip");
                File.WriteAllBytes(zipFilePath, zipFileBytes);
                #endregion

                #region Extract the contents of the zip file
                string extractDir = Path.Combine(tempDir, "extracted");
                ZipFile.ExtractToDirectory(zipFilePath, extractDir);
                #endregion

                #region Read the JSON file from the extracted directory
                string jsonFilePath = Path.Combine(extractDir, "data.json");
                string json = File.ReadAllText(jsonFilePath);
                #endregion

                #region Deserialize JSON to list of objects
                List<T> objects = JsonSerializer.Deserialize<List<T>>(json)!;
                #endregion

                #region Clean up temporary files
                File.Delete(zipFilePath);
                Directory.Delete(tempDir, true);
                #endregion

                #region Return the list of objects
                response.HasError = false;
                response.ReturnValue = new List<object>() { };
                foreach (T obj in objects)
                {
                    response.ReturnValue.Add(obj!);
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Catch Errors
                response.ErrorMessage = ex.Message;
                return response;
                #endregion
            }
        }
        public IResponse CreateFile<T>(byte[] zipFileBytes, string filepath, string filename)
        {
            IResponse response = new Response();
            try
            {
                #region Create Directory Path
                string newFile = Path.Combine(filepath, filename);
                #endregion

                #region Write the zip file bytes to the given filepath
                File.WriteAllBytes(newFile, zipFileBytes);
                #endregion

                #region Return Success Outcome
                response.HasError = false;
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Catch Errors
                response.ErrorMessage = ex.Message;
                return response;
                #endregion
            }
        }
    }
}
