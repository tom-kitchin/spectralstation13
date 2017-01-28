using UnityEngine;
using UnityEngine.Networking;
using Services.Networking;

public class ServerTimeRenderer : MonoBehaviour
{
    string activeType;
    TextMesh textMesh;

    // Use this for initialization
    void Start ()
    {
        textMesh = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (activeType == null)
        {
            if (NetworkServer.active)
            {
                activeType = "server";
            } else if (NetworkClient.active)
            {
                activeType = "client";
            }
        }

        if (activeType == "server")
        {
            textMesh.text = Network.time.ToString();
            return;
        } else if (activeType == "client")
        {
            if (SpectreClient.networkClient != null)
            {
                if (SpectreClient.networkClient.isConnected)
                {
                    textMesh.text = SpectreClient.serverTime.ToString();
                    return;
                }
            }
            textMesh.text = "Server Time Unavailable";
        }
    }
}
