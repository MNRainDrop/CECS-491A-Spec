function extractData(jsonData) {
    var dynamicContent = document.querySelector(".dynamic-content");
    dynamicContent.innerHTML = ''; // Clear previous content

    // Iterate over each object in the array
    jsonData.forEach(function(obj) {
        // Extract data from the object
        var charity = obj.charity;
        var description = obj.description;
        var link = obj.link;

        // Create HTML elements
        var charityDiv = document.createElement('div');
        charityDiv.classList.add('charity');

        var charityName = document.createElement('h3');
        charityName.textContent = charity;

        var descriptionPara = document.createElement('p');
        descriptionPara.textContent = description;

        var linkAnchor = document.createElement('a');
        linkAnchor.textContent = 'Learn More';
        linkAnchor.href = link;
        linkAnchor.target = '_blank'; // Open link in new tab

        // Append elements to the container
        charityDiv.appendChild(charityName);
        charityDiv.appendChild(descriptionPara);
        charityDiv.appendChild(linkAnchor);

        dynamicContent.appendChild(charityDiv);
    });
}
function createDonateYourCarView(){
    fetchWithTokens('http://localhost:5212/DonateYourCar/RetrieveCharities', 'GET','')
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