// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

"use strict";

//const { axios } = require('axios');
//import axios from 'axios';

//var connection = new signalR.HubConnectionBuilder().withUrl("/homeHub")
//    .withAutomaticReconnect()
//    .build();

var chatId = 0;

homeConnection.on("PrivateChatCreated", function (data) {
    console.log(data);

    chatId = data.id;

    //debugger;
    console.log('start display data');
    var getChatButton = document.createElement("a");
    getChatButton.href = "get_chat_button";

    var chatListItem = document.createElement("div");
    chatListItem.classList.add("chat_list_item");

    console.log('1');
    var chatDetail = document.createElement("div");
    chatDetail.classList.add("chat_detail");

    console.log('2');
    var chatImg = document.createElement("div");
    chatImg.classList.add("chat_img");

    console.log('3');
    var chatDetailInfo = document.createElement("div");
    chatDetailInfo.classList.add("chat_detail_info");

    console.log('4');
    var chatName = document.createElement("h5");
    chatName.appendChild(document.createTextNode(data.name));

    console.log('5');
    var chatDate = document.createElement("span");
    chatDate.classList.add("chat_date");
    chatDate.appendChild(document.createTextNode(data.lastMessageDate));

    console.log('6');
    var p = document.createElement("p");
    p.appendChild(document.createTextNode(data.lastMessageAuthor + ' : ' + data.lastMessage));

    console.log('create tree');

    getChatButton.appendChild(chatListItem);

    chatListItem.appendChild(chatDetail);
    console.log('add chatDetail');

    chatDetail.appendChild(chatImg);
    chatDetail.appendChild(chatDetailInfo);
    chatDetailInfo.appendChild(chatName);
    chatDetailInfo.appendChild(p);
    chatName.appendChild(chatDate);

    //    var p = document.createElement("p");
    //    p.appendChild(document.createTextNode(data.text));

    //    var message_date = document.createElement("span");
    //    message_date.classList.add("message_date");
    //    message_date.appendChild(document.createTextNode(data.timestamp));

    //    messageInfo.appendChild(messageText);
    //    messageText.appendChild(p);
    //    messageText.appendChild(message_date);

    console.log('add new chat');
    document.getElementById("get_chat_button").appendChild(chatListItem);
    //document.querySelector('.inbox_chat').append(chatListItem);
    //debugger;
    console.log("data has been displayed");
})


//TODO

//$(function () {

//    $("#get_chat_button").click(function (e) {
//        // Stop the normal navigation
//        e.preventDefault();

//        //Build the new URL
//        var url = $(this).attr("href");
//        var id = $("#id").val();
//        url = url.replace("dummyDate", date);

//        //Navigate to the new URL
//        window.location.href = url;

//    });
//});













//connection.on("PrivateChatCreatedToAll", function (data) {
//    console.log(data + 'created');
//})

//var connectPrivateChat = function () {
//    const bodyForm = new FormData();
//    bodyForm.set('connectionId', _connectionId);
//    bodyForm.append('groupName', '@Model.Name');
//    bodyForm.append('participantConId', '@Model.UsersConnectionId.Last()')

//    console.log('@Model.UsersConnectionId.Last()');
//    //debugger;
//    axios.post('Home/ConnectToPrivateChat', bodyForm)
//        //axios.post(url,null)
//        .then(res => {
//            console.log("connect to Private Chat", res);
//        })
//        .catch(err => {
//            console.log("Failed to connect private Chat", err);
//        })
//}


//connection.start()
//    .then(function () {
//        connection.invoke('GetConnectionId')
//            .then(function (connectionId) {
//                _connectionId = connectionId
//                console.log("connected to homeHub");
//                connectPrivateChat();
//            })
//    })
//    .catch(function (err) {
//        console.log(err)
//    })


//var notifyAboutCreating = function (event) {
//    //if(event)
//    event.preventDefault();

//    //debugger;
//    var data = new FormData(event.target);
//    //data.append('connectionId', _connectionId);

//    //document.getElementById('message-input').value = '';

//    axios.post('Home/SendNotificationAboutCreatingChat', data)
//        .then(res => {
//            console.log("Notification delivered");
//            //debugger;
//        })
//        .catch(err => {
//            console.log("Failed to send notification");
//        })

//}