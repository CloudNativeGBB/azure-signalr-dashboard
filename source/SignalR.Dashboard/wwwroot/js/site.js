// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

let dashboardServiceURL = 'http://localhost:7071';
let state = {
    connection: {},
    userID: '',
    accessToken: '',
    button: {},
    div: {}
};

state.button.authenticate = document.getElementById('authenticate');
state.button.create = document.getElementById('create-async');
state.div.output = document.getElementById('output');
state.div.progressbar = document.getElementsByClassName('progress-bar progress-bar-striped progress-bar-animated');

state.button.create.disabled = true;

function parseJwt(token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
};

const authenticateProcess = () => {

    var requestOptions = {
        method: 'POST',
        redirect: 'follow'
    };

    let authenticateServiceURL = `${dashboardServiceURL}/api/1.0/authenticate`;
    fetch(authenticateServiceURL, requestOptions)
        .then(response => response.text())
        .then((result) => {
            let tokenResponse = JSON.parse(result);
            let obj = parseJwt(tokenResponse.token);
            state.accessToken = tokenResponse.token;
            state.userID = obj.userID;

            // configure signalr connection
            state.connection = new signalR.HubConnectionBuilder()
                .withUrl(`${dashboardServiceURL}/api/1.0/${state.userID}`, { accessTokenFactory: () => state.accessToken })
                .configureLogging(signalR.LogLevel.Information)
                .withAutomaticReconnect()
                .build();

            // connect with signalr
            state.connection.start()
                .then(() => console.log)
                .catch((err) => {
                    UpdateErrorOutput(err);
                });

            // signalr events
            state.connection.on('sendUpdate', sendUpdate);
            state.connection.onclose(() => updateOutput('Disconnected'));

            state.button.authenticate.disabled = false;
            state.button.create.disabled = false;
        })
        .catch((error) => {
            updateErrorOutput(`There was an error: ${error}`);
            state.button.authenticate.disabled = false;
        });
};

const createProcess = () => {
    state.div.progressbar[0].setAttribute('ariavaluenow', '0');
    state.div.progressbar[0].setAttribute('style', 'width: 0%');

    let data = { name: 'sample' };

    let createProcessServiceURL = `${dashboardServiceURL}/api/1.0/create`;
    fetch(createProcessServiceURL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': state.accessToken
        },
        body: JSON.stringify(data)
    }).then((res) => {
        if (res.ok) {
            updateOutput('Request submitted');
        }
    }).catch((err) => {
        updateErrorOutput(`There was an error: ${err}`);
    });
};

const updateOutput = (msg) => {
    state.div.output.innerHTML += `${msg}<br/>`;
};

const updateErrorOutput = (msg) => {
    state.div.output.innerHTML += `<li><div class="alert alert-danger" role="alert">${msg}</div></li>`;
};

const updateProgressBar = (value) => {
    state.div.progressbar[0].setAttribute('ariavaluenow', `${value}`);
    state.div.progressbar[0].setAttribute('style', `width: ${value}%`);
};

const sendUpdate = (status) => {
    switch (status.messageType) {
        case "progress":
            updateProgressBar(parseFloat(status.content));
            break;
        case "message":
            updateOutput(status.content);
            break;
        case "error":
            updateErrorOutput(status.content);
            break;
        case "completed":
            updateOutput(status.content);
            state.button.create.disabled = false;
            break;
        default:
            break;
    }
};

state.button.authenticate.addEventListener('click', () => {
    authenticateProcess();
    state.button.authenticate.disabled = true;
});

state.button.create.addEventListener('click', () => {
    createProcess();
    state.button.create.disabled = true;
});