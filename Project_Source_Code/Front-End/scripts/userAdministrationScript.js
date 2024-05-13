//#region display marketplace 
function displayUserAdministration() {
    var dynamicContent = document.querySelector(".dynamic-content");
    var currPage;
    try {
        currPage = parseInt(document.getElementById('current-page').innerText);
    }
    catch {
        currPage = 1;
    }
    while (dynamicContent.lastElementChild) {
        dynamicContent.removeChild(dynamicContent.lastElementChild);
    }

    var VehicleMarketplace = document.createElement('div');
    VehicleMarketplace.id = 'vehicle-marketplace';

    dynamicContent.appendChild(VehicleMarketplace);

    var dynamicContent = document.getElementById('vehicle-marketplace');
    var html = "<div class=VPMcontainer>";
 
    var buttons = document.createElement('div');
    buttons.id = 'vehicle-details-buttons';
    dynamicContent.appendChild(buttons);
    dynamicContent.style.display = "block";
    generateModifyButton(buttons);
    generateAccountDeletionButton(buttons);
    generateUserInfoRequestButton(buttons);
}


function generateModifyButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Modify';

    button.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        const buttons = document.getElementById("vehicle-details-buttons");
        alert("Modify button clicked");
        //add function here to make it do something 
        
    });
    content.appendChild(button);
}


function generateAccountDeletionButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Account Deletion';

    button.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        const buttons = document.getElementById("vehicle-details-buttons");
        alert("Account deletion button clicked");
        //add function here to make it do something 
        
        
    });
    content.appendChild(button);
}

function generateUserInfoRequestButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Info Request';

    button.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        const buttons = document.getElementById("vehicle-details-buttons");
        alert("Info request button clicked");
        //add function here to make it do something
        RequestingInfo();
         
        
    });
    content.appendChild(button);
}

function RequestingInfo(){
    fetchWithTokens('http://localhost:8004/RequestUserData/UserDataRequest', 'POST',"")
    .then(function (response) {
        if (response.status == 200) { 
            response.json()
            .then(function(data) {
                alert("Email sent");
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

function exrtactData(jsonData) {
    //var data = JSON.parse(jsonData);
    //page generation 
    var dynamicContent = document.querySelector(".dynamic-content");
    var html = "<div class=VPMcontainer>";
    jsonData.forEach(function(data) {
        // Access values from each object
        var VIN = data.userName;
        var make = data.address;
        var model = data.name;
        
        // Create HTML content with the extracted values
       
        // html += "<div class=vehicle-listings>";
        // html += '<img src=' + "VPM-resource/car.png" + '>'; 
        // html += '<h2> Vehicle' + '</h2>';
        html += `
        <div id=${VIN} class=vehicle-listings>
                <ul>
                    <li>${VIN}</li>
                    <li>${make}</li>
                    <li>${model}</li>
                </ul>
            </div>
        `;
    });   
    html += "</div>"; 
    html += "<div id=detail></div>";
    dynamicContent.innerHTML = html;

   
}

function DisplayAllAccounts()
{
    fetchWithTokens('http://localhost:8004/RequestUserData/RetrieveAllAccount', 'POST','')
    .then(function (response) {
        if (response.status == 200) { 
            response.json()
            .then(function(data) {
                // Check if data is an array or object
                if (Array.isArray(data) && data.length > 0) {
                    //alert("VP's retrieved");
                    exrtactData(data);
                    var vehicles = document.getElementsByClassName('vehicle-listings');
                    for (let i = 0; i < vehicles.length; i++) {
                        vehicles[i].addEventListener('click', (event) => {
                            event.stopPropagation();
                            displayDetailMarketplace(vehicles[i].id);
                        })
                    }
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
    
