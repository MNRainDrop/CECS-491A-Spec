function fetchWithTokens(url, method, body) {
    var _a;
    // Fetch the tokens from session storage
    var idToken = sessionStorage.getItem('IDToken');
    var accessToken = sessionStorage.getItem('AccessToken');
    var refreshToken = sessionStorage.getItem('RefreshToken');
    // Create the headers object with the tokens
    var headers = {
        'Authorization': (_a = "Bearer ".concat(idToken)) !== null && _a !== void 0 ? _a : '',
        'X-Access-Token': accessToken !== null && accessToken !== void 0 ? accessToken : '',
        'X-Refresh-Token': refreshToken !== null && refreshToken !== void 0 ? refreshToken : '',
        'Content-Type': 'application/json' // Set the Content-Type header for the request body
    };
    if (method === 'GET')
    {
        return fetch(url, {
            method: method,
            headers: headers
        });
    }
    else {
        return fetch(url, {
            method: method,
            headers: headers,
            body: JSON.stringify(body)
        });
    }
    
};

function changeCSS(file) {
    var head = document.getElementsByTagName('head')[0];
    if (head.lastChild.id !== 'cssLink')
    {
        head.removeChild(head.lastChild);
    }
    if (file != null)
    {
        var style = document.createElement('link');
        style.href = file;
        style.type = 'text/css'
        style.rel = 'stylesheet'
        head.append(style);
    }
};

// 
function incrementPages() {
    var content = document.getElementById('current-page');
    var value = parseInt(content.innerText);
    value += 1;
    content.innerHTML = String(value);
};

function decrementPages() {
    var content = document.getElementById('current-page');
    var value = parseInt(content.innerText);
    value -= 1;
    if (value == 0) {
        value = 1;
    }
    content.innerHTML = String(value);
};

function pages(functionCall) {
    var dynamicContent = document.getElementsByClassName('dynamic-content')[0];
    var pages = document.createElement('nav');
    pages.id = 'pages';
    
    var back = document.createElement('li');
    back.id = 'back';
    back.innerText = '<';
    var currentPage = document.createElement('p');
    currentPage.id = 'current-page';
    currentPage.innerText = '1';
    var next = document.createElement('li');
    next.id = 'next';
    next.innerText = '>';

    next.addEventListener('click', function () {
        incrementPages();
        functionCall()
    });
    back.addEventListener('click', function () {
        decrementPages();
        functionCall()
    });

    pages.appendChild(back);
    pages.appendChild(currentPage);
    pages.appendChild(next);

    dynamicContent.appendChild(pages);
    functionCall();
};
