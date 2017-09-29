using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickCounter : MonoBehaviour {
    public static bool isSecondChance = false;
    public bool isVisible;
    public static int BrickCount = 0;

    //private void OnSpawned()
    //{
    //    isVisible = true;
    //    BrickCount++;
    //}

    private void Awake()
    {
        EventManager.OnGameStart += EventManager_OnGameStart;
    }

    private void EventManager_OnGameStart()
    {
        BrickCount = 0;
        isSecondChance = false;
        isVisible = false;
    }

    private void Update()
    {
        if (transform.position.y >= 4.8f && isVisible && isSecondChance)
        {
            isVisible = false;
            BrickCount--;
            //print(BrickCount);
        }
    }
}
