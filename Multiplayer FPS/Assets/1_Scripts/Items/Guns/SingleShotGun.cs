using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        Debug.Log($"Shooting ({itemInfo.itemName})");

        //shoot a ray from the middle of my screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        //set the origin of the shot to the cameras position
        ray.origin = cam.transform.position;

        //Check if the raycast hit
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"Hit ({hit.collider.gameObject.name})");

            //if what you hit has a damageable component on it then take damage
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
        }
    }
}
