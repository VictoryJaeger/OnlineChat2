// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.



//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();



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

//var joinGroup = function () {
//    // var url = 'Chat/JoinChatAsync/' + _connectionId + '/@Model.Name'
//    const bodyForm = new FormData();
//    bodyForm.set('connectionId', _connectionId);
//    bodyForm.append('groupName', '@Model.Name');

//    axios.post('Chat/JoinChatAsync', bodyForm)
//        //axios.post(url,null)
//        .then(res => {
//            console.log("Group joined!", res);
//        })
//        .catch(err => {
//            console.log("Failed to join this group", err);
//        })
//}

//connection.start()
//    .then(function () {
//        connection.invoke('GetConnectionId')
//            .then(function (connectionId) {
//                _connectionId = connectionId
//                joinGroup();
//            })
//    })
//    .catch(function (err) {
//        console.log(err)
//    })


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