using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Steamworks;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }

public class Player : NetworkBehaviour
{
    public static Player i;
    [SyncVar]
    public string steamName;
    [SyncVar]
    public int playerID;

    [SerializeField] ToggleEvent onToggleShared;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;

    void Start()
    {
        EnablePlayer();

        if (isLocalPlayer)
        {
            i = this;
            if (SteamManager.Initialized)
            {
                CmdSetName(SteamFriends.GetPersonaName());
            }
            CmdReportConnection();
        }

        GameObject.Find("NetController").GetComponent<NetworkManagerHUD>().showGUI = false;
    }

    [Command]
    void CmdReportConnection()
    {
        GameStatus.i.ReportConnected(netId);
    }

    [Command]
    void CmdSetName(string name)
    {
        steamName = name;
    }

    [Command]
    public void CmdConnected(int ID)
    {
        playerID = ID;
        GetComponent<PlayerCore>().RpcIdentify(ID);
        RpcReportConnected(ID);
    }

    [ClientRpc]
    void RpcReportConnected(int ID)
    {
        if (isLocalPlayer)
        {
            UI_Chat.i.AddSystemMessageToChat("Server Connected");
            UI_HUD.i.targetID = ID;
        }
        else
        {
            UI_Chat.i.AddSystemMessageToChat(steamName + " has connected!");
        }
    }

    void DisablePlayer()
    {
        //if (isLocalPlayer) ;

        onToggleShared.Invoke(false);

        if (isLocalPlayer)
        {
            onToggleLocal.Invoke(false);
            Camera.main.GetComponent<CameraControl>().target = null;
        }
        else
            onToggleRemote.Invoke(false);
    }

    void EnablePlayer()
    {
        //if (isLocalPlayer) ;

        onToggleShared.Invoke(true);

        if (isLocalPlayer)
        {
            onToggleLocal.Invoke(true);
            Camera.main.GetComponent<CameraControl>().target = transform;
            Camera.main.GetComponent<CameraControl>().originalTarget = transform;
        }
        else
            onToggleRemote.Invoke(true);
    }

    [Command]
    public void CmdSendMessage(string message)
    {
        RpcUpdateChats(message, steamName);
    }

    [ClientRpc]
    void RpcUpdateChats(string message, string sender)
    {
        UI_Chat.i.AddMessageToChat(message, sender);
    }
}