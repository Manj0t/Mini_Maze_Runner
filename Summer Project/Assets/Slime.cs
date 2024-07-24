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
    float moveSpeed = 1.2f;
    float collisionOffset = 0.02f;
    int directionCoolDown;
    Vector2 wanderDirection;
    PathFinding findPath;
    GameObject player;
    List<PathNode> path;
    float distance;
    private Vector3 lastPlayerPosition;
    public Tilemap tilemap;


    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        // directionCoolDown = Random.Range(1, 500);
        // wanderDirection = new Vector2(Random.Range(1f, 2f), Random.Range(1f, 2f));
        player = GameObject.FindGameObjectWithTag("Player");
        lastPlayerPosition = player.transform.position;
        findPath = new PathFinding(20, 20, tilemap);
        UpdatePathToPlayer();
        findPath = new PathFinding(20, 20, tilemap);
            int startX = (int)(transform.position.x / 0.16f);
            int startY = (int)(transform.position.y / 0.16f);
            int endX = (int)(player.transform.position.x / 0.16f);
            int endY = (int)(player.transform.position.y / 0.16f);
        // path = new List<PathNode>();
            path = findPath.FindPath(startX, startY, endX, endY);
    }

    private void Update(){
        distance = Vector3.Distance(transform.position, player.transform.position);
        if (searchRadius >= distance) {
            // Your logic for when the player is within search radius
        }
    }

    private void FixedUpdate() {
        LookForPlayer();
    }

    private void LookForPlayer() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        if (searchRadius >= distance) {
        //     findPath = new PathFinding(20, 20, tilemap);
        //     int startX = (int)(transform.position.x / 0.16f);
        //     int startY = (int)(transform.position.y / 0.16f);
        //     int endX = (int)(player.transform.position.x / 0.16f);
        //     int endY = (int)(player.transform.position.y / 0.16f);
        // // path = new List<PathNode>();
        // path = findPath.FindPath(startX, startY, endX, endY);
            Debug.Log(path.Count);
            searchRadius = 2f;
            // Debug.Log(path.Count);
            if(path.Count > 0){
                PathNode currentNode = path[0];
                path.RemoveAt(0);
                if(currentNode == null){
                    currentNode = path[0];
                    path.RemoveAt(0);
                }
                Vector3 nodePosition = new Vector3(currentNode.x * 0.16f, currentNode.y * 0.16f);
                Vector2 direction = (nodePosition - transform.position).normalized;
                TryMove(direction);
            }else{
                findPath = new PathFinding(20, 20, tilemap);
                int startX = (int)(transform.position.x / 0.16f);
                int startY = (int)(transform.position.y / 0.16f);
                int endX = (int)(player.transform.position.x / 0.16f);
                int endY = (int)(player.transform.position.y / 0.16f);
            // path = new List<PathNode>();
                path = findPath.FindPath(startX, startY, endX, endY);
            }
            // rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        } else {
            searchRadius = DEFAULT_RADIUS;
        }
    }

    private void UpdatePathToPlayer() {
        // Your logic to update the path to the player
    }

    private bool TryMove(Vector2 direction) {
        Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, direction.magnitude * moveSpeed * Time.fixedDeltaTime, collisionLayer);
        if (hit.collider == null)
        {
            rb.MovePosition(newPosition);
            return true;
        }
        return false;
    }
}
