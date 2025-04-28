using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NodeListCommunicationManager : MonoBehaviour
{
    public static NodeListCommunicationManager singleton;

    public ServerInfo curServerInfo = new ServerInfo();

    public string AuthKey = "NodeListServerDefaultKey";
    private const string Server = "http://20.190.46.42:8889";


    public string InstanceServerID = string.Empty;

    private void Awake()
    {
        if(singleton!=null && singleton!= this) { Destroy(this); } else { singleton = this; }
    }
   
    public void AddUpdateServerEntry()
    {
        StartCoroutine(AddUpdateInternal());
    }

    public void RemoveServerEntry()
    {
        StartCoroutine(RemoveServerInternal());
    }

    private IEnumerator AddUpdateInternal()
    {
        WWWForm serverData = new WWWForm();
        serverData.AddField("serverKey",AuthKey);
        bool addingServer = false;

        if (string.IsNullOrEmpty(InstanceServerID))
        {
            addingServer = true;
        }
        else
        {
            serverData.AddField("serverUuid",InstanceServerID);
        }

        serverData.AddField("serverName", curServerInfo.Name);
        serverData.AddField("serverPort", curServerInfo.Port);
        serverData.AddField("serverPlayers", curServerInfo.PlayerCount);
        serverData.AddField("serverCapacity", curServerInfo.PlayerCapacity);
        serverData.AddField("serverExtras", curServerInfo.ExtraInfo);

        UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Post(Server + "/add", serverData);

        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if(www.responseCode ==200)
        {
            if (addingServer)
            {
                InstanceServerID = www.downloadHandler.text;
            }
        }
        else
        {
            Debug.LogError("couldn't add the server");

        }
        yield break;
    }

    private IEnumerator RemoveServerInternal()
    {
        WWWForm serverData = new WWWForm();
        serverData.AddField("serverKey", AuthKey);
        serverData.AddField("serverUuid", InstanceServerID);

        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Post(Server + "/remove", serverData))
        {
            yield return www.SendWebRequest();
            if (www.responseCode ==200)
            {
                Debug.Log("Servidor Removido exitosamente");

            }
            else
            {
                Debug.LogError("No se pudo remover el servidor");
            }
        }
        yield break;
    }
}
