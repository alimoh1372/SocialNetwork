var chatFormEl;
var requestAnchors;
var TableAllUsers;
var ulFriendsEl;
var inputSearchAllUsers;
var currentUserName = '';
var currentUserId = 0;
var container = document.getElementById('toBlur');
var roomListEl = document.getElementById('roomList');
var chatHistoryDiv = document.getElementById('chatHistory');
var activeUserIdToChat = 0;
var currentTableRowInAllUsersWorkOnIt;
var tableFriendOfFriends;
var tbodyFriendsOfFriend;
var mutualfriendNumber = 0;

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
chatConnection.on('updateRequestRowAddAcceptButton', updateRequestRowAddAcceptButton);

chatConnection.on('handleAfterAcceptedRequest', handleAfterAcceptedRequest);

chatConnection.on('ShowError', AlertError);


chatConnection.on('addNewMessageToChatHistoryUlEl', addNewMessageToChatHistoryUlEl);

chatConnection.on('EditMessage', EditMessage);



//EditMessage in the front
function EditMessage(message) {
    let editedMessage = message;
    if (!editedMessage && editedMessage.length == 0)
        return;
    if (!currentUserId && currentUserId == 0) {
        currentUserId = getCurrentUserId();
    }
    //Adding message to the chat history break each part to apart function such createBody,CreateImage,CreateMessageContent...

    let messageDiv = document.getElementById(message.id);
    if (!messageDiv && messageDiv.length == 0) {
        AlertError("the message dive not found");
        return;
    }
    let messageBodyText = messageDiv.getElementsByClassName('text')[0];

    //Delete text area after saving 
    messageBodyText.textContent = message.messageContent;
    let editMessageDiv = messageDiv.querySelector('div[data-editMessage]');
    editMessageDiv.remove();
}

//Add new message(Json) to chat history
function addNewMessageToChatHistoryUlEl(message) {
    let newMessage = JSON.parse(message);
    //Check if the active user id is that user send message then add message Item
    if ((currentUserId == newMessage.FkFromUserId && activeUserIdToChat == newMessage.FkToUserId) ||
        (currentUserId == newMessage.FkToUserId && activeUserIdToChat == newMessage.FkFromUserId)) {
        appendChatItem(newMessage);
    } else {
        AlertError("You Have a message from:" + newMessage.SenderFullName);
    }
}
//Handle the   handleAfterAcceptedRequest 

function handleAfterAcceptedRequest(userIdReuestSentFromIt, userIdRequestAcceptByIt,countMutualFriend) {
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
    mutualfriendNumber = countMutualFriend;
    ChangeTheStatusToFriend(currentTableRowInAllUsersWorkOnIt, countMutualFriend);
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
    newFiriendLiEl.classList.add("col-md-6");
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
    var html = newFiriendLiEl.innerHTML + '<div class="details"><div class="name row"><span class="col-md-8">' + _name + '</span><span class="col-md-4">Mutual Friend:'+mutualfriendNumber+'</span></div></div>' +
        '<div class="type row"><a data-sendMessage=' + _userId + ' class="btn btn-success" > <i class="fa fa-send" style="color: #9400d3"></i> Send Message</a ><a data-FriendsOfFriend="'+_userId+'" class="btn btn-primary"><i class="fa fa-users" style="color: #40e0d0"></i> Show friends</a></div>';
    newFiriendLiEl.innerHTML = html;


    if (ulFriendsEl && ulFriendsEl.length == 0) {
        ulFriendsEl = document.getElementById('UlFriends');
    }
    ulFriendsEl.appendChild(newFiriendLiEl);

    //Add eventListener to the tab button
    addEventListenerToFriendLiElMessageButton(newFiriendLiEl);
    //Add eventListener to the Show friend button 
    addEventListenerToFriendLiElShowFriendButton(newFiriendLiEl);
    
}


