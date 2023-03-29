using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Car : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float speedIncreasePerSecond = 1.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private bool useTouchPosition = true;

    [SerializeField] private Direction direction = Direction.Middle;

    private Rigidbody rigidBody = null;
    private float originalDrag = 0.0f;

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
        rigidBody = GetComponent<Rigidbody>();
        originalDrag = rigidBody.drag;
    }

    void FixedUpdate()
    {
        movePerFrameFixed();
    }

    void OnCollisionEnter(Collision other)
    {
        {
            Debug.Log($"{other.collider.tag}");

            if (other.collider.CompareTag("Obstacle"))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    public void steer(int directionValue)
    {
        direction = (Direction)directionValue;
    }

    public void steer(Direction direction)
    {
        this.direction = direction;
    }

    void movePerFrameFixed()
    {
        handleTouch();

        rotateFixed();

        moveForwardFixed();

        speed += speedIncreasePerSecond * Time.fixedDeltaTime;
    }


    void handleTouch()
    {
        if (!useTouchPosition)
        {
            return;
        }

        if (Touch.activeFingers.Count == 1)
        {
            Touch activeTouch = Touch.activeFingers[0].currentTouch;

            if (activeTouch.began)
            {
                if (touchedRightSideOfScreen(activeTouch))
                {
                    steer(Direction.Right);
                }
                else
                {
                    steer(Direction.Left);
                }
            }
        }
    }

    void rotateFixed()
    {
        float yRotation = (int)direction * rotationSpeed * Time.fixedDeltaTime;
        Quaternion relativeRotation = Quaternion.Euler(0.0f, yRotation, 0.0f);

        rigidBody.MoveRotation(rigidBody.rotation * relativeRotation);
    }

    void moveForwardFixed()
    {
        rigidBody.AddRelativeForce(
            Vector3.forward * speed * Time.fixedDeltaTime, ForceMode.Impulse);
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
}
