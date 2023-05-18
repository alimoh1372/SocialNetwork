
//var activeRoomId = '';
var chatFormEl;
var requestAnchors;
/*var UserForm = document.getElementById('userDefine');*/
var currentUserName = '';
var currentUserId = 0;
var container = document.getElementById('toBlur');
var roomListEl = document.getElementById('roomList');
var roomHistoryEl = document.getElementById('chatHistory');
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
//UserForm.addEventListener('submit', function (e) {
//    e.preventDefault();
//    chatConnection.invoke('SetName', currentUserName)
//        .catch((err) => alert("An exception accord on server side..."));
//    openChatSegment();
//});
//Update the row of request by row request id
//chatConnection.on('updateRequestRow', updateRequestStatusToPending);
chatConnection.on('updateRequestRowAddAcceptButton', updateRequestRowAddAcceptButton)

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


//Update current user row after send relation request on request sended to it
function updateRequestRowAddAcceptButton(userIdRequestSentTo) {
    //let rows = document.querySelectorAll('tr[data-userid]')
    let rows = document.querySelectorAll("tr[data-userid]");
    rows.forEach(function(tr){
        let useridAtt = tr.getAttribute('data-userid');
        if (useridAtt != '' && Number(useridAtt) == userIdRequestSentTo) {
            let tds = row[0].getElementsByTagName('td');
            tds[2].innerHTML = "Requested relation to you";
            tds[3].innerHTML = '<i class="text-warning fa fa-question-circle"></i> <span class="label label-warning">Request Came</span>';
            tds[4].innerHTML = '<td><a class="btn btn-success" data-acceptRequest=' + userIdRequestSentTo + '" href="">Accept</a></td>';
            return;
        }
    });
    
   
}

//Update given user Id row in request table to the request pending
function updatePendingRequestRow(userId) {
    var rows = document.querySelectorAll('tr[data-userid]')
    // var row = document.querySelector('tr[data-userid=' + CSS.escape(userIdRequestSentTo) + ']');

    var tds = row.querySelectorAll('td');
    tds[2].innerHTML = "You Sent Request";
    td[3].innerHTML = 'Request <span class="label label-warning">Pending...</span>';
    td[4].innerHTML = '';
}

function loadChatList(userNameList) {
    debugger;
    if (!userNameList.length)
        return;
    const userNames = JSON.parse(userNameList);
    loadRooms(userNames);
}

function sendMessage(text) {
    if (text && text.length) {
        // TODO: Send an agent message
    }
}
function getCurrentUserId() {
    return Number(document.getElementById('currentUser').getAttribute('data-currentUserId'));
}
function ready() {
    // TODO: Start the hub connections

    chatConnection.start();

    var chatFormEl = document.getElementById('chatForm');

    currentUserId = getCurrentUserId();
    chatFormEl.addEventListener('submit', function (e) {
        e.preventDefault();
        debugger;
        //var text = e.target[0].value;
        //e.target[0].value = '';
        //sendMessage(text);
    });

    var ulFriendEl = document.getElementById('UlFriends');

    //AddEventListener to all request anchor

    requestAnchors = document.querySelectorAll('a[data-requestedUserId]');
    for (var i = 0; i < requestAnchors.linkToRequest; i++) {

    }
    requestAnchors.forEach((item) => {
        item.addEventListener('click', function (e) {

            e.preventDefault();
            var userRequestSendToIt = Number(e.target.getAttribute('data-requestedUserId'));
            if (currentUserId == 0)
                currentUserId = getCurrentUserId();
            linkToRequest(currentUserId, userRequestSendToIt);



        });
    });
}
function linkToRequest(currentUserId, userRequestSendToIt) {

    chatConnection.invoke('SendUserRelationRequest', currentUserId, userRequestSendToIt);

}
function switchActiveRoomTo(id) {
    if (id === activeRoomId) return;

    if (activeRoomId) {
        // TODO: Leave the room
    }

    activeRoomId = id;
    removeAllChildren(roomHistoryEl);

    if (!id) return;

    // TODO: Join the room
    // TODO: Load the room history
}




//roomListEl.addEventListener('click', function (e) {
//    roomHistoryEl.style.display = 'block';

//    setActiveRoomButton(e.target);

//    var roomId = e.target.getAttribute('data-id');
//    switchActiveRoomTo(roomId);
//});

function setActiveRoomButton(el) {
    var allButtons = roomListEl.querySelectorAll('a.list-group-item');

    allButtons.forEach(function (btn) {
        btn.classList.remove('active');
    });

    el.classList.add('active');
}

function loadRooms(rooms) {
    if (!rooms.length) return;


    switchActiveRoomTo(null);
    removeAllChildren(roomListEl);

    for (var i = 0; i < rooms.length; i++) {
        var item = rooms[i];
        var currentId = item.Id;
        var currentName = item.UserName;
        if (!currentName) continue;

        var roomButton = createRoomButton(currentId, currentName);
        roomListEl.appendChild(roomButton);
    }
    //rooms.forEach(function (item) {
    //    var currentId = item.id;
    //    var currentName = item.name;
    //    if (!currentName) return;

    //    var roomButton = createRoomButton(currentId, currentName);
    //    roomListEl.appendChild(roomButton);
    //});
}

function createRoomButton(id, userName) {
    var anchorEl = document.createElement('a');
    anchorEl.className = 'list-group-item list-group-item-action d-flex justify-content-between align-items-center';
    anchorEl.setAttribute('data-id', id);
    anchorEl.textContent = userName;
    anchorEl.href = '#';

    return anchorEl;
}

function addMessages(messages) {
    if (!messages) return;

    messages.forEach(function (m) {
        addMessage(m.senderName, m.sentAt, m.text);
    });
}

function addMessage(name, time, message) {
    var nameSpan = document.createElement('span');
    nameSpan.className = 'name';
    nameSpan.textContent = name;

    var timeSpan = document.createElement('span');
    timeSpan.className = 'time';
    var friendlyTime = moment(time).format('H:mm');
    timeSpan.textContent = friendlyTime;

    var headerDiv = document.createElement('div');
    headerDiv.appendChild(nameSpan);
    headerDiv.appendChild(timeSpan);

    var messageDiv = document.createElement('div');
    messageDiv.className = 'message';
    messageDiv.textContent = message;

    var newItem = document.createElement('li');
    newItem.appendChild(headerDiv);
    newItem.appendChild(messageDiv);

    roomHistoryEl.appendChild(newItem);
    roomHistoryEl.scrollTop = roomHistoryEl.scrollHeight - roomHistoryEl.clientHeight;
}

function removeAllChildren(node) {
    if (!node) return;

    while (node.lastChild) {
        node.removeChild(node.lastChild);
    }
}

document.addEventListener('DOMContentLoaded', ready);