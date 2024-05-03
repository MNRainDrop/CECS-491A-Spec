// Note, doing the import function breaks the files functionality. I am unsure why this is the case, but I have not found a solution
//import { fetchWithTokens } from "./FetchWithTokens";
function generateVehicleProfileRetrieval() {
    fetchWithTokens('http://localhost:8082/CarHealthRating/RetrieveProfiles', 'POST', '')
    .then(function (response) {
        if (response.status == 200) { 
            response.json()
            .then(function(data) {
                // Check if data is an array or object
                if (Array.isArray(data) && data.length > 0) {
                    //alert("VP's retrieved");
                    parseAndDisplayData(data);
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

function parseAndDisplayData(data) {
    var dynamicContent = document.querySelector(".dynamic-content");
    var html = "<div>Vehicle Profiles</div>";
    html += "<select id='vehicleSelect'>";
    data.forEach(function(entry, index) {
        html += "<option value='" + index + "'>" + entry.join(", ") + "</option>";
    });
    html += "</select>";
    html += "<button id='calculateButton'>Calculate</button>";
    dynamicContent.innerHTML = html;

// Add event listener to Calculate button
document.getElementById("calculateButton").addEventListener("click", function() {
    var selectedOption = document.getElementById("vehicleSelect").options[document.getElementById("vehicleSelect").selectedIndex];
    var selectedContent = selectedOption.textContent || selectedOption.innerText;
    var indexOfComma = selectedContent.indexOf(",");
    var selectedVIN = selectedContent.substring(0 , indexOfComma).trim(); // Extract VIN
    //dynamicContent.innerHTML += selectedVIN; // Display VIN (optional)
    generateCarHealthRatingView(selectedVIN); // Pass VIN to function
});



}

function generateCarHealthRatingView(vin) {
    fetchWithTokens('http://localhost:8082/CarHealthRating/GetRanking', 'POST', vin)
        .then(function (response) {
            if (response.status == 200) 
            {
                response.json().then(function(data) {
                    if (true) 
                    {
                        var dynamicContent = document.querySelector(".dynamic-content");
                        
                        // Clear previous content of the specific div with class "clearable-div"
                        var clearableDiv = dynamicContent.querySelector(".clearable-div");

                        if (!clearableDiv) {
                            clearableDiv = document.createElement("div");
                            clearableDiv.classList.add("clearable-div");
                            dynamicContent.appendChild(clearableDiv);
                        } else {
                            // If clearableDiv exists and has content, clear it
                            if (clearableDiv.hasChildNodes()) {
                                clearableDiv.innerHTML = "";
                            }
                        }

                        // Add a line break
                        dynamicContent.appendChild(document.createElement("br"));

                        var integersArray = data[0];
                        var stringsArray = data[1];
                        var number = data[2];

                        // Sum up the integers array
                        var pointsEarned = integersArray.reduce((acc, cur) => acc + cur, 0);

                        // Create elements to display the information
                        var totalScoreElement = document.createElement("div");
                        totalScoreElement.textContent = "Total Score: " + pointsEarned + "/" + number;
                        clearableDiv.appendChild(totalScoreElement);

                        clearableDiv.appendChild(document.createElement("br"));
                        var maxScoreElement = document.createElement("div");
                        maxScoreElement.textContent = "Maximum Possible Points: " + number;
                        clearableDiv.appendChild(maxScoreElement);


                        // Add a line break
                        clearableDiv.appendChild(document.createElement("br"));

                        var pointsEarnedElement = document.createElement("div");
                        pointsEarnedElement.textContent = "Points Earned with each corresponding event:";
                        clearableDiv.appendChild(pointsEarnedElement);

                        // Add a line break
                        clearableDiv.appendChild(document.createElement("br"));

                        // Display points earned with each corresponding event
                        for (var i = 0; i < integersArray.length; i++) {
                            var eventElement = document.createElement("div");
                            eventElement.textContent = stringsArray[i] + ": " + integersArray[i];
                            clearableDiv.appendChild(eventElement);
                        }

                    }
                }).catch(function (error) {
                    alert(error);
                });
            }
            else if (response.status == 204)
            {
                alert("Not enough maintenance history on the vehicle you selected");
            } 
            else 
            {
                alert("Something went wrong! Response status: " + response.status);
            }
        }).catch(function (error) {
            alert(error);
        });
}

