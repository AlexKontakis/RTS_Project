using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Archer_fire : MonoBehaviour
{
    public unit_manager um;
    public Attacking atk;
    public bool is_firing;
    public float timer;
    public float fire_spd;
    public GameObject projectile;
    public NavMeshAgent agent;
    
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        is_firing = false;
        foreach(var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
        agent = transform.gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.GetComponent<unit_properties>().ordered == true)
        {
            timer = 0;
        }
        if(transform.gameObject.GetComponent<Attacking>().targets.Count > 0 && transform.GetComponent<unit_properties>().ordered == false)
        {
            if (is_firing == true)
            {

                timer += Time.deltaTime;
                if (timer >= fire_spd)
                {

                    timer = 0;
                    if (transform.gameObject.GetComponent<Attacking>().targets[0].transform.GetComponent<unit_properties>().type == "Obstacle")
                    {
                        int i = 1;
                        
                        projectile.GetComponent<projectile1>().target = transform.gameObject.GetComponent<Attacking>().targets[0];
                    }
                    else
                    {
                        projectile.GetComponent<projectile1>().target = transform.gameObject.GetComponent<Attacking>().targets[0];
                    }
                   
                    Instantiate(projectile, transform.position, transform.rotation);



                }
            }
            else
            {
                agent.speed = transform.GetComponent<unit_properties>().SPD;
                agent.stoppingDistance = 0;
            }
        }
       
    }
}
