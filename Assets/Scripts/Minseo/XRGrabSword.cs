using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRGrabSword : XRBaseInteractable
{
    public ConfigurableJoint joint;
    public Rigidbody swordRb;
    public Rigidbody followTarget;

    protected override void Awake()
    {
        base.Awake();
        joint = GetComponent<ConfigurableJoint>();
        swordRb = GetComponent<Rigidbody>();
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

}
