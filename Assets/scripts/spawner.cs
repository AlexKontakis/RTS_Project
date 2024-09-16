using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public string Key;
    public GameObject Spawn;
    public unit_manager um;

    void Start()
    {
        foreach(var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            Instantiate(Spawn, transform.position, transform.rotation);
            um.RecheckEnemy();
            um.RecheckFriendly();
           
        }

    }
}
