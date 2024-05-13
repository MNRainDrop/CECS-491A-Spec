using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services.FormatCompressorService
{
    public interface IFormatCompressorService
    {
        /// <summary>
        /// Converts a list of objects into a byte[] that represents a zip folder<br>
        /// The zip folder contains a file with the objects converted to the format specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <returns>IResponse containing byte[] representing compressed file</returns>
        IResponse Compress<T>(List<T> objects);

        /// <summary>
        /// Converts a zipped file into the specified object</br>
        /// Ensure you use the same format you used for compression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="zipFileBytes"></param>
        /// <returns>IResponse Containing list of "T" objects</returns>
        IResponse Extract<T>(byte[] zipFileBytes);
    }
}
