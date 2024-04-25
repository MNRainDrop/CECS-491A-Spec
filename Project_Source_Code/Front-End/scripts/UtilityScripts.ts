function fetchWithTokens(url: string, method: string, body: any) {  
    // Fetch the tokens from session storage
    const idToken = sessionStorage.getItem('IDToken');
    const accessToken = sessionStorage.getItem('AccessToken');
    const refreshToken = sessionStorage.getItem('RefreshToken');
    // Create the headers object with the tokens
    const headers : HeadersInit = {
        'Authorization': `Bearer ${idToken}` ?? '',
        'X-Access-Token': accessToken ?? '',
        'X-Refresh-Token': refreshToken ?? '',
        'Content-Type': 'application/json' // Set the Content-Type header for the request body
    };
    return fetch(url, {
        method: method,
        headers: headers,
        body: JSON.stringify(body)
    });
};

// 
function incrementPages() {
    var content = document.getElementById('current-page');
    var value = parseInt(content!.innerText);
    value += 1;
    content!.innerHTML = String(value);
}

function decrementPages() {
    var content = document.getElementById('current-page');
    var value = parseInt(content!.innerText);
    value -= 1;
    if (value == 0){
        value = 1;
    }        
    content!.innerHTML = String(value);
}

function pages(viewFunction) {
    // view function is the function that you should pass in to get this to work
    var content = document.getElementById('pages');

    content!.innerHTML = `
        <li id='back'><</li>
        <p id='current-page'>1</p>
        <li id='next'>></li>
    `;
    var next = document.getElementById('next');
    var back = document.getElementById('back');

    next!.addEventListener('click', () => {
        incrementPages();
        viewFunction();
    });
    back!.addEventListener('click', () => {
        decrementPages();
        viewFunction();
    });
}