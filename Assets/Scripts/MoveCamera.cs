using UnityEngine;
using Mirror;

public class MoveCamera : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.position = player.transform.position;
    }
}