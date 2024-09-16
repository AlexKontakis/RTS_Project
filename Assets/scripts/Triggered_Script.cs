using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggered_Script : MonoBehaviour
{
    public string faction;
    public float range;
    public GameObject spawn,self;
    public Gamemode_Manager gm;
    public unit_manager um;
    public bool near;
    public int Mob_Count;

    void Start()
    {
        
        self = transform.gameObject;
        foreach(var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
            gm = s.gameObject.GetComponent<Gamemode_Manager>();
        }
    }

    
    void Update()
    {
        if(faction == "Enemy")
        {
            if(um.Friendlies_alive.Count > 0)
            {
                for(int i = 0; i < um.Friendlies_alive.Count; i++)
                {
                    if((um.Friendlies_alive[i].transform.position - self.transform.position).magnitude <= range)
                    {
                        near = true;
                    }
                }
                if(near == true)
                {
                    for(int i = 0; i < Mob_Count; i++)
                    {
                        Instantiate(spawn, self.transform.position, self.transform.rotation);
                    }
                    near = false;
                    gm.spawners.Remove(self);
                    Destroy(self);
                }
            }

        }
        else if(faction == "Friendly")
        {
            if (um.Enemies_alive.Count > 0)
            {
                for (int i = 0; i < um.Enemies_alive.Count; i++)
                {
                    if ((um.Enemies_alive[i].transform.position - self.transform.position).magnitude <= range)
                    {
                        near = true;
                    }
                }
                if (near == true)
                {
                    for (int i = 0; i < Mob_Count; i++)
                    {
                        Instantiate(spawn, self.transform.position, self.transform.rotation);
                    }
                }
            }
        }
    }
}
