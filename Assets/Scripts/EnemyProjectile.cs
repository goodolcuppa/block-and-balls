using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float timeToLive = 4f;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0)
        {
            Destroy(this.gameObject);
        }
            
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ground" && collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "EnemyProjectile")
        {
            if (collision.gameObject.tag == "Projectile" && (transform.localScale.x == 1 || collision.gameObject.GetComponent<Projectile>().isBuckshot) || collision.gameObject.tag == "Player")
            {
                Destroy(this.gameObject);
            }
        }
    }
}
