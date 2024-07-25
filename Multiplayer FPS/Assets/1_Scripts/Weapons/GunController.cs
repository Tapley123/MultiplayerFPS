using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class GunController : MonoBehaviour
{
    [ReadOnly] [SerializeField] private PlayerController playerController;
    [Expandable][SerializeField] GunData gunData;

    [ReadOnly][SerializeField] private int currentAmmo;
    [ReadOnly][SerializeField] private bool reloading = false;
    [ReadOnly][SerializeField] private float timeSinceLastShot;

    private void Awake()
    {
        playerController = this.transform.root.GetComponent<PlayerController>();
        currentAmmo = gunData.magSize;
    }

    private void OnEnable()
    {
        PlayerInput.shootInput += Shoot;
    }

    private void OnDisable()
    {
        PlayerInput.shootInput -= Shoot;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;


        //Debugging gunshot
        if(Input.GetMouseButtonDown(0))
        {
            //shoot a ray from the middle of my screen
            Ray ray = playerController.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            //set the origin of the shot to the cameras position
            ray.origin = playerController.cam.transform.position;
            // Draw the debug ray for visualization
            Debug.DrawRay(ray.origin, ray.direction * gunData.maxDistance, Color.red, gunData.maxDistance);
        }
    }

    bool CanShoot() => !reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);
    
    public void Shoot()
    {
        Debug.Log($"Shoot {gunData.name}!");

        if(currentAmmo > 0)
        {
            Debug.Log($"Has Ammo");

            if (CanShoot())
            {
                Debug.Log($"Can Shoot");

                //shoot a ray from the middle of my screen
                Ray ray = playerController.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                //set the origin of the shot to the cameras position
                ray.origin = playerController.cam.transform.position;

                //Check if the raycast hit
                if (Physics.Raycast(ray, out RaycastHit hit, gunData.maxDistance, gunData.canShootLayers))
                {
                    Debug.Log($"*Hit: {hit.transform.name}");
                }

                currentAmmo--;
                timeSinceLastShot = 0;
                OnGunShot();
            }
            else
            {
                Debug.LogError($"Cant Shoot");
            }
        }
        else
        {
            Debug.LogError($"No Ammo");
        }
    }

    private void OnGunShot()
    {
        
    }
}
