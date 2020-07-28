// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

//var importCdn = document.createElement('script');

//importCdn.setAttribute('src', '~/js/signalr/dist/browser/signalr.js/signalr.min.js');

//document.head.appendChild(importCdn);



//var connection = new signalR.HubConnectionBuider()
//    .withUrl("/chatHub")
//    .configureLogging(signalR.LogLevel.Information)
//    .build();




//var _connectionId = '';

//connection.on("ReceiveMessage", function (data) {
//    console.log(data);

//    var message = document.createElement("div");
//    message.classList.add("message");

//    var header = document.createElement("header");
//    header.appendChild(document.createTextNode(data.name));

//    var p = document.createElement("p");
//    p.appendChild(document.createTextNode(data.text));

//    var footer = document.createElement("footer")
//    footer.appendChild(document.createTextNode(data.timestamp));

//    message.appendChild(header);
//    message.appendChild(p);
//    message.appendChild(footer);

//    document.querySelector('.chat-body').append(message);
//})

//connection.start()
//    .then(function () {
//        connection.invoke('getConnectionString')
//            .then(function (connectionId) {
//                _connectionId = connectionId
//                joinGroup();
//            })
//    })
//    .catch(function (err) {
//        console.log(err)
//    })

//var joinGroup = function () {
//    var url = '/Chat/JoinGroupAsync/' + _connectionId + '/@@Model.Name'
//    axios.post(url, null)
//        .then(res => {
//            console.log("Group joined!", res);
//        })
//        .catch(err => {
//            console.err("Failed to join this group", err);
//        })
//}

//var form = null;

//var sendMessage = function (event) {
//    event.preventDefault();

//    var data = new FormData(event.target);

//    document.getElementById('message-input').value = '';


//    axios.post('Chat/SendMessageAsync', data)
//        .then(res => {
//            console.log("Message Sent");
//        })
//        .catch(err => {
//            console.log("Failed to send message");
//        })

//}