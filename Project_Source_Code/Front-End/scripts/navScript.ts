// Note, doing the import function breaks the files functionality. I am unsure why this is the case, but I have not found a solution
import { fetchWithTokens, incrementPages } from "./UtilityScripts";
// function fetchWithTokens(url: string, method: string, body: any) {  
//     // Fetch the tokens from session storage
//     const idToken = sessionStorage.getItem('IDToken');
//     const accessToken = sessionStorage.getItem('AccessToken');
//     const refreshToken = sessionStorage.getItem('RefreshToken');
//     // Create the headers object with the tokens
//     const headers : HeadersInit = {
//         'Authorization': `Bearer ${idToken}` ?? '',
//         'X-Access-Token': accessToken ?? '',
//         'X-Refresh-Token': refreshToken ?? '',
//         'Content-Type': 'application/json' // Set the Content-Type header for the request body
//     };
//     return fetch(url, {
//         method: method,
//         headers: headers,
//         body: JSON.stringify(body)
//     });
// };

document.addEventListener("DOMContentLoaded", () => {
    //const rentalFleetNav = document.getElementById("rental-fleet-view");
    //const inventoryManagementNav = document.getElementById("inventory-management-view");
    //const vehicleMarketPlaceNav = document.getElementById("vehicle-marketplace-view");
    
    const logOutNav = document.getElementById("log-out");
    logOutNav!.addEventListener("click", logOut)
    const refreshPermissionsNav = document.getElementById("refresh-permissions");
    refreshPermissionsNav?.addEventListener("click", refreshUserTokens)
    const vehicleProfileNav = document.getElementById("vehicle-profile-view");
    vehicleProfileNav!.addEventListener("click", doAThing => {alert("beep boop")});
    const rentalFleetNav = document.getElementById("rental-fleet-view");
    rentalFleetNav!.addEventListener("click", generateRentalDefaultView);
});

function generateRentalDefaultView () {
    var permissionGranted;
    fetchWithTokens('http://localhost:8081/Rentals/GetAuthStatus', 'POST', '')
    .then(function (response) {
        if(response.status == 204){
            alert("permission granted!!!")
            const dynamicContent = document.querySelector(".dynamic-content");
            dynamicContent!.innerHTML = `
            <div id="fleet-button-div">
                <button id="submit-username">Submit</button>
            </div>
            `;
        } else {
            alert("Permission to view denied");
        }
    }).catch(error => {
        permissionGranted = false;
        alert(error);
    });
};

function logOut (){
    // Remove the tokens from storage
    sessionStorage.removeItem ('IDToken');
    sessionStorage.removeItem ('AccessToken');
    sessionStorage.removeItem ('RefreshToken');

    // Give user confirmation they've logged out
    const dynamicContent = document.querySelector(".dynamic-content");
    dynamicContent!.innerHTML = `
    <div id="logout-view">
        Logged Out!!!
    </div>
    `;
    
    // Lets the view show for 5 seconds, then reloads the page
    setTimeout(() => {
        location.reload();
    }, 5000 ); 
}
function refreshUserTokens(){
    fetchWithTokens('http://localhost:8080/Auth/refreshTokens', 'POST', '')
    .then(response => {
        if (response.ok){
            return response.json();
        } else {
            alert("Refresh Failed! " + response.statusText);
            return;
        }
    })
    .then (data => {
        try{
            sessionStorage.setItem ('IDToken', data.idToken);
            sessionStorage.setItem ('AccessToken' , data.accessToken);
            alert("Your session has been refreshed!!!!");    
        } catch {};
        
    })
    .catch(error => {
        alert("An error occurred while Refreshing your session: " + error);
    });
}