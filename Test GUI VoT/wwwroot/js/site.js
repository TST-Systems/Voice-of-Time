// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.onload = function(){
    createServerItem("Hallo there", "sources/Pictures/VoT_logo.jpg");
    createServerItem("Zweiter Server");
    createServerItem("Hallo there", "sources/Pictures/VoT_logo.jpg");
    createServerItem("Zweiter Server");
    createServerItem("Hallo there", "sources/Pictures/VoT_logo.jpg");
    createServerItem("Zweiter Server");
    createServerItem("Hallo there", "sources/Pictures/VoT_logo.jpg");
    createServerItem("Zweiter Server");
    createServerItem("Hallo there", "sources/Pictures/VoT_logo.jpg");
    createServerItem("Zweiter Server");
    createServerItem("Zweiter Server");
    

    createPrivateChatOrGroup("Name",undefined, 3);
    createPrivateChatOrGroup("Name",undefined, 3);
    createPrivateChatOrGroup("Name",undefined, 3);
    createPrivateChatOrGroup("Name");
    createPrivateChatOrGroup("Name",undefined, 3);
    createPrivateChatOrGroup("Name");
    createPrivateChatOrGroup("Name",undefined, 3);
    createPrivateChatOrGroup("Name");
    createPrivateChatOrGroup("Name",undefined, 3);
    createPrivateChatOrGroup("Name");
    createPrivateChatOrGroup("Name",undefined, 3);
    createPrivateChatOrGroup("Name");
    

    createChannel("Name123", "sources/Pictures/VoT_logo.jpg", 3);
    createChannel("Name123", "sources/Pictures/VoT_logo.jpg", 3);
    createChannel("Name123", undefined, 5);
    createChannel("Name123", "sources/Pictures/VoT_logo.jpg", 3);
    createChannel("Name123");
    createChannel("Name123", "sources/Pictures/VoT_logo.jpg", 3);

    receiveMessage("Hallo du da hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");
    receiveMessage("Hallo du da", "User 1", "./sources/Pictures/VoT_logo.jpg", "02.01.2023");
    dispatchedMessage("Hab du", "You");
    dispatchedMessage("nichts zu tun?");
    
    //Scrollbar on Bottom
        var objDiv = document.getElementById("TextChat");
        objDiv.scrollTop = objDiv.scrollHeight;
};
