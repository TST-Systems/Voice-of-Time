function sendMessage(name = "Unbekannt", picture = "./sources/Pictures/blank-profile-picture.png") {
    let messageValue = document.getElementById("MessageContent").value;

    const message = document.createElement("div");
    const profileImage = document.createElement("img");
    const messageData = document.createElement("div");
    const profileName = document.createElement("p");
    const messageContent = document.createElement("p");
    const timeElement = document.createElement("p");
    

    if (messageValue != ""){
        messageContent.innerHTML = messageValue;
        profileImage.src = picture;
        profileName.innerHTML = name;

        var currentdate = new Date(); 
        time = currentdate.getDate() + "."
                + (currentdate.getMonth()+1)  + "." 
                + currentdate.getFullYear() + " "  
                + currentdate.getHours() + ":"  
                + currentdate.getMinutes() + ":" 
                + currentdate.getSeconds();

        timeElement.innerHTML = time;

        message.appendChild(profileImage);

        messageData.appendChild(profileName);
        messageData.appendChild(messageContent);
        messageData.appendChild(timeElement);
        message.appendChild(messageData);

        profileImage.className = "sendMessageProfileImg";

        messageData.className = "sendMessageData"
        profileName.className = "sendMessageProfileName";
        messageContent.className = "sendMessageContent";
        timeElement.className = "sendMessageTime";

        profileImage.alt = "avatar";

        document.getElementById("TextChat").appendChild(message);
        message.className = "sendMessage";

       //Clear Message
        document.getElementById("MessageContent").value = ''; 

        //set scrollbar on Bottom
        var objDiv = document.getElementById("TextChat");
        objDiv.scrollTop = objDiv.scrollHeight;
    }
} 

function dispatchedMessage(messageValue, name = "Unbekannt", picture = "./sources/Pictures/blank-profile-picture.png", time = "01.01.2000 0:00") {
    const message = document.createElement("div");
    const profileImage = document.createElement("img");
    const messageData = document.createElement("div");
    const profileName = document.createElement("p");
    const messageContent = document.createElement("p");
    const timeElement = document.createElement("p");
    

    if (messageValue != ""){
        messageContent.innerHTML = messageValue;
        profileImage.src = picture;
        profileName.innerHTML = name;
        timeElement.innerHTML = time;

        message.appendChild(profileImage);

        messageData.appendChild(profileName);
        messageData.appendChild(messageContent);
        messageData.appendChild(timeElement);
        message.appendChild(messageData);

        profileImage.className = "sendMessageProfileImg";

        messageData.className = "sendMessageData"
        profileName.className = "sendMessageProfileName";
        messageContent.className = "sendMessageContent";
        timeElement.className = "sendMessageTime";

        profileImage.alt = "avatar";

        document.getElementById("TextChat").appendChild(message);
        message.className = "sendMessage";

       //Clear Message
        document.getElementById("MessageContent").value = ''; 
    }
} 

function receiveMessage (messageValue, name = "Unbekannt", picture = "./sources/Pictures/blank-profile-picture.png", time = "01.01.2000 0:00") {
    const message = document.createElement("div");
    const profileImage = document.createElement("img");
    const messageData = document.createElement("div");
    const profileName = document.createElement("p");
    const messageContent = document.createElement("p");
    const timeElement = document.createElement("p");

    if (messageValue != ""){
        messageContent.innerHTML = messageValue;
        profileImage.src = picture;
        profileName.innerHTML = name;
        timeElement.innerHTML = time;

        message.appendChild(profileImage);

        messageData.appendChild(profileName);
        messageData.appendChild(messageContent);
        messageData.appendChild(timeElement);

        message.appendChild(messageData);

        profileImage.className = "receiveMessageProfileImg";

        messageData.className = "receiveMessageData";
        profileName.className = "receiveMessageProfileName";
        messageContent.className = "receiveMessageContent";
        timeElement.className = "receiveMessageTime";

        profileImage.alt = "avatar";
        
        document.getElementById("TextChat").appendChild(message);
        message.className = "receiveMessage";
       //Clear Message
        document.getElementById("MessageContent").value = ''; 
    }
}

