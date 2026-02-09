using UnityEngine;

public class Boss_Abilities : MonoBehaviour
{
    public string Boss;
    public float timer;
    public unit_manager um;
    public GameObject AreaOfEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(var s in FindObjectsOfType<unit_manager>()){
            um = s;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Bandit Boss fight
        if(Boss == "Bandit_Boss")
        {
            if(GetComponent<unit_properties>().HP <= GetComponent<unit_properties>().MAX_HP/2){
                GetComponent<unit_properties>().ATK = GetComponent<unit_properties>().ATK*2;
                AreaOfEffect.GetComponent<Deal_dmg_per_timer>().cooldown = AreaOfEffect.GetComponent<Deal_dmg_per_timer>().cooldown/2;
            }
        }
       
    }
}
