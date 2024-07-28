using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public LayerMask collisionLayer;
    // Private Fields
    float health = 5;
    Rigidbody2D rb;
    float moveDistance = 0.16f;
    float moveSpeed = 2f;
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
    Vector2 cellOffset = new Vector2(0.16f / 2, 0.16f / 2);


    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        lastPlayerPosition = player.transform.position;
    }

    private void Update() {
        LookForPlayer();
    }

    private void LookForPlayer() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        if (true) {
            searchRadius = 2f;
            if (path == null || path.Count <= 0) {
                Debug.Log("Get Path");
                findPath = new PathFinding(60, 60, tilemap);
                int startX = (int)(transform.position.x / 0.16f);
                int startY = (int)(transform.position.y / 0.16f);
                int endX = (int)(player.transform.position.x / 0.16f);
                int endY = (int)(player.transform.position.y / 0.16f);
                path = findPath.FindPath(startX, startY, endX, endY);
                if (path != null && path.Count > 0) {
                    targetPosition = new Vector2(path[0].x * 0.16f, path[0].y * 0.16f) + cellOffset;
                }
            } else {
                MoveTowardsTarget();
            }
        } else {
            searchRadius = DEFAULT_RADIUS;
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
        RaycastHit2D hit = Physics2D.Raycast(rb.position, newPosition, newPosition.magnitude * moveSpeed * Time.fixedDeltaTime, collisionLayer);
        if (hit.collider == null) {
            rb.MovePosition(newPosition);
            return true;
        }
        return false;
    }
}