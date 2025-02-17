using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 2f;

    private Transform target;
    private int pathIndex = 0;
    public bool fromLastWave = false;

    private void Start()
    {
        target = LevelManager.main.path[pathIndex];
    }

    private void Update()
    {
        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;

            if (pathIndex == LevelManager.main.path.Length && fromLastWave != true)
            {
                WaveManagerTest.onEnemyDestroy.Invoke();
                Destroy(gameObject);
                LevelManager.main.baseHealth -= EnemyHealth.main.healthInfo;
                return;
            }
            /*else if (pathIndex == LevelManager.main.path.Length && fromLastWave != false)
            {
                WaveManager.onEnemyDestroy.Invoke();
                Destroy(gameObject);
                return;
            }*/
            else 
            {
                target = LevelManager.main.path[pathIndex];
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

}

