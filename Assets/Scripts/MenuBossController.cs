using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBossController : MonoBehaviour
{
    public GameObject projectile;
    private float fireCooldown;

    void Update()
    {
        // get reload time
        if (fireCooldown <= 0)
        {
            GameObject newProjectile = Instantiate(projectile, transform.position + transform.forward, transform.rotation);
            newProjectile.transform.localScale = transform.localScale;
            fireCooldown = 0.5f;
        }
        transform.Rotate(0f, -Time.deltaTime  * 20f, 0f);
        fireCooldown -= Time.deltaTime;
    }
}
