using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile1 : MonoBehaviour
{
    public float _InitialVelocity;
    public float _Angle;
    public Transform _FirePoint;
    public float _Height;
    private Camera cam;
    public List<GameObject> shooters = new List<GameObject>();
    public GameObject target;
    public Vector3 target_pos;
    public float dis;
    public float time_update;

    private void Start()
    {
        foreach(var s in FindObjectsOfType<Archer_fire>())
        {
            shooters.Add(s.gameObject);
        }
        float min = 0;
        int minshooter = 0;
        for(int i = 0; i < shooters.Count; i++)
        {
            if(i == 0)
            {
                min = (transform.position - shooters[i].transform.position).magnitude;
                minshooter = i;
              
            }
            else
            {
                if((transform.position - shooters[i].transform.position).magnitude < min)
                {
                    min = (transform.position - shooters[i].transform.position).magnitude;
                    minshooter = i;
                }
            }
        }
        _FirePoint = shooters[minshooter].transform;
       
        target_pos = target.transform.position;
        Vector3 direction = target_pos - _FirePoint.position;
        Vector3 groundDirection = new Vector3(direction.x, 0, direction.z);
        dis = (transform.position - target_pos).magnitude;
        Vector3 targetPos = new Vector3(groundDirection.magnitude, direction.y, 0);
        targetPos.z = 0;
        float height = targetPos.y + targetPos.magnitude / 2f;
        height = Mathf.Max(0.01f, height);
        float angle;
        float v0;
        float time;
        transform.Rotate(transform.rotation.x + 100, transform.rotation.y, transform.rotation.z);
        CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);
        StopAllCoroutines();
        StartCoroutine(Coroutine_Movement(groundDirection.normalized, v0, angle, time));
        
        
    }
    private void Update()
    {
        //transform.LookAt(target_pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - target_pos),3* Time.deltaTime);
        


    }
    private void CalculatePath(Vector3 targetPos,float angle,out float v0,out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float v1 = Mathf.Pow(xt, 2) * g;
        float v2 = 2 * xt * Mathf.Sin(angle) * Mathf.Cos(angle);
        float v3 = 2 * yt * Mathf.Pow(Mathf.Cos(angle), 2);
        v0 = Mathf.Sqrt(v1 / (v2 - v3));
        time = xt / (v0 * Mathf.Cos(angle));
       
    }

    private float QuadraticEquation(float a, float b, float c, float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }
    private void CalculatePathWithHeight(Vector3 targetPos,float h, out float v0, out float angle, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float b = Mathf.Sqrt(2 * g * h);
        float a = (-0.5f * g);
        float c = -yt;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;
        time_update = time;
        angle = Mathf.Atan(b * time / xt);

        v0 = b / Mathf.Sin(angle);
    }
    IEnumerator Coroutine_Movement(Vector3 direction,float v0,float angle,float time)
    {
        float t = 0; // time
        while (t < time)
        {
            float x = v0 * t * Mathf.Cos(angle);
            float y = v0 * t * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(t, 2f);
            transform.position = _FirePoint.position + direction * x + Vector3.up * y;
            t += Time.deltaTime;
            yield return null;
        }
    }
   
}
