using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemode_Manager : MonoBehaviour
{
    //test//
    public Material green;
    //test
    public static string Gamemode;
    public unit_manager um;
    public GameObject Capture_point,Base ,spawn, Boss;
    public float Timer, loseTimer;
    public bool IN, Win, Lose;
    public Button Exit;
    public static int current_id, goldReward, foodReward;
    public List<GameObject> spawners = new List<GameObject>();
    public GameObject defeat;
    public int Otp = 0;
    public int spawnersCount = 0;
    public int test = 0;
    public Objective_Tracker ot;

    public int enemyCount;
    public Scene currentScene;
    public bool flag = false;
    public int Sspears, Sarchers;//temp number of all surviving unit types in order for the coresponding static values to be updated and then given to the next scene
    public string Game_mode;


    void Start()
    {
        
        
        
       
    }
    
   

    void Update()
    {
       if(flag == false){
        //defeat.SetActive(false);
        currentScene = SceneManager.GetActiveScene();
        Otp = 0;
        loseTimer = 0;
        Sarchers = 0;
        Sspears = 0;
        if(currentScene.buildIndex == 0){
            Gamemode = "World_Map";
        }
        foreach (var s in FindObjectsOfType<Objective_Tracker>()){
            ot = s;
        }
        foreach (var s in FindObjectsOfType<CapsuleCollider>())
        {
            
            if (s.CompareTag("Finish"))
            {
                Capture_point = s.gameObject;
            }
            if(s.gameObject.GetComponent<unit_properties>() != null)
            {
                if(s.gameObject.GetComponent<unit_properties>().boss == true)
                {
                    Boss = s.gameObject;
                }
            }
        }
        IN = false;
        Win = false;
        Timer = 0;
        foreach (var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
        foreach(var s in FindObjectsOfType<spawner>()){
            spawners.Add(s.gameObject);
        }
        flag = true;
       }
        //Siege//
       if (Gamemode == "Siege")
            {
                if(Base != null && Base.GetComponent<unit_properties>() != null && Base.GetComponent<unit_properties>().HP <= 0)
                {
                    Lose = true;
                }
                if (um != null && um.Friendlies_alive != null && um.Friendlies_alive.Count > 0 && Capture_point != null)
                {
                    int units_in = 0;
                    for (int i = 0; i < um.Friendlies_alive.Count; i++)
                    {
                        if (um.Friendlies_alive[i] != null && (um.Friendlies_alive[i].transform.position - Capture_point.transform.position).magnitude <= 10)
                        {
                            units_in++;
                        }
                    }
                    if (units_in > 0)
                    {
                        IN = true;
                    }
                    else
                    {
                        IN = false;
                        Timer = 0;
                    }
                }
                
                if (IN == true)
                {
                    Timer += Time.deltaTime;
                    if (Timer >= 10)
                    {
                        if (Capture_point != null)
                        {
                            var renderer = Capture_point.GetComponent<MeshRenderer>();
                            if (renderer != null)
                            {
                                renderer.material = green;
                            }
                        }
                        Win = true;
                    }
                }
                if(Lose == true)
                {
                    if(Otp == 0)
                    {
                        Otp = 1;
                        if (um != null)
                        {
                            um.Available_Units();
                        }
                    }
                    defeat.SetActive(true);
                    loseTimer += Time.deltaTime;
                    if(loseTimer >= 5)
                    {
                        Gamemode_Manager.Gamemode = "World_Map";
                        SceneManager.LoadScene(sceneBuildIndex: 0);
                    }

                }
                if (Win == true)
                {
                    if (Otp == 0)
                    {
                        Otp = 1;
                        if (um != null)
                        {
                            um.Available_Units();
                        }
                    }

                    Town_Manager.victory = true;
                    if (Exit != null)
                    {
                        Exit.gameObject.SetActive(true);
                    }
                }
            }
        //Dungeon//
       if (Gamemode == "Dungeon")
        {
            
           if(ot.Objective1 == true && ot.Objective2 == true && ot.Objective3 == true && ot.Objective4 == true){
                Win = true;
        
           }
            
            if (Win == true)
            {
                Dungeon_manager.victory = true;
                if (Exit != null)
                {
                    Exit.gameObject.SetActive(true);
                }
            }
        }
           
        
       
       

    }

    public void Exit_To_World_Map()
    {
        if(Gamemode == "Siege")
        {
            Resource_Manager.Gold += Town_Manager.goldReward;
            Resource_Manager.Food += Town_Manager.foodReward;
        }
        else if(Gamemode == "Dungeon")
        {
            Resource_Manager.Materials += Dungeon_manager.matReward;
        }


        Gamemode_Manager.Gamemode = "World_Map";
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}
