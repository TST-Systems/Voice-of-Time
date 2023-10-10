function createPrivateChatOrGroup (profileName, profilePicture = "./sources/Pictures/blank-profile-picture.svg", numberGroupMember = 0) {
    const chatElement = document.createElement("div");
    const profPicture = document.createElement("img");
    const profName = document.createElement("p");

    profPicture.src=profilePicture;
    profPicture.alt="avatar";

    profName.innerHTML = profileName;

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
}

function createChannel (profileName, profilePicture = "./sources/Pictures/blank-profile-picture.svg", numberGroupMember = 1) {
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
