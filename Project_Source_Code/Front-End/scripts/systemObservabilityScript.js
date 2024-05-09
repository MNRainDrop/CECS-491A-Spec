'use strict';
(async function() {
    function generateUsageDashboardView(){
        changeCSS('styles/SOstyles.css')
        var dynamicContent = document.getElementsByClassName('dynamic-content')[0];
        while (dynamicContent.lastElementChild) {
            dynamicContent.removeChild(dynamicContent.lastElementChild);
        }
        var KPI = document.createElement('div');
        KPI.id = 'kpi';

        dynamicContent.appendChild(KPI);
        
        generateUsageDashboardDivs();
        populateLoginDiv();
        populateAccountCreationDiv();
        populateLongestViews();
        populateMostVisitedViews();
        populateMostRegisteredVehicles();
        populateVehicleCreationAttempts();
        populateLogs();

    }
    window.generateUsageDashboardView = generateUsageDashboardView

    function generateUsageDashboardDivs() {
        generateTimeFrame();
        generateKPIs();
    }

    function generateTimeFrame() {
        var dynamicContent = document.getElementsByClassName('dynamic-content')[0];

        var timeFrame = document.createElement('paragraph');
        timeFrame.innerHTML = 'Showing KPIs from the last ';

        var dropDown = document.createElement('select');
        dropDown.id = 'time-frame';

        var sixMonths = document.createElement('option')
        sixMonths.id = '6';
        sixMonths.innerText = '6 Months';
        var twelveMonths = document.createElement('option')
        twelveMonths.id = '12';
        twelveMonths.innerText = '12 Months'
        var twentyFourMonths = document.createElement('option');
        twentyFourMonths.id = '24';
        twentyFourMonths.innerText = '24 Months';

        dynamicContent.appendChild(timeFrame);
        dynamicContent.appendChild(dropDown);

        dropDown.appendChild(sixMonths);
        dropDown.appendChild(twelveMonths);
        dropDown.appendChild(twentyFourMonths);

        dropDown.onchange = function() {
            populateAllKPIs(months)
        }
    }

    function generateKPIs() {
        var dynamicContent = document.getElementsByClassName('dynamic-content')[0];
        var kpi = document.getElementById('kpi');

        var loginAttempts = document.createElement('div');
        loginAttempts.classList.add('kpi')
        loginAttempts.id = 'login-attempts'

        var accountCreationAttempts = document.createElement('div');
        accountCreationAttempts.classList.add('kpi')
        accountCreationAttempts.id = 'account-creation-attempts'

        var longestViews = document.createElement('div');
        longestViews.classList.add('kpi')
        longestViews.id = 'longest-views'

        var mostVisitedViews = document.createElement('div');
        mostVisitedViews.classList.add('kpi')
        mostVisitedViews.id = 'most-visited-views'

        var mostRegisteredVehicles = document.createElement('div');
        mostRegisteredVehicles.classList.add('kpi')
        mostRegisteredVehicles.id = 'most-registered-vehicles'

        var vehicleCreationAttempts = document.createElement('div');
        vehicleCreationAttempts.classList.add('kpi')
        vehicleCreationAttempts.id = 'vehicle-creation-attempts'
        
        var logs = document.createElement('div');
        logs.classList.add('logs')
        logs.id = 'logs'

        dynamicContent.appendChild(kpi);
        kpi.appendChild(loginAttempts);
        kpi.appendChild(accountCreationAttempts);
        kpi.appendChild(longestViews);
        kpi.appendChild(mostVisitedViews);
        kpi.appendChild(mostRegisteredVehicles);
        kpi.appendChild(vehicleCreationAttempts);
        dynamicContent.appendChild(logs);
    }

    function populateAllKPIs(months) {
        
    }

    function populateLoginDiv() {
        var dynamicContent = document.getElementById('login-attempts')
        var head = document.createElement('h1');
        head.innerText = 'Login Attempts';

        dynamicContent.appendChild(head);
    }

    function populateAccountCreationDiv() {
        var dynamicContent = document.getElementById('account-creation-attempts')
        var head = document.createElement('h1');
        head.innerText = 'Account Creation Attempts';

        var ul = document.createElement('ul');

        // fetchWithTokens() 
        //     .then(response => {
        //         if (!response.ok) {
        //             throw 'Something went wrong trying to get this information'
        //         }
        //         return response.json();
        //     })
        //     .then(data => {
        //         if (data.length == 0) {
        //             throw 'Something went wrong trying to get this information'
        //         }
        //         else {
        //             data.forEach(element => {
                        
        //             });
        //         }
        //     })
        //     .catch(error => {

        //     })

        // Test Data
        for(let i = 0; i < 10; i++) {
            var li = document.createElement('li');
            li.innerText = 'Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World ';
            ul.appendChild(li);
                
        }

        dynamicContent.appendChild(head);
        dynamicContent.appendChild(ul);
    }

    function populateLongestViews() {
        var dynamicContent = document.getElementById('longest-views')
        var head = document.createElement('h1');
        head.innerText = 'Longest Views';

        dynamicContent.appendChild(head);
    }

    function  populateMostVisitedViews() {
        var dynamicContent = document.getElementById('most-visited-views')
        var head = document.createElement('h1');
        head.innerText = 'Most Visited Views';

        dynamicContent.appendChild(head);
    }

    function populateMostRegisteredVehicles() {
        var dynamicContent = document.getElementById('most-registered-vehicles')
        var head = document.createElement('h1');
        head.innerText = 'Most Registered Vehicles';

        dynamicContent.appendChild(head);
    }

    function populateVehicleCreationAttempts() {
        var dynamicContent = document.getElementById('vehicle-creation-attempts')
        var head = document.createElement('h1');
        head.innerText = 'Vehicle Creation Attempts';

        dynamicContent.appendChild(head);
    }

    function populateLogs() {
        var dynamicContent = document.getElementById('logs')
        var head = document.createElement('h1');
        head.innerText = 'Logs';

        dynamicContent.appendChild(head);

        var ul = document.createElement('ul');

        // Test Data
        for(let i = 0; i < 10; i++) {
            var li = document.createElement('li');
            li.innerText = 'Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World Hello World ';
            ul.appendChild(li);
                
        }
        dynamicContent.appendChild(ul);
    }
    
})(window);