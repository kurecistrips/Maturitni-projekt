using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEditor;
using UnityEngine;

public class ProjectileTower : MonoBehaviour
{
    public Transform firingPoint;
    public Transform rotatingPoint;
    public GameObject projectilePrefab;
    public LayerMask enemyLayer;

    [SerializeField] private List<EnemyHealth> targetsInRange = new();
    [SerializeField] private EnemyHealth currentTarget;
    [SerializeField] private TargetingStyle currentTargetingStyle = TargetingStyle.First;
    private TargetingStyle _previousTargetingStyle;
    [SerializeField] private Tower tower;

    private float smoothTime = 0.1f;
    public float projectileSpeed;
    private float nextAttackTime = 0f;
    private float detectionRadius;

    private void Awake()
    {
        _previousTargetingStyle = currentTargetingStyle;   
    }

    private void Update()
    {
        detectionRadius = tower.attackRange;
        DetectEnemies();
        GetCurrentTarget();

        if (currentTarget == null) return;
        RotateTowardsTarget();
        if (_previousTargetingStyle != currentTargetingStyle)
        {
            HandleTargetStyleSwitch();
        }

        if (nextAttackTime <= 0f)
        {
            Attack();
            nextAttackTime = 1f / tower.attackSpeed;
        }
        nextAttackTime -= Time.deltaTime;
    }

    private void DetectEnemies()
    {
        targetsInRange.Clear();

        Collider2D[] detectedEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        foreach (var collider in detectedEnemies)
        {
            EnemyHealth enemy = collider.GetComponent<EnemyHealth>();
            if (enemy != null && (!enemy.isHidden || (enemy.isHidden && tower.hiddenDetection)))
            {
                targetsInRange.Add(enemy);
            }
        }
    }

    private void Attack()
    {

        GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.SetTarget(currentTarget.transform);
            projectileScript.SetDamage(tower.damage);
            projectileScript.SetExplosionRadius(tower.explosionRadius);
            projectileScript.SetSpeed(tower.projectileSpeed);
        }
    }

    private void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2(currentTarget.transform.position.y - transform.position.y,
        currentTarget.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        rotatingPoint.rotation = Quaternion.Slerp(rotatingPoint.rotation, targetRotation, smoothTime);
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
}


