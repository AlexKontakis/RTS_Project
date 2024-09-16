using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Secondary_CapturePoint : MonoBehaviour
{
    public unit_manager um;
    public float timer, Capture_range, Capture_Time;
    public bool flag;
    public int x,y; //x counts the number of selected units in range, y counts the ammount of enemy units in range//
    public GameObject captured;
    
    // Start is called before the first frame update
    void Start()
    {
        captured.SetActive(false);
        flag = false;
        timer = 0;
        foreach(var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(um.us.Count > 0)
        {
            x = 0;
            y = 0;
            for(int i = 0; i< um.us.Count; i++)
            {
                if((um.us[i].transform.position - transform.position).magnitude <= Capture_range)
                {
                    x++;
                    
                }
            }
            for(int i = 0; i< um.Enemies_alive.Count; i++)
            {
                if((um.Enemies_alive[i].transform.position - transform.position).magnitude <= Capture_range)
                {
                    y++;
                }
            }
            if(x > 0 && y == 0)
            {
                flag=true;
            }
            else
            {
                timer = 0;
                flag=false;
            }
            if(flag == true)
            {
                timer += Time.deltaTime;
            }
            if(timer >= Capture_Time)
            {
                timer = 0;
                flag = false;
               
                captured.SetActive(true);
                Destroy(transform.gameObject);
            }
        }

    }
}
