using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breach_order : MonoBehaviour
{
    private RaycastHit hit;
    public Camera cam;
    private Vector3 mp;
    public unit_manager um;
    public GameObject obstacle;

    public int t;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var s in FindObjectsOfType<unit_manager>())
        {
            um = s.gameObject.GetComponent<unit_manager>();
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)){
            mp = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Obstacle"))
                {
                    obstacle = hit.collider.gameObject;
                    if (um.us.Count > 0)
                    {
                        for(int i = 0; i < um.us.Count; i++)
                        {
                            um.us[i].GetComponent<Attacking>().breach = true;

                            um.us[i].GetComponent<Attacking>().breach_target = obstacle;


                        }
                    }                    
                }
                else
                {
                    if (um.us.Count > 0)
                    {
                        for (int i = 0; i < um.us.Count; i++)
                        {
                            um.us[i].GetComponent<Attacking>().breach = false;
                            um.us[i].GetComponent<Attacking>().attacking = false;
                        }
                    }
                    obstacle = null;
                }
            }
        }
    }

    
}
