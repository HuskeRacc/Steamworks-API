using UnityEngine;
using Mirror;

public class MoveCamera : NetworkBehaviour
{
    public Transform player;
    
    void Update()
    {
        if(!isOwned) { return; }
        transform.position = player.transform.position;
    }
}