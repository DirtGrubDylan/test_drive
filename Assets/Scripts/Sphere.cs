using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sphere : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        Debug.Log($"Collided with {other.collider.tag} on layer {other.gameObject.layer}");

        if (other.collider.CompareTag("Obstacle"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
