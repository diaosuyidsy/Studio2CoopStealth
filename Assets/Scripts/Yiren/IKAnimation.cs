using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKAnimation : MonoBehaviour
{
    [Header("Body")]
    public Vector3 bodyPositionOffset;
    private Vector3 bodyPosition;

    [Header("Head")]
    [Range(0,1)]
    public float ikLookAtAmount = 0f;
    public Transform lookObj = null;

    [Header("Hands")]
    public Transform rightHandObj = null;
    public Transform leftHandObj = null;
    [Range(0, 1)]
    public float ikRightHandPositionAmount = 0f;
    [Range(0, 1)]
    public float ikRightHandRotationAmount = 0f;
    [Range(0, 1)]
    public float ikLeftHandPositionAmount = 0f;
    [Range(0, 1)]
    public float ikLeftHandRotationAmount = 0f;

    [Header("Elbow")]
    public Transform rightElbowObj = null;
    public Transform leftElbowObj = null;
    [Range(0, 1)]
    public float ikRightElbowPositionAmount = 0f;
    [Range(0, 1)]
    public float ikLeftElbowPositionAmount = 0f;

    [Header("Feet")]
    public Transform rightFootObj = null;
    public Transform leftFootObj = null;
    [Range(0, 1)]
    public float ikRightFootPositionAmount = 0f;
    [Range(0, 1)]
    public float ikRightFootRotationAmount = 0f;
    [Range(0, 1)]
    public float ikLeftFootPositionAmount = 0f;
    [Range(0, 1)]
    public float ikLeftFootRotationAmount = 0f;

    [Header("Knee")]
    public Transform rightKneeObj = null;
    public Transform leftKneeObj = null;
    [Range(0, 1)]
    public float ikRightKneePositionAmount = 0f;
    [Range(0, 1)]
    public float ikLeftKneePositionAmount = 0f;

    private Animator animator;
    private bool isBodyInitialized = false;
    private bool isIKOn = false;

    #region Awake
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    #endregion
    #region OnAnimatorIK

    public void SetIKOn()
    {
        isIKOn = true;
    }
    
    public void SetIKOff()
    {
        isIKOn = false;
    }

    public void setHandPos(Transform r, Transform l)
    {
        rightHandObj = r;
        leftHandObj = l;
    }
    
    //a callback for calculating IK
    void OnAnimatorIK(int layerIndex)
    {
        if (!animator)
            return;

        if (!isIKOn)
        {
            return;
        }
        BodyPositionControl();
        iKLookAtControl();
        iKRightHandControl();
        iKLeftHandControl();
        iKRightElbowControl();
        iKLeftElbowControl();
        iKRightFootControl();
        iKLeftFootControl();
        iKRightKneeControl();
        iKLeftKneeControl();
    }
    #endregion
    #region BODY
    private void BodyPositionControl()
    {
        if (bodyPositionOffset == Vector3.zero)
            return;

        if (!isBodyInitialized)
        {
            bodyPosition = animator.bodyPosition;
            isBodyInitialized = true;
        }

        animator.bodyPosition = bodyPosition + bodyPositionOffset;
    }
    #endregion
    #region HEAD
    private void iKLookAtControl()
    {
        if (ikLookAtAmount < 0.01f)
            return;

        if (lookObj == null)
            return;

        animator.SetLookAtWeight(ikLookAtAmount);
        animator.SetLookAtPosition(lookObj.position);
       
    }
    #endregion
    #region HAND
    private void iKRightHandControl()
    {
        if (ikRightHandPositionAmount < 0.01f && ikRightHandRotationAmount < 0.01f)
            return;

        if (rightHandObj == null)
            return;

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikRightHandPositionAmount);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikRightHandRotationAmount);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
    }

    private void iKLeftHandControl()
    {
        if (ikLeftHandPositionAmount < 0.01f && ikLeftHandRotationAmount < 0.01f)
            return;

        if (leftHandObj == null)
            return;

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikLeftHandPositionAmount);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikLeftHandRotationAmount);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
    }
    #endregion
    #region ELBOW
    private void iKRightElbowControl()
    {
        if (ikRightElbowPositionAmount < 0.01f)
            return;

        if (rightElbowObj == null)
            return;

        animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ikRightElbowPositionAmount);
        animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowObj.position);
    }

    private void iKLeftElbowControl()
    {
        if (ikLeftElbowPositionAmount < 0.01f)
            return;

        if (leftElbowObj == null)
            return;

        animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ikLeftElbowPositionAmount);
        animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowObj.position);
    }
    #endregion
    #region FOOT
    private void iKRightFootControl()
    {
        if (ikRightFootPositionAmount < 0.01f && ikRightFootRotationAmount < 0.01f)
            return;

        if (rightFootObj == null)
            return;

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikRightFootPositionAmount);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, ikRightFootRotationAmount);
        animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootObj.position);
        animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootObj.rotation);
    }

    private void iKLeftFootControl()
    {
        if (ikLeftFootPositionAmount < 0.01f && ikLeftFootRotationAmount < 0.01f)
            return;

        if (leftFootObj == null)
            return;

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikLeftFootPositionAmount);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, ikLeftFootRotationAmount);
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootObj.position);
        animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootObj.rotation);
    }
    #endregion
    #region KNEE
    private void iKRightKneeControl()
    {
        if (ikRightKneePositionAmount < 0.01f)
            return;

        if (rightKneeObj == null)
            return;

        animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, ikRightKneePositionAmount);
        animator.SetIKHintPosition(AvatarIKHint.RightKnee, rightKneeObj.position);
    }

    private void iKLeftKneeControl()
    {
        if (ikLeftKneePositionAmount < 0.01f)
            return;

        if (leftKneeObj == null)
            return;

        animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, ikLeftKneePositionAmount);
        animator.SetIKHintPosition(AvatarIKHint.LeftKnee, leftKneeObj.position);
    }
    #endregion
}
