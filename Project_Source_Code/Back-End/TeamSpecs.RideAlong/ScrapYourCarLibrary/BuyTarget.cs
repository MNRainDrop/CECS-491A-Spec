using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.ScrapYourCarLibrary
{
    public class BuyTarget : IBuyTarget
    {
        private IGenericDAO _dao;
        private ILogService _logger;
        public BuyTarget(IGenericDAO dao, ILogService logger)
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
        public IResponse GetMatchingBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse GetSentBuyRequests(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse GetToMeBuyRequests(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse RemoveBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse SetBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse UpdateBuyRequest(IBuyRequest updatedRequest)
        {
            throw new NotImplementedException();
        }
    }
}
