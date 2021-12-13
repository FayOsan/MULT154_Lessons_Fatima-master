using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RemoveNetworkHUD : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject netMgr = GameObject.Find("NetWorkManager");
        NetworkManagerHUD netMgrHUD = netMgr.GetComponent<NetworkManagerHUD>();
        netMgrHUD.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
