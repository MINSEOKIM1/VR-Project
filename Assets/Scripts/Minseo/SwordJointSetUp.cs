using UnityEngine;

public class SwordJointSetup : MonoBehaviour
{
    public Rigidbody followBody;

    void Start()
    {
        var joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = followBody;

        // Position Drive
        JointDrive posDrive = new JointDrive();
        posDrive.positionSpring = 1200;
        posDrive.positionDamper = 80;
        posDrive.maximumForce = Mathf.Infinity;

        joint.xDrive = posDrive;
        joint.yDrive = posDrive;
        joint.zDrive = posDrive;

        // Rotation Drive
        JointDrive rotDrive = new JointDrive();
        rotDrive.positionSpring = 2500;
        rotDrive.positionDamper = 100;
        rotDrive.maximumForce = Mathf.Infinity;

        joint.angularXDrive = rotDrive;
        joint.angularYZDrive = rotDrive;

        joint.rotationDriveMode = RotationDriveMode.Slerp;
    }
}
