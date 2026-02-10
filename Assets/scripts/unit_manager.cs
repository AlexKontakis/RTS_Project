using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Animations;
using TMPro;
using System.IO;
using System.Linq;

public class unit_manager : MonoBehaviour
{
    //Lists of friendly units alive
    public List<GameObject> Friendlies_alive = new List<GameObject>();
    //Lists of enemy units alive
    public List<GameObject> Enemies_alive = new List<GameObject>();
    //List of selected Units
    public List<GameObject> us = new List<GameObject>();
    //Lists of unit types
    public List<string> fileLinesF = new List<string>();//friendlies
    public List<string> fileLinesE = new List<string>();//enemies
    public List<string> EMU = new List<string>();//enemy melee unit list
    public List<string> FMU = new List<string>();//friendly melee unit list
    public List<string> ERU = new List<string>();//enemy ranged unit list
    public List<string> FRU = new List<string>();//friendly ranged unit list

    // Optimized lookup sets for unit type categories
    public HashSet<string> FRUSet = new HashSet<string>();
    public HashSet<string> ERUSet = new HashSet<string>();

    public List<GameObject> spawnList = new List<GameObject>();//list of all spawners for instantiation of units at startup

    public GameObject spawn;

    // double click select all visible to camera of same type
   
    
    public GameObject doubleTarget;
   
    public int clickcounter = 0;
    public float dctimer = 0;
    public bool dcflag = false;

    //
    public int cur_unit_lim = 0;
    public int unit_lim = 20;

    

    public Camera cam;
    Vector3 mp;
    public bool isdragging = false;
    private RaycastHit hit1, hit2;
    public bool flag, ordered, reached;
    public GameObject Destination, start;
    public int un_count;
    public Material yellow;
    public static int Spearmen,Archer;
    public Town_Manager tm;
    public Vector3 start_pos;
    public static bool F;
    public Gamemode_Manager gm;
    public GameObject Commander;

    //test
    public bool test;
    public int s;

    public GameObject Spear_Pref;//prefabs
    public GameObject Archer_Pref;
    public GameObject Commander_Pref;

    // Formation control
    public FormationController formationController;

    //drag select//
    private void OnGUI()
    {
        if (isdragging == true)
        {
            var rect = ScreenHelper.GetScreenRect(mp, Input.mousePosition);
            ScreenHelper.DrawScreenRect(rect, Color.clear);
            ScreenHelper.DrawScreenRectBorder(rect, 5, Color.green);


        }
    }
    private bool IsWithinSelectionBounds(Transform transform)
    {
        // Use the same camera reference that is used for raycasts / gameplay,
        // instead of relying on Camera.main (which may be a different camera or null)
        var camera = cam != null ? cam : Camera.main;
        if (camera == null) return false;

        var viewportBounds = ScreenHelper.GetViewportBounds(camera, mp, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(transform.position));
    }

