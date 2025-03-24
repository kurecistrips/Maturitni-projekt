using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 2f;

    private float baseSpeed;

    private Transform target;
    private int pathIndex = 0;
    public int pathIndx;
    public bool fromLastWave = false;

    public static EnemyMovement main;

    private void Start()
    {
        baseSpeed = moveSpeed;
        target = LevelManager.main.path[pathIndex];
    }

    private void Update()
    {
        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;
            pathIndx = pathIndex;

            if (pathIndex == LevelManager.main.path.Length && fromLastWave != true)
            {
                WaveManagerTest.onEnemyDestroy.Invoke();
                Destroy(gameObject);
                LevelManager.main.baseHealth -= EnemyHealth.main.healthInfo;
                return;
            }
            else if (pathIndex == LevelManager.main.path.Length && fromLastWave != false)
            {
                Destroy(gameObject);
                LevelManager.main.baseHealth -= EnemyHealth.main.healthInfo;
                return;
            }
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

    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }

    public void Freeze(float stop)
    {
        moveSpeed = stop;
    }
    public void ResetSpeed()
    {
        moveSpeed = baseSpeed;
    }

}

