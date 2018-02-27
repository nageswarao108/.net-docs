﻿var connection = new signalR.HubConnection('chat');

connection.on('Send', (timestamp, user, message) => {
    var encodedUser = user;
    var encodedMsg = message;
    var listItem = document.createElement('li');
    listItem.innerHTML = timestamp + ' <b>' + encodedUser + '</b>: ' + encodedMsg;
    document.getElementById('messages').appendChild(listItem);
});

document.getElementById('send').addEventListener('click', event => {
    var msg = document.getElementById('message').value;
    var usr = document.getElementById('user').value;

    connection.invoke('Send', usr, msg).catch(err => showErr(err));
    event.preventDefault();
});

function showErr(msg) {
    var listItem = document.createElement('li');
    listItem.setAttribute("style", "color: red");
    listItem.innerHTML = msg;
    document.getElementById('messages').appendChild(listItem);
}

connection.start().catch(err => showErr(err));