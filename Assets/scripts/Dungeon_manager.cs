using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Dungeon_manager : MonoBehaviour
{
    public unit_manager um;
    public List<GameObject> Dungeons = new List<GameObject>();
    //static Lists
    
    public static List<Dungeon_properties> D = new List<Dungeon_properties>();
    public static int matReward;
    public bool flag, arrived;
    public Gamemode_Manager gm;
    public TMP_Text Reward,Clear;
    public Button Enter;
    public Image Panel;
    public GameObject cDungeon;
    public static int dungeonid;
    public static bool victory;
    public static Vector3 dungeon;
    public int distanceTillArrive;

    //test//
    public bool v;
    
    void Start()
    {
        int y = 0;
        arrived = false;
        flag = false;
        foreach(var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
        foreach(var s in FindObjectsOfType<Dungeon_properties>())
        {
            AddToList(s.gameObject, y);
            y++;
        }
        if (unit_manager.F == false)
        {
            y = 0;
            foreach (var s in FindObjectsOfType<Dungeon_properties>())
            {
                AddStatics(s.gameObject, y);
                y++;
            }
        }
        for (int i = 0; i < Dungeons.Count; i++)
        {
            Dungeons[i].GetComponent<Dungeon_properties>().identification = i;
        }
        foreach(var s in FindObjectsOfType<Gamemode_Manager>())
        {
            gm = s;
        }
    }

    
    void Update()
    {
        v = victory;
        if (um.us.Count > 0)
        {
            
            arrived = false;
            for(int i = 0;i<Dungeons.Count; i++)
            {
                
                for(int j = 0; j < um.us.Count; j++)
                {
                    if((um.us[j].transform.position - Dungeons[i].transform.position).magnitude <= distanceTillArrive)
                    {
                        arrived = true;
                        cDungeon = Dungeons[i];
                        dungeon = Dungeons[i].transform.position;
                        matReward = cDungeon.GetComponent<Dungeon_properties>().mats;
                    }
                }
                if(arrived == true)
                {
                    Panel.gameObject.SetActive(true);
                    if(cDungeon.GetComponent<Dungeon_properties>().clear == "Not Clear")
                    {
                        Reward.gameObject.SetActive(true);
                        Reward.text = "Reward: "+ cDungeon.GetComponent<Dungeon_properties>().mats.ToString();
                        Clear.gameObject.SetActive(true);
                        Clear.text = cDungeon.GetComponent<Dungeon_properties>().clear;
                        Enter.gameObject.SetActive(true);
                        um.Available_Units();
                        
                    }
                    else if(cDungeon.GetComponent<Dungeon_properties>().clear == "Clear")
                    {
                        Reward.gameObject.SetActive(false);
                        Clear.gameObject.SetActive(true);
                        Clear.text = cDungeon.GetComponent<Dungeon_properties>().clear;
                        Enter.gameObject.SetActive(false);
                    }
                }
                else
                {
                    Panel.gameObject.SetActive(false);
                }
            }
            if(victory == true)
            {
                for(int i = 0; i < D.Count; i++)
                {
                    if(cDungeon.GetComponent<Dungeon_properties>().identification == D[i].I)
                    {
                        cDungeon.GetComponent<Dungeon_properties>().clear = "Clear";
                        D[i].C = "Clear";
                    }
                }
                victory = false;
            }
        }
    }

    public void EnterDungeon()
    {
        dungeonid = cDungeon.GetComponent<Dungeon_properties>().identification;
        victory = false;
        Gamemode_Manager.Gamemode = "Dungeon";
        SceneManager.LoadScene(sceneBuildIndex: 2);
    }

    void AddStatics(GameObject obj,int y)
    {
        bool flag2 = false;
        for (int i = 0; i < D.Count; i++)
        {
            if (obj.GetComponent<Dungeon_properties>().identification == D[i].I)
            {
                flag2 = true;
            }
        }
        if (flag2 == false)
        {
            D.Add(new Dungeon_properties(obj.GetComponent<Dungeon_properties>().clear, obj.GetComponent<Dungeon_properties>().identification));

        }
    }

    void AddToList(GameObject obj,int y)
    {
        for(int i = 0; i < Dungeons.Count; i++)
        {
            if(obj == Dungeons[i])
            {
                flag = true;
            }
        }
        if(flag == false)
        {
           
            Dungeons.Add(obj);
            
        }
        flag = false;
    }
}
