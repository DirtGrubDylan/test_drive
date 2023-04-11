using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCar : MonoBehaviour
{
    [SerializeField] private Rigidbody sphere = null;
    [SerializeField] private float forwardAcceleration = 10.0f;
    [SerializeField] private float forwardAccelerationScalar = 1000.0f;
    [SerializeField] private float turningSpeed = 100.0f;
    [SerializeField] private float additionalGravityForce = 10.0f;
    [SerializeField] private float additionalGravityForceScalar = 100.0f;
    [SerializeField] private float dragInAir = 0.1f;
    [SerializeField] private Transform rayToFindGroundStartingPoint = null;
    [SerializeField] private float rayLengthToFindGround = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 spherePositionOffset = Vector3.zero;
    private float initialSphereDrag = 0.0f;
    private Direction currentDirection = Direction.Middle;
    private float forwardAccelerationInput = 0.0f;
    private float steerInput = 0.0f;
    private bool grounded = true;

    // Start is called before the first frame update
    void Start()
    {
        spherePositionOffset = transform.position - sphere.transform.position;

        sphere.transform.SetParent(null);

        initialSphereDrag = sphere.drag;
    }

    // Update is called once per frame
    void Update()
    {
        forwardAccelerationInput = forwardAcceleration * forwardAccelerationScalar;

        steerInput = (int)currentDirection * turningSpeed * Time.deltaTime;

        transform.position = sphere.transform.position + spherePositionOffset;

        if (isGrounded())
        {
            transform.rotation =
                Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * steerInput);
        }

        Debug.Log($"Speed: {Mathf.RoundToInt(sphere.velocity.magnitude)}");
    }

    void FixedUpdate()
    {
        if (isGrounded())
        {
            sphere.drag = initialSphereDrag;
            sphere.AddForce(transform.forward * forwardAccelerationInput);
        }
        else
        {
            sphere.drag = dragInAir;
            sphere.AddForce(-Vector3.up * additionalGravityForce * additionalGravityForceScalar);
        }
    }

    bool isGrounded()
    {
        RaycastHit raycastHit;
        Vector3 startingPosition = rayToFindGroundStartingPoint.position;

        return Physics.Raycast(startingPosition, -transform.up, out raycastHit, groundLayer);
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
