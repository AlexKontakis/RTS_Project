using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    
    public GameObject Spawn;
    public string faction;
    public float range;
    public GameObject self;
    public Gamemode_Manager gm;
    public unit_manager um;
    public bool near;
    public int Mob_Count;
    public string Type;
    public string SubType;
    public float timer;
    public GameObject Destination;
    public int Waves;
    public float TimeBetweenWaves;

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
        if(SubType == "Proximity")
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
                        Instantiate(Spawn, self.transform.position, self.transform.rotation);
                    }
                    near = false;
                    gm.spawners.Remove(self);
                    Destroy(self);
                }
            }
        }
        if(SubType == "Waves" && near == true){
            timer += Time.deltaTime;
            if(Waves >= 0 && timer >= TimeBetweenWaves){
                timer = 0;
                
                if(Waves > 0){
                    for(int i = 0; i < Mob_Count; i++)
                    {
                        Instantiate(Spawn, self.transform.position, self.transform.rotation);
                    }
                }
                Waves--;
                
            }
            else if(Waves < 0){
                gm.spawners.Remove(self);
                Destroy(self);
            }
        }
        

    }
}
