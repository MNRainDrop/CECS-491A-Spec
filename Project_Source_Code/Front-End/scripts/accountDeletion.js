function generateAccountDeletionButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Account Deletion';

    button.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        const buttons = document.getElementById("account-deletion-button");

        // Ask the user for confirmation
        var confirmation = confirm("Are you sure you want to delete your account?\nDeleting your account will delete all data relating to you.\n Vehicle data will remain on the app unless deleted beforehand.");
        
        if (confirmation) {
            // User confirmed deletion, perform deletion operation
            alert("Account deletion initiated...");
            
            deleteUsersAccount();

        } else {
            // User cancelled deletion
            alert("Account deletion cancelled.");
        }
    });

    content.appendChild(button);
}

function deleteUsersAccount() {
    // Make a request to delete the user's account
    fetchWithTokens(CONFIG["ip"] + ':' + CONFIG["ports"]["deletion"]+'/Deletion/DeleteMyAccount', 'POST', "")
        .then(response => {
            if (response.ok) {
                // Account deletion successful
                return response.text(); // Parse the response text
            } else {
                // Account deletion failed
                throw new Error('Account deletion failed.'); // Throw an error with a generic message
            }
        })
        .then(data => {
            // Show success message from the server response
            alert(data);

            location.reload();
            // Additional actions after successful deletion, if needed
        })
        .catch(error => {
            // Show an alert for any errors
            alert('Error deleting account: ' + error.message);
        });
}
