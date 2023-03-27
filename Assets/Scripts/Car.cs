using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Car : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float speedIncreasePerSecond = 1.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private bool useTouchPosition = true;

    [SerializeField] private Direction direction = Direction.Middle;

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

    }

    // Update is called once per frame
    void Update()
    {
        movePerFrame();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.tag}");

        if (other.CompareTag("Obstacle"))
        {
            SceneManager.LoadScene("MainMenu");
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

    void movePerFrame()
    {
        handleTouch();

        rotate();

        moveForward();

        speed += speedIncreasePerSecond * Time.deltaTime;
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

    void rotate()
    {
        transform.Rotate(0.0f, (int)direction * rotationSpeed * Time.deltaTime, 0.0f);
    }

    void moveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
