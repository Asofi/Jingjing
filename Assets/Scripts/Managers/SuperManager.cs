using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperManager : MonoBehaviour {

    private static SuperManager instance;
    public static SuperManager Instance { get { return instance; } }

    public GameManager GM;
    public GUIManager GUIManager;
    public ScoreManager ScoreManager;
    public ShopManager ShopManager;
    public Spawner Spawner;

    private void Awake()
    {
        instance = this;
    }

}
