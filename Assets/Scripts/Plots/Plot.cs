using Unity.VisualScripting;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    private Color startColor;
    private GameObject towerGO;
    public Tower tower;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }
    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    public void OnMouseDown()
    {
        if (tower != null)
        {
            tower.OpenTowerUI();
            return;
        }    

        GameObject towerToBuild = LoadoutManager.main.GetSelectedTower();
        if (towerToBuild == null) return;
        LoadoutManager.main.AfterPlacement();
        towerGO = Instantiate(towerToBuild.gameObject, transform.position, Quaternion.identity);
        tower = towerGO.GetComponent<Tower>();

        Debug.Log($"{towerToBuild} {transform.position}");
    }
}
