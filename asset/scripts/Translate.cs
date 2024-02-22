using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translate : MonoBehaviour
{
    public float translationSpeed = 2.0f; // Adjust this to control the translation speed
    public float translationDistance = 5.0f; // Adjust this to control the distance of translation
    private Vector3 initialPosition;
    private int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the target position based on the direction
        Vector3 targetPosition = initialPosition + Vector3.forward* direction * translationDistance;

        // Move the object towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, translationSpeed * Time.deltaTime);

        // Check if the object has reached the target position
        if (transform.position == targetPosition)
        {
            // Reverse the direction
            direction *= -1;
        }
    }
}
