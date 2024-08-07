using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour {
    Animator animator;
    public float Health {
        set {
            health = value;
            if (health <= 0) 
                Defeated();
            else
                TakeDamage();
            
        }
        get {
            return health;
        }
    }
    const float DEFAULT_RADIUS = 1f;
    public float searchRadius = DEFAULT_RADIUS;
    public ContactFilter2D contactFilter;
    public LayerMask collisionLayer;
    // Private Fields
    float health = 5;
    Rigidbody2D rb;
    float moveDistance = 0.16f;
    float moveSpeed = 3f;
    float collisionOffset = 0.02f;
    int directionCoolDown;
    Vector2 wanderDirection;
    PathFinding findPath;
    GameObject player;
    List<PathNode> path;
    float distance;
    private Vector3 lastPlayerPosition;
    public Tilemap tilemap;
    int i = 0;
    int g = 0;
    private Vector2 targetPosition;
    bool canMove = true;
    public float knockbackForce = 0.05f;
    public float knockbackDuration = 0.2f;
    Vector2 cellOffset = new Vector2(0.16f / 2, 0.16f / 2);


    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        lastPlayerPosition = player.transform.position;
        animator = GetComponent<Animator>();
    }

    private void Update() {
        LookForPlayer();
    }
    public void TakeDamage(){
        animator.SetTrigger("TakeDamage");
        canMove = false;
        Vector2 playerPosition = player.transform.position;
        Vector2 direction = (rb.position - playerPosition).normalized;
        direction.y = 0;
        direction.Normalize();
        Debug.Log(knockbackForce);
        StartCoroutine(ApplyKnockback(direction));
        
    }
    private IEnumerator ApplyKnockback(Vector2 direction) {
        float timer = 0;
        while (timer < knockbackDuration) {
            rb.velocity = direction * knockbackForce;
            timer += Time.deltaTime;
            yield return null;
        }
        rb.velocity = Vector2.zero;
        allowMovement();
    }
    public void allowMovement() {
        canMove = true;
    }
     
    public void Defeated() => animator.SetTrigger("Defeated");
    public void RemoveEnemy() => Destroy(gameObject);
    private void LookForPlayer() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        g++;
        if (canMove) {
            searchRadius = 2f;
            if (path == null || path.Count <= 0 || Math.Abs(lastPlayerPosition.x - player.transform.position.x) > 0.5f || Math.Abs(lastPlayerPosition.y - player.transform.position.y) > 0.5f) {
                Debug.Log("Get Path");
                findPath = new PathFinding(100, 100, tilemap);
                int startX = (int)(transform.position.x / 0.16f);
                int startY = (int)(transform.position.y / 0.16f);
                int endX = (int)(player.transform.position.x / 0.16f);
                int endY = (int)(player.transform.position.y / 0.16f);
                path = findPath.FindPath(startX, startY, endX, endY);
                lastPlayerPosition = player.transform.position;
                if (path != null && path.Count > 0) {
                    targetPosition = new Vector2(path[0].x * 0.16f, path[0].y * 0.16f) + cellOffset;
                }
            } else {
                MoveTowardsTarget();
            }
        }
    }

    private void MoveTowardsTarget() {
        if (path == null || path.Count == 0) {
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        Vector2 newPosition = currentPosition + direction * moveSpeed * Time.deltaTime;
        Vector3 cellOffsetdraw = new Vector3(0.16f / 2, 0.16f / 2, 0);

    // Draw the path with adjusted positions
    for (int i = 0; i < path.Count - 1; i++) {
        Vector3 start = findPath.grid.GetWorldPosition(path[i].x, path[i].y) + cellOffsetdraw;
        Vector3 end = findPath.grid.GetWorldPosition(path[i + 1].x, path[i + 1].y) + cellOffsetdraw;
        Debug.DrawLine(start, end, Color.red, 500f);
    }
        if (Vector2.Distance(currentPosition, targetPosition) <= moveSpeed * Time.deltaTime) {
            rb.MovePosition(targetPosition);
            path.RemoveAt(0);
            if (path.Count > 0) {
                targetPosition = new Vector2(path[0].x * 0.16f, path[0].y * 0.16f) + cellOffset;
            }
        } else {
            TryMove(newPosition);
        }
    }

    private bool TryMove(Vector2 newPosition) {
        // Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        // RaycastHit2D hit = Physics2D.Raycast(rb.position, newPosition, newPosition.magnitude * moveSpeed * Time.fixedDeltaTime, collisionLayer);

            rb.MovePosition(newPosition);
            return true;
        return false;
    }
}