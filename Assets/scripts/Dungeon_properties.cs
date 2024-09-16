using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon_properties : MonoBehaviour
{
    public int mats;//rewards//
    public string clear;//cleared//
    
    public Dungeon_manager dm;
    public Resource_Manager rm;
    public bool conq;
    public static bool conquered;
    public int identification;

    private string c;
    private int i;

    //test
    public float timer;

    public string C { get => c; set => c = value; }
    public int I { get => i; set => i = value; }

    public Dungeon_properties(string c, int i)
    {
        this.c = c;
        this.i = i;
    }

    void Start()
    {
        conquered = false;
        foreach (var s in FindObjectsOfType<Dungeon_manager>())
        {
            dm = s;

        }
        foreach (var s in FindObjectsOfType<Resource_Manager>())
        {
            rm = s;

        }
        timer = 0;

    }

    
    void Update()
    {
        if (conq == true)
        {
            clear = "Clear";
            conquered = true;
        }
        if (Dungeon_manager.D.Count > 0)
        {
            for (int i = 0; i < Dungeon_manager.D.Count; i++)
            {
                if (dm.Dungeons[i] == transform.gameObject)
                {
                    clear = Dungeon_manager.D[i].C;
                }
            }
        }




    }
}
