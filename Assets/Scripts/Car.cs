using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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

    public bool hasMotor = false;
    public bool hasSteering = false;
}

public class Car : MonoBehaviour
{

    [SerializeField] private float initialMotorTorque = 10.0f;
    [SerializeField] private float motorTorqueIncreasePerSecond = 1.0f;
    [SerializeField] private float maxMotorTorque = 1000.0f;
    [SerializeField] private float maxSteeringAngle = 20.0f;
    [SerializeField] private float antiRollForce = 5000.0f;
    [SerializeField] private Transform meshParentTransform = null;
    [SerializeField] private Transform centerOfMass = null;


    [SerializeField] private WheelCollider frontLeft = null;
    [SerializeField] private WheelCollider frontRight = null;
    [SerializeField] private WheelCollider backLeft = null;
    [SerializeField] private WheelCollider backRight = null;
    [SerializeField] private TextMeshProUGUI text = null;

    [SerializeField] private List<AxleInfo> axels;



    private Rigidbody rigidbody = null;
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
        rigidbody = GetComponent<Rigidbody>();

        currentMotorTorque = initialMotorTorque;

        // This is needed since the car models needed to be rotated 180 around the y-axis to face
        // forward; which is a requirement for the WheelCollider.
        initialMeshParentRotation = meshParentTransform.transform.localRotation;

        rigidbody.centerOfMass = centerOfMass.transform.localPosition;
    }

    void Update()
    {
        updateTextWithSpeed();
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

        applyAntiRoll(axle.leftWheelCollider, axle.rightWheelCollider);

        moveWheelModel(axle.leftWheelCollider, axle.leftWheelModel);
        moveWheelModel(axle.rightWheelCollider, axle.rightWheelModel);
    }

    void applyAntiRoll(WheelCollider leftWheel, WheelCollider rightWheel)
    {
        WheelHit hit;
        float upForceLeft = 1.0f;
        float upForceRight = 1.0f;
        bool leftIsGrounded = leftWheel.GetGroundHit(out hit);

        if (leftIsGrounded)
        {
            upForceLeft =
                (-leftWheel.transform.InverseTransformPoint(hit.point).y - leftWheel.radius)
                    / leftWheel.suspensionDistance;
        }

        bool rightIsGrounded = rightWheel.GetGroundHit(out hit);

        if (rightIsGrounded)
        {
            upForceRight =
                (-rightWheel.transform.InverseTransformPoint(hit.point).y - rightWheel.radius)
                    / rightWheel.suspensionDistance;
        }


        float scaledAntiRollForce = (upForceLeft - upForceRight) * antiRollForce;

        if (leftIsGrounded)
        {
            rigidbody.AddForceAtPosition(
                leftWheel.transform.up * -scaledAntiRollForce,
                leftWheel.transform.position);
        }

        if (rightIsGrounded)
        {
            rigidbody.AddForceAtPosition(
                rightWheel.transform.up * -scaledAntiRollForce,
                rightWheel.transform.position);
        }
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

    void updateTextWithSpeed()
    {
        float mph = getMph(frontLeft);

        text.text = $"Motor Torque: {currentMotorTorque} | RPM: {frontLeft.rpm}\nMPH: {mph}";
    }

    float getMph(WheelCollider collider)
    {
        // radius is in meters
        float circumFerenceM = 2.0f * 3.14f * collider.radius;
        float speedOnKmh = (circumFerenceM / 1000.0f * collider.rpm) * 60;

        // converting kmh to mph
        return speedOnKmh * 0.62f;
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