    void Start()
    {
        //Spearmen = 5;
        
        dcflag = false;
        //test
        test = false;

        foreach (var s in FindObjectsOfType<Gamemode_Manager>())
        {
            gm = s;
        }
        foreach( var s in FindObjectsOfType<MeshRenderer>())
        {
            if (s.CompareTag("Spawn"))
            {
                spawn = s.gameObject;
            }
        }
        foreach(var s in FindObjectsOfType<unit_properties>()){
            if(s.gameObject.GetComponent<unit_properties>().type == "Commander"){
                Commander = s.gameObject;
            }
        }
        foreach(var s in FindObjectsOfType<Town_Manager>())
        {
            tm = s;
        }
        foreach(var s in FindObjectsOfType<Spawner_Properties>())
        {
            spawnList.Add(s.gameObject);
        }
        // Auto-find FormationController if not wired in the Inspector
        if (formationController == null)
        {
            formationController = FindObjectOfType<FormationController>();
        }
        if(transform.GetComponent<Town_Manager>() != null && transform.GetComponent<Dungeon_manager>() != null)
        {
            if (Town_Manager.victory == true && Gamemode_Manager.Previous_Gamemode == "Siege")
            {
                Commander.transform.position = Town_Manager.Town;
                //GameObject Unit = Instantiate(Commander_Pref, Dungeon_manager.dungeon, transform.rotation);
            }
            else if (Dungeon_manager.victory == true && Gamemode_Manager.Previous_Gamemode == "Dungeon")
            {
                Commander.transform.position = Dungeon_manager.dungeon;
                //GameObject Unit = Instantiate(Commander_Pref, Dungeon_manager.dungeon, transform.rotation);
            }
        }
        if (Spearmen > 0)
        {   
            
            for (int i = 0; i < Spearmen; i++)
            {
                GameObject Unit = Instantiate(Spear_Pref, spawn.transform.position, transform.rotation);
                 
                Formation(i, spawn.transform.position,Unit.GetComponent<NavMeshAgent>());
            }
            
            
            
        }
        if (Archer > 0)
        {
            if (transform.GetComponent<Town_Manager>() != null && transform.GetComponent<Dungeon_manager>() != null)
            {

                if (Town_Manager.victory == true)
                {
                    GameObject Unit = Instantiate(Commander_Pref, Dungeon_manager.dungeon, transform.rotation);
                }
                else if (Dungeon_manager.victory == true)
                {
                    GameObject Unit = Instantiate(Commander_Pref, Dungeon_manager.dungeon, transform.rotation);
                }
            }
            else
            {
                
                for (int i = 0; i < Archer; i++)
                {
                    GameObject Unit = Instantiate(Archer_Pref, spawn.transform.position, transform.rotation);

                    Formation(i, spawn.transform.position, Unit.GetComponent<NavMeshAgent>());
                }
            }
        }
        reached = false;
        ordered = false;
        flag = false;
        //friendly - use paths relative to the current project instead of hard-coded absolute paths
        string F_path = Path.Combine(Application.dataPath, "Unit_Types", "F_Unit_Types.txt");
        fileLinesF = File.ReadAllLines(F_path).ToList();
        F_path = Path.Combine(Application.dataPath, "Unit_Types", "Melee_FUnits.txt");
        FMU = File.ReadAllLines(F_path).ToList();
        F_path = Path.Combine(Application.dataPath, "Unit_Types", "Ranged_FUnits.txt");
        FRU = File.ReadAllLines(F_path).ToList();
        // Build fast-lookup sets for ranged unit types
        FRUSet.Clear();
        foreach (var t in FRU)
        {
            FRUSet.Add(t);
        }
        foreach (var s in FindObjectsOfType<NavMeshAgent>())
        {
            GameObject p = s.gameObject;
            for (int i = 0; i < fileLinesF.Count; i++)
            {
                if (p.GetComponent<unit_properties>().type == fileLinesF[i])
                {
                    Friendlies_alive.Add(p);
                }
            }
        }
        //enemy
        string E_path = Path.Combine(Application.dataPath, "Unit_Types", "E_Unit_Types.txt");
        fileLinesE = File.ReadAllLines(E_path).ToList();
        E_path = Path.Combine(Application.dataPath, "Unit_Types", "Melee_EUnits.txt");
        EMU = File.ReadAllLines(E_path).ToList();
        E_path = Path.Combine(Application.dataPath, "Unit_Types", "Ranged_EUnits.txt");
        ERU = File.ReadAllLines(E_path).ToList();
        ERUSet.Clear();
        foreach (var t in ERU)
        {
            ERUSet.Add(t);
        }
        foreach (var s in FindObjectsOfType<NavMeshAgent>())
        {
            GameObject p = s.gameObject;
            for (int i = 0; i < fileLinesE.Count; i++)
            {
                if (p.GetComponent<unit_properties>().type == fileLinesE[i])
                {
                    Enemies_alive.Add(p);
                }
            }
        }
        if(Town_Manager.victory == true || Dungeon_manager.victory == true)
        {
            for (int i = 0; i < Friendlies_alive.Count; i++)
            {
                AddToList(Friendlies_alive[i], true);
            }
            
            
            
        }

    }


