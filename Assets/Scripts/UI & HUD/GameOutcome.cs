using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOutcome : MonoBehaviour
{
    public GameObject canvas;
    private bool isActive = false;

    private void Start()
    {
        canvas.SetActive(isActive);

    }

    public void Activate()
    {
        isActive = true;
        canvas.SetActive(isActive);
    }
}
