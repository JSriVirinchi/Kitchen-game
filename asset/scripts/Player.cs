using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private GameInput gameInput;
    [Header("Health Management")]
    [SerializeField] private int health = 3; // Player's health attribute
    [SerializeField] private Image[] healthPoints;
    [Header("Game Over")]
    [SerializeField] private TextMeshProUGUI gameOverText;  // Drag the GameOverText UI element here
    private bool isGameOver = false;
    [Header("Position and Rotation UI")]
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI rotationText;
    [Header("Restart")]
    [SerializeField] private GameObject restartButton;


    private Rigidbody rb; // reference to the rigidbody component
    private bool isGrounded = true; // flag to check if the player is grounded
    private bool isWalking;
    private Vector3 lastInteractDir;



    private void Start()
    {
        gameInput.OnInteractActions += GameInput_OnInteractActions;
        rb = GetComponent<Rigidbody>();
    }

    private void GameInput_OnInteractActions(object sender, System.EventArgs e)
    {
        //Debug.Log("hi");
        Vector2 inputVector = gameInput.GetMovementDirNormalised();
        Vector3 movedir = new Vector3(inputVector.x, 0f, inputVector.y);

        //Debug.Log(movedir);
        //Debug.Log(lastInteractDir);

        float interactDistance = 2f;

        if (movedir != Vector3.zero)
        {
            lastInteractDir = movedir;
            //Debug.Log("zero movedir");
        }

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounters clearCounters))
            {
                clearCounters.Interact();
                //Debug.Log("clearCOunter");
            }
        }

    }

    private void Update()
    {
        if (!isGameOver)
        {
            HandleInput();
            HandleInteraction();
            UpdateUI();
        }
            

    }


    private void HandleInteraction()
    {

        Vector2 inputVector = gameInput.GetMovementDirNormalised();
        Vector3 movedir = new Vector3(inputVector.x, 0f, inputVector.y);

        float interactDistance = 2f;

        if (movedir != Vector3.zero)
        {
            lastInteractDir = movedir;
        }

        if (Physics.Raycast(transform.position, movedir, out RaycastHit raycastHit, interactDistance))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounters clearCounters))
            {
                //clearCounters.Interact();
                //Debug.Log("HandleInteraction");
            }
        }
    }

    private void HandleInput()
    {
        Vector2 inputVector = gameInput.GetMovementDirNormalised();

        Vector3 movedir = new Vector3(inputVector.x, 0f, inputVector.y);

        float playerRadius = .7f;
        float playerHeight = 2f;
        float moveDistance = Time.deltaTime * moveSpeed;
        Vector3 bodyTop = transform.position + (Vector3.up * playerHeight);


        bool canMove = !Physics.CapsuleCast(transform.position, bodyTop, playerRadius, movedir, moveDistance);

        if (!canMove)
        {
            // checking if it can move in X direction
            Vector3 moveDirX = new Vector3(movedir.x, 0, 0);
            canMove = !Physics.CapsuleCast(transform.position, bodyTop, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                movedir = moveDirX;
            }
            else
            {
                // Checking if it can move in Z direction
                Vector3 moveDirZ = new Vector3(0, 0, movedir.z);
                canMove = !Physics.CapsuleCast(transform.position, bodyTop, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    movedir = moveDirZ;
                }
            }

        }

        if (canMove)
        {
            transform.position += Time.deltaTime * moveSpeed * movedir;
        }


        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, movedir, Time.deltaTime * rotateSpeed);

        if (movedir != Vector3.zero)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isGrounded = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("SpawnedObjects"))
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("SpawnedObjects"))
        {
            if (collision.gameObject.name == "SpawnManagerCapsule(Clone)")
            {
                ModifyHealth(1);
            }
            else if (collision.gameObject.name == "SpawnManagerCube(Clone)")
            {
                ModifyHealth(-1);
            }

            UpdateHealthUI();
            Destroy(collision.gameObject);
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void ModifyHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, 3); // Ensure health stays between 0 and 3
        if (health <= 0 && !isGameOver)
        {
            UpdateHealthUI();
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        gameOverText.gameObject.SetActive(true); // Show the game over text
        gameOverText.text = "Game Over!";
        restartButton.SetActive(true);
        // Here you can also stop any ongoing Coroutines, timers, etc., if necessary.
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthPoints.Length; i++)
        {
            healthPoints[i].enabled = i < health;
        }
    }

    private void UpdateUI()
    {
        // Update position text with the X, Z values
        positionText.text = $"Position: X = {transform.position.x:F2}, Z = {transform.position.z:F2}";

        // Get the Y rotation value between 0 and 360 degrees
        float yRotation = transform.eulerAngles.y;
        yRotation = (yRotation + 360) % 360;
        rotationText.text = $"Rotation: Y = {yRotation:F2}°";
    }

    public void RestartGame()  // Call this method when the restart button is clicked
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Reloads the current scene
    }

}
