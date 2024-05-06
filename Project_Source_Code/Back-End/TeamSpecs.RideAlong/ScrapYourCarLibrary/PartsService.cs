using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary
{
    public class PartsService : IPartsService
    {
        private IPartsTarget _target;
        public PartsService(IPartsTarget target)
        {
            _target = target;
        }

        private IResponse createSuccessResponse(object? item)
        {
            // Pack it up and ship it
            IResponse successResponse = new Response();
            successResponse.HasError = false;
            if (item is not null)
            {
                successResponse.ReturnValue = new List<object>() { item };
            }
            return successResponse;
        }

        /// <summary>
        /// Used to insert a list of parts all at once
        /// If any one part fails, it will be added to return value
        /// If parts failed, IResponse.hasError == True
        /// </summary>
        /// <param name="parts"></param>
        /// <returns>Either success outcome, or fail outcome with list of failed parts/</returns>
        public IResponse CreateParts(List<ICarPart> parts)
        {
            IResponse response = new Response();
            response.HasError = false;
            response.ReturnValue = new List<object>();

            IResponse checkIfExistResponse = _target.GetMatchingParts(parts);
            if (checkIfExistResponse.ReturnValue is not null && checkIfExistResponse.ReturnValue.Count > 0)
            {
                response.HasError = true;
                response.ReturnValue = checkIfExistResponse.ReturnValue;
                response.ErrorMessage += "One or more Parts Already Exist\n";
            }
            else
            {
                foreach (var part in parts)
                {
                    IResponse setCarResponse = _target.SetCarPart(part);
                    if (setCarResponse.HasError == true)
                    {
                        response.HasError = true;
                        response.ReturnValue.Add(part);
                        response.ErrorMessage += "Part Failed Insert: " + setCarResponse.ErrorMessage + "\n";
                        continue;
                    }
                }
            }
            if (response.HasError == false)
            {
                return createSuccessResponse(null);
            }
            return response;
        }

        public IResponse GetUserParts(long uid)
        {
            IResponse getUserPartsResponse = _target.GetUserParts(uid);
            if (getUserPartsResponse.HasError)
            {
                if (getUserPartsResponse.ErrorMessage is null)
                {
                    getUserPartsResponse.ErrorMessage = "Unknown error occurred at target layer or below";
                }
            }
            return getUserPartsResponse;
        }

        public IResponse GetMatchingParts(ICarPart part)
        {
            IResponse response = _target.GetMatchingParts(new List<ICarPart>() { part });
            if (response.HasError)
            {
                if (response.ErrorMessage is null)
                {
                    response.ErrorMessage = "Unknown error occurred at target layer or below";
                }
            }
            return response;
        }

        public IResponse RemoveParts(List<ICarPart> parts)
        {
            IResponse response = new Response();
            response.HasError = false;

            response.ReturnValue = new List<object>();
            foreach (var part in parts)
            {
                IResponse partsFromDBResponse = _target.GetMatchingParts(new List<ICarPart> { part });
                if (partsFromDBResponse.HasError == true || partsFromDBResponse.ReturnValue!.Count != 1)
                {
                    if (partsFromDBResponse.ErrorMessage == "No Rows Returned")
                    {
                        response.HasError = true;
                        response.ErrorMessage += "Part does not exist\n";
                        response.ReturnValue.Add(part);
                    }
                    else if (partsFromDBResponse.ErrorMessage is not null)
                    {
                        response.HasError = true;
                        return partsFromDBResponse;
                    }
                    else
                    {
                        response.HasError = true;
                        response.ErrorMessage = "Unknown error occurred at target layer or below";
                        return response;
                    }
                }

                ICarPart partToDelete = (ICarPart)partsFromDBResponse.ReturnValue!.First();

                IResponse targetResponse = _target.RemoveParts(partToDelete);
                if (targetResponse.HasError)
                {
                    response.HasError = true;
                    response.ErrorMessage += "Part could not be deleted: " + targetResponse.ErrorMessage + "\n";
                    response.ReturnValue.Add(partToDelete);
                }
            }
            return response;
        }

        public IResponse AddListingToPart(IListing listing)
        {

            ICarPart part = listing.part;
            IResponse validatePartResponse = _target.GetMatchingParts(new List<ICarPart>() { part });
            if (validatePartResponse.HasError || validatePartResponse.ReturnValue is null || validatePartResponse.ReturnValue.Count != 1)
            {
                validatePartResponse.HasError = true;
                if (validatePartResponse.ErrorMessage is null)
                {
                    validatePartResponse.ErrorMessage = "An unknown error occurred at target layer or below";
                }
                return validatePartResponse;
            }

            ICarPart validPart = (CarPart)validatePartResponse.ReturnValue.First();
            listing.part = validPart;

            IResponse tryAddResponse = _target.SetListing(listing);
            if (tryAddResponse.HasError)
            {
                if (tryAddResponse.ErrorMessage is null)
                {
                    tryAddResponse.ErrorMessage = "An unknown error occurred at target layer or below";
                }
            }

            return tryAddResponse;
        }
        /// <summary>
        /// Will Return all the listings for parts that have listings
        /// will ignore parts that have no listings
        /// It is possible no listings will be returned, if none of the parts passed in have listings
        /// </summary>
        /// <param name="parts"></param>
        /// <returns>List of listings</returns>
        public IResponse RetrievePartListings(List<ICarPart> parts)
        {
            IResponse response = new Response();
            response.ReturnValue = new List<object>();
            foreach (ICarPart part in parts)
            {
                IResponse getPartListing = _target.GetPartListing(part);
                #region error checking
                if (getPartListing.HasError == true)
                {
                    if (getPartListing.ErrorMessage == "No Rows Returned")
                    {
                        continue;
                    }
                    else
                    {
                        if (getPartListing.ErrorMessage is null)
                        {
                            getPartListing.ErrorMessage = "Unknown error occurred at target layer or below";
                        }
                        return getPartListing;
                    }
                }
                #endregion

                response.ReturnValue.Add(getPartListing.ReturnValue!.First());
            }
            response.HasError = false;
            return response;
        }
        /// <summary>
        /// Will Return all the users listings
        /// No listings will be returnd if the user does not have any
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>List of listings</returns>
        public IResponse RetrieveUserListings(long uid)
        {
            IResponse response = new Response();

            IResponse getUserPartsResponse = _target.GetUserParts(uid);
            if (getUserPartsResponse.HasError)
            {
                if (getUserPartsResponse.ErrorMessage is null)
                {
                    getUserPartsResponse.ErrorMessage = "Unknown error occurred at target layer or below";
                }
                return getUserPartsResponse;
            }

            response = RetrievePartListings((List<ICarPart>)getUserPartsResponse.ReturnValue!);
            if (response.HasError == true)
            {
                if (response.ErrorMessage is null)
                {
                    response.ErrorMessage = "Unknown Error occurred while retreiving listings";
                }
            }
            return response;
        }

        public IResponse DeleteListings(List<IListing> listings)
        {
            IResponse response = new Response();
            response.HasError = false;
            response.ReturnValue = new List<object>();
            foreach (IListing listing in listings)
            {
                IListing validListing;
                #region validate listing exists
                var validateListing = _target.GetPartListing(listing.part);
                if (validateListing.HasError == true)
                {
                    response.HasError = true;
                    response.ErrorMessage += "Listing could not be found: " + validateListing.ErrorMessage;
                    response.ReturnValue.Add(listing);
                    continue;
                }
                validListing = (Listing)validateListing.ReturnValue!.First();
                #endregion

                #region try Delete
                var tryDelete = _target.RemoveListing(validListing.part);
                if (tryDelete.HasError == true)
                {
                    response.HasError = true;
                    response.ErrorMessage += "Listing could not be deleted: " + tryDelete.ErrorMessage;
                    response.ReturnValue.Add(listing);
                    continue;
                }
                #endregion
            }
            return response;
        }

        public IResponse UpdateListing(IListing listing)
        {
            IResponse response = new Response();

            IListing validListing;
            #region validate listing exists
            var validateListing = _target.GetPartListing(listing.part);
            if (validateListing.HasError == true)
            {
                response.ErrorMessage += "Listing could not be found: " + validateListing.ErrorMessage;
                return response;
            }
            validListing = (IListing)validateListing.ReturnValue!.First();
            #endregion

            #region try update
            var tryUpdate = _target.AmendListing(validListing);
            if (tryUpdate.HasError == true)
            {
                response.HasError = true;
                response.ErrorMessage += "Listing could not be updated: " + tryUpdate.ErrorMessage;
            }
            #endregion
            return response;
        }
    }
}
