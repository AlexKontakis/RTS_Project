using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggle_cam_controller : MonoBehaviour
{
    public int i;
    public bool cam_enabled = true;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            i++;
            if(i == 1)
            {
                transform.GetComponent<cam_manager>().enabled = false;
                cam_enabled = false;
            }
            else
            {
                transform.GetComponent<cam_manager>().enabled = true;
                cam_enabled = true;
                i = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Resource_Manager.Gold += 500;
            Resource_Manager.Food += 500;
            Resource_Manager.Materials += 500;
        }
        
    }
}
