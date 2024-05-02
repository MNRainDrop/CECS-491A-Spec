'use strict';

(function () {
    function extractData(jsonData,make, model, year) {
        //var data = JSON.parse(jsonData);
        var dynamicContent = document.querySelector(".dynamic-content");
        //var temp = "http://localhost:3000/";
        var html = "<div class=DYCcontainer>";
        jsonData.forEach(function(data) {
            // Access values from each object
            var charity = data.Name;
            var description = data.Description;
            var link = data.Link;
            
            // Create HTML content with the extracted values
            //html += "<a href='" + link + "' target='_blank'>";
            //html += "<a href='#' onclick='handleCharityClick(\"" + link + "\")'>"; // Call handleCharityClick function
            html += "<a href='#' onclick='handleCharityClick(\"" + link + "\", \"" + make + "\", \"" + model + "\", \"" + year + "\")'>";
            html += "<div class=charity-listings>";
            html += '<h2> Charity:' + charity+'</h2>';
            html += '<p>description: ' + description + '</p>';
            html += '</div>'; 
            html += "</a>";
    
        });   
        html += "</div>"; 
        dynamicContent.innerHTML = html;
       }
    
    function createDonateYourCarView(make, model, year){
        var permissionGranted;
        fetchWithTokens('http://localhost:5212/DonateYourCar/GetAuthStatus', 'POST','')
            .then(function (response) {
            if (response.status == 204) {
                var dynamicContent = document.querySelector(".dynamic-content");
                dynamicContent.innerHTML = "";
                // replace the parameter inside changeCSS() to the path of the css file you need
                changeCSS("styles/DYCstyles.css");
                fetchWithTokens('http://localhost:5212/DonateYourCar/RetrieveCharities', 'GET','')
                .then(function (response) {
                    if (response.status == 200) { 
                        response.json()
                        .then(function(data) {
                            // Check if data is an array or object
                            if (Array.isArray(data) && data.length > 0) {
                                
                                extractData(data,make, model, year);
                            } 
                        })
                        .catch(function (error) {
                            alert("Error parsing JSON: " + error);
                        });
                    } 
                    else {
                        // Handle response as string
                        response.text()
                        .then(function(text) {
                            alert(text); // Alert the string response
                            var dynamicContent = document.querySelector(".dynamic-content");
                            dynamicContent.innerHTML = text;
                        })
                        .catch(function (error) {
                            alert("Error reading response text: " + error);
                        });
                    }
                }); 
            }
            else {
                alert("Permission to view denied");
            }
        }).catch(function (error) {
            permissionGranted = false;
            alert(error);
        })
    }
    window.createDonateYourCarView = createDonateYourCarView;
    
    function handleCharityClick(link, make, model, year) {
        // Generate the dynamic link with the extracted values
        var dynamicLink = link + "?make=" + encodeURIComponent(make) +
        "&model=" + encodeURIComponent(model) +
        "&year=" + encodeURIComponent(year);
        
        // Open the dynamic link in a new tab
        window.open(dynamicLink, '_blank');
    }
})(window);