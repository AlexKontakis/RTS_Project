using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_Properties : MonoBehaviour
{
    public GameObject unit,spawn_location,spawn_destination;
    public float spawn_time,spawn_cost,timer;
    public bool flag;
    public int spawn_ammount, spawn_priceG, spawn_priceF, spawn_priceM;
    public string type;
    public unit_manager um;
    public Resource_Manager rm;

    void Start()
    {
        spawn_location = transform.gameObject;
        foreach (var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
            
        }

    }

    void Update()
    {
        if((spawn_ammount > 0 && um.unit_lim > um.cur_unit_lim) )
        {
            
            timer += Time.deltaTime;
            if(timer >= spawn_time)
            {
               
                spawn_ammount--;
                timer = 0;
                GameObject Unit = Instantiate(unit, spawn_location.transform.position, spawn_location.transform.rotation);
                Unit.GetComponent<unit_properties>().Spawned = true;
                Unit.GetComponent<unit_properties>().spawner = spawn_destination;
            }
            
        }
        else
        {
            timer = 0;
        }
    }
}
