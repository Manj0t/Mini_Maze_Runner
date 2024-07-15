using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float Health {
        set {
            health = value;
            if (health <= 0) {
                Destroy(gameObject);
            }
        }
        get {
            return health;
        }
    }
    const float DEFAULT_RADIUS = 1f;
    public float searchRadius = DEFAULT_RADIUS;
    public ContactFilter2D contactFilter;
    // Private Fields
    float health = 5;
    Rigidbody2D rb;
    float moveSpeed = 1.2f;
    float collisionOffset = 0.02f;
    int directionCoolDown;
    Vector2 wanderDirection;
    PathFinding path;
    GameObject player;
    private Vector3 lastPlayerPosition;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        directionCoolDown = Random.Range(1, 500);
        wanderDirection = new Vector2(Random.Range(1f, 2f), Random.Range(1f, 2f));
        player = GameObject.FindGameObjectWithTag("Player");
        lastPlayerPosition = player.transform.position;
        UpdatePathToPlayer();
    }

    private void FixedUpdate() {
        LookForPlayer();
    }

    private void LookForPlayer() {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (searchRadius >= distance) {
            searchRadius = 2f;

            if (lastPlayerPosition != player.transform.position) {
                lastPlayerPosition = player.transform.position;
                UpdatePathToPlayer();
            }

            if (currentNode != null) {
                Vector2 direction = (currentNode.position - transform.position).normalized;
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

                if (Vector2.Distance(transform.position, currentNode.position) < 0.1f && currentNode.parent != null) {
                    currentNode = currentNode.parent;
                }
            }
        } else {
            searchRadius = DEFAULT_RADIUS;
        }
    }

    private void UpdatePathToPlayer() {

    }

    private void Wander() {
        if (directionCoolDown <= 0) {
            wanderDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            directionCoolDown = Random.Range(1, 5);
        }
        rb.MovePosition(rb.position + wanderDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private bool TryMove(Vector2 direction) {
        Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, direction.magnitude * moveSpeed * Time.fixedDeltaTime);
        if (hit.collider == null) {
            rb.MovePosition(newPosition);
            return true;
        }
        return false;
    }

    private void ChangeDirection() {
        float randomAngle = Random.Range(0f, 360f);
        wanderDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        directionCoolDown = Random.Range(1, 500);
    }
}
