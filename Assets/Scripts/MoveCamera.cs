using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MoveCamera : NetworkBehaviour
{
    public GameObject cam;
    public Transform player;

    void Update()
    {
        if (!IsOwner)
        {
            cam.SetActive(false);
            return;
        }
        transform.position = player.transform.position;
    }
}