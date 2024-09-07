using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int health;
    public bool isTargetable;
    public bool scaleWithHealth;
    [Header("Movement Settings")]
    public float speed;
    private Vector3 targetPoint;
    public bool canMove;
    [Header("Fire Settings")]
    public float fireCooldown;
    private float currentFireCooldown;
    public bool randomFirerate;
    public GameObject projectile;
    [Header("Miscellaneous Settings")]
    public Slider healthBar;

    void Start()
    {
        SetHealth(health);
        currentFireCooldown = 1f;
    }
    
    void Update()
    {
        // get new target point when at current target
        if (Vector3.Distance(transform.position, targetPoint) < 2f)
        {
            GetTargetPoint();
        }

        // set rotation and movement
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
        }

        RaycastHit[] hitInfo = Physics.SphereCastAll(transform.position, 75f, transform.forward, 1f);
        foreach (RaycastHit hit in hitInfo)
        {
            if (hit.collider.tag == "Player")
            {
                transform.LookAt(new Vector3(hit.collider.transform.position.x, transform.position.y, hit.collider.transform.position.z));
            }
        }

        // fire on cooldown reset
        if (currentFireCooldown <= 0)
        {
            GameObject newProjectile = Instantiate(projectile, transform.position + transform.forward, transform.rotation);
            newProjectile.transform.localScale = transform.localScale;
            currentFireCooldown = randomFirerate ? Random.Range(fireCooldown * 0.5f, fireCooldown * 1.5f) : fireCooldown;
        }

        currentFireCooldown -= Time.deltaTime;
    }

    private void GetTargetPoint()
    {
        targetPoint = new Vector3(Random.Range(-20f, 20f), transform.position.y, Random.Range(0f, 20f));
    }

    public void SetHealth(int value)
    {
        health = value;

        if (health <= 0) 
        {
            Destroy(this.gameObject);
        }

        if (scaleWithHealth)
        {
            transform.localScale = new Vector3(health, health, health);
        }

        if (!isTargetable)
        {
            GetComponentInChildren<Slider>().value = health/10f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Projectile")
        {
            SetHealth(health - 1);
        }
    }
}
