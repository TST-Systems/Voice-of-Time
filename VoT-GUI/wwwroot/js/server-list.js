//Get mouse coordinates 
    var mousePositionX = 0; 
    var mousePositionY = 0;
    
    // Getting 'Info' div in js hands
    var server = document.getElementById('ServerList');
    
    // Creating function that will tell the position of cursor
    // PageX and PageY will getting position values and show them in P
    function tellPos(p){
        mousePositionX = p.pageX; 
        mousePositionY = p.pageY;
    }
    server.addEventListener('mousemove', tellPos, false);

    var body = document.getElementById("body");
    body.onclick = () => {document.getElementById("ContextMenuServer").style.display = "none";};
//

//Server IDs
    const serverIDs = new Array();

const { ipcRenderer } = require("electron");

function createServerItem(serverID, url, name = url, imageSource = "./sources/Pictures/blank-profile-picture.svg") {
    //delete Server Button
    document.getElementById("ServerAddButton").remove();

    const serverElement = document.createElement("div");
    const image = document.createElement("img");

    image.src=imageSource;
    image.alt = "avatar";
    image.title = name;

    //on click connect to server
    

    serverElement.appendChild(image);
    

    const server = document.getElementById("ServerList");
    server.appendChild(serverElement); 

    serverElement.id = serverID;
    serverIDs.push(serverID);

    serverElement.oncontextmenu = function() {rightClickOnServer(serverID)};

    serverElement.addEventListener("contextmenu", (e) => {e.preventDefault()});
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
    document.getElementById("ServerSettings").style.display = "none";

    const serverName = document.getElementById("UserName").value;
    const serverIP = document.getElementById("ServerIP").value; 

    //electron comunication
    const reply = ipcRenderer.send("createServer", {serverName, serverIP });
    
    ipcRenderer.on('createServer-reply', (event, serverID) => {
        createServerItem(serverID, serverName, serverIP);
        console.log(serverID);
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









