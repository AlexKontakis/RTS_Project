using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Button_Functionality : MonoBehaviour
{
    public Camera cam;
    Vector3 mp;
    private RaycastHit hit1, hit2;
    public GameObject selected_building,panel;
    
    public Button u1, u2;
    public TMP_Text T1, T2;

    public unit_manager um;
    
   
    void Start()
    {
        panel.SetActive(false);
        u1.gameObject.SetActive(false);
        u2.gameObject.SetActive(false);
        T1.gameObject.SetActive(false);
        T2.gameObject.SetActive(false);

        foreach (var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selected_building = null;
        }
        if (Input.GetMouseButtonDown(0))
        {
            mp = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit1))
            {
                if (hit1.collider.gameObject.CompareTag("building"))
                {
                    selected_building = hit1.collider.gameObject;
                }
               
            }
        }
        if(selected_building != null)
        {
            if(selected_building.GetComponent<Spawner_Properties>().type == "Melee")
            {
                panel.SetActive(true);
                u2.gameObject.SetActive(false);
                T2.gameObject.SetActive(false);
                u1.gameObject.SetActive(true);
                T1.gameObject.SetActive(true);
                T1.SetText("Melee units training: " + selected_building.GetComponent<Spawner_Properties>().spawn_ammount);
            }
            else if(selected_building.GetComponent<Spawner_Properties>().type == "Archer")
            {
                panel.SetActive(true);
                u1.gameObject.SetActive(false);
                T1.gameObject.SetActive(false);
                u2.gameObject.SetActive(true);
                T2.gameObject.SetActive(true);
                T2.SetText("Ranged units training: " + selected_building.GetComponent<Spawner_Properties>().spawn_ammount);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit2))
                {
                    selected_building.GetComponent<Spawner_Properties>().spawn_destination.transform.position = hit2.point;
                }
            }
        }
        else
        {
            panel.SetActive(false);
            u1.gameObject.SetActive(false);
            u2.gameObject.SetActive(false);
            T1.gameObject.SetActive(false);
            T2.gameObject.SetActive(false);
        }

    }

    public void spawn()
    {
        um.RecheckFriendly();
        if(um.Friendlies_alive.Count < um.unit_lim) {
            if (selected_building.GetComponent<Spawner_Properties>().spawn_ammount < 10 && (Resource_Manager.Gold >= selected_building.GetComponent<Spawner_Properties>().spawn_priceG && Resource_Manager.Food >= selected_building.GetComponent<Spawner_Properties>().spawn_priceF && Resource_Manager.Materials >= selected_building.GetComponent<Spawner_Properties>().spawn_priceM))
            {
                Resource_Manager.Gold -= selected_building.GetComponent<Spawner_Properties>().spawn_priceG;
                Resource_Manager.Food -= selected_building.GetComponent<Spawner_Properties>().spawn_priceF;
                Resource_Manager.Materials -= selected_building.GetComponent<Spawner_Properties>().spawn_priceM;
                selected_building.GetComponent<Spawner_Properties>().spawn_ammount++;
            }
        }

    }
}
