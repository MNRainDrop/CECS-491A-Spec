const generateViewButton = document.getElementById("vehicle-marketplace-view");
//generateViewButton.addEventListener("click", generateMainView);

function exrtactData(jsonData) {
    //var data = JSON.parse(jsonData);
    var dynamicContent = document.querySelector(".dynamic-content");
    var temp = "https://example.com";
    var html = "<div class=container>";
    jsonData.forEach(function(data) {
        // Access values from each object
        var VIN = data.vin;
        var make = data.make;
        var model = data.model;
        
        // Create HTML content with the extracted values
        html += "<a href=temp>";
        html += "<div class=vehicle-listings>";
        html += '<img src=' + "VPM-resource/car.png" + '>'; 
        html += '<h2> Vehicle' + '</h2>';
        html += '<p>VIN: ' + VIN + '</p>';
        html += '<p>Make: ' + make + '</p>';
        html += '<p>Model: ' + model + '</p>';
        html += '</div>'; 
        html += "</a>";

    });   
    html += "</div>"; 
    dynamicContent.innerHTML = html;

   
}

function displayMarketplace() {
    fetchWithTokens('http://localhost:5104/VehicleMarketplace/GetVehicleMarketplace', 'GET','')
    .then(function (response) {
        if (response.status == 200) { 
            response.json()
            .then(function(data) {
                // Check if data is an array or object
                if (Array.isArray(data) && data.length > 0) {
                    //alert("VP's retrieved");
                    exrtactData(data);
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

