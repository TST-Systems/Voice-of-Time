function createServerItem(name, url="0.0.0.0", imageSource = "./sources/Pictures/blank-profile-picture.png") {
    const serverElement = document.createElement("div");
    const image = document.createElement("img");
    const doBreak = document.createElement("br");

    image.src=imageSource;
    image.alt = "avatar";
    image.title = name;

    //on click connect to server
    //image.onclick = func();

    serverElement.appendChild(image);


    const server = document.getElementById("ServerList");
    server.appendChild(serverElement); 
    serverElement.addEventListener("contextmenu", (e) => {e.preventDefault()});
    serverElement.oncontextmenu = rightClickOnServer();
    
};


function rightClickOnServer(){
    document.getElementById("ContextMenuServer").style.display = "left";
    /*
    var e = window.Event;

    var posX = e.X;
    var posY = e.Y;

    contextMenu.style.marginLeft = posX;
    contextMenu.style.marginTop = posY;
    */
}





