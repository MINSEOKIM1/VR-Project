using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRGrabSword : XRBaseInteractable
{
    public ConfigurableJoint joint;
    public Rigidbody swordRb;
    public Rigidbody followTarget;

    public float attackThreshold = 6f;

    protected override void Awake()
    {
        base.Awake();
        joint = GetComponent<ConfigurableJoint>();
        swordRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float speed = swordRb.linearVelocity.magnitude;
        Debug.Log(speed);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        
        transform.position = followTarget.position;
        transform.rotation = followTarget.rotation;
        
        JointDrive posDrive = new JointDrive();
        posDrive.positionSpring = 20000;
        posDrive.positionDamper = 80;
        posDrive.maximumForce = Mathf.Infinity;

        joint.xDrive = posDrive;
        joint.yDrive = posDrive;
        joint.zDrive = posDrive;

        // Rotation Drive
        JointDrive rotDrive = new JointDrive();
        rotDrive.positionSpring = 5000;
        rotDrive.positionDamper = 100;
        rotDrive.maximumForce = Mathf.Infinity;

        joint.angularXDrive = rotDrive;
        joint.angularYZDrive = rotDrive;
        joint.slerpDrive = rotDrive;

        joint.rotationDriveMode = RotationDriveMode.Slerp;
        
        joint.connectedBody = followTarget;  
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        Release();
    }
    
    public void Release()
    {
        JointDrive posDrive = new JointDrive();
        posDrive.positionSpring = 0;
        posDrive.positionDamper = 0;
        posDrive.maximumForce = Mathf.Infinity;

        joint.xDrive = posDrive;
        joint.yDrive = posDrive;
        joint.zDrive = posDrive;

        // Rotation Drive
        JointDrive rotDrive = new JointDrive();
        rotDrive.positionSpring = 0;
        rotDrive.positionDamper = 0;
        rotDrive.maximumForce = Mathf.Infinity;

        joint.angularXDrive = rotDrive;
        joint.angularYZDrive = rotDrive;
        joint.slerpDrive = rotDrive;


        joint.rotationDriveMode = RotationDriveMode.Slerp;
        
        joint.connectedBody = null;
        
        swordRb.linearVelocity = Vector3.zero;
        swordRb.angularVelocity = Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        float speed = swordRb.linearVelocity.magnitude;
    
        if (speed > attackThreshold)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                BattleManager.Instance.Attack(5);
                StartCoroutine(GhostThrough());
            }
        }
    }

    private IEnumerator GhostThrough()
    {
        GetComponent<Collider>().isTrigger = true;
        
        yield return new WaitForSeconds(0.2f);
        
        GetComponent<Collider>().isTrigger = false;
    }
}
