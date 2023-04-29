using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandIK : MonoBehaviour
{
    public AvatarIKGoal avatarIKGoal = AvatarIKGoal.LeftHand;
    [Range(0f, 1f)] public float positionWeight = 1f;
    [Range(0f, 1f)] public float rotationWeight = 1f;

    public Transform handleTransform;
    public Animator mainAnimator;

    public Transform LeftHand;
    public Animator ThisAnimator;

    public void OnAnimatorIK(int layerIndex)
    {
        ThisAnimator.SetIKPositionWeight(avatarIKGoal, positionWeight);
        ThisAnimator.SetIKRotationWeight(avatarIKGoal, rotationWeight);
        ThisAnimator.SetIKPosition(avatarIKGoal, handleTransform.position);
        ThisAnimator.SetIKRotation(avatarIKGoal, handleTransform.rotation);


        ThisAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        ThisAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        ThisAnimator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHand.position);
        ThisAnimator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHand.rotation);
    }

}
