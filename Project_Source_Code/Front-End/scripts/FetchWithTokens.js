export function fetchWithTokens( url ) {
    const IDToken = sessionStorage.getItem('IDToken');
    const AccessToken = sessionStorage.getItem('AccessToken');
    const RefreshToken = sessionStorage.getItem('RefreshToken');
    if (IDToken && AccessToken && RefreshToken) {
        options.headers = {"Content-Type": "application/json"};
        options.headers.Authorization = `Bearer ${jwt}`;

      }
      // Make the HTTP request using fetch
      return fetch(url, options);
}