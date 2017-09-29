using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    public Rigidbody2D rb;

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnSpawned()
    {
        GetComponent<SpriteRenderer>().sprite = SuperManager.Instance.ShopManager.CurrentSkin.Ball;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, rb.velocity, Color.green);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        AudioManager.PlayAudio("Ball");
    }

}
