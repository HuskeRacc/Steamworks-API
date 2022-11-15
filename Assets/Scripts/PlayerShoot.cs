using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    public PlayerWeapon weapon;

    [SerializeField] private Camera cam;

    [SerializeField] private LayerMask mask;

    private void Start()
    {
        if(cam == null)
        {
            Debug.LogError("PlayerShoot: No Camera Referenced");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && isLocalPlayer)
        {
            Shoot();
        }
    }

    [Client]
    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
           if(hit.collider.CompareTag("PLAYER_TAG"))
            {
                CmdPlayerShot(hit.collider.name);
            }
        }
    }

    [Command]
    void CmdPlayerShot(string _ID)
    {
        Debug.Log(_ID + " has been shot.");
    }

}
