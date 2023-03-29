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

public class Car : MonoBehaviour
{

    [SerializeField] private float initialMotorTorque = 10.0f;
    [SerializeField] private float motorTorqueIncreasePerSecond = 1.0f;
    [SerializeField] private float maxMotorTorque = 1000.0f;
    [SerializeField] private float maxSteeringAngle = 20.0f;
    [SerializeField] private Transform meshParentTransform = null;
    [SerializeField] private List<AxleInfo> axels;


    private float currentMotorTorque = 0.0f;
    private Direction currentDirection = Direction.Middle;
    private Quaternion initialMeshParentRotation = Quaternion.identity;

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

        // This is needed since the car models needed to be rotated 180 around the y-axis to face
        // forward; which is a requirement for the WheelCollider.
        initialMeshParentRotation = meshParentTransform.transform.localRotation;
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

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Obstacle"))
        {
            SceneManager.LoadScene("MainMenu");
        }
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

    void moveWheelModel(WheelCollider collider, Transform model)
    {
        Vector3 colliderWorldPosition;
        Quaternion colliderWorldRotation;

        collider.GetWorldPose(out colliderWorldPosition, out colliderWorldRotation);

        model.transform.position = colliderWorldPosition;
        model.transform.rotation = colliderWorldRotation * initialMeshParentRotation;
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
}
