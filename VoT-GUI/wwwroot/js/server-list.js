//ipcRenderer electron
const { ipcRenderer } = require("electron");

function createServerItem(serverID, url, name = url, imageSource = "sources/Pictures/blank-profile-picture.svg") {
    //delete Server Button
    document.getElementById("ServerAddButton").remove();

    const serverElement = document.createElement("div");
    const image = document.createElement("img");

    image.src=imageSource;
    image.alt = "avatar";
    image.title = name;

    serverElement.appendChild(image);
    

    const server = document.getElementById("ServerList"); 

    //on click connect to server
    serverElement.addEventListener("click", () => {
        //electron comunication
        ipcRenderer.send("connectServer", url);
        
        ipcRenderer.on('connectServer-reply', (event, serverID) => {
            listChats();
        });
        
    });

    server.appendChild(serverElement);
    serverElement.id = serverID;

    serverElement.oncontextmenu = function () { rightClickOnServer(serverID) };

    serverElement.addEventListener("contextmenu", (e) => { e.preventDefault() });
    createServerAddButton();
};



function rightClickOnServer(serverID){
    const contentMenu = document.getElementById("ContextMenuServer");
    contentMenu.style.display = "block";
    
    contentMenu.style.marginTop  = "calc("+String(mousePositionY)+"px - 50px)";
    contentMenu.style.marginLeft = String(mousePositionX)+"px";
    
    document.getElementById("ChangeSettings").onclick = () => {
        document.getElementById("ServerSettings").style.display = "block";

    }

    document.getElementById("DeleteServer").onclick = () => {
        document.getElementById(String(serverID)).remove();
    };

    document.getElementById("ConnectToServer").onclick = () => {
        document.getElementById("ServerSettings").style.display = "block";
    };

}

//Click add Server
document.getElementById("ButtonAddServer").onclick = () => {
    //document.getElementById("ServerSettings").style.display = "none";

    const UserName = document.getElementById("UserName").value;
    const serverIP = document.getElementById("ServerIP").value; 

    //electron comunication
    ipcRenderer.send("createServer", {UserName, serverIP });
    
    ipcRenderer.on('createServer-reply', (event, serverID) => {
        createServerItem(serverID, serverIP);
        document.getElementById("ServerSettings").style.display = "none";

        listChats();
    });

}

function createServerAddButton() {
    const serverElement = document.createElement("div");
    const image = document.createElement("img");

    image.src="./sources/Pictures/add-button.png";
    image.alt = "avatar";

    //on click connect to server
    image.onclick = function () {
        document.getElementById("ServerSettings").style.display = "block";
    }

    serverElement.appendChild(image);
    

    const server = document.getElementById("ServerList");
    server.appendChild(serverElement); 

    serverElement.id = "ServerAddButton"
}









