using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // variables
    Vector2 bounds = new Vector2(3f,3f);

    public float moveSpeed = 0.04f;
    public float jumpSpeed = 0.12f;
    public float gravity = 0.003f;

    float y_spd = 0;

    int jumpBuffer = 0;
    int coyoteBuffer = 0;
    
    // Start is called before the first frame update
    void Start(){
        bounds = GetComponent<BoxCollider2D>().bounds.size;
    }

    // Update is called once per frame
    void Update(){
        // temp movement variables
        float x_spd = 0;
        // round off position to make it easier to work with
        Vector2 position = new Vector2(Mathf.Round(transform.position.x * 1000) / 1000, Mathf.Round(transform.position.y * 1000) / 1000);

        // gravity
        if(!collision(position + (0.1f * Vector2.down))){
            if(y_spd > -0.4f){
                // terminal velocity
                y_spd -= gravity;
            }
            coyoteBuffer -= 1;
        }else{
            coyoteBuffer = 12;
        }

        // check control inputs
        if(Input.GetKey(KeyCode.A)){
            // left
            x_spd -= moveSpeed;
        }
        if(Input.GetKey(KeyCode.D)){
            // right
            x_spd += moveSpeed;
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            jumpBuffer = 5;
        }
        if(jumpBuffer > 0){
            // jump
            if(collision(position + (0.1f * Vector2.down)) || coyoteBuffer > 0){
                y_spd = jumpSpeed;
                coyoteBuffer = 0;
            }
            jumpBuffer -= 1;
        }

        // check collisions
        while(Mathf.Abs(x_spd) > 0 && collision(position + (x_spd * Vector2.right))){
            if(x_spd <= -0.01f){
                x_spd += 0.01f;
            }else if(x_spd >= 0.01f){
                x_spd -= 0.01f;
            }
        }
        while(Mathf.Abs(y_spd) > 0 && collision(position + (y_spd * Vector2.up))){
            if(y_spd <= -0.01f){
                y_spd += 0.01f;
            }else if(y_spd >= 0.01f){
                y_spd -= 0.01f;
            }else{
                y_spd = 0;
            }
        }

        // move player
        transform.position = new Vector3(position.x + x_spd, position.y + y_spd, 0f);
    }

    bool collision(Vector2 pos){
        // checks at a specific position
        return (Physics2D.BoxCastAll(pos, bounds, 0f, Vector2.right, 0f).Length > 1);
    }
}