    void Update()
    {
        
       
        cur_unit_lim = Friendlies_alive.Count;
       //double click select all visible units of same type mechanic
       if(dcflag == true)
        {
            dctimer += Time.deltaTime;
        }
       //

        //check if it has order//
        if (us.Count > 0)
        {
            int x = 0;
            for (int i = 0; i < us.Count; i++)
            {
                if ((Destination.transform.position - us[i].transform.position).magnitude <= 10)
                {
                    x++;
                }
            }
            if (x == us.Count)
            {
                reached = true;
            }
            if (reached == false)
            {
                ordered = true;
                for(int i = 0; i < us.Count; i++)
                {
                    us[i].GetComponent<unit_properties>().ordered = true;//!!
                }
            }
            else
            {
                ordered = false;
                for (int i = 0; i < us.Count; i++)
                {
                    us[i].GetComponent<unit_properties>().ordered = false;//!!
                }
            }
        }
        //left click functions//
        if (Input.GetMouseButtonDown(0))
        {
            mp = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit1))
            {
                if (hit1.collider.gameObject.CompareTag("ground"))
                {
                    if (us.Count > 0)
                    {
                        reached = true;
                        ordered = false;
                        for(int i = 0; i < us.Count; i++)
                        {
                            us[i].GetComponent<unit_properties>().ordered = false;// still goes to the target but since its not selected atm then it's pathing can be interupted by enemy encounters
                        }
                    }
                    isdragging = true;
                    RemoveFromList();
                    clickcounter = 0;
                    dcflag = false;
                    dctimer = 0;
                    doubleTarget = null;
                   
                }
                else if(hit1.collider.gameObject.GetComponent<CapsuleCollider>() != null)
                {
                    
                    RemoveFromList();
                    AddToList(hit1.collider.gameObject, true);
                    dcflag = true;
                    clickcounter++;
                    
                    if (GameObject.ReferenceEquals(hit1.collider.transform.gameObject,doubleTarget))
                    {
                        dctimer = 0;
                        clickcounter = 2;
                        dcflag = false;

                    }
                    else if (!GameObject.ReferenceEquals(hit1.collider.transform.gameObject, doubleTarget))
                    {
                        dctimer = 0;
                        clickcounter = 1;
                       
                        doubleTarget = hit1.collider.transform.gameObject;
                    }
                    if (clickcounter == 2)
                    {
                        
                        
                        clickcounter = 0;
                        //clickcounter = 0;
                        for(int i = 0; i < Friendlies_alive.Count; i++)
                        {
                            if (doubleTarget.transform.GetComponent<unit_properties>().type == Friendlies_alive[i].transform.GetComponent<unit_properties>().type)
                            {
                                Vector3 screenPoint = cam.WorldToViewportPoint(Friendlies_alive[i].transform.position);
                                if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
                                {
                                    AddToList(Friendlies_alive[i], true);
                                }
                            }
                        }
                    }

                }
                else
                {
                    isdragging = true;
                }
            }
        }
        //right click functionality//
        if (Input.GetMouseButtonDown(1))
        {
            ordered = true;
            reached = false;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit2))
            {
                if (hit2.collider.gameObject.CompareTag("ground"))
                {
                    Destination.transform.position = hit2.point;
                    if (us.Count > 0)
                    {
                        // Mark selected units as ordered
                        for (int i = 0; i < us.Count; i++)
                        {
                            us[i].GetComponent<unit_properties>().ordered = true;//!
                        }

                        // Use FormationController's current formation (default: loose box)
                        // so that movement keeps the last chosen formation.
                        if (formationController != null)
                        {
                            formationController.ApplyCurrentFormationAt(hit2.point);
                        }
                        /*else
                        {
                            // Fallback to legacy per-index formation if controller is missing
                            for (int i = 0; i < us.Count; i++)
                            {
                                Formation(un_count, hit2.point, us[i].GetComponent<NavMeshAgent>());
                                un_count++;
                            }
                            un_count = 0;
                        }*/
                    }
                }
            }
        }
        //releasing left click//
        if (Input.GetMouseButtonUp(0))
        {
            isdragging = false;
        }
        //Drag n Select//
        if (Input.GetMouseButtonUp(0))
        {
            // When releasing the left mouse button, select all friendly, alive units
            // whose screen position lies within the drag rectangle.
            foreach (var agent in FindObjectsOfType<NavMeshAgent>())
            {
                var props = agent.gameObject.GetComponent<unit_properties>();
                if (props == null) continue;

                // Only select player's units
                if (props.faction != "Friendly") continue;

                // Only select alive units
                if (props.HP <= 0) continue;

                if (IsWithinSelectionBounds(agent.transform))
                {
                    AddToList(agent.gameObject, true);
                    props.highlight.SetActive(true);
                }
            }

            isdragging = false;
        }



    }
    public void Available_Units()
    {
        //Setting how many units and what units the player has available//
        if(Gamemode_Manager.Gamemode != "World_Map"){
            int spears = 0; // spearmen
            if (Friendlies_alive.Count > 0)
            {
                for (int i = 0; i < Friendlies_alive.Count; i++)
                {
                    if (Friendlies_alive[i].GetComponent<unit_properties>().type == "Spearman")
                    {
                        spears++;
                    }
                }
            }
            Spearmen = spears;
            int archers = 0; //archers
            if(Friendlies_alive.Count > 0)
            {
                for(int i = 0; i < Friendlies_alive.Count; i++)
                {
                    if(Friendlies_alive[i].GetComponent<unit_properties>().type == "Archer")
                    {
                        archers++;
                    }
                }
            }
            Archer = archers;
        }
       
    }
    void AddToList(GameObject obj, bool multiselect)
    {
        if (multiselect == false)
        {
            RemoveFromList();
        }
        for (int i = 0; i < us.Count; i++)
        {
            if (obj == us[i])
            {
                flag = true;
            }
        }
        if (flag == false)
        {
            if (obj.transform.gameObject.GetComponent<unit_properties>().HP > 0)
            {
                us.Add(obj);
                obj.GetComponent<unit_properties>().highlight.SetActive(true);
            }
        }
        flag = false;
    }
    public void RecheckFriendly()
    {
        GameObject frien;
        foreach(var s in FindObjectsOfType<NavMeshAgent>())
        {

            if (s.gameObject.GetComponent<unit_properties>().faction == "Friendly")
            {
                frien = s.gameObject;
                for (int i = 0; i < Friendlies_alive.Count; i++)
                {
                    if (frien == Friendlies_alive[i])
                    {
                        flag = true;
                    }
                }
                if (flag == false)
                {
                    Friendlies_alive.Add(frien);

                }
                flag = false;
            }
        }
       
    }
    public void RecheckEnemy()
    {
        GameObject obj;
        foreach(var s in FindObjectsOfType<NavMeshAgent>())
        {
            if(s.gameObject.GetComponent<unit_properties>().faction == "Enemy")
            {
                obj = s.gameObject;
                for (int i = 0; i < Enemies_alive.Count; i++)
                {
                    if (obj == Enemies_alive[i])
                    {
                        flag = true;
                    }
                }
                if (flag == false)
                {

                    Enemies_alive.Add(obj);

                }
                flag = false;
            }
        }
        
        
    }
    void RemoveFromList()
    {
        for (int i = 0; i < us.Count; i++)
        {
            us[i].GetComponent<unit_properties>().highlight.SetActive(false);

        }
        us.Clear();
    }
    void Formation(int i, Vector3 v, NavMeshAgent agent)
    {
        agent.stoppingDistance = 0;
        if (i == 0)
        {
            agent.SetDestination(v);
        }
        if (i == 1)
        {
            v = new Vector3(v.x + 2, v.y, v.z);
            agent.SetDestination(v);
        }
        if (i == 2)
        {
            v = new Vector3(v.x - 2, v.y, v.z);
            agent.SetDestination(v);
        }
        if (i == 3)
        {
            v = new Vector3(v.x, v.y, v.z + 2);
            agent.SetDestination(v);
        }
        if (i == 4)
        {
            v = new Vector3(v.x, v.y, v.z - 2);
            agent.SetDestination(v);
        }
        if (i == 5)
        {
            v = new Vector3(v.x + 2, v.y, v.z + 2);
            agent.SetDestination(v);
        }
        if (i == 6)
        {
            v = new Vector3(v.x + 2, v.y, v.z - 2);
            agent.SetDestination(v);
        }
        if (i == 7)
        {
            v = new Vector3(v.x - 2, v.y, v.z + 2);
            agent.SetDestination(v);
        }
        if (i == 8)
        {
            v = new Vector3(v.x - 2, v.y, v.z - 2);
            agent.SetDestination(v);
        }
        if (i == 9)
        {
            v = new Vector3(v.x, v.y, v.z + 4);
            agent.SetDestination(v);
        }
        if (i == 10)
        {
            v = new Vector3(v.x + 4, v.y, v.z);
            agent.SetDestination(v);
        }
        if (i == 11)
        {
            v = new Vector3(v.x + 4, v.y, v.z - 4);
            agent.SetDestination(v);
        }
        if (i == 12)
        {
            v = new Vector3(v.x - 4, v.y, v.z + 4);
            agent.SetDestination(v);
        }
        if (i == 13)
        {
            v = new Vector3(v.x + 2, v.y, v.z + 4);
            agent.SetDestination(v);
        }
        if (i == 14)
        {
            v = new Vector3(v.x - 2, v.y, v.z + 4);
            agent.SetDestination(v);
        }
        if (i == 15)
        {
            v = new Vector3(v.x + 2, v.y, v.z - 4);
            agent.SetDestination(v);
        }
        if (i == 16)
        {
            v = new Vector3(v.x - 2, v.y, v.z - 4);
            agent.SetDestination(v);
        }
        if (i == 17)
        {
            v = new Vector3(v.x - 4, v.y, v.z);
            agent.SetDestination(v);
        }
        if (i == 18)
        {
            v = new Vector3(v.x, v.y, v.z - 4);
            agent.SetDestination(v);
        }
        if (i == 19)
        {
            v = new Vector3(v.x + 4, v.y, v.z + 4);
            agent.SetDestination(v);
        }
        if (i == 20)
        {
            v = new Vector3(v.x - 4, v.y, v.z - 4);
            agent.SetDestination(v);
        }
        if (i == 21)
        {
            v = new Vector3(v.x + 4, v.y, v.z - 2);
            agent.SetDestination(v);
        }
        if (i == 22)
        {
            v = new Vector3(v.x + 4, v.y, v.z + 2);
            agent.SetDestination(v);
        }
        if (i == 23)
        {
            v = new Vector3(v.x - 4, v.y, v.z + 2);
            agent.SetDestination(v);
        }
        if (i == 24)
        {
            v = new Vector3(v.x - 4, v.y, v.z - 2);
            agent.SetDestination(v);
        }
    }
}
