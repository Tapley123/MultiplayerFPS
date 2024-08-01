using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using Photon.Pun;

public class GunController : MonoBehaviour
{
    //Settings
    [Expandable] public GunData gunData;

    public bool debugging = false;

    //connected components
    [Foldout("Connected Components")][SerializeField] private GunAnimator gunAnimator;
    [Foldout("Connected Components")][SerializeField] private Transform gunT;
    [Foldout("Connected Components")][SerializeField] private Transform endOfBarrel;
    [Foldout("Connected Components")][SerializeField] private GameObject muzzleFlash;
    [Foldout("Connected Components")][SerializeField] private Transform hipFirePos;
    [Foldout("Connected Components")][SerializeField] private Transform adsPos;
    [Foldout("Connected Components")][SerializeField] private Transform adsShootOrigin;
    [Foldout("Connected Components")][SerializeField] private Magazine magazine;
    [Foldout("Connected Components")]public Transform grabPointLeft;
    [Foldout("Connected Components")]public Transform grabPointRight;


    //GOT VALUES
    [Foldout("Got Values")][ReadOnly] public PlayerRefrences playerRefrences;
    [Foldout("Got Values")][ReadOnly][SerializeField] private PhotonView pv;
    [Foldout("Got Values")][ReadOnly][SerializeField] private int currentMagAmmo;
    [Foldout("Got Values")][ReadOnly][SerializeField] private int currentOverallAmmo;
    [Foldout("Got Values")][ReadOnly][SerializeField] private bool reloading = false;
    [Foldout("Got Values")][ReadOnly][SerializeField] private bool ads = false;
    [Foldout("Got Values")][ReadOnly] public float currentSpreadAngle = 0f;
    [Foldout("Got Values")][ReadOnly] [Range(0f, 1f)] public float hipFireRecoilNorm = 0f;
    [Foldout("Got Values")][ReadOnly] [SerializeField] private float timeSinceLastShot;
    [Foldout("Got Values")][ReadOnly] [SerializeField] private float lastShotTime = 0f;

    private void Awake()
    {
        //Ensure debugging is off on a build
#if !UNITY_EDITOR
        if (debugging)
            debugging = false;
#endif

        playerRefrences = this.transform.root.GetComponent<PlayerRefrences>();
        pv = this.GetComponent<PhotonView>();
        currentMagAmmo = gunData.magSize;
        currentOverallAmmo = gunData.ammoCapacity;

        //turn off muzzle flash
        if (muzzleFlash != null)
            muzzleFlash.SetActive(false);
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
        UpdateCalculataions();

        //Update Crosshairs
        if (playerRefrences.movingCrosshairParts.Count != 0)
            UpdateCrosshairs();

        //auto shooting
        if (gunData.automatic && CanShoot() && playerRefrences.playerInput.holdingShootButton && currentMagAmmo > 0)
        {
            Shoot();
        }
    }

    void UpdateCalculataions()
    {
        //calculate the time since the last bullet was shot
        timeSinceLastShot += Time.deltaTime; //(Set to 0 again on shot!)

        // Decrease the spread over time when not shooting
        if (Time.time - lastShotTime > gunData.recoilReturnSpeed * Time.deltaTime)
        {
            currentSpreadAngle = Mathf.Lerp(currentSpreadAngle, 0f, gunData.spreadDecreaseRate * Time.deltaTime);
        }

        //calculate the hip fire recoil normal value
        hipFireRecoilNorm = Mathf.Clamp01(currentSpreadAngle / gunData.maxSpreadAngle);
    }

    public float maxMoveCrosshair = 2f;
    //moves the crosshairs based on the recoil
    private void UpdateCrosshairs()
    {
        //loop through all of the crosshair parts
        foreach(CrosshairPart c in playerRefrences.movingCrosshairParts)
        {
            //move them based off the hip fire normalized
            //c.rectTransform.localPosition = Vector3.Lerp(c.startPosition, Vector3.up * maxMoveCrosshair, hipFireRecoilNorm);
            c.rectTransform.localPosition = Vector3.Lerp(c.startPosition, c.startPosition + c.rectTransform.up * maxMoveCrosshair, hipFireRecoilNorm);
        }
    }

