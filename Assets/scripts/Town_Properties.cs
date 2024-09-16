using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Town_Properties : MonoBehaviour
{
    public int rep, gold,food,recruits; //reputation needed,gold available,materials available,recruits vaialable//
    public string state;
    
    public Town_Manager tm;
    public Resource_Manager rm;
    public bool conq;
    public static bool conquered, mill, upgrading;
    public int identification;
    public bool flag, has_mill,is_upgrading;

    public float tribute_timer, upgrade_timer;
    public int gold_tribute,food_tribute;

    

    //for getters and setters//
    private string s;
    private int i;


    

    public string S { get => s; set => s = value; }
    public int I { get => i; set => i = value; }

    public Town_Properties(string s, int i)
    {
        this.s = s;
        this.i = i;
    }

    void Start()
    {
        is_upgrading = upgrading;
        has_mill = mill;
        flag = false;
        tribute_timer = 0;
        conquered = false;
        foreach (var s in FindObjectsOfType<Town_Manager>())
        {
            tm = s;

        }
        foreach (var s in FindObjectsOfType<Resource_Manager>())
        {
            rm = s;

        }

    }

    
    void Update()
    {
        if(is_upgrading == true)
        {
            upgrading = true;
            upgrade_timer += Time.deltaTime;
            if(upgrade_timer >= 20)
            {
                is_upgrading = false;
                upgrading = false;
                upgrade_timer = 0;
                has_mill = true;
                mill = true;
                tm.Mill.gameObject.SetActive(false);
            }
        }
        if(state == "Allied")
        {
            tribute_timer += Time.deltaTime;
            if(tribute_timer >= 10f)
            {
                tribute_timer = 0;
                Resource_Manager.Gold += gold_tribute;
                if(mill == true)
                {
                    Resource_Manager.Food += food_tribute;
                }
            }
        }
        
        if (conq == true)
        {
            state = "Allied";
            conquered = true;
        }
        if (Town_Manager.T.Count > 0)
        {
            for (int i = 0; i < Town_Manager.T.Count; i++)
            {
                if (tm.Towns[i] == transform.gameObject)
                {
                    state = Town_Manager.T[i].S;
                    
                }
            }
        }

        

    }
}
