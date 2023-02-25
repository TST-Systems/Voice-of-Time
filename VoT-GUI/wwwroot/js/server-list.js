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



function createServerItem(serverID, name, url="0.0.0.0", imageSource = "./sources/Pictures/blank-profile-picture.svg") {
    //delete Server Button
    document.getElementById("ServerAddButton").remove();

    const serverElement = document.createElement("div");
    const image = document.createElement("img");

    image.src=imageSource;
    image.alt = "avatar";
    image.title = name;

    //on click connect to server
    //image.onclick = func();

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
    //Create Server ID
        var prove = true;
        var newServerID;
        while(true) {
            randomID = Math.floor(Math.random() * 1000000);
            for (var i = 0; i < serverIDs.length; i++){
                if (randomID == serverIDs[i]) prove = false;
            }
            if (prove) {
                newServerID = randomID;
                break;
            } 
        } 
    //

    const serverName = document.getElementById("ServerName").value;
    const serverIP = document.getElementById("ServerIP").value;    

    createServerItem(newServerID, serverName, serverIP);

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









