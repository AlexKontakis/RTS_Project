using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave_spawner : MonoBehaviour
{
    public string Key;
    public GameObject Spawn, Target, Unit;
    public unit_manager um;

    void Start()
    {
        foreach (var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            Unit = Instantiate(Spawn, transform.position, transform.rotation);
            Unit.GetComponent<Attacking>().targets.Add(Target);
            Unit.GetComponent<Attacking>().breach = true;
            
            
            
            um.RecheckEnemy();
            um.RecheckFriendly();

        }

    }
}
