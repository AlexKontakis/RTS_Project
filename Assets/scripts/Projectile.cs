using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int DMG,lifetime;
    public float timer;
    public string type;
    public bool hasHit;
    void Start()
    {
        timer = 0;
    }

    
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= lifetime)
        {
            Destroy(transform.gameObject);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<unit_properties>() as unit_properties != null)
        {
            if(type == "Enemy")
            {
                if (other.gameObject.GetComponent<unit_properties>().faction == "Friendly")
                {
                    other.gameObject.GetComponent<unit_properties>().HP -= DMG;
                    transform.gameObject.GetComponent<projectile1>().enabled = false;
                    transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    transform.parent = other.transform;
                }
            }
            else if(type == "Friendly")
            {
                if (other.gameObject.GetComponent<unit_properties>().faction == "Enemy")
                {
                    hasHit = true;
                    other.gameObject.GetComponent<unit_properties>().HP -= DMG;
                    transform.gameObject.GetComponent<projectile1>().enabled = false;
                    transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    transform.parent = other.transform;
                }
            }
        }
    }
}
