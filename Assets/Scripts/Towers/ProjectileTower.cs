using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

public class ProjectileTower : MonoBehaviour
{
    public Transform firingPoint; //bod, ze kterého věž střílí
    public Transform rotatingPoint; //bod, ze kterého věž střílí
    public GameObject projectilePrefab; //prefab projektilu
    public LayerMask enemyLayer; //vrstva pro detekci nepřátel

    [SerializeField] private List<EnemyHealth> targetsInRange = new(); //seznam nepřátelských jednotek v dosahu
    [SerializeField] private EnemyHealth currentTarget;  //aktuálně zaměřená nepřátelská jednotka
    [SerializeField] private TargetingStyle currentTargetingStyle = TargetingStyle.First; //aktuální styl zaměřování
    private TargetingStyle _previousTargetingStyle; //předchozí styl zaměřování
    [SerializeField] private Tower tower; //odkaz na věž

    private float smoothTime = 0.1f; //rychlost otáčení
    public float projectileSpeed; //rychlost projektilu
    private float nextAttackTime = 0f; //časovač pro další útok
    private float detectionRadius; //rádius pro detekci nepřátelských jednotek
 
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
        DetectEnemies(); //detelce nepřátel v dosahu
        GetCurrentTarget(); //výběr aktuálního cíle

        if (currentTarget == null) return;
        if (!tower.isStunned) //pokud je věž omráčená, tak nemůže střílet
        {
            RotateTowardsTarget(); //otáčení věže k cíly
            if (_previousTargetingStyle != currentTargetingStyle)
            {
                HandleTargetStyleSwitch(); //zpracování změny stylu zaměřování
            }

            if (nextAttackTime <= 0f)
            {
                Attack();
                nextAttackTime = 1f / tower.attackSpeed; //resetování časovače
            }
            nextAttackTime -= Time.deltaTime;
        }
        
    }

    private void DetectEnemies() //funkce na detekci nepřátelských jednotek
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

    private void Attack() //funkce na útok nepřátelských jednotek
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

    private void RotateTowardsTarget() //funkce na otáčení se k cíly
    {
        float angle = Mathf.Atan2(currentTarget.transform.position.y - transform.position.y,
        currentTarget.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        rotatingPoint.rotation = Quaternion.Slerp(rotatingPoint.rotation, targetRotation, smoothTime);
    }

    private void GetCurrentTarget() //funkce na styl zaměřování
    {
        if (targetsInRange.Count <= 0)
        {
            currentTarget = null;
            return;
        }

        targetsInRange = targetsInRange.Where(e => e != null && e.GetComponent<EnemyMovement>() != null)
        .OrderByDescending(e => e.GetComponent<EnemyMovement>().pathIndx)
        .ToList();  //seřazení nepřátelských jednotek podle jejich pozice na cestě pomocí lambdy

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
    
    private void HandleTargetStyleSwitch() //funkce na převádění nové zaměřovacího stylu
    {
        _previousTargetingStyle = currentTargetingStyle;
        GetCurrentTarget(); //aktualizace cíle podle nového stylu
    }
    
    public void SwitchTargetingStyle() //funkce na přepnutí dalšího stylu zaměřování
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


