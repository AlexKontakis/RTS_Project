using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam_manager : MonoBehaviour
{
    public GameObject cam;
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    private RaycastHit hit;
    public Camera cam2;
    public bool freeze_zoom;
    public float test = 0;

    private void Start()
    {
        freeze_zoom = false;
    }



    void Update()
    {
        Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
        if (Physics.Raycast(ray, out hit))
        {
            test = (transform.position - hit.point).magnitude;
            /*if ((transform.position - hit.point).magnitude <= 40)
            {
                freeze_zoom = true;
            }
            else
            {
                freeze_zoom = false;
            }*/
        }


        Vector3 pos = transform.position;

        if ((Input.mousePosition.y >= Screen.height - panBorderThickness))//dexia
        {
            pos.x += (panSpeed * Time.deltaTime) * 4;
        }
        if ((Input.mousePosition.y <= panBorderThickness))//aristera
        {
            pos.x -= (panSpeed * Time.deltaTime) * 4;
        }
        if ((Input.mousePosition.x >= Screen.width - panBorderThickness))//pisw
        {
            pos.z -= (panSpeed * Time.deltaTime) * 4;
        }
        if ((Input.mousePosition.x <= panBorderThickness))//mpros
        {
            pos.z += (panSpeed * Time.deltaTime) * 4;
        }
        if ((Input.GetAxis("Mouse ScrollWheel") > 0f) && freeze_zoom == false)//zoom in
        {
            pos.y -= 20;
            pos.x += 20;
        }
        if ((Input.GetAxis("Mouse ScrollWheel") < 0f))//zoom out
        {
            pos.y += 20;
            pos.x -= 20;
        }


        cam.transform.position = pos;
    }
}
