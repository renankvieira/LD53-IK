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

    public void OnAnimatorIK(int layerIndex)
    {
        mainAnimator.SetIKPositionWeight(avatarIKGoal, positionWeight);
        mainAnimator.SetIKRotationWeight(avatarIKGoal, rotationWeight);
        mainAnimator.SetIKPosition(avatarIKGoal, handleTransform.position);
        mainAnimator.SetIKRotation(avatarIKGoal, handleTransform.rotation);
    }

}
