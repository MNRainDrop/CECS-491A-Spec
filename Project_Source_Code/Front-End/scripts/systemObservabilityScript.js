'use strict';
(async function() {
    function generateUsageDashboardView(){
        fetchWithTokens(CONFIG["ip"] + ':' + CONFIG["ports"]["systemObservability"]+'/SystemObservability/PostAuthStatus', 'POST', '')
            .then(function (response) {
            if (response.status == 204) {
                // replace the parameter inside changeCSS() to the path of the css file you need
                changeCSS('styles/SOstyles.css')
                var dynamicContent = document.getElementsByClassName('dynamic-content')[0];
                while (dynamicContent.lastElementChild) {
                    dynamicContent.removeChild(dynamicContent.lastElementChild);
                }
                var KPI = document.createElement('div');
                KPI.id = 'kpi';

                dynamicContent.appendChild(KPI);
                
                generateUsageDashboardDivs();
                populateAllKPIs();
                
                var refreshInterval = setInterval(populateAllKPIs, 60000);

                var menuItems = document.querySelectorAll('.menu-item');
                menuItems.forEach(function(element) {
                    element.addEventListener('click', () => {
                        clearInterval(refreshInterval)
                        alert('removed interval')
                    })
                })
            }
            else {
                alert("Permission to view denied");
            }
            }).catch(function (error) {
                alert(error);
            })
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
            populateAllKPIs()
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
        var dropDown = document.getElementById('time-frame');
        var months = dropDown.options[dropDown.selectedIndex].id
        fetchWithTokens(CONFIG["ip"] + ':' + CONFIG["ports"]["systemObservability"]+'/SystemObservability/PostGetKPIs', 'POST', months)
            .then(response => {
                if (!response.ok) {
                    throw "Could not process request";
                }
                else {
                    return response.json();
                }
            })
            .then(data => {
                populateLoginDiv(data[0].value)
                populateAccountCreationDiv(data[1].value)
                populateLongestViews(data[2].value)
                populateMostVisitedViews(data[3].value)
                populateMostRegisteredVehicles(data[4].value)
                populateVehicleCreationAttempts(data[5].value)
            })
            .catch(error => {
                alert(error)
            });
        fetchWithTokens(CONFIG["ip"] + ':' + CONFIG["ports"]["systemObservability"]+'/PostGetLogs', 'POST', months)
            .then(response => {
                if (!response.ok) {
                    throw "Could not process request";
                }
                else {
                    return response.json();
                }
            })
            .then(data => {
                populateLogs(data)
            })
            .catch(error => {
                alert(error)
            });
    }

    function populateLoginDiv(data) {
        var dynamicContent = document.getElementById('login-attempts')
        while (dynamicContent.lastElementChild) {
            dynamicContent.removeChild(dynamicContent.lastElementChild);
        }
        var head = document.createElement('h1');
        head.innerText = 'Login Attempts';
        var ul = document.createElement('ul');

        for(var element of data) {
            var li = document.createElement('li');
            li.innerText = `${element.logTime}\t${element.logContext}`
            ul.appendChild(li);
        }

        dynamicContent.appendChild(head);
        dynamicContent.appendChild(ul);
    }

    function populateAccountCreationDiv(data) {
        var dynamicContent = document.getElementById('account-creation-attempts')
        while (dynamicContent.lastElementChild) {
            dynamicContent.removeChild(dynamicContent.lastElementChild);
        }
        var head = document.createElement('h1');
        head.innerText = 'Account Creation Attempts';

        var ul = document.createElement('ul');

        for(var element of data) {
            var li = document.createElement('li');
            li.innerText = `${element.logTime}\t${element.logContext} `
            ul.appendChild(li);
        }

        dynamicContent.appendChild(head);
        dynamicContent.appendChild(ul);
    }

    function populateLongestViews(data) {
        var dynamicContent = document.getElementById('longest-views')
        while (dynamicContent.lastElementChild) {
            dynamicContent.removeChild(dynamicContent.lastElementChild);
        }
        var head = document.createElement('h1');
        head.innerText = 'Longest Views in seconds';

        var ul = document.createElement('ul');

        for(var element of data) {
            var li = document.createElement('li');
            li.innerText = `${element.timeInSeconds} seconds: ${element.feature}`
            ul.appendChild(li);
        }

        dynamicContent.appendChild(head);
        dynamicContent.appendChild(ul);
    }

    function  populateMostVisitedViews(data) {
        var dynamicContent = document.getElementById('most-visited-views')
        while (dynamicContent.lastElementChild) {
            dynamicContent.removeChild(dynamicContent.lastElementChild);
        }
        var head = document.createElement('h1');
        head.innerText = 'Most Visited Views';

        var ul = document.createElement('ul');

        for(var element of data) {
            var li = document.createElement('li');
            li.innerText = `${element.count} Views: ${element.featureName}`
            ul.appendChild(li);
        }

        dynamicContent.appendChild(head);
        dynamicContent.appendChild(ul);
    }

    function populateMostRegisteredVehicles(data) {
        var dynamicContent = document.getElementById('most-registered-vehicles')
        while (dynamicContent.lastElementChild) {
            dynamicContent.removeChild(dynamicContent.lastElementChild);
        }
        var head = document.createElement('h1');
        head.innerText = 'Most Registered Vehicles';

        var ul = document.createElement('ul');

        for(var element of data) {
            var li = document.createElement('li');
            li.innerText = `${element.count} Vehicles: ${element.make} ${element.model} ${element.year} `
            ul.appendChild(li);
        }

        dynamicContent.appendChild(head);
        dynamicContent.appendChild(ul);
    }

    function populateVehicleCreationAttempts(data) {
        var dynamicContent = document.getElementById('vehicle-creation-attempts')
        while (dynamicContent.lastElementChild) {
            dynamicContent.removeChild(dynamicContent.lastElementChild);
        }
        var head = document.createElement('h1');
        head.innerText = 'Vehicle Creation Attempts';

        var ul = document.createElement('ul');

        for(var element of data) {
            var li = document.createElement('li');
            li.innerText = `${element.logTime}\t${element.logContext} `
            ul.appendChild(li);
        }

        dynamicContent.appendChild(head);
        dynamicContent.appendChild(ul);
    }

    function populateLogs(data) {
        console.log(data)
        var dynamicContent = document.getElementById('logs')
        while (dynamicContent.lastElementChild) {
            dynamicContent.removeChild(dynamicContent.lastElementChild);
        }
        var head = document.createElement('h1');
        head.innerText = 'Logs';

        var ul = document.createElement('ul');

        for(var element of data) {
            var li = document.createElement('li');
            li.innerText = `${element.logTime}\t${element.logLevel}\t${element.logCategory}\t${element.logContext} `
            ul.appendChild(li);
        }

        dynamicContent.appendChild(head);
        dynamicContent.appendChild(ul);
    }
    
})(window);