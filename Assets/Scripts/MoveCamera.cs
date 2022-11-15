using UnityEngine;
using Mirror;

public class MoveCamera : MonoBehaviour
{
    public bool paused;
    public Transform player;

    void Update()
    {
        if(!paused)
        transform.position = player.transform.position;
    }
}