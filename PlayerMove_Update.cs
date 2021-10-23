using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove_Update : MonoBehaviour
{
    // variables
    Vector2 bounds = new Vector2(3f,3f);
    Vector2 position = new Vector2(0f,0f);

    public float moveSpeed = 0.04f;
    public float jumpSpeed = 0.12f;
    public float gravity = 0.003f;

    float y_spd = 0;

    int jumpBuffer = 0;
    int coyoteBuffer = 0;
    
    /*
    Initializes bounds (our rectangular hitbox bounds) and position (a vector2-converted version of the object's position)
    */
    void Start(){
        bounds = GetComponent<BoxCollider2D>().bounds.size;
        position = transform.position;
    }

    /*
    Updates movement variables based on key input and gravity, moves the object
    -- Warning --
    * This uses the standard Update() function for everything and is thus tied to the framerate with which the game is running.
      If we want to have it perform the same regardless of framerate, best practice is to put our movement, collisions and 
      gravity updates into FixedUpdate() and keep the input in this function.
    */
    void Update(){
        // temp movement variables
        float x_spd = 0;
        position = transform.position;

        // gravity
        if(!collision(position + (0.01f * Vector2.down))){
            // player is not grounded
            if(y_spd > -0.4f){
                // terminal velocity
                y_spd -= gravity;
            }
            // decrease the coyote buffer
            coyoteBuffer -= 1;
        }else{
            // player is grounded
            y_spd = 0;
            // refill the coyote buffer
            coyoteBuffer = 6;
        }

        // check control inputs
        /*
        -- Note --
        * We add/subtract rather than directly setting so that if both keys are held down, nothing happens
        */
        if(Input.GetKey(KeyCode.A)){
            // left
            x_spd -= moveSpeed;
        }
        if(Input.GetKey(KeyCode.D)){
            // right
            x_spd += moveSpeed;
        }
        /*
        -- Note --
        * This gives a 5-frame window for the jump to be valid after the key is pressed to account for humans not being frame-perfect
        * The coyote buffer gives a 3-frame window for the jump to be valid after the player begins falling
        */
        if(Input.GetKeyDown(KeyCode.Space)){
            // fill the jump buffer
            jumpBuffer = 5;
        }
        if(jumpBuffer > 0){
            // jump if the buffer is applicable (checks for ground collision or if the coyote buffer is in effect)
            if(collision(position + (0.01f * Vector2.down)) || coyoteBuffer > 0){
                y_spd = jumpSpeed;
                coyoteBuffer = 0;
            }
            jumpBuffer -= 1;
        }

        // check collisions
        /*
        Lerps the player out of colliding with a wall by decreasing the temp variables and re-checking the collision.
        -- Note --
        * It currently uses repeated halving to do so, but linear lerping can give more accuracy.
        */
        while(Mathf.Abs(x_spd) > 0 && collision(position + (x_spd * Vector2.right))){
            x_spd /= 2f;
            if(Mathf.Abs(x_spd) < 0.01f){
                x_spd = 0;
            }
        }
        while(Mathf.Abs(y_spd) > 0 && collision(position + (y_spd * Vector2.up))){
            y_spd /= 2f;
            if(Mathf.Abs(y_spd) < 0.01f){
                y_spd = 0;
            }
        }

        // move player
        /*
        Moves the player in the x and y directions according to x_spd and y_spd, as Vector3.right and Vector3.up are unit vectors
        -- Note --
        * We can also do this by modifying the position components individually (ex. transform.position[0] += x_spd; ) with the same 
          result.
        */
        transform.position += (x_spd * Vector3.right);
        transform.position += (y_spd * Vector3.up);
    }

    /*
    Simulates placing the object's hitbox (as given by bounds) at a given position and checks for collisions with other objects.
    -- Warnings --
    * This method relies on the hitbox always colliding with the player's collision mask, and thus expects at least one collision 
      when called. This stops working when the check does not collide with the player's mask, but fixing this issue requires 
      iterating over all the boxcast's results every time the function is called, which can sap performance.
    */
    bool collision(Vector2 pos){
        // checks at a specific position
        return (Physics2D.BoxCastAll(pos, bounds, 0f, Vector2.right, 0f).Length > 1);
    }
}
