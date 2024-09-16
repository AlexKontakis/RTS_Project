using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Resource_Manager : MonoBehaviour
{
    public static int Gold, Food, Materials, Reputation, Population;
    public TMP_Text G, F, M, R, P;

    public unit_manager um;

    void Start()
    {
       foreach( var s in FindObjectsOfType<unit_manager>())
        {
            um = s;
        }
    }


    void Update()
    {
        Population = um.Friendlies_alive.Count;

        G.text = "Gold: " + Gold.ToString();
        F.text = "Food: " + Food.ToString();
        M.text = "Materials: " + Materials.ToString();
        R.text = "Reputarion: " + Reputation.ToString();
        P.text = "Population: " + Population.ToString();

    }
}
