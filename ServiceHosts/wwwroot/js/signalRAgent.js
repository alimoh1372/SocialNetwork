﻿


var chatFormEl;
var requestAnchors;
var TableAllUsers;
var ulFriendsEl;
var inputsearchAllUsers;
var currentUserName = '';
var currentUserId = 0;
var container = document.getElementById('toBlur');
var roomListEl = document.getElementById('roomList');
var roomHistoryEl = document.getElementById('chatHistory');
var currentTableRowInAllUsersWorkOnIt;
//Initial chatHubConnection
var chatConnection = new signalR.HubConnectionBuilder()
    .withUrl('/chatHub')
    .build()


chatConnection.onclose(function () {
    //Try to reconnect 5 time
    tryToReconnect();
});

function tryToReconnect() {
    var isError = false;
    var counter = 0;
    var error = '';

    while (isError || counter > 4) {

        setTimeout(function () {
            chatConnection.start()
                .catch((err) => {
                    error = err;
                });
        },
            5000);
        counter = counter + 1;
        isError = (error.length < 1);
    }
    if (isError) {
        alert(error + '\n Please call admin... Or try later...');
    } else {
        window.location.reload(true);
    }
}


//Update Table of user that sent request after it done
//Give the id of user that request sent for it
chatConnection.on('updateRequestRowAddPending', updateRequestRowAddPendingToIt);

//Update requested user row in AllUsers Table and add Accept button to it
//Give the id of user that send relationship request
chatConnection.on('updateRequestRowAddAcceptButton', updateRequestRowAddAcceptButton)

chatConnection.on('handleAfterAcceptedRequest', handleAfterAcceptedRequest);

chatConnection.on('ShowError', AlertError);

//Handle the   handleAfterAcceptedRequest 

function handleAfterAcceptedRequest(userIdReuestSentFromIt, userIdRequestAcceptByIt) {
    //check if currentTableRowInAllUsersWorkOnIt isn't null or more than one
    //if it is so set currentTableRowInAllUsersWorkOnIt
    if (!(currentTableRowInAllUsersWorkOnIt && currentTableRowInAllUsersWorkOnIt.length == 1)) {
        let rows = document.querySelectorAll('tr[data-userid]');

        for (var i = 0; i < rows.length; i++) {
            let useridAtt = rows[i].getAttribute('data-userid');
            if (useridAtt != ''
                && (Number(useridAtt) == userIdReuestSentFromIt)
                || (Number(useridAtt) == userIdRequestAcceptByIt)) {
                currentTableRowInAllUsersWorkOnIt = rows[i];
                break;
            }
        }
    }
    ChangeTheStatusToFriend(currentTableRowInAllUsersWorkOnIt);
}

//Change the status of row to the accepted and its styles
function ChangeTheStatusToFriend(_currentTableRowInAllUsersWorkOnIt) {
    let useridAtt = _currentTableRowInAllUsersWorkOnIt.getAttribute('data-userid');
    if (!useridAtt) {
        AlertError("There isn't any row to work");
        return;
    }
    let tds = _currentTableRowInAllUsersWorkOnIt.getElementsByTagName('td');
    tds[2].innerHTML = " You already friend";
    tds[3].innerHTML = ' <i class="fa fa-check-circle"></i> <span class="label label-success"> Friend</span>';
    tds[4].innerHTML = '';
    let name = tds[1].textContent.trim();

    AddNewItemToFriends(name, useridAtt);


}

//method Add new friend on friend tab that on page load functionaly add accepted users to the friend tab

function AddNewItemToFriends(_name, _userId) {

    let newFiriendLiEl = document.createElement('li');
    newFiriendLiEl.classList.add("col-md-3");
    newFiriendLiEl.setAttribute('data-friendUserId', _userId);
    var div = document.createElement('div');
    div.classList.add('img');
    var image = document.createElement('img');
    image.classList.add("img-responsive");
    image.alt = "Profile Picture";
    image.title = "Profile Picture";
    image.src = "/Images/DefaultProfile.png";
    div.appendChild(image);
    newFiriendLiEl.appendChild(div);
    var html = newFiriendLiEl.innerHTML + '<div class="details"><div class="name"><span>' + _name + '</span></div></div>' +
        '<div class="type"><a data-sendMessage=' + _userId + ' class="btn btn-success" > <i class="fa fa-send" style="color: #9400d3"></i> Send Message</a ></div>';
    newFiriendLiEl.innerHTML = html;


    if (ulFriendsEl && ulFriendsEl.length == 0) {
        ulFriendsEl = document.getElementById('UlFriends');
    }
    ulFriendsEl.appendChild(newFiriendLiEl);

    //Add eventListener to the tab button
    addEventListenerToFriendLiElMessageButton(newFiriendLiEl);
}









