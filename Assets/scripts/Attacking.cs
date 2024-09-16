using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Attacking : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();
    public NavMeshAgent agent;
    public GameObject ctarget, listed_self;
    public bool attacking, provoked;
    public bool breach; //shows if unit has been ordered to break down an obstacle//
    public GameObject breach_target, temp; // breach target obj//
    public float timer, atk_spd, range, atk_range, atk_dmg;
    public unit_manager um;
    public int attack_count;
    public Breach_order bo;


    public int t=0;

   
    


    void Start()
    {
        attack_count = 0;
        foreach (var s in FindObjectsOfType<unit_manager>())
        {
            um = s.gameObject.GetComponent<unit_manager>();
            bo = s.gameObject.GetComponent<Breach_order>();
        }
        provoked = false;
        attacking = false;
        agent = transform.gameObject.GetComponent<NavMeshAgent>();
        atk_spd = transform.gameObject.GetComponent<unit_properties>().ATK_SPD;
        range = transform.gameObject.GetComponent<unit_properties>().RANGE;
        atk_range = transform.gameObject.GetComponent<unit_properties>().ATK_RANGE;
        atk_dmg = transform.gameObject.GetComponent<unit_properties>().ATK;
    }


    void Update()
    {
        
        

       

        if (transform.GetComponent<unit_properties>().faction == "Enemy")
        {
            if(um.Friendlies_alive.Count > 0)
            {
                for (int i = 0; i < um.Friendlies_alive.Count; i++)
                {
                    if ((um.Friendlies_alive[i].transform.position - transform.position).magnitude <= range)
                    {
                        if (um.Friendlies_alive[i].GetComponent<unit_properties>().HP > 0)
                        {
                            Add(um.Friendlies_alive[i], false);
                            provoked = true;
                        }
                    }
                }
            }
        }
        else if (transform.GetComponent<unit_properties>().faction == "Friendly")
        {
          if(breach == false)
            {
                if (um.Enemies_alive.Count > 0)
                {
                    for (int i = 0; i < um.Enemies_alive.Count; i++)
                    {
                        if ((um.Enemies_alive[i].transform.position - transform.position).magnitude <= range)
                        {
                            if (um.Enemies_alive[i].GetComponent<unit_properties>().HP > 0)
                            {

                                Add(um.Enemies_alive[i], false);
                                provoked = true;
                            }
                        }
                    }
                }
            }
          else
          {

               
                provoked = true;
                if((breach_target.transform.position - transform.position).magnitude <= range)
                {
                    Add(breach_target, false);
                    agent.Stop();
                    agent.ResetPath();
                }
                else
                {
                    agent.SetDestination(breach_target.transform.position);
                }
                
            }
        }
       
        //getting to target//
        if (targets.Count > 0)
        {
            //optimizing targets list//

            if (breach == true)
            {
                provoked = true;
                
                //enemies prioritize units over flag//
                if(transform.GetComponent<unit_properties>().faction == "Enemy")
                {
                    if(targets.Count > 1)
                    {
                        if(targets[0].GetComponent<unit_properties>().type == "Obstacle")
                        {
                            t++;
                            GameObject temp = targets[0];
                            targets.Remove(temp);
                            targets.Add(temp);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (targets[i].GetComponent<unit_properties>().type != "Obstacle")
                        {
                            targets.Remove(targets[i]);
                        }
                    }
                }
                
                  
                    
                
                //**************************************//
            }
            else
            {
                for (int i = 0; i < targets.Count; i++)
                {

                    if (targets[i].GetComponent<unit_properties>().HP <= 0)
                    {

                        targets.Remove(targets[i]);
                    }
                    if ((targets[i].transform.position - transform.position).magnitude > transform.gameObject.GetComponent<unit_properties>().RANGE)
                    {

                        targets.Remove(targets[i]);
                    }
                    if (targets[i].GetComponent<unit_properties>().type == "Obstacle")
                    {
                        targets.Remove(targets[i]);
                    }

                }
            }
            //optimizing targets list//

            ctarget = targets[0];
            if (provoked == true)
            {
                if (transform.GetComponent<unit_properties>().type != "Obstacle")
                {
                    if(ctarget.GetComponent<unit_properties>().HP <= 0)
                    {
                        targets.Remove(ctarget);
                    }
                    
                    if (transform.GetComponent<unit_properties>().faction  == "Enemy")
                    {
                        if (targets.Count > 0)
                        {
                            bool g = false;
                            transform.LookAt(targets[0].transform.position);
                            
                            for (int i = 0; i < um.ERU.Count; i++)
                            {
                                if (transform.gameObject.GetComponent<unit_properties>().type == um.ERU[i])
                                {
                                    g = true;
                                }
                            }
                            if (g == true)
                            {
                                if (transform.GetComponent<Archer_fire>().is_firing == true)
                                {
                                    agent.Stop();
                                    agent.ResetPath();
                                }
                                else
                                {
                                    agent.SetDestination(targets[0].transform.position);
                                    agent.stoppingDistance = 1;
                                }
                            }
                            else
                            {
                                agent.SetDestination(targets[0].transform.position);
                                agent.stoppingDistance = 2;
                            }
                        }
                    }
                    else
                    {
                        if (um.ordered == false)
                        {
                            if (targets.Count > 0)
                            {
                                transform.LookAt(targets[0].transform.position);
                                bool b = false;
                                for (int i = 0; i < um.FMU.Count; i++)
                                {
                                    if (transform.gameObject.GetComponent<unit_properties>().type == um.FRU[i])
                                    {
                                        b = true;
                                    }
                                }
                                
                                if (b == true)
                                {
                                    if (transform.GetComponent<Archer_fire>().is_firing == true)
                                    {
                                        agent.Stop();
                                        agent.ResetPath();
                                    }
                                    else
                                    {
                                        agent.SetDestination(targets[0].transform.position);
                                        agent.stoppingDistance = 1;
                                    }
                                }
                                else
                                {
                                    agent.SetDestination(targets[0].transform.position);
                                    agent.stoppingDistance = 2;
                                }
                            }

                        }
                    }
                    /////
                    
                    
                }
               
            }
        }
        //attacking//
        if (ctarget != null)
        {
            bool g = false;
            for (int i = 0; i < um.FRU.Count; i++)
            {
                if (transform.gameObject.GetComponent<unit_properties>().type == um.FRU[i])
                {
                    g = true;
                }
            }
            for (int i = 0; i < um.ERU.Count; i++)
            {
                if (transform.gameObject.GetComponent<unit_properties>().type == um.ERU[i])
                {
                    g = true;
                }
            }
            if (g == true)
            {
                transform.gameObject.GetComponent<Archer_fire>().is_firing = true;

            }
            else
            {
                if ((ctarget.transform.position - transform.position).magnitude <= atk_range)
                {
                    timer += Time.deltaTime;
                    attacking = true;
                }
                else
                {
                    
                    attacking = false;
                }
            }
        }
        else
        {
            bool g = false;
            for (int i = 0; i < um.FRU.Count; i++)
            {
                if (transform.gameObject.GetComponent<unit_properties>().type == um.FRU[i])
                {
                    g = true;
                }
            }
            for (int i = 0; i < um.ERU.Count; i++)
            {
                if (transform.gameObject.GetComponent<unit_properties>().type == um.ERU[i])
                {
                    g = true;
                }
            }
            if (g == true)
            {
                transform.gameObject.GetComponent<Archer_fire>().is_firing = false;

            }
            else
            {
                attacking = false;
            }
        }
        if (attacking == true)
        {
            if (timer >= atk_spd)
            {
                attack_count++;
                if (attack_count > 2)
                {
                    attack_count = 1;
                }
                
                ctarget.gameObject.GetComponent<unit_properties>().HP -= atk_dmg;
                timer = 0;
                
            }
        }
        else
        {
            attack_count = 0;
           

        }
        //calming down//
        if(transform.GetComponent<unit_properties>().faction == "Friendly")
        {
            if ((targets.Count == 0 || transform.GetComponent<unit_properties>().ordered == true) && breach == false)
            {
                bool g = false;
                for (int i = 0; i < um.FRU.Count; i++)
                {
                    if (transform.gameObject.GetComponent<unit_properties>().type == um.FRU[i])
                    {
                        g = true;
                    }
                }
                if (g == true)
                {
                    transform.gameObject.GetComponent<Archer_fire>().is_firing = false;
                }
                timer = 0;
                attacking = false;
                provoked = false;
                ctarget = null;
            }
        }
        else
        {
            if (targets.Count == 0 && breach == false)
            {
                bool g = false;
                for (int i = 0; i < um.FRU.Count; i++)
                {
                    if (transform.gameObject.GetComponent<unit_properties>().type == um.FRU[i])
                    {
                        g = true;
                    }
                }
                if (g == true)
                {
                    transform.gameObject.GetComponent<Archer_fire>().is_firing = false;
                }
                timer = 0;
                attacking = false;
                provoked = false;
                ctarget = null;
            }
        }
        
    }

    void Add(GameObject target, bool flag)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (target == targets[i])
            {
                flag = true;
            }
        }
        if (flag == false)
        {
            targets.Add(target);
        }
        flag = false;
    }
    
}
