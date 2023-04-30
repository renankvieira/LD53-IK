using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class RagdollController : MonoBehaviour
{
    [Header("Config")]
    [Range(0f, 2f)] public float desiredRatio = 1f;

    public bool useToggler= false;
    public float ratioUp = 1f;
    public float ratioDown = 0.25f;

    public bool useXSpring;
    public bool useYzSpring;
    public bool useMass;

    public float dragOnDeath = 0f;
    public float angularDragOnDeath = 0f;

    public Vector3 pushForceOnDeath = new Vector3(1000f, 1000f, 1000f);

    [Header("References")]
    public ConfigurableJoint hipJoint;

    [Header("Control")]
    public bool characterIsUp = true;
    public List<JointData> jointDatas;
    public float lastusedRatio = 1f;
    public ConstantForce[] upForces;
    public CopyLimb[] copyLimbs;

    [Header("Debug")]
    public bool desconnectBody = true;
    public bool freeMotion = true;

    void Start()
    {
        ConfigurableJoint[] joints = GetComponentsInChildren<ConfigurableJoint>();
        upForces = GetComponentsInChildren<ConstantForce>();
        copyLimbs = GetComponentsInChildren<CopyLimb>();

        foreach (ConfigurableJoint joint in joints)
        {
            JointData jointData = new JointData();
            jointData.Initialize(joint, useXSpring, useYzSpring, useMass);
            jointDatas.Add(jointData);
        }

        lastusedRatio = desiredRatio;
        foreach (JointData data in jointDatas)
            data.SetToRatio(desiredRatio);



        //Debug.LogWarning("Warning: possible bad code: joint.xDrive = xDrive instead of angularDrive");
    }

    void FixedUpdate()
    {
        if (useToggler)
            desiredRatio = characterIsUp ? ratioUp : ratioDown;

        if (desiredRatio == lastusedRatio)
            return;

        lastusedRatio = desiredRatio;
        foreach (JointData data in jointDatas)
            data.SetToRatio(desiredRatio);
    }

    public void OnEnemyDeath()
    {
        characterIsUp = false;
        //hipjoint is free

        upForces.ToggleAll(false);
        copyLimbs.ToggleAll(false);

        if (desconnectBody)
            hipJoint.connectedBody = null;
        if (freeMotion)
        {
            hipJoint.xMotion = ConfigurableJointMotion.Free;
            hipJoint.yMotion = ConfigurableJointMotion.Free;
            hipJoint.zMotion = ConfigurableJointMotion.Free;
        }

        AddForce();
    }

    [Button]
    public void AddForce()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.drag = dragOnDeath;
            rb.angularDrag = angularDragOnDeath;

            Vector3 forceVector = pushForceOnDeath;
            //Vector3 directionFromPlayer = transform.position - GameSceneContents.Instance.toyPlayer.transform.position;
            Vector3 directionFromPlayer = transform.position - Vector3.zero;
            Debug.LogWarning("Xablau");
            directionFromPlayer = directionFromPlayer.normalized;
            forceVector.x *= directionFromPlayer.x;
            forceVector.z *= directionFromPlayer.z;

            rb.AddForce(forceVector);
        }
    }

    [System.Serializable]
    public class JointData
    {
        public ConfigurableJoint joint;
        public float initialAngularXPositionSpring;
        public float initialAngularYZPositionSpring;
        public float initialMassScale;

        bool useXSpring = false;
        bool useYzSpring = false;
        bool useMass = false;

        public void Initialize(ConfigurableJoint joint, bool useXSpring, bool useYzSpring, bool useMass)
        {
            this.joint = joint;
            initialAngularXPositionSpring = joint.angularXDrive.positionSpring;
            initialAngularYZPositionSpring = joint.angularYZDrive.positionSpring;
            initialMassScale = joint.massScale;

            this.useXSpring = useXSpring;
            this.useYzSpring = useYzSpring;
            this.useMass = useMass;
        }

        public void SetToRatio(float ratio)
        {
            if (useXSpring)
            {
                JointDrive xDrive = joint.angularXDrive;
                xDrive.positionSpring = initialAngularXPositionSpring * ratio;
                joint.angularXDrive = xDrive;
            }

            if (useYzSpring)
            {
                JointDrive yzDrive = joint.angularYZDrive;
                yzDrive.positionSpring = initialAngularYZPositionSpring * ratio;
                joint.angularYZDrive = yzDrive;
            }

            if (useMass)
            {
                joint.massScale = initialMassScale * ratio;
            }
        }
    }


}
