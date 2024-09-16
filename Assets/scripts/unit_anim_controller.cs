using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class unit_anim_controller : MonoBehaviour
{
    public unit_manager um;
    public Animator anim;
    public GameObject self;
    public bool test;

    
    void Start()
    {
        test = false;
        foreach(var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
        anim = transform.GetChild(2).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       if(transform.GetComponent<NavMeshAgent>().hasPath == true && (transform.GetComponent<NavMeshAgent>().destination - transform.position).magnitude > 3 && transform.GetComponent<Attacking>().attacking == false)
        {
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
        }
      
       if(transform.GetComponent<Attacking>().attacking == true)
        {
            anim.SetBool("fighting", true);
           

        }
        else
        {
            anim.SetBool("fighting", false);
        }
      

    }
}
