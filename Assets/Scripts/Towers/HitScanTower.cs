using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HitScanTower : MonoBehaviour
{
    public LayerMask enemyLayer;
    public Transform firingPoint;
    public Transform rotatingPoint;
    private float nextAttackTime = 0f;

    [SerializeField] private List<EnemyHealth> targetInRange = new();
    [SerializeField] private EnemyHealth currentTarget;

    void Update()
    {
        if (Time.time > nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + (1f / Tower.main.attackSpeed);
        }
    } 
    public void Attack()
    {
        RaycastHit2D hit = Physics2D.Raycast(firingPoint.position, firingPoint.right, Tower.main.attackSpeed, enemyLayer);
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                if (enemy.isHidden && !Tower.main.hiddenDetection) return;
                enemy.TakeDamage(Tower.main.damage);
                
            }
        }
    }
    private void OggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

        }
    }
    private void OTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            
        }
    }

    private void OnDrawGizmos() //debug testing
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, transform.forward, Tower.main.attackRange);
    }

    public void AddTargetToInRangeList(EnemyHealth target)
    {
        targetInRange.Add(target);
    }
    public void RemoveTargetFromInRangeList(EnemyHealth target)
    {
        targetInRange.Remove(target);
    }

    private void GetCurrentTarget()
    {

    }



}
