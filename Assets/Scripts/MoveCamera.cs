using UnityEngine;

using UnityEngine.SceneManagement;

public class MoveCamera : MonoBehaviour
{
    public GameObject cam;
    public Transform player;

    void Update()
    {/*
        if (!IsOwner)
        {
            cam.SetActive(false);
            return;
        }
        */
        transform.position = player.transform.position;
    }
}