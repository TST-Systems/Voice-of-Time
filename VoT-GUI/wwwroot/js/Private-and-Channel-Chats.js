function createPrivateChatOrGroup(Chat_ID, Chat_Name, profilePicture = "./sources/Pictures/blank-profile-picture.svg", numberGroupMember = 0) {
    const chatElement = document.createElement("div");
    const profPicture = document.createElement("img");
    const profName = document.createElement("p");

    profPicture.src=profilePicture;
    profPicture.alt="avatar";

    profName.innerHTML = Chat_Name;

    chatElement.appendChild(profPicture);
    chatElement.appendChild(profName);
    profName.className = "profName";

    if (numberGroupMember != 0){
        const member = document.createElement("p");
        member.innerHTML = numberGroupMember + " members";
        chatElement.appendChild(member);
        member.className = "numberOfMembers";
    }

    const privatChat = document.getElementById("ChatList");

    privatChat.appendChild(chatElement);
    chatElement.id = Chat_ID;

    document.getElementById(Chat_ID).onclick = readChat(Chat_ID);
}

function createChannel (Chat_Name, profilePicture = "./sources/Pictures/blank-profile-picture.svg", numberGroupMember = 1) {
    const chatElement = document.createElement("div");
    const profPicture = document.createElement("img");
    const profName = document.createElement("p");

    profPicture.src=profilePicture;
    profPicture.alt="avatar";

    profName.innerHTML = profileName;

    chatElement.appendChild(profPicture);
    chatElement.appendChild(profName);
    profName.className = "profName";

    const member = document.createElement("p");
    member.innerHTML = numberGroupMember + " members";
    chatElement.appendChild(member);
    member.className = "numberOfMembers";

    const privatChat = document.getElementById("ChannelList");

    privatChat.appendChild(chatElement);
}

const chatlist = document.getElementById("ChatList");
chatlist.oncontextmenu =  () => {
    const contentMenu = document.getElementById("ContextMenuPrivateChat");
    contentMenu.style.display = "block";

    contentMenu.style.marginTop = "calc(" + String(mousePositionY) + "px - 50px)";
    contentMenu.style.marginLeft = String(mousePositionX) + "px";

    document.getElementById("CreateNewChat").onclick = () => {
        document.getElementById("ChatSettings").style.display = "block";

    }
}
chatlist.addEventListener("contextmenu", (e) => { e.preventDefault() });



document.getElementById("ButtonAddChat").onclick = () => {

}


function listChats() {
    ipcRenderer.send("listChats", "");
    var chatIDs;
    var ChatNames;

    ipcRenderer.on('listChats-replyIDs', (event, chat) => {
        
        chat.forEach((chatElement) => {
            console.log(chatElement.item1);
            console.log(chatElement.item2);
            createPrivateChatOrGroup(chatElement.item1, chatElement.item2);
        });
        
        
    });

    
}