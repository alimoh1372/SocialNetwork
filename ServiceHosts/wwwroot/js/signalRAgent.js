var chatFormEl;
var requestAnchors;
var TableAllUsers;
var ulFriendsEl;
var inputsearchAllUsers;
var currentUserName = '';
var currentUserId = 0;
var container = document.getElementById('toBlur');
var roomListEl = document.getElementById('roomList');
var chatHistoryDiv = document.getElementById('chatHistory');
var activeUserIdToChat = 0;
var currentTableRowInAllUsersWorkOnIt;


//Initial chatHubConnection
var chatConnection = new signalR.HubConnectionBuilder()
    .withUrl('/chatHub')
    .build();

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


chatConnection.on('addNewMessageToChatHistoryUlEl', addNewMessageToChatHistoryUlEl);



//Add new message(Json) to chat history
function addNewMessageToChatHistoryUlEl(message) {
    let newMessage = JSON.parse(message);
    appendChatItem(newMessage);
}
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









function addEventListenerToFriendLiElMessageButton(_newFriendLiEl) {
    var messageButton = _newFriendLiEl.querySelector('a[data-sendMessage]');
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
            LoadChat(currentUserId, activeUserIdToChat);
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
function appendChatItems(chatItems) {
    let chatItemsJs = JSON.parse(chatItems);
    if (!chatItemsJs)
        AlertError('There is error On ajaxRequest request...');
    chatItemsJs.forEach((_chatMessage) => {
        //Add a message item to the chat history ul
        appendChatItem(_chatMessage);
    });
    chatHistoryDiv = document.getElementById('chatHistory');
    let items = chatHistoryDiv.querySelectorAll('div.conversation-item');
    const latestItem = items[items.length - 1];
    if (latestItem && latestItem.length > 0)
        latestItem.scrollIntoView(true);



}

//Appand New Message to chat history
function appendChatItem(message) {
    if (!message && message.length == 0)
        return;
    if (!currentUserId && currentUserId == 0) {
        currentUserId = getCurrentUserId();
    }
    //Adding message to the chat history break each part to apart function such createBody,CreateImage,CreateMessageContent...

    let messageDiv = CreateMessageDivEl(message);

    messageDiv.appendChild(CreateUserDiv(message));

    messageDiv.appendChild(CreateMessageBoyDiv(message));

    chatHistoryDiv = document.getElementById('chatHistory');
    chatHistoryDiv.appendChild(messageDiv);

    messageDiv.scrollIntoView();

}


//Create message Div element using message object(id,FkFromUserId,FromUserFullName,FkToUserId,ReceiverFullName,MessageContent);
function CreateMessageDivEl(message) {
    let messageDiv = document.createElement('div');
    messageDiv.id = message.Id;
    if (!currentUserId)
        currentUserId = getCurrentUserId();
    if (message.FkFromUserId == currentUserId) {
        messageDiv.classList.add('conversation-item', 'item-right', 'clearfix');
    } else {
        messageDiv.classList.add('conversation-item', 'item-left', 'clearfix');
    }
    messageDiv.setAttribute('data-messageId', message.Id);
    return messageDiv;

}



//Create User Div image of user
function CreateUserDiv(message) {
    let userDiv = document.createElement('div');
    userDiv.classList.add('conversation-user');
    let imgEl = document.createElement('img');
    imgEl.src = '/Images/DefaultProfile.png';
    imgEl.classList.add('img-responsive');
    userDiv.appendChild(imgEl);
    return userDiv;
}
//Create message Body of div name,time,message content
function CreateMessageBoyDiv(message) {
    let messageBodyDiv = document.createElement('div');
    messageBodyDiv.classList.add('conversation-body');

    let messageNameDiv = document.createElement('div');
    messageNameDiv.classList.add('name');
    messageNameDiv.textContent = message.SenderFullName;
    messageBodyDiv.appendChild(messageNameDiv);
    let messageTimeDiv = document.createElement('div');
    messageTimeDiv.classList.add('time', 'hidden-xs');
    messageTimeDiv.textContent = moment(message.CreationDate).fromNow();
    messageBodyDiv.appendChild(messageTimeDiv);

    let messageBodyText = document.createElement('div');
    messageBodyDiv.classList.add('text');
    messageBodyText.textContent = message.MessageContent;
    messageBodyDiv.appendChild(messageBodyText);
    return messageBodyDiv;
}

//Remove All Chat History
function removeAllChildren(_node) {
    if (!_node) return;

    while (_node.lastChild) {
        _node.removeChild(_node.lastChild);
    }
}

//switch active tab to the given tab
function SwitchTabTo(liHrefAtt) {
    var divTabToActive = document.getElementById('divTabToActive');
    let tabs = divTabToActive.getElementsByTagName('li');
    for (var i = 0; i < tabs.length; i++) {
        let aEl = tabs[i].querySelector('a');
        let href = aEl.getAttribute('href');
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
        let currentRow = tRows[i];

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

    //AddEventListener to the sendMessage in tab friends
    let liElFriends = ulFriendsEl.querySelectorAll('li[data-friendUserId]');
    for (var i = 0; i < liElFriends.length; i++) {
        let currentLiFriend = liElFriends[i];
        addEventListenerToFriendLiElMessageButton(currentLiFriend);
    }

    //Add EventListener to the sendMessage button in tab chat

    chatFormEl.addEventListener('submit', function (e) {
        e.preventDefault();
        if (!activeUserIdToChat || activeUserIdToChat == 0)
            return;
        let text = e.target[0].value;
        e.target[0].value = "";
        if (!text && text.length == 0) {
            AlertError('Please Enter your message...');
            return;
        }

        chatConnection.invoke('SendMessage', currentUserId, activeUserIdToChat, text)
            .catch((err) => alert("An exception accord on sending message from server side..."));

    });
    //AddEventListener to all request anchor

    requestAnchors = document.querySelectorAll('a[data-requestedUserId]');
    requestAnchors.forEach((item) => {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            var userRequestSendToIt = Number(e.target.getAttribute('data-requestedUserId'));

            if (currentUserId == 0)
                currentUserId = getCurrentUserId();
            sendRequestOfRelationShip(currentUserId, userRequestSendToIt);
        });
    });
    //AddEventListener to all request revert pending to handle accept button operation
    var acceptAnchors = document.querySelectorAll('a[data-acceptRequest]');
    acceptAnchors.forEach((el) => {
        el.addEventListener('click',
            function (e) {
                e.preventDefault();
                let userIdRequestFrom = Number(e.target.getAttribute('data-acceptRequest'));
                if (!userIdRequestFrom)
                    return;
                if (!currentUserId)
                    currentUserId = getCurrentUserId();
                handleAcceptRequestButton(currentUserId, userIdRequestFrom);
            });
    });
}

document.addEventListener('DOMContentLoaded', ready);

