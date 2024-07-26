using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RootMotion.FinalIK;
using NaughtyAttributes;
using static System.TimeZoneInfo;

public class HandTransitioner : MonoBehaviour
{
    [Header("Connected Components")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PhotonView pv;
    [SerializeField] private FullBodyBipedIK ik; // Assign your FullBodyBipedIK component in the inspector
    [SerializeField] private Transform leftIntermediateT; //when changing where the hand is supposed to go this is set as the target and then it is moved to its position
    [SerializeField] private Transform rightIntermediateT; //when changing where the hand is supposed to go this is set as the target and then it is moved to its position

    //left
    public float transitionDurationLeft = 1f; // Duration of the transition in seconds
    private float transitionTimeLeft;
    [ReadOnly] public bool moveLeftHandToTarget = false;
    [ReadOnly][SerializeField] private Transform previousLeftTarget;
    [ReadOnly][SerializeField] private Transform newLeftTarget;
    

    void Start()
    {
        //ik.solver.leftHandEffector.target = leftHandTarget;
        //ik.solver.rightHandEffector.target = rightHandTarget;
    }

    void Update()
    {
        if(moveLeftHandToTarget)
        {
            transitionTimeLeft += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimeLeft / transitionDurationLeft);

            leftIntermediateT.position = Vector3.Lerp(previousLeftTarget.position, newLeftTarget.position, t);

            //finished
            if (t >= 1f)
            {
                //stop from continuing
                moveLeftHandToTarget = false;

                //set the new target you made it to
                ik.solver.leftHandEffector.target = newLeftTarget;
            }
        }
    }


    [SerializeField] Transform targetMag;
    [Button]
    void SetTargetMag()
    {
        SetTargets(targetMag, transitionDurationLeft);
    }

    [SerializeField] Transform targetMagDisposal;
    [Button]
    void SetTargetMagDisposal()
    {
        SetTargets(targetMagDisposal, transitionDurationLeft);
    }

    void SetTargets(Transform target, float timeToTransition = 1)
    {
        //set the amount of time it should take
        transitionDurationLeft = timeToTransition;

        //store previous target
        previousLeftTarget = ik.solver.leftHandEffector.target;

        //position the intermediate Target at the current one
        leftIntermediateT.position = ik.solver.leftHandEffector.target.position;
        //rotate the intermediate Target to match the current one
        leftIntermediateT.rotation = ik.solver.leftHandEffector.target.rotation;
        
        //make the intermediate target the new target
        ik.solver.leftHandEffector.target = leftIntermediateT;

        //set the new target goal
        newLeftTarget = target;

        //reset timer
        transitionTimeLeft = 0f;

        //set the state to start the moving
        moveLeftHandToTarget = true;
    }
}
