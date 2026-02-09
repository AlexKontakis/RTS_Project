using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Objective_Tracker : MonoBehaviour
{
    public bool Objective1,Objective2,Objective3,Objective4;
    public int Scenario = 0;
    public unit_manager um;
    public Gamemode_Manager gm;
    public int Obj1SpawnerCount,Obj1EnemiesCount;
    public int Obj3SpawnerCount,Obj3EnemiesCount;
    public int Obj4SpawnerCount,Obj4EnemiesCount;
    public float timer = 0;
    public GameObject CaptureObj;
    public float CaptureRadius;
    public List<GameObject> Defenders = new List<GameObject>(); //List to determine how many defenders of an objective are in range
    public List<GameObject> Attackers = new List<GameObject>(); //List to determine how many attackers of an objective are in range
    public bool test1;
    public float test2;
    private int t1 = 0;

    void Start(){

        foreach(var s in FindObjectsOfType<unit_manager>()){
            um = s;
        }
        foreach(var s in FindObjectsOfType<Gamemode_Manager>()){
            gm = s;
        }
        Objective1 = false;
        Objective2 = false;
        Objective3 = false;
        Objective4 = false;
        //Scenario = UnityEngine.Random.Range(1, 5);
        Scenario = 1;
    }

    void Update(){
        //Bandit Camp
        if(Scenario == 1){
            //Objective 1
            Obj1SpawnerCount = 0;
            Obj1EnemiesCount = 0;
            if(gm.spawners.Count > 0){
                for(int i = 0; i < gm.spawners.Count; i++)
                {
                    if(gm.spawners[i].GetComponent<spawner>().Type == "Objective1"){
                        gm.spawners[i].GetComponent<spawner>().Active = true;
                        Obj1SpawnerCount++;
                    }
                } 
            }
            if(um.Enemies_alive.Count > 0)
            {
                for(int i = 0; i < um.Enemies_alive.Count; i++){
                    if(um.Enemies_alive[i].GetComponent<unit_properties>().boss == false && um.Enemies_alive[i] != CaptureObj){
                        Obj1EnemiesCount++;
                    }
                }
            }
            if(Obj1SpawnerCount == 0 && Obj1EnemiesCount == 0 && um.Friendlies_alive.Count > 0){
                Objective1 = true;
            }
            //Objective 2
            foreach(var s in FindObjectsOfType<unit_properties>()){
                if(s.gameObject.GetComponent<unit_properties>().IsObjectiveOf == "Scenario1"){
                    CaptureObj = s.gameObject;
                }    
            }
            if(um.Friendlies_alive.Count > 0){ //check if there are friendlies near the capture objective
                for(int i = 0; i < um.Friendlies_alive.Count; i++){
                    if((um.Friendlies_alive[i].transform.position - CaptureObj.transform.position).magnitude <= CaptureRadius){
                        if(Attackers.Count > 0){
                            t1 = 0;
                            for(int j = 0; j < Attackers.Count; j++){
                                if(Attackers[j] == um.Friendlies_alive[i] || um.Friendlies_alive[i] == CaptureObj){
                                    t1++;
                                }
                                if(Attackers[j].GetComponent<unit_properties>().HP <= 0){
                                    Attackers.Remove(Attackers[j]);
                                }
                            }
                            if(t1 == 0 ){
                                Attackers.Add(um.Friendlies_alive[i]);
                            }
                        }
                        else{
                            Attackers.Add(um.Friendlies_alive[i]);
                        }                
                    }
                    else{
                        if(Attackers.Count > 0){
                            for(int j = 0; j < Attackers.Count; j++){
                                if(Attackers[j] == um.Friendlies_alive[i]){
                                    Attackers.Remove(um.Friendlies_alive[i]);
                                }
                            }
                        }    
                    }
                }
            }
            if(um.Enemies_alive.Count > 0){ //check if there are enemies near the capture objective
                for(int i = 0; i < um.Enemies_alive.Count; i++){
                    if((um.Enemies_alive[i].transform.position - CaptureObj.transform.position).magnitude <= CaptureRadius){
                        if(Defenders.Count > 0){
                            t1 = 0;
                            for(int j = 0; j < Defenders.Count; j++){
                                if(Defenders[j] == um.Enemies_alive[i] || um.Enemies_alive[i] == CaptureObj){
                                    t1++;
                                    
                                }
                                if(Defenders[j].GetComponent<unit_properties>().HP <= 0){
                                    Defenders.Remove(Defenders[j]);
                                }
                            }
                            if(t1 == 0){
                                Defenders.Add(um.Enemies_alive[i]);
                            }
                        }
                        else{
                            if(um.Enemies_alive[i] != CaptureObj){
                                Defenders.Add(um.Enemies_alive[i]);
                            }
                        }
                    }
                    else{
                         if(Defenders.Count > 0){
                            for(int j = 0; j < Defenders.Count; j++){
                                if(Defenders[j] == um.Enemies_alive[i]){
                                    Defenders.Remove(um.Enemies_alive[i]);
                                }
                            }
                        }
                    }
                }
            }
            if(Objective1 == true && Attackers.Count > 0 && Defenders.Count == 0 && Objective2 == false){
                timer += Time.deltaTime;
                if(timer >= 5){
                    timer = 0;
                    CaptureObj.GetComponent<unit_properties>().faction = "Friendly";
                    CaptureObj.GetComponent<unit_properties>().type = "Archer";
                    CaptureObj.GetComponent<unit_properties>().HP = CaptureObj.GetComponent<unit_properties>().MAX_HP;
                    CaptureObj.GetComponent<Attacking>().targets.Clear();
                    um.Enemies_alive.Remove(CaptureObj);
                    Objective2 = true;
                }
            }
            else if(Objective1 == true && Attackers.Count == 0){
                timer = 0;
            }
            //Objective 3
            if(Objective1 == true && Objective2 == true){
                Obj3SpawnerCount = 0;
                Obj3EnemiesCount = 0;
                if(gm.spawners.Count > 0){
                    for(int i = 0; i < gm.spawners.Count; i++)
                    {
                        if(gm.spawners[i].GetComponent<spawner>().Type == "Objective3"){
                            gm.spawners[i].GetComponent<spawner>().Active = true;
                            Obj3SpawnerCount++;
                        }
                    }
                }
                if(um.Enemies_alive.Count > 0){
                    for(int i = 0; i < um.Enemies_alive.Count; i++){
                        if(um.Enemies_alive[i].GetComponent<unit_properties>().boss == false){
                            um.Enemies_alive[i].GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(CaptureObj.transform.position);
                            Obj3EnemiesCount++;
                        }
                    }
                }
                
                
                if(Obj3SpawnerCount == 0 && Obj3EnemiesCount == 0 && um.Friendlies_alive.Count > 0){
                    Objective3 = true;
                }


                
                
            }
            //Objective 4
            if(Objective1 == true && Objective2 == true && Objective3 == true){

                Obj4SpawnerCount = 0;
                Obj4EnemiesCount = 0;
                if(gm.spawners.Count > 0){
                    for(int i = 0; i < gm.spawners.Count; i++)
                    {
                        if(gm.spawners[i].GetComponent<spawner>().Type == "Objective4"){
                            gm.spawners[i].GetComponent<spawner>().Active = true;
                            Obj4SpawnerCount++;
                        }
                    }
                    if(um.Enemies_alive.Count > 0)
                    {
                        for(int i = 0; i < um.Enemies_alive.Count; i++){
                            if(um.Enemies_alive[i] != CaptureObj){
                                Obj4EnemiesCount++;
                            }
                        }
                    }
                    
                    
                }
                if(um.Enemies_alive.Count > 0)
                {
                    for(int i = 0; i < um.Enemies_alive.Count; i++){
                        if(um.Enemies_alive[i] != CaptureObj){
                            Obj4EnemiesCount++;
                        }
                    }
                }
                if(Obj4SpawnerCount == 0 && Obj4EnemiesCount == 0){
                    Objective4 = true;
                }
            }
        }//TBA
        else if (Scenario == 2){

        }//TBA
        else if (Scenario == 3){

        }//TBA
        else{

        }
    }
}
