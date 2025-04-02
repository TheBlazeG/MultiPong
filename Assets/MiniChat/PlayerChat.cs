using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Unity.VisualScripting;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class PlayerChat : NetworkBehaviour
{
    public TextMeshPro info;
    private string mensaje;
    [SyncVar(hook = nameof(UpdateUI))]
    public string visibleMessage;

    public GameObject bubble;

    [SyncVar(hook = nameof(SetColor))]
    public Color color;
    public SpriteRenderer sr;

    public LayerMask bubbleMask;

    #region Unity Callbacks

    /// <summary>
    /// Add your validation code here after the ba  se.OnValidate(); call.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        foreach(char c in Input.inputString)
        {
            switch (c)
            {
                case '\b':
                    if (mensaje.Length!=0)
                    {
                        mensaje = mensaje.Substring(0, mensaje.Length - 1);
                    }
                    break;
                case '\n':
                case '\r':
                    CommandSendMessage(mensaje);
                    mensaje = "";
                    break;

                default:
                    mensaje += c;
                    break;
            }
            commandUpdateUI(mensaje);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            CommandShoot(ray.origin, ray.direction);
        }
        
    }

    [Command]
    private void CommandShoot(Vector2 origen, Vector2 direccion)
    {
        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, 100, bubbleMask);
        

        if (hit)
        {
            if (hit.collider.gameObject.TryGetComponent<Bubble>(out Bubble bubu))
            {
                bubu.WasHit();
                Debug.Log("le pegu�Ea la burbuja");
            }
        }
    }

    #region UIUpdate

    [Command]
    private void commandUpdateUI(string s)
    {
        visibleMessage = s;
    }
    private void UpdateUI(string oldMessage, string newMessage)
    {
        info.text = newMessage;
    }

    #endregion

    [Command]
    private void CommandSendMessage(string msj) 
    {
        var bub = Instantiate(bubble, new Vector2(Random.Range(-4, 4), -7), Quaternion.identity);
        NetworkServer.Spawn(bub);
        ClientSendMessage(msj, bub.GetComponent<Bubble>());
        
    }
    [ClientRpc]
    private void ClientSendMessage(string msj, Bubble bub)
    {
        
        bub.GetComponent<Bubble>().Initialize(msj);
    }

    #endregion

    [Command]
    private void CommandSetColor(Color newColor)
    {
        color = newColor;
    }


    private void SetColor(Color oldColor, Color newColor)
    {
        sr.color = newColor;
    }

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer() { }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer() { }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient() 
    {

        CommandSetColor(GameObject.FindFirstObjectByType<PlayerInfo>().color);
    
    }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() { }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer() { }

    /// <summary>
    /// Called when the local player object is being stopped.
    /// <para>This happens before OnStopClient(), as it may be triggered by an ownership message from the server, or because the player object is being destroyed. This is an appropriate place to deactivate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStopLocalPlayer() {}

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnectionToClient parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() { }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }

    #endregion
}
