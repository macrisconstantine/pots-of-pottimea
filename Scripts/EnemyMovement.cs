using UnityEngine;

/// <summary>
/// This script is for the purpose of controlling the enemy's animator based on its transform movement
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    // Allows for limiting the functional code if the character does not have extra movement animations
    [SerializeField] bool is2DMovement;

    // Declaring animator component to streamline code
    private Animator animator;

    // Naming of directions to manipulate animator parameters
    private int north = 0;
    private int south = 1;
    private int west = 2;

    // These variables are used to determine the enemy's movement
    float lastPosX = 0;
    float lastPosY = 0;

    // Direction defaulted to south and invert vector defined
    private string direction = "S";
    private Vector3 invertSpriteVector = new Vector3(-1f, 1f, 1f);

    // Initiallizes x and y last position variables and sets animator variable
    private void Start()
    {
        lastPosX = transform.position.x;
        lastPosY = transform.position.y;
        animator = GetComponent<Animator>();
    }

    // Controls the animator based on the object's movement
    void FixedUpdate()
    {
        // If change in y axis is greater than change in x axis, set speed as 1
        if (System.Math.Abs(lastPosX - transform.position.x) < System.Math.Abs(transform.position.y - lastPosY))
        {
            animator.SetFloat("speed", 1);

            // If positive change, set direction north
            if (transform.position.y > lastPosY)
            {
                if (!is2DMovement)
                    animator.SetInteger("direction", north);
                direction = "N";
            // If positive change, set direction north
            }
            // Else, it is a negative change, set direction south
            else
            {
                if (!is2DMovement)
                    animator.SetInteger("direction", south);
                direction = "S";
                transform.localScale = Vector3.one;
            }
        }

        // If change in x axis is greater than change in y axis, set speed as 1 and direction as west
        else if (System.Math.Abs(lastPosX - transform.position.x) > System.Math.Abs(transform.position.y - lastPosY))
        {
            animator.SetFloat("speed", 1);
            if (!is2DMovement)
                animator.SetInteger("direction", west);
            direction = "W";
        }

        // If no change in either axis, set speed to 0 and display the appropriate idle animation
        else if (lastPosX == transform.position.x && lastPosY == transform.position.y)
        {
            animator.SetFloat("speed", 0);
            if (!is2DMovement)
            {
                if (direction == "N") animator.SetInteger("direction", north);
                else if (direction == "S") animator.SetInteger("direction", south);
                else if (direction == "W") animator.SetInteger("direction", west);
                else animator.SetInteger("direction", south);
            }
        }

        // Invert scale according to positive or negative change in x axis
        if (lastPosX > transform.position.x)
            transform.localScale = Vector3.one;
        else if (lastPosX < transform.position.x)
            transform.localScale = invertSpriteVector;
        
        // Update last position of x and y
        lastPosX = transform.position.x;
        lastPosY = transform.position.y;
    }
}

