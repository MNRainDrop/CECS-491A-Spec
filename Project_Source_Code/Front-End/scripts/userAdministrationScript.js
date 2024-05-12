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
        
    });
    content.appendChild(button);
}