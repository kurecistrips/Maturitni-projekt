using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public EnemyMovement enemyMovement;
    public float attackRange;
    public float stunDurationToTowers;
    private bool canAttack = true;
    [SerializeField] private float setAttackCooldown;
    private float attackCooldown;
    [SerializeField] private float movementDelay;
    private List<Tower> towersInRange = new List<Tower>();
    [SerializeField] private LayerMask towerLayer;
    
    private void Start()
    {
        attackCooldown = setAttackCooldown;
    }

    private void Update()
    {
        DetectTowers();

        if (towersInRange.Count >= 5 && canAttack == true)
        {
            canAttack = false;
            StartCoroutine(StunAttack());
        }
        

        if (canAttack == false && attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        else if (canAttack == false && attackCooldown <= 0)
        {
            attackCooldown = setAttackCooldown;
            canAttack = true;
        }
    }

    private IEnumerator StunAttack()
    {
        enemyMovement.Freeze(0);
        

        foreach (Tower tower in towersInRange)
        {
            tower.Stun(stunDurationToTowers);
        }

        yield return new WaitForSeconds(movementDelay);

        enemyMovement.ResetSpeed();

    }

    private void DetectTowers()
    {
        towersInRange.Clear();

        Tower[] allTowers = FindObjectsOfType<Tower>();
        
        foreach (Tower tower in allTowers)
        {
            float distance = Vector2.Distance(transform.position, tower.transform.position);
            if (distance <= attackRange)
            {
                towersInRange.Add(tower);
            }
        }
    }


}
