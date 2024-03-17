document.getElementById('registrationForm').addEventListener('submit', function (event) {
    event.preventDefault();
    const formData = new FormData(this as HTMLFormElement);
    const data = {};
    formData.forEach(function (value, key) {
        data[key] = value;
    });
    // Convert dateOfBirth to a valid DateTime format
    const dateOfBirth = new Date(data['dob']);
    const formattedDateOfBirth = dateOfBirth.toISOString();
    const requestData = {
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
                    const confirmationDiv = document.getElementById('confirmation');
                    confirmationDiv.style.display = 'block';
                    const confirmationMessage = document.getElementById('confirmationMessage');
                    confirmationMessage.textContent = 'User created successfully';
                    return;
                }
                // Parse JSON data
                return response.json();
            } else {
                throw new Error('Account Registration failed.');
            }
        })
        .then(function (data) {
            if (data) {
                const confirmationDiv = document.getElementById('confirmation');
                confirmationDiv.style.display = 'block';
                const confirmationTable = document.getElementById('confirmationTable') as HTMLTableElement;

                // Clear previous content
                confirmationTable.innerHTML = '';

                // Populate the table with account information
                for (const key in data) {
                    const row = confirmationTable.insertRow();
                    const cell1 = row.insertCell(0);
                    const cell2 = row.insertCell(1);
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
    (document.getElementById('registrationForm') as HTMLFormElement).reset();
});

document.getElementById('confirmNo').addEventListener('click', function () {
    document.getElementById('confirmation').style.display = 'none';
});

