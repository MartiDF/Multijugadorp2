using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float salt = 10f;
    [SerializeField] GameObject bullet;

    public Vector2 moveValue;
    bool dreta = true;
    bool isGrounded = false; 
    Rigidbody2D rb;

    // Add a LayerMask for the ground detection
    [SerializeField] LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position + Vector3.down * 0.45f, 0.2f, groundLayer);
        float velY = rb.velocity.y;
        if(isGrounded && moveValue.y > 0)
        {
            velY = salt;
        }
        rb.velocity = new Vector2(moveValue.x, velY);   
    }

    public void Move(Vector2 value)
    {
        moveValue = value * speed;

        if (value.x > 0) dreta = true;
        else if (value.x < 0) dreta = false;

    }

    public void Shoot()
    {
        Bullet _bullet = GameObject.Instantiate(bullet, transform.position, transform.rotation).GetComponent<Bullet>();
        _bullet.direccio = dreta ? Vector3.right : Vector3.left;
        _bullet.transform.position += _bullet.direccio;
    }

}