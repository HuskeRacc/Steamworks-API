using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    string dontDrawLayerName = "NoDraw";
    [SerializeField]
    GameObject playerGFX;

    Camera mainCam;
    private void Start()
    {
        if (Camera.main != null)
        {
            mainCam = Camera.main;
            mainCam.gameObject.SetActive(false);
        }

        // Disable playerGFX for localplayer
        SetLayerRecursively(playerGFX, LayerMask.NameToLayer(dontDrawLayerName));
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