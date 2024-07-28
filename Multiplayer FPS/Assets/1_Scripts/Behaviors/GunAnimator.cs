using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Photon.Pun;

public class GunAnimator : MonoBehaviour
{
    [ResizableTextArea][SerializeField] private string description = "Animates the Gun Sway and recoil";
    public GunController gunController;

    private Quaternion originalRotation;
    private Quaternion recoilRotation;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        //online and not mine
        if (PhotonNetwork.IsConnected && !gunController.playerRefrences.pv.IsMine) { return; }
        //if paused then dont sway
        if (PauseManager.Instance.paused) { return; }

        HandleSway();
        HandleRecoil();
    }

    private void HandleSway()
    {
        // Get mouse input
        float mouseX = gunController.playerRefrences.playerInput.mouseX * gunController.gunData.swayMultiplier;
        float mouseY = gunController.playerRefrences.playerInput.mouseY * gunController.gunData.swayMultiplier;

        // Calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        // Get the overall target rotation
        Quaternion targetRotation = rotationX * rotationY;

        // Rotate with sway
        transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation * targetRotation * recoilRotation, gunController.gunData.smooth * Time.deltaTime);
    }

    private void HandleRecoil()
    {
        // Lerp back to the original rotation
        recoilRotation = Quaternion.Slerp(recoilRotation, Quaternion.identity, gunController.gunData.recoilReturnSpeed * Time.deltaTime);
    }

    public void ApplyRecoil()
    {
        // Add recoil
        recoilRotation *= Quaternion.Euler(-gunController.gunData.recoilAmount, Random.Range(-gunController.gunData.recoilAmount, gunController.gunData.recoilAmount), 0);
    }
}
