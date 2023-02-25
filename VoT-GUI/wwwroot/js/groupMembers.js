function showChatMembers(){
    const chat = document.getElementById("Chat");
    const button = document.getElementById("showHideGroupMemberButton");
    const chatMemberBox = document.getElementById("GroupMerbers");
    const image = document.getElementById("blueArrow");

    //Arrow points to the right
    image.style.transform = "scaleX(-1)";
    
    //animation
    var animationTime = "1s";
    chat.style.animationDuration = animationTime;
    button.style.animationDuration = animationTime;
    chatMemberBox.style.animationDuration = animationTime;

    //chat.style.animationFillMode = "forwards";
    //button.style.animationFillMode = "forwards";
    //chatMemberBox.style.animationFillMode = "forwards";

    chat.style.animationName = "decreaseChat";
    button.style.animationName = "moveButtonLeft";
    chatMemberBox.style.animationName = "showChatMebers";

    chat.addEventListener("animationend", () => {chat.style.width = "calc(100% - 230px - 50px - 230px)";});
    button.addEventListener("animationend", () => {button.style.marginLeft = "calc(100% - 1vh - 40px - 230px)";});
    chatMemberBox.addEventListener("animationend", () => {chatMemberBox.style.marginLeft = "calc(100% - 230px)";});
}

function hideChatMembers(){
    const chat = document.getElementById("Chat");
    const button = document.getElementById("showHideGroupMemberButton");
    const chatMemberBox = document.getElementById("GroupMerbers");
    const image = document.getElementById("blueArrow");

    //Arrow points to the left
    image.style.transform = "scaleX(1)";

    //animation 
    var animationTime = "1s";
    chat.style.animationDuration = animationTime;
    button.style.animationDuration = animationTime;
    chatMemberBox.style.animationDuration = animationTime;

    //chat.style.animationFillMode = "forwards";
    //button.style.animationFillMode = "forwards";
    //chatMemberBox.style.animationFillMode = "forwards";


    chat.style.animationName = "increaseChat";
    button.style.animationName = "moveButtonRight";
    chatMemberBox.style.animationName = "hideChatMebers";

    chat.addEventListener("animationend", () => {chat.style.width = "calc(100% - 230px - 50px)";});
    button.addEventListener("animationend", () => {button.style.marginLeft = "calc(100% - 1vh - 40px)";});
    chatMemberBox.addEventListener("animationend", () => {chatMemberBox.style.marginLeft = "calc(100%)";});   
}

var firstclick = true;
function ArrowButtonCick (){
    if (firstclick){
        showChatMembers();
        firstclick = false;
    }
    else {
        hideChatMembers();
        firstclick = true;
    }
}

function createChatMember(profileName, profilePicture = "./sources/Pictures/blank-profile-picture.svg"){
    const chatElement = document.createElement("div");
    const profPicture = document.createElement("img");
    const profName = document.createElement("p");

    profPicture.src=profilePicture;
    profPicture.alt="avatar";

    profName.innerHTML = profileName;

    chatElement.appendChild(profPicture);
    chatElement.appendChild(profName);
    profName.className = "profName";


    const chatMember = document.getElementById("ListGroupMembers");

    chatMember.appendChild(chatElement);
}


