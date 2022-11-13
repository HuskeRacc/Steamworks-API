using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] string noDrawLayerName = "PlayerGFX";
    [SerializeField] GameObject playerGFX;
    Camera sceneCamera;

    private void Start()
    {
        //Disable player GFX for local player
        if (IsLocalPlayer)
        {
            SetLayerRecursively(playerGFX, LayerMask.NameToLayer(noDrawLayerName));
        }

        sceneCamera = Camera.main;
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(false);
        }
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
