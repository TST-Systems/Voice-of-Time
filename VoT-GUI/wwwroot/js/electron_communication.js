const { ipcRenderer } = require("electron");

document.getElementById("sync-msg").addEventListener("click", () => {
    const reply = ipcRenderer.sendSync("sync-msg", "ping");
    const message = `Synchronous message reply: ${reply}`;
    document.getElementById('sync-reply').innerHTML = message;
});