using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyLimb : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private Transform targetLimb;
    [SerializeField] private ConfigurableJoint m_ConfigurableJoint;
    Quaternion targetInitialRotation;

    public void Initialize(Transform targetLimb)
    {
        this.targetLimb = targetLimb;
        //this.m_ConfigurableJoint = configurableJoint;
    }

    void Start()
    {
        if (this.m_ConfigurableJoint == null)
            this.m_ConfigurableJoint = this.GetComponent<ConfigurableJoint>();
        this.targetInitialRotation = this.targetLimb.transform.localRotation;
    }

    private void FixedUpdate() {
        this.m_ConfigurableJoint.targetRotation = CopyRotation();
    }

    private Quaternion CopyRotation() {
        return Quaternion.Inverse(this.targetLimb.localRotation) * this.targetInitialRotation;
    }
}
