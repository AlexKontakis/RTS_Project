using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision_dmg : MonoBehaviour
{
    //this object is enviromental dmg meaning it damages all things regardless of its faction as long as it is a unit it gets damaged if it touches this
    
    public int dmg;
    public float life_time;
    public bool on;
    void Start()
    {
        on = false;
        dmg = dmg * (int)transform.localScale.x;
    }

   
    void Update()
    {
        /*life_time += Time.deltaTime;
        if(life_time > 20f && on == true)
        {
            transform.gameObject.SetActive(false);
        }*/
    }

    public void OnCollisionEnter(Collision other)
    {
        if(other.collider.transform.GetComponent<unit_properties>() != null)
        {
            on = true;
            other.collider.transform.GetComponent<unit_properties>().HP -= dmg;
        }
    }
}
