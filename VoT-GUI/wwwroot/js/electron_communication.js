const { ipcRenderer } = require("electron");

document.getElementById("ServerList").addEventListener("click", () => {
    const reply = ipcRenderer.sendSync("sync-msg", );
});