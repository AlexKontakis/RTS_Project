using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Deal_dmg_per_timer : MonoBehaviour
{
    public int Dmg;
    public float timer;
    public float cooldown;
    public bool attack, flg;
    public List<GameObject> Affected  = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= cooldown){
            timer = 0;
            attack = true;
            for(int i = 0; i< Affected.Count; i++){
                Affected[i].GetComponent<unit_properties>().HP -= Dmg;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.GetComponent<unit_properties>().faction == "Friendly")
       {
            flg = false;
            
            for(int i = 0; i < Affected.Count; i++){
                if(other.gameObject == Affected[i]){
                    flg = true;
                }
            }
            if(flg == false){
                Affected.Add(other.gameObject);  
            }
            
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        Affected.Remove(other.gameObject);
    }
}