function addEventListenerToFriendLiElMessageButton(_newFiriendLiEl) {
    var messageButton = _newFiriendLiEl.querySelector('a[data-sendMessage]')
    messageButton.addEventListener('click',
        function (e) {
            e.preventDefault();
            activeUserIdToChat = Number(messageButton.getAttribute('data-sendMessage'));
            if (!activeUserIdToChat)
                return;

            //switch on tab chat 
            SwitchTabTo('#tab-chat');

            //Load Chat history
            let chatHistoryDiv = document.getElementById("chatHistory");

            //Remove chat history that load befor
            removeAllChildren(chatHistoryDiv);

            //Load current user and friend chat history
            LoadChat(currentUserId, activeUserToChat);
        });
}
function LoadChat(_currentUserId, _activeUserToChat) {

    $.ajax({
        type: "GET",
        url: "/ChatPage?handler=LoadChatHistory",
        data: {
            "currentUserId": _currentUserId,
            "activeUserToChat": _activeUserToChat
        },
        success: function (response) {
            appendChatItems(response);
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}
//Append list of message to the chat history place
function appendChatItems(chatItems)
{
    let chatItemsJs = JSON.parse(chatItems)
    if (!chatItemsJs)
        AlertError('There is error On ajaxRequest request...');
    chatItemsJs.forEach((_chatMessage) => {
        //Add a message item to the chat history ul
        appendChatItem(_chatMessage);
    });
}

function appendChatItem(message) {
    if (!message && message.length == 0)
        return;
    if (!currentUserId && currentUserId==0) {
        currentUserId = getCurrentUserId();
    }
    //ToDo:Implementing Adding message to the chat history break each part to apart function such createBody,CreateImage,...

}





//Remove All Chat History
function removeAllChildren(_node) {
    if (!_node) return;

    while (_node.lastChild) {
        _node.removeChild(_node.lastChild);
    }
}
var activeUserToChat;
//switch active tab to the given tab
function SwitchTabTo(liHrefAtt) {
    var divTabToActive = document.getElementById('divTabToActive');
    let tabs = divTabToActive.getElementsByTagName('li');
    for (var i = 0; i < tabs.length; i++) {
        let aEl = tabs[i].querySelector('a');
        let href = aEl.getAttribute('href')
        if (href == liHrefAtt)
            aEl.click();
    }
}

//Update All user table after request sent and be done successfully 
//on user page that request sent to it 
//Give the user id that request create by it
function updateRequestRowAddAcceptButton(userIdRequestSentFromIt) {
    const rows = document.querySelectorAll('tr[data-userid]');
    for (var i = 0; i < rows.length; i++) {
        let useridAtt = rows[i].getAttribute('data-userid');
        if (useridAtt != '' && Number(useridAtt) === userIdRequestSentFromIt) {
            currentTableRowInAllUsersWorkOnIt = rows[i];
            let tds = rows[i].getElementsByTagName('td');
            tds[2].innerHTML = "Requested relation to you";
            tds[3].innerHTML = '<i class="text-warning fa fa-question-circle"></i> <span class="label label-warning">Request Came</span>';
            tds[4].innerHTML = '<td><a class="btn btn-success" data-acceptRequest=' + userIdRequestSentFromIt + '" href="">Accept</a></td>';
            let acceptButton = tds[4].querySelector('a[data-acceptRequest]');
            if (acceptButton && acceptButton.length > 0)
                acceptButton.addEventListener('click', function (e) {
                    if (currentUserId <= 0)
                        currentUserId = getCurrentUserId();

                    handleAcceptRequestButton(currentUserId, userIdRequestSentFromIt);
                });
            break;
        }
    }
}

//Update current user row after send relation request on request sended to it
function updateRequestRowAddPendingToIt(userIdRequestFromIt) {
    const rows = document.querySelectorAll('tr[data-userid]')
    //let rows = document.querySelectorAll("tr[data-userid]");
    for (var i = 0; i < rows.length; i++) {
        let useridAtt = rows[i].getAttribute('data-userid');
        if (useridAtt != '' && Number(useridAtt) === userIdRequestFromIt) {
            currentTableRowInAllUsersWorkOnIt = rows[i];
            let tds = rows[i].getElementsByTagName('td');
            tds[2].innerHTML = "You Sent Request";
            tds[3].innerHTML = 'Request <span class="lable lable-warning">Pending...</span>';
            tds[4].innerHTML = '';
            break;
        }
    }
}

function AlertError(message) {
    alert(message);
}

//InvokeSend Request from current user to the user Request Send To It
function sendRequestOfRelationShip(currentUserId, userRequestSendToIt) {

    chatConnection.invoke('SendUserRelationRequest', currentUserId, userRequestSendToIt);

}


function handleAcceptRequestButton(_currentUserId, _userIdRequestSentFromIt) {
    chatConnection.invoke('AcceptRequest', _currentUserId, _userIdRequestSentFromIt);
}

//Get The current user Id that sign in
function getCurrentUserId() {
    return Number(document.getElementById('currentUser').getAttribute('data-currentUserId'));
}

//Filter users that userName contains givent name
function filterUsersBy(name) {
    if (TableAllUsers && TableAllUsers.length)
        TableAllUsers = document.getElementById('TableAllUsers');

    let tRows = TableAllUsers.getElementsByTagName('tbody')[0].getElementsByTagName('tr');
    for (var i = 0; i < tRows.length; i++) {
        let currentRow = tRows[0];

        //Get name of use in table row
        let tdName = currentRow.getElementsByTagName('td')[1];
        let currentNameofUser = tdName.textContent.trim();

        if (!currentNameofUser.includes(name)) {
            tRows[i].style.display = 'none';
        }
        else {
            tRows[i].style.display = '';
        }
    }

}


//Doing on content loaded operations start hub connection get needed element and add eventlistener to some
function ready() {

    chatConnection.start();

    chatFormEl = document.getElementById('chatForm');
    ulFriendsEl = document.getElementById('UlFriends');
    currentUserId = getCurrentUserId();
    TableAllUsers = document.getElementById('TableAllUsers');
    inputsearchAllUsers = document.getElementById('searchAllUsers');

    //Event listener to filter users when changed
    inputsearchAllUsers.addEventListener('input', (e) => {

        let text = e.target.value;
        filterUsersBy(text);
    });

    //TODO:AddEventListener to the sendMessage in tab friends




    //AddEventListener to all request anchor

    requestAnchors = document.querySelectorAll('a[data-requestedUserId]');
    requestAnchors.forEach((item) => {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            var userRequestSendToIt = Number(e.target.getAttribute('data-requestedUserId'));
            debugger;
            if (currentUserId == 0)
                currentUserId = getCurrentUserId();
            sendRequestOfRelationShip(currentUserId, userRequestSendToIt);
        });
    });
    //AddEventListener to all request revert pending to handle accept button operation
    var acceptAnchors = document.querySelectorAll('a[data-acceptRequest]');
    acceptAnchors.forEach((el) => {
        el.addEventListener('click', function (e) {
            e.preventDefault();
            let userIdRequestFrom = Number(e.target.getAttribute('data-acceptRequest'))
            if (!userIdRequestFrom)
                return;
            if (!currentUserId)
                currentUserId = getCurrentUserId();
            handleAcceptRequestButton(currentUserId, userIdRequestFrom);
        })
    });
}

document.addEventListener('DOMContentLoaded', ready);














//function loadChatList(userNameList) {
//    debugger;
//    if (!userNameList.length)
//        return;
//    const userNames = JSON.parse(userNameList);
//    loadRooms(userNames);
//}



//function sendMessage(text) {
//    if (text && text.length) {
//        // TODO: Send an agent message
//    }
//}



//function switchActiveRoomTo(id) {
//    if (id === activeRoomId) return;

//    if (activeRoomId) {
//        // TODO: Leave the room
//    }

//    activeRoomId = id;
//    removeAllChildren(roomHistoryEl);

//    if (!id) return;

//    // TODO: Join the room
//    // TODO: Load the room history
//}




////roomListEl.addEventListener('click', function (e) {
////    roomHistoryEl.style.display = 'block';

////    setActiveRoomButton(e.target);

////    var roomId = e.target.getAttribute('data-id');
////    switchActiveRoomTo(roomId);
////});

//function setActiveRoomButton(el) {
//    var allButtons = roomListEl.querySelectorAll('a.list-group-item');

//    allButtons.forEach(function (btn) {
//        btn.classList.remove('active');
//    });

//    el.classList.add('active');
//}

//function loadRooms(rooms) {
//    if (!rooms.length) return;


//    switchActiveRoomTo(null);
//    removeAllChildren(roomListEl);

//    for (var i = 0; i < rooms.length; i++) {
//        var item = rooms[i];
//        var currentId = item.Id;
//        var currentName = item.UserName;
//        if (!currentName) continue;

//        var roomButton = createRoomButton(currentId, currentName);
//        roomListEl.appendChild(roomButton);
//    }
//    //rooms.forEach(function (item) {
//    //    var currentId = item.id;
//    //    var currentName = item.name;
//    //    if (!currentName) return;

//    //    var roomButton = createRoomButton(currentId, currentName);
//    //    roomListEl.appendChild(roomButton);
//    //});
//}

//function createRoomButton(id, userName) {
//    var anchorEl = document.createElement('a');
//    anchorEl.className = 'list-group-item list-group-item-action d-flex justify-content-between align-items-center';
//    anchorEl.setAttribute('data-id', id);
//    anchorEl.textContent = userName;
//    anchorEl.href = '#';

//    return anchorEl;
//}

//function addMessages(messages) {
//    if (!messages) return;

//    messages.forEach(function (m) {
//        addMessage(m.senderName, m.sentAt, m.text);
//    });
//}

//function addMessage(name, time, message) {
//    var nameSpan = document.createElement('span');
//    nameSpan.className = 'name';
//    nameSpan.textContent = name;

//    var timeSpan = document.createElement('span');
//    timeSpan.className = 'time';
//    var friendlyTime = moment(time).format('H:mm');
//    timeSpan.textContent = friendlyTime;

//    var headerDiv = document.createElement('div');
//    headerDiv.appendChild(nameSpan);
//    headerDiv.appendChild(timeSpan);

//    var messageDiv = document.createElement('div');
//    messageDiv.className = 'message';
//    messageDiv.textContent = message;

//    var newItem = document.createElement('li');
//    newItem.appendChild(headerDiv);
//    newItem.appendChild(messageDiv);

//    roomHistoryEl.appendChild(newItem);
//    roomHistoryEl.scrollTop = roomHistoryEl.scrollHeight - roomHistoryEl.clientHeight;
//}

//function removeAllChildren(node) {
//    if (!node) return;

//    while (node.lastChild) {
//        node.removeChild(node.lastChild);
//    }
//}

/*chatConnection.on('responseSetName', setNameResult);*/
//chatConnection.on('ReceiveMessage', addMessage);
//chatConnection.on('loadUserNamesChatWithThem', loadChatList);
// TODO: Initialize hub connections


////make a function to start connection 

//function S() {
//    var conresult = '';
//    chatConnection.start()
//        .catch(function (err) {
//            console.alert("!!!Atention Error:\n" + err);
//            conresult = err;
//        });
//    if (conresult.length > 0) {

//    } else {
//        openChatSegment();
//    }

//}

//make a handle connection when connection is disconnected
//chatConnection.onclose(function () {
//    handleDisconnected();
//});

////handle changes of page after connection started
//function openChatSegment() {

//    var enterYourNameWarningEl = document.getElementById('enterNameInfo');
//    enterYourNameWarningEl.style.display = 'none';
//}

//public method give handler to retry connect again to server
//function handleDisconnected(retryFunc) {
//    container.classList.add('blured');
//    console.log('Reconnecting in 5 seconds...');
//    setTimeout(retryFunc, 5000);
//}

//function setNameResult(result) {
//    if (result) {
//        //if the user exist before
//        //ToDO: implement add history of chat
//    } else {
//        //if the user isn't exit before
//        const message = 'Hi Dear ' + currentUserName + ' welcome to the ChatRoom';
//        const timeOffsetUtcNow = new Date().getTimezoneOffset(0);
//        addMessage(currentUserName, timeOffsetUtcNow, message);
//    }
//}
//UserForm.addEventListener('submit', function (e) {
//    e.preventDefault();
//    chatConnection.invoke('SetName', currentUserName)
//        .catch((err) => alert("An exception accord on server side..."));
//    openChatSegment();
//});
