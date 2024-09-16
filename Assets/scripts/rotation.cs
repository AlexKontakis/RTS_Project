using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotation : MonoBehaviour
{
    public bool open;
    public float x,y = 0;
  
    // Start is called before the first frame update
    void Start()
    {
        open = false;
        x = -60;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            open = true;
        }
        if(open == true)
        {
           
            y = transform.rotation.x;
            if (y > -0.7)//idk why it was trial and error to get the value + some eye observation dont judge me future me
            {

                transform.Rotate(0, -(x*Time.deltaTime) , 0);
            }
           
           
        }
    }
}
