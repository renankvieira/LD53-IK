using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]


//TODO: Auto add EnemyRigidbodyCollider
//TODO: Auto change layer and tag (Enemy)

public class CloneToRagdoll : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectLinks
    {
        public GameObject hip; // root or hip (centered object)
        public GameObject torso;
        public GameObject head;

        public GameObject shoulderL;
        public GameObject upperArmL;
        public GameObject lowerArmL;
        public GameObject handL;

        public GameObject shoulderR;
        public GameObject upperArmR;
        public GameObject lowerArmR;
        public GameObject handR;

        public GameObject upperLegL;
        public GameObject lowerLegL;
        public GameObject upperLegR;
        public GameObject lowerLegR;
    }

    [Header("Config")]
    public Vector3 boxColliderSize = new Vector3(0.25f, 0.5f, 0.25f);
    public float sphereColliderSize = 0.25f;
    public float capsuleColliderSize = 0.25f;
    public float capsuleColliderHeight = 0.5f;
    public bool useExampleLinkNames = false;

    [Header("References")]
    public GameObjectLinks gameObjectLinks;
    public Animator mainAnimator;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Control")]
    public GameObject cloneMainObject;
    public GameObjectLinks clonedLinks;

#if UNITY_EDITOR
    [Button]
    public void CreateActiveRagdoll()
    {
        //UnityEditor.Undo.RecordObject(transform.parent.gameObject, "Ragdoll Creation");
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        UnityEditor.Undo.RecordObject(this, "Ragdoll Fetch References");
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);

        if (mainAnimator != null)
            mainAnimator.enabled = false;

        CloneMainObject();

        SetupClonedObjects();
        SetupSpecialObjects();
        //SetupChildren();

        if (mainAnimator != null)
            mainAnimator.enabled = true;
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.enabled = false;
            cloneMainObject.GetComponent<CloneToRagdoll>().skinnedMeshRenderer.enabled = true;
        }
    }

    void CloneMainObject()
    {
        cloneMainObject = Instantiate(gameObject);
        cloneMainObject.transform.parent = transform.parent;
        cloneMainObject.transform.localPosition = transform.localPosition;
        cloneMainObject.transform.localRotation = transform.localRotation;
        cloneMainObject.transform.localScale = transform.localScale;
        cloneMainObject.name = "ActiveRagdoll";
        clonedLinks = cloneMainObject.GetComponent<CloneToRagdoll>().gameObjectLinks;
    }

    void SetupClonedObjects()
    {
        GameObject objectToSetup;

        objectToSetup = cloneMainObject;
        AddRigidbody(objectToSetup, true);


        SetupPiece(clonedLinks.hip, gameObjectLinks.hip, false, 1, 3, 3, ColliderType.BOX, cloneMainObject.GetRb(), 8000, true); //locked Hip, different than source
        SetupPiece(clonedLinks.torso, gameObjectLinks.torso, false, 1, 3, 3, ColliderType.BOX, clonedLinks.hip.GetRb(), 4500, true);
        SetupPiece(clonedLinks.head, gameObjectLinks.head, false, 1, 3, 3, ColliderType.SPHERE, clonedLinks.torso.GetRb(), 2000, true);

        SetupPiece(clonedLinks.shoulderL, gameObjectLinks.shoulderL, false, 1, 3, 3, ColliderType.NONE, clonedLinks.torso.GetRb(), 1000, true);
        SetupPiece(clonedLinks.upperArmL, gameObjectLinks.upperArmL, false, 1, 3, 3, ColliderType.CAPSULE, clonedLinks.shoulderL.GetRb(), 4000, true);
        SetupPiece(clonedLinks.lowerArmL, gameObjectLinks.lowerArmL, false, 1, 3, 3, ColliderType.CAPSULE, clonedLinks.upperArmL.GetRb(), 4000, true);
        SetupPiece(clonedLinks.handL, gameObjectLinks.handL, false, 1, 3, 3, ColliderType.CAPSULE, clonedLinks.lowerArmL.GetRb(), 4000, true);

        SetupPiece(clonedLinks.shoulderR, gameObjectLinks.shoulderR, false, 1, 3, 3, ColliderType.NONE, clonedLinks.torso.GetRb(), 1000, true);
        SetupPiece(clonedLinks.upperArmR, gameObjectLinks.upperArmR, false, 1, 3, 3, ColliderType.CAPSULE, clonedLinks.shoulderR.GetRb(), 4000, true);
        SetupPiece(clonedLinks.lowerArmR, gameObjectLinks.lowerArmR, false, 1, 3, 3, ColliderType.CAPSULE, clonedLinks.upperArmR.GetRb(), 4000, true);
        SetupPiece(clonedLinks.handR, gameObjectLinks.handR, false, 1, 3, 3, ColliderType.CAPSULE, clonedLinks.lowerArmR.GetRb(), 4000, true);


        SetupPiece(clonedLinks.upperLegL, gameObjectLinks.upperLegL, false, 1, 3, 3, ColliderType.CAPSULE, clonedLinks.torso.GetRb(), 4000, true);
        SetupPiece(clonedLinks.lowerLegL, gameObjectLinks.lowerLegL, false, 1, 3, 3, ColliderType.CAPSULE, clonedLinks.upperLegL.GetRb(), 4000, true);

        SetupPiece(clonedLinks.upperLegR, gameObjectLinks.upperLegR, false, 1, 3, 3, ColliderType.CAPSULE, clonedLinks.torso.GetRb(), 4000, true);
        SetupPiece(clonedLinks.lowerLegR, gameObjectLinks.lowerLegR, false, 1, 3, 3, ColliderType.CAPSULE, clonedLinks.upperLegR.GetRb(), 4000, true);
    }

    void SetupSpecialObjects()
    {
        ConfigurableJoint hipJoint = clonedLinks.hip.AddOrGetComponent<ConfigurableJoint>();
        SoftJointLimitSpring hipLimitSpring = hipJoint.linearLimitSpring;
        hipLimitSpring.spring = 120f;
        hipLimitSpring.damper = 0.5f;
        hipJoint.linearLimitSpring = hipLimitSpring;

        clonedLinks.head.AddComponent<ConstantForce>().force = Vector3.up * 80;
    }

    enum ColliderType { BOX, SPHERE, CAPSULE, NONE }

    void SetupPiece(GameObject clone, GameObject original, bool isKinematic = false, float mass = 1, float drag = 0, float angularDrag = 0.05f, ColliderType colliderType = ColliderType.NONE, Rigidbody connectedBody = null, float angularDriveSpring = 4000, bool lockedJointMotion = true)
    {

        AddRigidbody(clone, isKinematic, mass, drag, angularDrag);
        if (colliderType == ColliderType.BOX)
            AddBoxCollider(clone);
        else if (colliderType == ColliderType.SPHERE)
            AddSphereCollider(clone);
        else if (colliderType == ColliderType.CAPSULE)
            AddCapsuleCollider(clone);
        AddConfigurableJoint(clone, connectedBody, angularDriveSpring, lockedJointMotion);
        AddCopyLimb(clone, original);
    }

    Rigidbody AddRigidbody(GameObject go, bool isKinematic = false, float mass = 1, float drag = 0, float angularDrag = 0.05f)
    {
        Rigidbody rb = go.AddOrGetComponent<Rigidbody>();
        rb.isKinematic = isKinematic;
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;

        return rb;
    }

    BoxCollider AddBoxCollider(GameObject go)
    {
        BoxCollider bc = go.AddOrGetComponent<BoxCollider>();
        bc.size = boxColliderSize;
        return bc;
    }

    SphereCollider AddSphereCollider(GameObject go)
    {
        SphereCollider sc = go.AddOrGetComponent<SphereCollider>();
        sc.radius = sphereColliderSize;
        return sc;
    }

    CapsuleCollider AddCapsuleCollider(GameObject go)
    {
        CapsuleCollider cc = go.AddOrGetComponent<CapsuleCollider>();
        cc.radius = capsuleColliderSize;
        cc.height = capsuleColliderHeight;
        return cc;
    }


    ConfigurableJoint AddConfigurableJoint(GameObject go, Rigidbody connectedBody, float angularDriveSpring,
        bool lockedJointMotion = true)
    {
        ConfigurableJoint joint = go.AddOrGetComponent<ConfigurableJoint>();
        joint.connectedBody = connectedBody;

        JointDrive xDrive = joint.angularXDrive;
        JointDrive yzDrive = joint.angularYZDrive;
        xDrive.positionSpring = angularDriveSpring;
        yzDrive.positionSpring = angularDriveSpring;
        joint.angularXDrive = xDrive;
        joint.angularYZDrive = yzDrive;

        ConfigurableJointMotion jointMotionLock = lockedJointMotion ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
        joint.xMotion = jointMotionLock;
        joint.yMotion = jointMotionLock;
        joint.zMotion = jointMotionLock;

        return joint;
    }

    CopyLimb AddCopyLimb(GameObject clone, GameObject original)
    {
        CopyLimb copyLimb = clone.AddOrGetComponent<CopyLimb>();
        copyLimb.Initialize(original.transform);
        return copyLimb;
    }


    [Button]
    public void EraseLinkReferences()
    {
        gameObjectLinks = new GameObjectLinks();
    }

    [Button]
    public void FetchLinkReferences()
    {
        UnityEditor.Undo.RecordObject(this, "Ragdoll Fetch References");
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);

        Transform[] children = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            string childName = CleanUp(child.gameObject.name);
            //print(childName);
            string testedName;

            testedName = useExampleLinkNames ? "hip" : "root.x";
            if (gameObjectLinks.hip == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.hip = child.gameObject;
            testedName = useExampleLinkNames ? "torso" : "spine_01.x";
            if (gameObjectLinks.torso == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.torso = child.gameObject;
            testedName = useExampleLinkNames ? "head" : "head.x";
            if (gameObjectLinks.head == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.head = child.gameObject;

            testedName = useExampleLinkNames ? "shoulderL" : "shoulder.l";
            if (gameObjectLinks.shoulderL == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.shoulderL = child.gameObject;
            testedName = useExampleLinkNames ? "upperArmL" : "arm_stretch.l";
            if (gameObjectLinks.upperArmL == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.upperArmL = child.gameObject;
            testedName = useExampleLinkNames ? "lowerArmL" : "forearm_stretch.l";
            if (gameObjectLinks.lowerArmL == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.lowerArmL = child.gameObject;
            testedName = useExampleLinkNames ? "handL" : "hand.l";
            if (gameObjectLinks.handL == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.handL = child.gameObject;

            testedName = useExampleLinkNames ? "shoulderR" : "shoulder.r";
            if (gameObjectLinks.shoulderR == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.shoulderR = child.gameObject;
            testedName = useExampleLinkNames ? "upperArmR" : "arm_stretch.r";
            if (gameObjectLinks.upperArmR == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.upperArmR = child.gameObject;
            testedName = useExampleLinkNames ? "lowerArmR" : "forearm_stretch.r";
            if (gameObjectLinks.lowerArmR == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.lowerArmR = child.gameObject;
            testedName = useExampleLinkNames ? "handR" : "hand.r";
            if (gameObjectLinks.handR == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.handR = child.gameObject;

            testedName = useExampleLinkNames ? "upperLegL" : "thigh_stretch.l";
            if (gameObjectLinks.upperLegL == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.upperLegL = child.gameObject;
            testedName = useExampleLinkNames ? "lowerLegL" : "leg_stretch.l";
            if (gameObjectLinks.lowerLegL == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.lowerLegL = child.gameObject;

            testedName = useExampleLinkNames ? "upperLegR" : "thigh_stretch.r";
            if (gameObjectLinks.upperLegR == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.upperLegR = child.gameObject;
            testedName = useExampleLinkNames ? "lowerLegR" : "leg_stretch.r";
            if (gameObjectLinks.lowerLegR == null && CleanUp(childName) == CleanUp(testedName))
                gameObjectLinks.lowerLegR = child.gameObject;
        }
    }
#endif

    string CleanUp(string s)
    {
        return s.Replace(" ", "").Replace(".", "").Replace("_", "").ToLower();
    }

}
