using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using Photon.Pun;

public class GunController : MonoBehaviour
{
    //Settings
    [Expandable][SerializeField] GunData gunData;

    //connected components
    [Foldout("Connected Components")][SerializeField] private Transform hipFirePos;
    [Foldout("Connected Components")][SerializeField] private Transform adsPos;

    //GOT VALUES
    [Foldout("Got Values")][ReadOnly][SerializeField] private PlayerController playerController;
    [Foldout("Got Values")][ReadOnly][SerializeField] private PhotonView pv;
    [Foldout("Got Values")][ReadOnly][SerializeField] private int currentMagAmmo;
    [Foldout("Got Values")][ReadOnly][SerializeField] private int currentOverallAmmo;
    [Foldout("Got Values")][ReadOnly][SerializeField] private bool reloading = false;
    [Foldout("Got Values")][ReadOnly][SerializeField] private float timeSinceLastShot;

    private void Awake()
    {
        playerController = this.transform.root.GetComponent<PlayerController>();
        pv = this.GetComponent<PhotonView>();
        currentMagAmmo = gunData.magSize;
        currentOverallAmmo = gunData.ammoCapacity;
    }

    private void OnEnable()
    {
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        PlayerInput.shootInput += Shoot;
        PlayerInput.reloadInput += StartReload;
        
        DisplayAmmo();
    }

    private void OnDisable()
    {
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        PlayerInput.shootInput -= Shoot;
        PlayerInput.reloadInput -= StartReload;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        ADS();

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

    public void StartReload()
    {
        //not already reloading and have ammo to reload with
        if(!reloading && currentOverallAmmo > 0)
        {
            //reload
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        reloading = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        reloading = false;

        //get the amount of bullets missing from your current magazine
        int magBulletMissingCount = gunData.magSize - currentMagAmmo;

        //if you have enough in reserve to refil your mag
        if(currentOverallAmmo >= magBulletMissingCount)
        {
            currentOverallAmmo -= magBulletMissingCount;
            //set the current mag to be full
            currentMagAmmo = gunData.magSize;
        }
        //not enough ammo in reserve to fully refil tour mag
        else
        {
            //add all remaining bullets to the magazine
            currentMagAmmo += currentOverallAmmo;
            //set the reserve ammo count to 0
            currentOverallAmmo = 0;
        }

        DisplayAmmo();
    }

    void ADS()
    {
        //Aiming
        if (playerController.playerInput.isAiming)
        {
            // Smoothly move the gun to the the ads position
            this.transform.position = Vector3.Lerp(this.transform.position, adsPos.position, Time.deltaTime * gunData.adsSpeed);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, adsPos.rotation, Time.deltaTime * gunData.adsSpeed);

            //smoothly zoom the camera in
            playerController.cam.fieldOfView = Mathf.Lerp(playerController.cam.fieldOfView, 50f, Time.deltaTime * gunData.adsSpeed);
        }
        //Hip Firing
        else
        {
            // Smoothly move the gun to the hip fire position
            this.transform.position = Vector3.Lerp(this.transform.position, hipFirePos.position, Time.deltaTime * gunData.adsSpeed);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, hipFirePos.rotation, Time.deltaTime * gunData.adsSpeed);

            //smoothly zoom the camera out
            playerController.cam.fieldOfView = Mathf.Lerp(playerController.cam.fieldOfView, 60f, Time.deltaTime * gunData.adsSpeed);
        }

        // Snapping to position to avoid overshooting
        if (Vector3.Distance(this.transform.position, playerController.playerInput.isAiming ? adsPos.position : hipFirePos.position) < gunData.positionSnapTolerance)
        {
            this.transform.position = playerController.playerInput.isAiming ? adsPos.position : hipFirePos.position;
        }
        if (Quaternion.Angle(this.transform.rotation, playerController.playerInput.isAiming ? adsPos.rotation : hipFirePos.rotation) < gunData.rotationSnapTolerance)
        {
            this.transform.rotation = playerController.playerInput.isAiming ? adsPos.rotation : hipFirePos.rotation;
        }
    }

    bool CanShoot() => !reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);
    
    public void Shoot()
    {
        if (currentMagAmmo <= 0) { Debug.LogError("No Ammo in Mag!"); return; }
        if (!CanShoot()) { Debug.LogError("Shooting Criteria has not been met!"); return; }

        Debug.Log($"Shoot {gunData.name}!");

        currentMagAmmo--;
        timeSinceLastShot = 0;
        OnGunShot();

        //shoot a ray from the middle of my screen
        Ray ray = playerController.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        //set the origin of the shot to the cameras position
        ray.origin = playerController.cam.transform.position;

        //Check if the raycast hit
        if (Physics.Raycast(ray, out RaycastHit hit, gunData.maxDistance, gunData.canShootLayers))
        {
            Debug.Log($"*Hit: {hit.transform.name}");

            //if what you hit has a damageable component on it then take damage
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(gunData.damage);

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

    void DisplayAmmo()
    {
        playerController.text_AmmoCount.text = $"{currentMagAmmo}/{gunData.magSize}\n{currentOverallAmmo}";
    }

    private void OnGunShot()
    {
        DisplayAmmo();
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        //get an array of every collider in a .3 meter radius of the hit position
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);

        if (colliders.Length != 0)
        {
            //position it ever so slightly in front of the hit position
            Vector3 spawnPos = hitPosition + hitNormal * 0.001f;
            //
            Quaternion spawnRot = Quaternion.LookRotation(hitNormal, Vector3.up) * gunData.bulletImpactPrefab.transform.rotation;

            //spawn the impact prefab
            GameObject bulletImpactObject = Instantiate(gunData.bulletImpactPrefab, spawnPos, spawnRot);

            //parent the impact prefab to the collider it hit (for example hit players collider now is child of player)
            bulletImpactObject.transform.SetParent(colliders[0].transform);

            //delete the bullet impact after 10 seconds
            Destroy(bulletImpactObject, 10f);
        }
    }
}
