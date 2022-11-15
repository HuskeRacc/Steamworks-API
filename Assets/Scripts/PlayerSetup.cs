using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] string noDrawLayerName = "PlayerGFX";
    [SerializeField] GameObject playerGFX;
    Camera sceneCamera;

    [SerializeField] string remoteLayerName = "RemotePlayer";

    private void Start()
    {
        //Disable player GFX for local player
        if (isLocalPlayer)
        {
            SetLayerRecursively(playerGFX, LayerMask.NameToLayer(noDrawLayerName));
        }

        if(!isLocalPlayer)
        {
            AssignRemoteLayer();
        }

        sceneCamera = Camera.main;
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(false);
        }

        RegisterPlayer();
    }

    void RegisterPlayer()
    {
        string _ID = "Player " + GetComponent<NetworkIdentity>().netId;
        transform.name = _ID;
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {

    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach(Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

}
