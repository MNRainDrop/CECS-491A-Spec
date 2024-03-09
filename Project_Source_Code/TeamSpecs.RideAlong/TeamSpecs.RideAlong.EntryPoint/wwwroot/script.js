document.getElementById('registrationForm').addEventListener('submit', function (event) {
    event.preventDefault();
    var formData = new FormData(this);
    var data = {};
    formData.forEach(function (value, key) {
        data[key] = value;
    });
    // Convert dateOfBirth to a valid DateTime format
    var dateOfBirth = new Date(data['dob']);
    var formattedDateOfBirth = dateOfBirth.toISOString();
    var requestData = {
        userName: data['username'],
        accountType: data['accountType'],
        dateOfBirth: formattedDateOfBirth
    };
    fetch('/api/UserAdministration', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestData)
    })
        .then(function (response) {
        if (response.ok) {
            // Check if response has content
            if (response.status === 204) {
                // No content, handle accordingly
                var confirmationDiv = document.getElementById('confirmation');
                confirmationDiv.style.display = 'block';
                var confirmationMessage = document.getElementById('confirmationMessage');
                confirmationMessage.textContent = 'User created successfully';
                return;
            }
            // Parse JSON data
            return response.json();
        }
        else {
            throw new Error('Account Registration failed.');
        }
    })
        .then(function (data) {
        if (data) {
            var confirmationDiv = document.getElementById('confirmation');
            confirmationDiv.style.display = 'block';
            var confirmationTable = document.getElementById('confirmationTable');
            // Clear previous content
            confirmationTable.innerHTML = '';
            // Populate the table with account information
            for (var key in data) {
                var row = confirmationTable.insertRow();
                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);
                cell1.textContent = key;
                cell2.textContent = data[key];
            }
        }
    })
        .catch(function (error) {
        // Do nothing here since we don't want to display confirmation on bad request
    });
});
document.getElementById('confirmYes').addEventListener('click', function () {
    document.getElementById('confirmation').style.display = 'none';
    document.getElementById('registrationForm').reset();
});
document.getElementById('confirmNo').addEventListener('click', function () {
    document.getElementById('confirmation').style.display = 'none';
});
//# sourceMappingURL=script.js.map