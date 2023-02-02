function createPrivateChatOrGroup (profileName, profilePicture = "./sources/Pictures/blank-profile-picture.png", numberGroupMember = 0) {
    const chatElement = document.createElement("div");
    const profPicture = document.createElement("img");
    const profName = document.createElement("p");

    profPicture.src=profilePicture;
    profPicture.alt="avatar";

    profName.innerHTML = profileName;

    if (numberGroupMember != 0){
        let member = document.createElement("p");
        member.innerHTML = numberGroupMember + " members";
        profName.appendChild(member);
    }

    chatElement.appendChild(profPicture);
    chatElement.appendChild(profName);

    const privatChat = document.getElementById("ChatList");

    privatChat.appendChild(chatElement);
}

function createChannel (profileName, profilePicture = "./sources/Pictures/blank-profile-picture.png", numberGroupMember = 1) {
    const chatElement = document.createElement("div");
    const profPicture = document.createElement("img");
    const profName = document.createElement("p");

    profPicture.src=profilePicture;
    profPicture.alt="avatar";

    profName.innerHTML = profileName;

    let member = document.createElement("p");
    member.innerHTML = numberGroupMember + " members";
    profName.appendChild(member);
    

    chatElement.appendChild(profPicture);
    chatElement.appendChild(profName);

    const privatChat = document.getElementById("ChannelList");

    privatChat.appendChild(chatElement);
}
