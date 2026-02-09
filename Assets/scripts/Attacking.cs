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

    public Objective_Tracker ot;


    public int t=0;

    private unit_properties props;
    public bool isRanged;

   
    


    void Start()
    {
        attack_count = 0;
        // Cache references instead of searching every frame
        var umInstance = FindObjectOfType<unit_manager>();
        if (umInstance != null)
        {
            um = umInstance;
            bo = umInstance.GetComponent<Breach_order>();
        }
        provoked = false;
        attacking = false;
        agent = GetComponent<NavMeshAgent>();
        props = GetComponent<unit_properties>();
        if (props != null)
        {
            atk_spd = props.ATK_SPD;
            range = props.RANGE;
            atk_range = props.ATK_RANGE;
            atk_dmg = props.ATK;
        }
        var otInstance = FindObjectOfType<Objective_Tracker>();
        if(otInstance != null){
            ot = otInstance;
        }

        /* Precompute whether this unit is ranged based on type lists
        if (um != null && props != null)
        {
            isRanged = um.FRUSet.Contains(props.type) || um.ERUSet.Contains(props.type);
        }*/
    }


    void Update()
    {
        
        

       

        if (props != null && props.faction == "Enemy")
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
        else if (props != null && props.faction == "Friendly")
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
                if(props != null && props.faction == "Enemy")
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
                if (props != null && props.type != "Obstacle")
                {
                    if(ctarget.GetComponent<unit_properties>().HP <= 0)
                    {
                        targets.Remove(ctarget);
                    }
                    
                    if (props.faction  == "Enemy")
                    {
                        if (targets.Count > 0)
                        {
                            transform.LookAt(targets[0].transform.position);

                            if (isRanged)
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

                                if (isRanged)
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
            if (isRanged)
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
                if (isRanged)
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
        flag = false;
        if(target != ot.CaptureObj){
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
    
}
