using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Public Members
    public float moveSpeed = 2f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    
    //Private Members
    Vector2 movementInput;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    bool canMove = true;
    
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        //Checks for movement input. Continues if it's not zero
        MovePlayer();
    }
    /*
    Function: MovePlayer
    Description: Takes player input and moves the player
    */
    private void MovePlayer(){
        //checks if player movement is locked
        if(canMove){
            if(movementInput != Vector2.zero){
                //checks for collisions
                bool success = TryMove(movementInput);
                //if there were collisions, checks for collisions on x or even y axis
                if(!success){
                    success = TryMove(new Vector2(movementInput.x, 0));
                }
                if(!success){
                    success = TryMove(new Vector2(0, movementInput.y));
                }
                animator.SetBool("isMoving", success);
            } else{
                animator.SetBool("isMoving", false);
            }  
            
            //Sets player direction
            if(movementInput.x > 0){
                spriteRenderer.flipX = false;
            } else if(movementInput.x < 0){
                spriteRenderer.flipX = true;
            }
        }
    }
    /*
    Function: TryMove
    Return Type: bool
    Parameters: Vector2 direction
    Description: will look for collisions and determine if object can be moved
    returns true if it moves, false otherwise
    */
    private bool TryMove(Vector2 direction){
        //checks if there is any player input at all
        if(direction == Vector2.zero){
            return false;
        }
        //count will be 0 if no collisions are detected
        int count = rb.Cast(direction, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);
        if(count == 0){
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;
        } else {
            return false;
        }
    }

    void OnMove(InputValue movementValue){
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("swordAttack");
    }
    /*
    Function: SwordAttack
    Description: Will activate player's sword attack
    */
    public void SwordAttack(){
        //Locks player movement
        canMove = false;
        //Checks which direction to attack
        if(spriteRenderer.flipX == true){
            swordAttack.AttackLeft();
        }else{
            swordAttack.AttackRight();
        }
    }
    /*
    Function: TryMove
    Description: Will end player attack
    */
    public void EndAttack(){
        //Unlocks player movement
        canMove = true;
        //stops player sword attack
        swordAttack.StopAttack();
    }

}