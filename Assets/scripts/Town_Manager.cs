using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Town_Manager : MonoBehaviour
{
    public unit_manager um;
    public List<GameObject> Towns = new List<GameObject>();

    //static list//
   
    
    public static List<Town_Properties> T = new List<Town_Properties>();
    
    public static int a;
    public bool flag,arrived;
    public Gamemode_Manager gm;
    public TMP_Text F, State, R;
    public Button S, Mill;
    public Image Panel;
    public GameObject ctown;
    public static int townid, goldReward, foodReward;
    public static bool victory;
    public static Vector3 Town;
    public bool test;
    
    void Start()
    {
        int y = 0;
        arrived = false;
        flag = false;
        foreach (var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
        foreach (var s in FindObjectsOfType<Town_Properties>())
        {
            AddToList(s.gameObject,y);
            y++;
        }
        if(unit_manager.F == false)
        {
            y = 0;
            foreach (var s in FindObjectsOfType<Town_Properties>())
            {
                Addstatics(s.gameObject,y);
                y++;
            }
        }
        
        foreach(var s in FindObjectsOfType<Gamemode_Manager>())
        {
            gm = s;
        }
    }


    void Update()
    {
        
        
        if (um.us.Count > 0)
        {
            
            arrived = false;
            for (int i = 0; i < Towns.Count; i++)
            {
               
                for (int j = 0; j < um.us.Count; j++)
                {
                    if ((um.us[j].transform.position - Towns[i].transform.position).magnitude <= 15)
                    {

                        arrived = true;
                        ctown = Towns[i];
                        Town = Towns[i].transform.position;
                        goldReward = ctown.GetComponent<Town_Properties>().gold;
                        foodReward = ctown.GetComponent<Town_Properties>().food;
                       
                    }

                }
                if (arrived == true)
                {
                    Panel.gameObject.SetActive(true);
                    if (ctown.GetComponent<Town_Properties>().state == "Hostile")
                    {
                        F.gameObject.SetActive(true);
                        F.text = "Food: " + ctown.GetComponent<Town_Properties>().food.ToString();
                        State.gameObject.SetActive(true);
                        State.text = "State: " + ctown.GetComponent<Town_Properties>().state;
                        R.gameObject.SetActive(true);
                        R.text = "Recruits: " + ctown.GetComponent<Town_Properties>().recruits.ToString();
                        um.Available_Units();
                        S.gameObject.SetActive(true);
                        
                    }
                    else if(ctown.GetComponent<Town_Properties>().state == "Allied")
                    {
                        F.gameObject.SetActive(true);
                        F.text = "Food: " + ctown.GetComponent<Town_Properties>().food.ToString();
                        State.gameObject.SetActive(true);
                        State.text = "State: " + ctown.GetComponent<Town_Properties>().state;
                        
                        um.Available_Units();
                        S.gameObject.SetActive(false);
                        if(ctown.GetComponent<Town_Properties>().has_mill == false)
                        {
                            Mill.gameObject.SetActive(true);
                        }
                        else
                        {
                            Mill.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    Panel.gameObject.SetActive(false);
                }



            }
            if(victory == true)
            {
                for(int i = 0; i < T.Count; i++)
                {
                    if (ctown.GetComponent<Town_Properties>().identification == T[i].I)
                    {
                        ctown.GetComponent<Town_Properties>().state = "Allied";
                        T[i].S = "Allied";
                    }
                }
                victory = false;
            }


        }
        
    }

    public void upgrade()
    {
        ctown.GetComponent<Town_Properties>().is_upgrading = true;
    }

    public void Siege()
    {
        unit_manager.F = true;
        townid = ctown.GetComponent<Town_Properties>().identification;
        victory = false;
        Gamemode_Manager.Gamemode = "Siege";
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    void Addstatics(GameObject obj,int y)
    {
        bool flag2 = false;
        for (int i = 0; i < T.Count; i++)
        {
            if (obj.GetComponent<Town_Properties>().identification == T[i].I)
            {
                flag2 = true;
            }
        }
        if (flag2 == false)
        {
            T.Add(new Town_Properties(obj.GetComponent<Town_Properties>().state, obj.GetComponent<Town_Properties>().identification));
          
        }
    }
    void AddToList(GameObject obj,int y)
    {
       

        for (int i = 0; i < Towns.Count; i++)
        {
            if (obj == Towns[i])
            {
                flag = true;
            }
        }
        if (flag == false)
        {
            Towns.Add(obj);
           
            

        }
        flag = false;
        
    }
}