    public void StartReload()
    {
        //not already reloading; have ammo to reload with; Less than a full mag
        if(!reloading && currentOverallAmmo > 0 && currentMagAmmo < gunData.magSize)
        {
            //reload
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        reloading = true;

        //move the hand to the magazine
        playerRefrences.handTransitioner.SetLeftHandTarget(magazine.transform, gunData.reloadTime / 3);
        //wait
        yield return new WaitForSeconds(gunData.reloadTime / 3);

        //play removeMag sound
        playerRefrences.soundEffectPlayer.Play(gunData.removeMagSound);

        //set the parent of the magazine to be the players intermediate hand target
        //magazine.transform.parent = playerController.handTransitioner.leftIntermediateT;

        //move magazine to dump
        playerRefrences.handTransitioner.SetLeftHandTarget(playerRefrences.magazineDump, gunData.reloadTime / 3);
        //wait
        yield return new WaitForSeconds(gunData.reloadTime/3);

        //move magazine to gun
        playerRefrences.handTransitioner.SetLeftHandTarget(magazine.transform, gunData.reloadTime / 3);
        //wait
        yield return new WaitForSeconds(gunData.reloadTime/3);

        //play insert mag sound
        playerRefrences.soundEffectPlayer.Play(gunData.insertMagSound);

        //move hand to grip
        playerRefrences.handTransitioner.SetLeftHandTarget(grabPointLeft);

        //set the parent of the magazine back to the gun
        //magazine.transform.parent = gunT;

        //finished reloading
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

    #region Aiming
    bool settingADS = false;
    void ADS()
    {
        //Aiming
        if (playerRefrences.playerInput.isAiming)
        {
            // Smoothly move the gun to the the ads position
            this.transform.position = Vector3.Lerp(this.transform.position, adsPos.position, Time.deltaTime * gunData.adsSpeed);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, adsPos.rotation, Time.deltaTime * gunData.adsSpeed);

            //smoothly zoom the camera in
            playerRefrences.cam.fieldOfView = Mathf.Lerp(playerRefrences.cam.fieldOfView, 50f, Time.deltaTime * gunData.adsSpeed);

            //set ads
            if(!settingADS && !ads)
            {
                settingADS = true;
                Invoke(nameof(SetADS), 3 / gunData.adsSpeed);
            } 
        }
        //Hip Firing
        else
        {
            // Smoothly move the gun to the hip fire position
            this.transform.position = Vector3.Lerp(this.transform.position, hipFirePos.position, Time.deltaTime * gunData.adsSpeed);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, hipFirePos.rotation, Time.deltaTime * gunData.adsSpeed);

            //smoothly zoom the camera out
            playerRefrences.cam.fieldOfView = Mathf.Lerp(playerRefrences.cam.fieldOfView, 60f, Time.deltaTime * gunData.adsSpeed);

            //set ads
            settingADS = false;
            ads = false;
            //if there is a crosshair and it is not active
            if (playerRefrences.crossHairGo != null && !playerRefrences.crossHairGo.activeInHierarchy)
            {
                //turn it on
                playerRefrences.crossHairGo.SetActive(true);
            }
            CancelInvoke(nameof(SetADS));
        }

        // Snapping to position to avoid overshooting
        if (Vector3.Distance(this.transform.position, playerRefrences.playerInput.isAiming ? adsPos.position : hipFirePos.position) < gunData.positionSnapTolerance)
        {
            this.transform.position = playerRefrences.playerInput.isAiming ? adsPos.position : hipFirePos.position;
        }
        if (Quaternion.Angle(this.transform.rotation, playerRefrences.playerInput.isAiming ? adsPos.rotation : hipFirePos.rotation) < gunData.rotationSnapTolerance)
        {
            this.transform.rotation = playerRefrences.playerInput.isAiming ? adsPos.rotation : hipFirePos.rotation;
        }
    }


    void SetADS()
    {
        ads = true;

        //if there is a crosshair and it is active
        if(playerRefrences.crossHairGo != null && playerRefrences.crossHairGo.activeInHierarchy)
        {
            //turn it off
            playerRefrences.crossHairGo.SetActive(false);
        }
    }
    #endregion

