using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
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
    [SerializeField] private TargetingStyle currentTargetingStyle = TargetingStyle.First;
    private TargetingStyle _previousTargetingStyle;

    [SerializeField] private Tower tower;


    private void Awake()
    {
        
    }

    private void Start()
    {
       
        _previousTargetingStyle = currentTargetingStyle;
    }

    private void Update()
    {
        if (_previousTargetingStyle != currentTargetingStyle)
        {
            HandleTargetStyleSwitch();
        }

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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            AddTargetToInRangeList(other.GetComponent<EnemyHealth>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            RemoveTargetFromInRangeList(other.GetComponent<EnemyHealth>());
        }
    }

    /*private void OnDrawGizmos() //debug testing
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, transform.forward, Tower.main.attackRange);
    }*/

    public void AddTargetToInRangeList(EnemyHealth target)
    {
        targetInRange.Add(target);
        GetCurrentTarget();
    }
    public void RemoveTargetFromInRangeList(EnemyHealth target)
    {
        targetInRange.Remove(target);
        GetCurrentTarget();
    }

    private void GetCurrentTarget()
    {
        if (targetInRange.Count <= 0)
        {
            currentTarget = null;
            return;
        }

        currentTarget = currentTargetingStyle switch
        {
            TargetingStyle.First => targetInRange.First(),
            TargetingStyle.Last => targetInRange.Last()
            
        };

    }
    
    private void HandleTargetStyleSwitch()
    {
        _previousTargetingStyle = currentTargetingStyle;
        GetCurrentTarget();
    }



}

namespace Enums
{
    public enum TargetingStyle
    {
        First,
        Last,
        Strong,
        Weak
    }    
}
