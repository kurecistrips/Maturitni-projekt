using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

public class HitScanTower : MonoBehaviour
{
    public LayerMask enemyLayer;
    public Transform firingPoint;
    public Transform rotatingPoint;
    private float nextAttackTime = 0f;

    [SerializeField] private List<EnemyHealth> targetsInRange = new();
    [SerializeField] private EnemyHealth currentTarget;
    [SerializeField] private TargetingStyle currentTargetingStyle = TargetingStyle.First;
    private TargetingStyle _previousTargetingStyle;
    private float smoothTime = 0.1f;

    [SerializeField] private Tower tower;
    [SerializeField] private ParticleSystem muzzleFlash;
    private float detectionRadius;
    private float attackSpeed;

    private void Awake()
    {
        _previousTargetingStyle = currentTargetingStyle;
        
    }
    private void Start()
    {
        tower.targetingText.text = $"Target: {currentTargetingStyle}";
    }

    private void Update()
    {
        detectionRadius = tower.attackRange;
        attackSpeed = tower.attackSpeed;
        DetectEnemies();
        GetCurrentTarget();

        if (currentTarget == null) return;
        if (!tower.isStunned)
        {
            RotateTowardsTarget();   
            if (_previousTargetingStyle != currentTargetingStyle)
            {
                HandleTargetStyleSwitch();
            }

            if (nextAttackTime <= 0f)
            {
                Attack();
                nextAttackTime = 1f / attackSpeed;
            }
            
        }
        nextAttackTime -= Time.deltaTime;  
    } 
    
    public void Attack()
    {
        

        Vector2 direction = (currentTarget.transform.position - firingPoint.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(firingPoint.position, direction, detectionRadius, enemyLayer);
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            muzzleFlash.Play();
            enemy.TakeDamage(tower.damage); 
            
        }
    }
    private void RotateTowardsTarget()
    {

        float angle = Mathf.Atan2(currentTarget.transform.position.y - transform.position.y,
        currentTarget.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;
    
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        rotatingPoint.rotation = Quaternion.Slerp(rotatingPoint.rotation, targetRotation, smoothTime);
    }

    /*private void OnDrawGizmos() //debug testing
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, transform.forward, Tower.main.attackRange);
    }*/

    private void DetectEnemies()
    {
        targetsInRange.Clear();

        Collider2D[] detectedEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        foreach (var collider in detectedEnemies)
        {
            EnemyHealth enemy = collider.GetComponent<EnemyHealth>();
            if (enemy != null && (!enemy.isHidden || (enemy.isHidden && tower.hiddenDetection)) && tower.isPlaced == true)
            {
                targetsInRange.Add(enemy);
            }
        }
    }


    private void GetCurrentTarget()
    {
        if (targetsInRange.Count <= 0)
        {
            currentTarget = null;
            return;
        }

        targetsInRange = targetsInRange.Where(e => e != null && e.GetComponent<EnemyMovement>() != null)
        .OrderByDescending(e => e.GetComponent<EnemyMovement>().pathIndx)
        .ToList();

        switch (currentTargetingStyle)
        {
            case TargetingStyle.First:
                currentTarget = targetsInRange.First();
                break;
            case TargetingStyle.Last:
                currentTarget = targetsInRange.Last();
                break;
            case TargetingStyle.Strong:
                currentTarget = targetsInRange.OrderBy(e => e.GetComponent<EnemyHealth>().healthInfo).First();
                break;
            case TargetingStyle.Weak:
                currentTarget = targetsInRange.OrderBy(e => e.GetComponent<EnemyHealth>().healthInfo).Last();
                break;
            default:
                break;
        };
    }
    
    private void HandleTargetStyleSwitch()
    {
        _previousTargetingStyle = currentTargetingStyle;
        GetCurrentTarget();
    }

    public void SwitchTargetingStyle()
    {
        currentTargetingStyle++;

        if ((int)currentTargetingStyle >= System.Enum.GetValues(typeof(TargetingStyle)).Length)
        {
            currentTargetingStyle = 0;
        }

        tower.targetingText.text = $"Target: {currentTargetingStyle}";

        HandleTargetStyleSwitch();
    }
}


