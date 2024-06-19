using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public float damage = 2;
    public Collider2D swordCollider;
    Vector2 rightAttackOffset;
    private void Start(){
        //gets position of sword box collider relative to the player
        rightAttackOffset = transform.localPosition;
    }
    /*
    Function: TryMove
    Description: Attacks to the right
    */
    public void AttackRight(){
        //enables sword collider
        swordCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
    }
    public void AttackLeft(){
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
    }
    public void StopAttack(){
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Slime"){
            Enemy slime = other.GetComponent<Enemy>();
            if(slime != null){
                slime.Health -= damage;
            }
        }
    }
}