    bool CanShoot() => !reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

   
    public void Shoot()
    {
        if (currentMagAmmo <= 0) { if(debugging) Debug.LogError("No Ammo in Mag!"); playerRefrences.soundEffectPlayer.Play(gunData.emptySound); return; }
        if (!CanShoot()) { if (debugging) Debug.LogError("Shooting Criteria has not been met!"); return; }

        if (debugging)
            Debug.Log($"Shoot {gunData.name}!");

        currentMagAmmo--;
        OnGunShot();

        //Setting up Ray
        Ray ray;
        //Aiming down sights
        if (ads)
        {
            //shoot a ray from the crosshairs (*Coming from the gun transform so already has recoil applied*)
            ray = new Ray(adsShootOrigin.position, adsShootOrigin.forward);
        }
        //hip fire
        else
        {
            //add to the spread
            currentSpreadAngle += gunData.spreadIncreaseRate / (timeSinceLastShot + 1);
            //make sure that the spread is clamped
            currentSpreadAngle = Mathf.Clamp(currentSpreadAngle, 0, gunData.maxSpreadAngle);
            // Adjust the ray direction based on the current spread angle
            Vector3 spreadDirection = GetSpreadDirection();
            ray = new Ray(playerRefrences.cam.transform.position, spreadDirection);
        }

        // Debug The Ray
        if (debugging)
            Debug.DrawRay(ray.origin, ray.direction * gunData.maxDistance, Color.red);

        //Check if the raycast hit
        if (Physics.Raycast(ray, out RaycastHit hit, gunData.maxDistance, gunData.canShootLayers))
        {
            if (debugging)
                Debug.Log($"*Hit: {hit.transform.name}");

            //if what you hit has a damageable component on it then take damage
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(gunData.damage);

            //bullet trail
            if(gunData.bulletTrail != null)
            {
                //TrailRenderer trail = Instantiate(gunData.bulletTrail, endOfBarrel.position, Quaternion.identity);
                GameObject trailGo = ObjectPoolManager.Instance.TakeFromPool(QueueType.BulletTrail, gunData.bulletTrail.gameObject, ObjectPoolManager.Instance.bulletTrailHolder);
                trailGo.transform.position = endOfBarrel.position;
                trailGo.transform.rotation = endOfBarrel.rotation;
                //trailGo.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                StartCoroutine(SpawnTrail(trailGo.GetComponent<TrailRenderer>(), hit));
            }

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

        lastShotTime = Time.time;
        timeSinceLastShot = 0;
    }

    Vector3 GetSpreadDirection()
    {
        // Calculate a random direction within the current spread angle
        float angle = UnityEngine.Random.Range(0f, currentSpreadAngle);
        float radius = Mathf.Sin(angle * Mathf.Deg2Rad);
        float x = radius * Mathf.Cos(UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad);
        float y = radius * Mathf.Sin(UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad);

        Vector3 direction = playerRefrences.cam.transform.forward + new Vector3(x, y, 0);
        return direction.normalized;
    }

    void DisplayAmmo()
    {
        playerRefrences.text_AmmoCount.text = $"{currentMagAmmo}/{gunData.magSize}\n{currentOverallAmmo}";
    }

    private void OnGunShot()
    {
        //update ammo count on ui
        DisplayAmmo();
        //make gun show recoil
        gunAnimator.ApplyRecoil();
        //play gunshot noise
        playerRefrences.soundEffectPlayer.Play(gunData.shotSound);
        //show muzzleflash
        if (muzzleFlash != null)
            StartCoroutine(MuzzleFlash());
    }

    IEnumerator MuzzleFlash()
    {
        // Get the current Euler angles
        Vector3 eulerAngles = muzzleFlash.transform.rotation.eulerAngles;
        // Set a random Z rotation
        eulerAngles.z = UnityEngine.Random.Range(0f, 360f);
        // Apply the new rotation
        muzzleFlash.transform.rotation = Quaternion.Euler(eulerAngles);

        //Turn on The muzzle flash
        muzzleFlash.SetActive(true);

        yield return new WaitForSeconds(0.05f);

        //Turn off the muzzle flash
        muzzleFlash.SetActive(false);
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        //store the start position of the trail
        Vector3 startPos = trail.transform.position;

        while(time < 1)
        {
            //smoothly move the trail to the hit point
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        //make sure the trail is at the hit point
        trail.transform.position = hit.point;

        //destroy the trail after it has faded out
        //Destroy(trail.gameObject, trail.time);
        yield return new WaitForSeconds(trail.time);
        ObjectPoolManager.Instance.ReturnToPool(QueueType.BulletTrail, trail.gameObject);
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
            //GameObject bulletImpactObject = Instantiate(gunData.bulletImpactPrefab, spawnPos, spawnRot);
            GameObject bulletImpactObject = ObjectPoolManager.Instance.TakeFromPool(QueueType.BulletImpact, gunData.bulletImpactPrefab, ObjectPoolManager.Instance.bulletImpactHolder);
            bulletImpactObject.transform.position = spawnPos;
            bulletImpactObject.transform.rotation = spawnRot;

            //parent the impact prefab to the collider it hit (for example hit players collider now is child of player)
            //bulletImpactObject.transform.SetParent(colliders[0].transform);

            //delete the bullet impact after 10 seconds
            //Destroy(bulletImpactObject, 10f);
            StartCoroutine(ObjectPoolManager.Instance.ReturnToPoolWithDelay(QueueType.BulletImpact, bulletImpactObject, 10f));
        }
    }
}
