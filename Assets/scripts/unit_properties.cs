using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class unit_properties : MonoBehaviour
{
    public float HP, MAX_HP, SPD, ATK, ATK_SPD, RANGE, ATK_RANGE;
    public bool ordered, Spawned, flag, active, boss;
    public string faction, type;
    public GameObject highlight,spawner,hp_bar;
    public Material dead,healthy,half,low;
    public float timer,healthper,HPS,temp;
    public unit_manager um;

    public Vector3 test;

    public Vector3 hpbs;
    
    void Start()
    {
       
        foreach (var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
        flag = false;
        timer = 0;
        
        if(type != "Obstacle")
        {
            transform.gameObject.GetComponent<NavMeshAgent>().speed = SPD;
            HPS = hp_bar.transform.localScale.z;
            hp_bar.GetComponent<MeshRenderer>().material = healthy;
        }

    }


    void Update()
    {
        bool g = false;
        //hp bar
        if(HP >= 0 && (type != "Obstacle"))
        {
            healthper = (HP * 100f) / MAX_HP;

            temp = HPS * (healthper / 100);
            hpbs = new Vector3(0.2f, 0.2f, temp);
            hp_bar.transform.localScale = hpbs;
            if (healthper > 60)
            {
                hp_bar.GetComponent<MeshRenderer>().material = healthy;
            }
            else if (healthper <= 60 && healthper > 30)
            {
                hp_bar.GetComponent<MeshRenderer>().material = half;
            }
            else
            {
                hp_bar.GetComponent<MeshRenderer>().material = low;
            }
        }
        
        
        
        //
        for(int i = 0; i < um.FRU.Count; i++)
        {
            if(type == um.FRU[i])
            {
                g = true;
               
            }
        }
        if(g == true)
        {
            if (ordered == false && transform.GetComponent<Archer_fire>().is_firing == true)
            {
                transform.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            }
            else
            {
                transform.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            }
        }
        else
        {
            if(type != "Obstacle")
            {
                if (ordered == false && transform.GetComponent<Attacking>().attacking == true)
                {
                    transform.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                }
                else
                {
                    transform.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
                }
            }
        }
        if (type == "Obstacle")
        {
            if(faction == "Enemy")
            {
                if (HP <= 0)
                {

                    active = false;
                    transform.gameObject.SetActive(false);

                    Destroy(transform);

                }
            }
           
        }
        
        else
        {
            if (flag == false)
            {
                timer += Time.deltaTime;
            }
            if (timer >= 2)
            {
                if (faction == "Enemy")
                {
                    um.RecheckEnemy();
                }
                else
                {
                    um.RecheckFriendly();
                }
                flag = true;
                transform.gameObject.GetComponent<Attacking>().enabled = true;
                timer = 0;
                
                if(faction != "Enemy")
                {
                    if(Spawned == true)
                    {
                        transform.GetComponent<NavMeshAgent>().SetDestination(spawner.transform.position);
                        Spawned = false;
                    }
                    
                    
                }
               
            }

            //check if it still has orders


            
           
            test = transform.GetComponent<NavMeshAgent>().destination;


            if (HP <= 0)
            {
                if(faction == "Enemy")
                {
                    um.Enemies_alive.Remove(transform.gameObject);
                }
                else
                {
                    um.Friendlies_alive.Remove(transform.gameObject);
                }
                transform.GetComponent<MeshRenderer>().material = dead;
                transform.GetComponent<Attacking>().enabled = false;
                hp_bar.SetActive(false);
                for(int i = 0;i< um.FRU.Count; i++)
                {
                    if(type == um.FRU[i])
                    {
                        transform.GetComponent<Archer_fire>().enabled = false;
                    }
                }
                for (int i = 0; i < um.ERU.Count; i++)
                {
                    if (type == um.ERU[i])
                    {
                        transform.GetComponent<Archer_fire>().enabled = false;
                    }
                }
            }
        }
    }
}
