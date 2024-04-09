// To note: unhiding certain navigation attribuites based on user claims --> admin has System obserbility

// Attach event listener to submit username button
const submitUsernameButton = document.getElementById("submit-username");
submitUsernameButton!.addEventListener("click", submitUsername);


// Checks user username
const isValidEmailAddress = (email: string): boolean =>
{
    const emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
};

// Checks user OTP
// Note forces OTP to have at least one lowercase, uppercase, number with at least characters in total
const isValidOTP = (OTP: string): boolean =>
{
    const otpPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/;
    return otpPattern.test(OTP);
}

// Function to handle submitting username
function submitUsername()
{
    const usernameInput = document.getElementById("username-input") as HTMLInputElement;
    const username = usernameInput.value.trim();

    // Check if username is not empty
    if (username)
    {
        if (isValidEmailAddress(username))
        {
            // Calls Web API controller -- login --> change when moved to Ride Along
            fetch("http://localhost:8080/Auth/startLogin",
                {
                method: "POST",
                body: JSON.stringify(username),
                headers: {
                    "Content-Type": "application/json"
                }
            })
                .then(response =>
                {
                    if (response.ok)
                    {
                        // If response is OK, display OTP view
                        showOTPView();
                        // Trying to attach original username input into text box
                        usernameInput.value = username;
                    }
                    else
                    {
                        // If response is not OK, display to user process failed
                        alert("Authentication failed!")
                        console.error("Error:", response.statusText);
                    }
                })
                .catch(error =>
                {
                    alert("Something went wrong!")
                    console.error("Error:", error);
                });
        }
        else
        {
            alert("Please retry entering your username!")
        }
    }
    else
    {
        alert("Username cannot be empty!")
        console.error("Username cannot be empty.");
    }
}

// Function to show OTP view
function showOTPView() {
    // Retrieving the page's dynamic html div
    const dynamicContent = document.querySelector(".dynamic-content");
    
    // Getting rid of the first submit button, for increased clarity
    var submitUsernameButton = document.getElementById("submit-username");
    submitUsernameButton!.remove();

    // Create a paragraph element to display the username
    var usernameParagraph = document.createElement('p');
    var usernameInputField = document.getElementById("username-input") as HTMLInputElement;
    usernameParagraph.textContent = 'Username: ' + usernameInputField!.value.trim();
    usernameParagraph.id = 'username-paragraph';
    
    // Remove the username input field
    var usernameInput = document.getElementById("username-input");
    usernameInput!.remove();

    // Creating the new HTML for the OTP view
    var otpContainer = document.createElement('div');
    otpContainer.innerHTML += `
        <div id="otp-container">
            <p>An OTP has been sent to your email address. Please enter the OTP:</p>
            <input type="text" id="otp-input" placeholder="Enter OTP">
            <button id="submit-otp">Submit</button>
        </div>
    `;
    otpContainer.id = 'otp-container';
    
    // Append the OTP view and the username paragraph to the dynamic content
    dynamicContent!.innerHTML = ''
    dynamicContent!.appendChild(usernameParagraph);
    dynamicContent!.appendChild(otpContainer);

    // Attach event listener to submit OTP button
    const submitOTPButton = document.getElementById("submit-otp");
    submitOTPButton!.addEventListener("click", submitOTP);
}

// Function to handle submitting OTP
function submitOTP() {
    const otpInput = document.getElementById("otp-input") as HTMLInputElement;
    const otp = otpInput.value.trim();
    // Retrieve the username from the paragraph element
    var usernameParagraph = document.querySelector("#username-paragraph");
    var username = usernameParagraph!.textContent!.replace('Username: ', '').trim();

    // Check if OTP is not empty
    if (otp && username)
    {
        if (isValidOTP(otp) && isValidEmailAddress(username))
        {
            var fetchResponse = false; // Assuming Fail response by default

            // Make fetch request to back - end
            fetch("http://localhost:8080/Auth/tryAuthentication",
                {
                method: "POST",
                body: JSON.stringify({username, otp}),
                headers: {
                    "Content-Type": "application/json"
                }
            })
            .then(response => { 
                if (response.ok)
                {
                    return response.json();
                } 
                else
                {
                    alert("Authentication failed!");
                    console.error("Error:", response.statusText);
                }
                
            })
            .then(data => {
                sessionStorage.setItem ('IDToken', data.idToken);
                sessionStorage.setItem ('AccessToken' , data.accessToken);
                sessionStorage.setItem ('RefreshToken' , data.refreshToken);
                showMainContent();
                unhideNavigation();
            })
            .catch(error =>
            {
                alert("Something went wrong!" + " " + error)
                console.error("Error:", error);
            });
            
        }
        else
        {
            alert("You have entered a bad OTP")
        }
    }
    else
    {
        alert("OTP cannot be empty!")
    }
}

// Function to show main content --> refers to Welcome page.
// Where we want access to all features such as VP, SL, CHR....
function showMainContent() {
    const dynamicContent = document.querySelector(".dynamic-content");
    dynamicContent!.innerHTML = `
        <div id="main-content">
            <p>Welcome, user! You are now logged in.</p>
            <p>Welcome to the <b>Ride-Along</b> Application!<br>We are currently working on this page to make it better suited for you!</p>
            <p>Work in progresss: Security, Vehicle Profile, and Service Log</p>
        </div>
    `;
}

// Function to unhide navigation
function unhideNavigation() {
    const navigation = document.getElementById("navigation");
    navigation!.classList.remove("hidden");

    // Add Event listeners to nagvigation items
    const navigationItems = navigation!.querySelectorAll('li');
    navigationItems.forEach(item => {
        item.addEventListener('click', handleNavigationItemClick);
    });
}

function handleNavigationItemClick(event) {
    // Handle navigation item click
    const selectedItem = event.target.id;
    // Perform actions based on the selected item, such as navigating to different pages or triggering specific functionality
    console.log(selectedItem + ' clicked');
}