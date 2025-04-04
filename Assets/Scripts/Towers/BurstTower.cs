using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

public class BurstTower : MonoBehaviour
{
    public LayerMask enemyLayer; //vrstva pro detekci nepřátel
    public Transform firingPoint; //bod, ze kterého věž střílí
    public Transform rotatingPoint; //bod na otáčení věžě

    [SerializeField] private List<EnemyHealth> targetsInRange = new(); //seznam nepřátelských jednotek v dosahu
    [SerializeField] private EnemyHealth currentTarget; //aktuálně zaměřená nepřátelská jednotka
    [SerializeField] private TargetingStyle currentTargetingStyle = TargetingStyle.First; //aktuální styl zaměřování
    private TargetingStyle _previousTargetingStyle; //předchozí styl zaměřování
    private float smoothTime = 0.1f; //rychlost otáčení
    [SerializeField] private Tower tower; //odkaz na věž
    [SerializeField] private ParticleSystem muzzleFlash; //efekt výstřelu
    private float detectionRadius; //rádius pro detekci nepřátelských jednotek
    private int burstFireNum; //počet řel za jednu salvu
    private float attackSpeed; //rychlost útoku
    private bool isShooting; //určuje, zda věž sřílí
    private float cooldownAfterBurst = 1f; //pauza mezi dávky

    private void Awake()
    {
        _previousTargetingStyle = currentTargetingStyle; //uložení počátečního stylu zaměřování
    }
    private void Start()
    {
        tower.targetingText.text = $"Target: {currentTargetingStyle}"; //nastavení UI textu zobrazující aktuální styl zaměřování
    }

    private void Update()
    {
        detectionRadius = tower.attackRange;
        burstFireNum = tower.timesToShoot;
        attackSpeed = tower.attackSpeed;
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
            if (!isShooting)
            {
                StartCoroutine(Attack(burstFireNum));
                isShooting = true;
            }
        } 
    }

    private IEnumerator Attack(int TimesToShoot) //funkce na útok nepřátelských jednotek s pomocí vestavené Unity delay funkce
    {
        
        for (int timesShot = 1; timesShot <= TimesToShoot; timesShot++)
        {
            if (currentTarget == null) //pokud věž nenajde rychle cíl, tak proběhne pauza
            {
                yield return new WaitForSeconds(cooldownAfterBurst);
                isShooting = false;
            }

            Vector2 direction = (currentTarget.transform.position - firingPoint.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(firingPoint.position, direction, detectionRadius, enemyLayer);
        
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                muzzleFlash.Play(); //spuštění efektu výstřelu 
                enemy.TakeDamage(tower.damage); //udělení poškození nepřátelským jednotkám
                
            }

            yield return new WaitForSeconds(1f / attackSpeed);
        }
        yield return new WaitForSeconds(cooldownAfterBurst);

        isShooting = false;
    }

    private void RotateTowardsTarget() //funkce na otáčení se k cíly
    {

        float angle = Mathf.Atan2(currentTarget.transform.position.y - transform.position.y,
        currentTarget.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;
    
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        rotatingPoint.rotation = Quaternion.Slerp(rotatingPoint.rotation, targetRotation, smoothTime);
    }
    private void DetectEnemies() //funkce na detekci nepřátelských jednotek
    {
        targetsInRange.Clear(); //vyčištění seznamu detekovaných nepřátel

        Collider2D[] detectedEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        foreach (var collider in detectedEnemies)
        {
            EnemyHealth enemy = collider.GetComponent<EnemyHealth>();
            if (enemy != null && (!enemy.isHidden || (enemy.isHidden && tower.hiddenDetection)) && tower.isPlaced == true)
            {
                targetsInRange.Add(enemy); //přidání detekovaných nepřátelských jednotek do seznamu
            }
        }
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
        .ToList(); //seřazení nepřátelských jednotek podle jejich pozice na cestě pomocí lambdy

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
