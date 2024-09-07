    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public float timeToLive = 4f;
    public bool isShielded;
    public bool isBuckshot;

    private bool hit = false;
    private float scale = 1f;   

    public Material hitMaterial;

    void Update()
    {

        if (!hit)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        
            timeToLive -= Time.deltaTime;
            if (timeToLive <= 0)
            {
                Destroy(this.gameObject);
            }
        }
        else 
        {
            if (scale < 3f)
            {
                scale += Time.deltaTime * 10f;
                transform.localScale = new Vector3(scale, scale, scale);
            }
            else 
            {
                Destroy(this.gameObject);
            }
        }
    }

    void Explode() 
    {
        hit = true;
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<MeshRenderer>().material = hitMaterial;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ground" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Projectile")
        {

            if(!isShielded)
            {
                Explode();
            }
            else if (isBuckshot)
            {
                return;
            }
            else if (collision.gameObject.tag == "Enemy")
            {
                Explode();
            }
        }
        else if (collision.gameObject.tag == "Projectile")
        {
            Physics.IgnoreCollision(this.GetComponent<SphereCollider>(), collision.collider);
        }
    }
}