// Add eventListener to the Show friend button in friend Li
function addEventListenerToFriendLiElShowFriendButton(friendEl) {
    let anchorFriendsOfFriend = friendEl.querySelector('a[data-FriendsOfFriend]');
    let friendUserId = anchorFriendsOfFriend.getAttribute('data-FriendsOfFriend');
        if (!friendUserId)
            return;
    anchorFriendsOfFriend.addEventListener('click',
            (e) => {
                e.preventDefault();
                removeAllChildren(tbodyFriendsOfFriend);
                SwitchTabTo("#tab-FriendsOfFriend");
                getAllFriendsOf(friendUserId);
            });
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
    imgEl.src = message.FromUserProfilePicture;



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
    messageTimeDiv.setAttribute('data-utcdate', message.CreationDate);
    messageTimeDiv.classList.add('time', 'hidden-xs');
    messageTimeDiv.textContent = moment(message.CreationDate).fromNow();
    messageBodyDiv.appendChild(messageTimeDiv);

    let messageBodyText = document.createElement('div');
    messageBodyText.classList.add('text');
    messageBodyText.textContent = message.MessageContent;
    messageBodyDiv.appendChild(messageBodyText);

    //Set the Edit message Feature to message life is less than 3 minute

    var messageEditBtn;
    var editMessageContentDiv;
    let creationTimePlus3Minute = moment(message.CreationDate).add(3, 'minutes').toDate();
    let currentUserId = getCurrentUserId();
    if (creationTimePlus3Minute > new Date() && currentUserId == message.FkFromUserId) {
        let millisecondCanEdit = creationTimePlus3Minute.getTime() - new Date().getTime();
        messageEditBtn = document.createElement('a');
        messageEditBtn.href = "";
        messageEditBtn.classList.add('btn', 'btn-primary', 'pull-right');
        messageEditBtn.textContent = 'Edit Message';
        messageEditBtn.setAttribute('data-editMessage', message.Id);
        messageBodyDiv.appendChild(messageEditBtn);

        messageEditBtn.addEventListener('click',
            function (e) {
                e.preventDefault();
                editMessageContentDiv = document.createElement('div');
                editMessageContentDiv.classList.add('text');
                let textArea = document.createElement('textarea');
                textArea.value = message.MessageContent;
                editMessageContentDiv.appendChild(textArea);
                editMessageContentDiv.setAttribute('data-editMessage', message.Id);
                let messageSaveEditBtn = document.createElement('a');
                messageSaveEditBtn.classList.add('btn', 'btn-success', 'pull-right');
                messageSaveEditBtn.textContent = 'Save';
                messageSaveEditBtn.setAttribute('data-editMessage', message.Id);
                editMessageContentDiv.appendChild(messageSaveEditBtn);
                messageSaveEditBtn.addEventListener('click',
                    function (e) {
                        e.preventDefault();
                        let editMessageContent = textArea.value;
                        let messageId = message.Id;
                        chatConnection.invoke('EditMessage', messageId, editMessageContent);

                    });
                messageBodyDiv.appendChild(editMessageContentDiv);
            });
        setTimeout(() => {
            let editMessageItems = document.querySelectorAll('[data-editMessage="' + message.Id + '"]');
            let messageId = message.Id;
            for (var i = 0; i < editMessageItems.length; i++) {
                let currentItem = editMessageItems[i];
                if (currentItem.getAttribute('data-editMessage') == messageId) {
                    currentItem.remove();
                }
            }
        }, millisecondCanEdit);
    }

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
function updateRequestRowAddAcceptButton(userIdRequestSentFromIt,relationMessage) {
    //TODO:Adding message to the requestRevert
    const rows = document.querySelectorAll('tr[data-userid]');
    for (var i = 0; i < rows.length; i++) {
        let useridAtt = rows[i].getAttribute('data-userid');
        if (useridAtt != '' && Number(useridAtt) === userIdRequestSentFromIt) {
            currentTableRowInAllUsersWorkOnIt = rows[i];
            let tds = rows[i].getElementsByTagName('td');
           
            let requestMessage = relationMessage;
            tds[2].innerHTML = "Request Message:"+requestMessage;
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
            let requestInput = tds[2].querySelector('input[type="text"]');
            if (!requestInput && requestInput.length != 1) {
                AlertError("request inpute not found...Please call administrator");
                return;
            }
            let requestMessage = requestInput.value;
            tds[2].innerHTML = "YourMessage:"+requestMessage;
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
function sendRequestOfRelationShip(currentUserId, userRequestSendToIt,requestMessage) {

    chatConnection.invoke('SendUserRelationRequest', currentUserId, userRequestSendToIt,requestMessage);

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
//Add the friends of friend to its tab

function appendFriendsOfFriend(userFriends) {
    let friends = JSON.parse(userFriends);
    for (var i = 0; i < friends.length; i++) {
        let currentFriend = friends[i];
        appendFriendOfFriend(currentFriend);
    }
}



//Append a friend to list of friend in tab friend of friends
function appendFriendOfFriend(currentFriend) {
    let tableRow = document.createElement('tr');
    tableRow.appendChild(createTableData(currentFriend.UserId));
    tableRow.appendChild(createTableData(currentFriend.Name + " " + currentFriend.LastName));
    tbodyFriendsOfFriend.appendChild(tableRow);
}
function createTableData(text) {
    let td = document.createElement('td');
    td.textContent = text;
    return td;
}
//Create a td element with given textContent


//Get All friends of a friend and return the json string 
function getAllFriendsOf(userId) {

    $.ajax({
        type: "GET",
        url: "/ChatPage?handler=FriendsOfUser",
        data: {
            "userId": userId
        },
        success: function (response) {
            appendFriendsOfFriend(response);
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}

//Update every message time from now
function updateMessageTime() {
    var times = document.querySelectorAll("div[data-utcdate]");
    for (var i = 0; i < times.length; i++) {
        let time = times[i];
        let utcTime = time.getAttribute("data-utcdate");
        time.textContent = moment(utcTime).fromNow();
    }
}


//Doing on content loaded operations start hub connection get needed element and add eventlistener to some
function ready() {

    chatConnection.start();

    chatFormEl = document.getElementById('chatForm');
    ulFriendsEl = document.getElementById('UlFriends');
    currentUserId = getCurrentUserId();
    TableAllUsers = document.getElementById('TableAllUsers');
    inputSearchAllUsers = document.getElementById('searchAllUsers');
    tableFriendOfFriends = document.getElementById('tableFriendsOfFriend');
    tbodyFriendsOfFriend = tableFriendOfFriends.querySelector('tbody');
    let anchorsFriendsOfFriend = ulFriendsEl.querySelectorAll('a[data-FriendsOfFriend]');
    for (var i = 0; i < anchorsFriendsOfFriend.length; i++) {
        let friendUserId = anchorsFriendsOfFriend[i].getAttribute('data-FriendsOfFriend');
        if (!friendUserId)
            return;
        anchorsFriendsOfFriend[i].addEventListener('click',
            (e) => {
                e.preventDefault();
                removeAllChildren(tbodyFriendsOfFriend);
                SwitchTabTo("#tab-FriendsOfFriend");
                getAllFriendsOf(friendUserId);
            });
    }

    //Event listener to filter users when changed
    inputSearchAllUsers.addEventListener('input', (e) => {

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
            var trElRequestRelationRow = document.querySelector('tr[data-userid="' + userRequestSendToIt + '"]');
            if (!trElRequestRelationRow && trElRequestRelationRow.length!=1) {
                AlertError("Cant find the tr of user:" + userRequestSendToIt + "please call administrator");
                return;
            }
            //Get the message value
            let inputEl = trElRequestRelationRow.querySelector('input[type="text"]');
            let relationMessageValue = inputEl.value;


            sendRequestOfRelationShip(currentUserId, userRequestSendToIt, relationMessageValue);
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

    //Update message time every 1 minute
    setInterval(updateMessageTime, 60000);
}

document.addEventListener('DOMContentLoaded', ready);

