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

