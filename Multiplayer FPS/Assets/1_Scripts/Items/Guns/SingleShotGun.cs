using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;
    PhotonView pv;
    [SerializeField] LayerMask canShootLayers;
    [SerializeField] float range = 100f;

    private void Awake()
    {
        //online
        if(PhotonNetwork.IsConnected)
        {
            //pv = this.transform.root.GetComponent<PhotonView>();
            pv = GetComponent<PhotonView>();
        }
    }

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        //Debug.Log($"Shooting ({itemInfo.itemName})");

        //shoot a ray from the middle of my screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        //set the origin of the shot to the cameras position
        ray.origin = cam.transform.position;

        //Check if the raycast hit
        if(Physics.Raycast(ray, out RaycastHit hit, range, canShootLayers))
        {
            //Debug.Log($"Hit ({hit.collider.gameObject.name})");

            //if what you hit has a damageable component on it then take damage
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);

            //online
            if (PhotonNetwork.IsConnected)
            {
                //Get the point in worldspace that was hit by the bullet
                pv.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            }
            //offline
            else
            {
                //Get the point in worldspace that was hit by the bullet

                RPC_Shoot(hit.point, hit.normal);
            }
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        //get an array of every collider in a .3 meter radius of the hit position
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);

        if(colliders.Length != 0)
        {
            //position it ever so slightly in front of the hit position
            Vector3 spawnPos = hitPosition + hitNormal * 0.001f;
            //
            Quaternion spawnRot = Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation;

            //spawn the impact prefab
            GameObject bulletImpactObject = Instantiate(bulletImpactPrefab, spawnPos, spawnRot);

            //parent the impact prefab to the collider it hit (for example hit players collider now is child of player)
            bulletImpactObject.transform.SetParent(colliders[0].transform);

            //delete the bullet impact after 10 seconds
            Destroy(bulletImpactObject, 10f);
        }
    }
}
