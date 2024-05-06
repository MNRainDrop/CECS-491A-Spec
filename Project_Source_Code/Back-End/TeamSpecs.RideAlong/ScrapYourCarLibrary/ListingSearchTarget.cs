using Microsoft.Extensions.Logging;
using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.ScrapYourCarLibrary
{
    public class ListingSearchTarget : IListingSearchTarget
    {
        private IGenericDAO _dao;
        private ILogService _logger;
        public ListingSearchTarget(IGenericDAO dao, ILogService logger)
        {
            _dao = dao;
            _logger = logger;
        }
        /// <summary>
        /// Utility function used to avoid repeated code
        /// Simply takes in an exception as a parameter, 
        /// then generates both a log and a tailored response object to the error recorded in the exeption
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Response object, with error message based on exception</returns>
        private IResponse createErrorResponse(Exception ex)
        {
            IResponse errorResponse = new Response();
            errorResponse.HasError = true;
            errorResponse.ErrorMessage = "Error retrieving user data: " + ex.Message;
            _logger.CreateLogAsync("Error", "Data Store", errorResponse.ErrorMessage, null);
            return errorResponse;
        }
        public IResponse GetListingsBySearch(ISearchParameters searchBy)
        {
            throw new NotImplementedException();
        }
    }
}
