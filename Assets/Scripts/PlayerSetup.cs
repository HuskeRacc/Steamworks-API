using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField] string noDrawLayerName = "PlayerGFX";
    [SerializeField] GameObject playerGFX;
    [SerializeField] GameObject localGFX;
    Camera sceneCamera;

    [SerializeField] string remoteLayerName = "RemotePlayer";
    [SerializeField] string LocalGFXLayerName = "LocalGFX";

    [SerializeField] GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    private void Start()
    {
        //Disable player GFX for local player
        if (isLocalPlayer)
        {
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            SetLayerRecursively(playerGFX, LayerMask.NameToLayer(noDrawLayerName));
        }
        else
        {
            AssignRemoteLayer();
            DisableComponents();
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
            SetLayerRecursively(localGFX, LayerMask.NameToLayer(LocalGFXLayerName));
        }

        sceneCamera = Camera.main;
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(false);
        }

        GetComponent<Player>().Setup();
    }

    private void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID,_player);
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);

        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
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
