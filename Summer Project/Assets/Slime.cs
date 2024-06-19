using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cainos.PixelArtTopDown_Basic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float Health{
        set{
            health = value;
            if(health <= 0){
                Destroy(gameObject);
            }
        }
        get{
            return health;
        }
    }
    const float DEFAULT_RADIUS = 1f;
    public float searchRadius = DEFAULT_RADIUS;
    public ContactFilter2D contactFilter;
    //Private Fields
    float health = 5;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    Rigidbody2D rb;
    float moveSpeed = 1.2f;
    float collisionOffset = 0.02f;
    int directionCoolDown;
    Vector2 wanderDirection;
    
    private void Start(){
        wanderDirection = new Vector2(Random.Range(1f, 2f), Random.Range(1f, 2f));
        rb = GetComponent<Rigidbody2D>();
        directionCoolDown = Random.Range(1, 500);
    }
    private void FixedUpdate(){
        LookForPlayer();
    }
    /*
    Function: LookForPlayer
    Return Type: N/A
    Parameters: N/A
    Description: Will search for players within a certain radius
    if found the enemy will move towards the player.
    */
    private void LookForPlayer(){
        //finds all players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players){
            //calculates the distance between enemy and player
            float distance = Vector3.Distance(transform.position, player.transform.position);
            //if player is within the search radius
            if(searchRadius >= distance){
                //search radius will increase if player is found
                searchRadius = 2f;
                //recalculate distance to in case player moved
                distance = Vector3.Distance(transform.position, player.transform.position);
                //Get the direction to move
                Vector2 direction = player.transform.position - transform.position;
                //Try to move in that direction
                bool success = TryMove(direction);
                //Try moving on the x or y axis if the enemy can't move directly to the player
                if(!success){
                    success = TryMove(new Vector2(player.transform.position.x, 0));
                    moveSpeed = 4f;
                }
                else if(!success){
                    success = TryMove(new Vector2(0, player.transform.position.y));
                    moveSpeed = 4f;
                }
                //increase move speed
                moveSpeed = 1.2f;
            }else{
                //if the player is not in the search radius, reset search radius to default radius and enable wandering
                Wander();
                searchRadius = DEFAULT_RADIUS;
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
        //count will be 0 if no collisions are detected
        if(direction == Vector2.zero){
            return false;
        }
        int count = rb.Cast(direction, contactFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);
        if(count == 0){
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;
        } else {
            return false;
        }
    }
    
    private void Wander(){
        directionCoolDown --;
        int count = rb.Cast(wanderDirection, contactFilter, castCollisions, 0.5f * Time.fixedDeltaTime + collisionOffset);
        if(count == 0 && directionCoolDown > 350){
            rb.MovePosition(rb.position - wanderDirection * 0.5f * Time.fixedDeltaTime);
        }
        else if(directionCoolDown <= 0){
            print("Change");
            ChangeDirection();
        }
    }

    private void ChangeDirection(){
        float randomAngle = Random.Range(0f, 360f);

        wanderDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        directionCoolDown = Random.Range(1, 500);
    }

}

