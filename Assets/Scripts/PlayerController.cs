using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 8f;
    private float fireCooldown;
    private Rigidbody rigidBody;

    // projectile objects
    public GameObject projectile;
    public GameObject shieldProjectile;
    public GameObject buckshotProjectile;
    private GameObject currentProjectile;
    public GameObject accessory;

    // ability info
    public GameObject shield;
    public bool canShield;
    public bool isShielded;
    public float shieldCooldown;
    private float shieldDuration;
    public float dashCooldown;
    private float dashSpeed = 32f;
    private float dashDuration;
    public float buckshotCooldown;
    private float buckshotSpread = 5f;
    private int buckshotCount = 6;
    public bool canBuckshot;

    private Camera mainCamera;

    public int health = 3;

    // materials
    public Material fullHealthMaterial;
    public Material highHealthMaterial;
    public Material lowHealthMaterial;
    public Material[] healthMaterials;

    void Start()
    {
        canShield = false;
        isShielded = false;
        canBuckshot = false;
        healthMaterials = new Material[] {lowHealthMaterial, highHealthMaterial, fullHealthMaterial};
        mainCamera = Camera.main;
        rigidBody = GetComponent<Rigidbody>();
        if (accessory != null)
        {
            GameObject accessoryInstance = Instantiate(accessory);
            accessoryInstance.transform.parent = transform;
            accessoryInstance.transform.localPosition = accessory.transform.position;
        }
    }

    void Update()
    {
        // get keyboard input
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        // move player
        Vector3 positionDirection = new Vector3(horizontal, 0f, vertical).normalized;
        transform.Translate(positionDirection * speed * Time.deltaTime, Space.World);

        // rotate towards mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            Vector3 mousePosition = hitInfo.point;
            transform.LookAt(new Vector3(mousePosition.x, transform.position.y, mousePosition.z));
        }

        // shoot on click
        if (Input.GetKey(KeyCode.Mouse0) && fireCooldown <= 0)
        {
            Instantiate(currentProjectile, transform.position + transform.forward, transform.rotation);
            fireCooldown = 0.2f;
        }

        fireCooldown -= Time.deltaTime;

        // shield ability
        if (Input.GetKey(KeyCode.Mouse1) && shieldCooldown <= 0 && canShield)
        {
            isShielded = true;
            shieldDuration = 1.5f;
            shieldCooldown = 15f;
        }

        if (isShielded)
        {
            shield.SetActive(true);
            currentProjectile = shieldProjectile;
            shieldDuration -= Time.deltaTime;
        }
        else
        {
            currentProjectile = projectile;
            shield.SetActive(false);
        }

        if (shieldDuration <= 0)
        {
            isShielded = false;
        }

        shieldCooldown -= Time.deltaTime;

        // dash ability
        if (Input.GetKey("left shift") && dashCooldown <= 0)
        {
            dashDuration = 0.2f;
            dashCooldown = 5f;
        }

        if (dashDuration > 0)
        {
            speed = dashSpeed;
        }
        else
        {
            speed = 8f;
        }

        dashCooldown -= Time.deltaTime;
        dashDuration -= Time.deltaTime;

        // buckshot ability
        if (Input.GetKey("space") && buckshotCooldown <= 0 && canBuckshot)
        {
            for (int i = 0; i < buckshotCount; i++)
            {
                // float randomY = transform.rotation.y + Random.Range(-buckshotSpread, buckshotSpread);
                // Quaternion buckshotRotation = Quaternion.Euler(transform.rotation.x, randomY, transform.rotation.z);

                Instantiate(buckshotProjectile, transform.position + transform.forward, transform.rotation);
            }
            buckshotCooldown = 30f;
        }

        buckshotCooldown -= Time.deltaTime;

        SetMaterial();
    }

    private void SetMaterial()
    {
        this.gameObject.GetComponentInChildren<MeshRenderer>().material = healthMaterials[health - 1];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Void")
        {
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.tag == "EnemyProjectile")
        {
            if (!isShielded)
            {
                health -= 1;

                if (health <= 0)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
