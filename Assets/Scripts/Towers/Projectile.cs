using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed; //rychlost projektilu
    private float explosionRadius; //rádius exploze
    public LayerMask enemyLayer;
    [SerializeField] private Rigidbody2D rb;
    //[SerializeField] private CircleCollider2D cirCollider2D;
    [SerializeField] private GameObject explosion;
    
    private int damage;
    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;	
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
    public void SetSpeed(float spd)
    {
        speed = spd;
    }
    public void SetExplosionRadius(float radius)
    {
        explosionRadius = radius;
        //cirCollider2D.radius = explosionRadius;
    }

    private void Update()
    {
        if (target == null) //pokud projektilu zmizí cíl, tak se zníčí
        {
            Destroy(gameObject);
            return;
        }
    }

    private void FixedUpdate()
    {
        if (!target) return;

        Vector2 direction = (target.position - transform.position).normalized;

        rb.velocity = direction * speed;

        float pRot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, pRot + 90);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform == target)
        {
            Instantiate(explosion.gameObject, transform.position, Quaternion.identity);
            Explode();
        }
    }
    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach(Collider2D collider in colliders)
        {
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }


}
