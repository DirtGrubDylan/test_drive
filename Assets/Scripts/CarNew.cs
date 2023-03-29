using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[System.Serializable]
public class AxleInfo
{
    // The axel's left wheel collider and model.
    public WheelCollider leftWheelCollider;
    public Transform leftWheelModel;

    // The axel's right wheel collider and model.
    public WheelCollider rightWheelCollider;
    public Transform rightWheelModel;

    public bool hasMotor;
    public bool hasSteering;
}

public class CarNew : MonoBehaviour
{
    [SerializeField] private float initialMotorTorque = 10.0f;
    [SerializeField] private float motorTorqueIncreasePerSecond = 1.0f;
    [SerializeField] private float maxMotorTorque = 1000.0f;
    [SerializeField] private float maxSteeringAngle = 20.0f;
    [SerializeField] private List<AxleInfo> axels;


    private float currentMotorTorque = 0.0f;
    [SerializeField] private Direction currentDirection = Direction.Middle;

    void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentMotorTorque = initialMotorTorque;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (AxleInfo axle in axels)
        {
            moveAxle(axle);
        }

        currentMotorTorque += motorTorqueIncreasePerSecond * Time.fixedDeltaTime;

        currentMotorTorque = Mathf.Clamp(currentMotorTorque, 0.0f, maxMotorTorque);
    }

    void moveAxle(AxleInfo axle)
    {
        if (axle.hasSteering)
        {
            axle.leftWheelCollider.steerAngle = getSteering();
            axle.rightWheelCollider.steerAngle = getSteering();
        }

        if (axle.hasMotor)
        {
            axle.leftWheelCollider.motorTorque = currentMotorTorque;
            axle.rightWheelCollider.motorTorque = currentMotorTorque;
        }

        moveWheelModel(axle.leftWheelCollider, axle.leftWheelModel);
        moveWheelModel(axle.rightWheelCollider, axle.rightWheelModel);
    }

    float getSteering()
    {
        return (int)currentDirection * maxSteeringAngle;
    }

    // TODO(dirtgrubdylan): Fix `moveWheelModel` rotations **AND** transformations.
    //     * Fix model rotation in Unity so that the models stop flipping around.
    //     * Fix collider transformations so the wheel models stop detatching from body model.
    void moveWheelModel(WheelCollider collider, Transform model)
    {
        Vector3 colliderPosition;
        Quaternion colliderRotation;

        collider.GetWorldPose(out colliderPosition, out colliderRotation);

        model.transform.position = colliderPosition;
        model.transform.rotation = colliderRotation;
    }

    bool touchedRightSideOfScreen(Touch touch)
    {
        return touch.screenPosition.x > Screen.width / 2;
    }

    public enum Direction : int
    {
        Left = -1,
        Middle = 0,
        Right = 1,
    }

    public void steer(int directionValue)
    {
        currentDirection = (Direction)directionValue;
    }

    public void steer(Direction direction)
    {
        this.currentDirection = direction;
    }


}
