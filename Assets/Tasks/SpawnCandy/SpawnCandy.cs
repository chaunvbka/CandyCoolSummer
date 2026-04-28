using System.Collections.Generic;
using Texell.CandyCoolSummer;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnCandy : MonoBehaviour
{

    public AnimationCurve myCurve;

    void Start()
    {

    }

    void Update()
    {
        // Get the y-value of the curve at the current time
        float curveValue = myCurve.Evaluate(Time.time);

        // Use that value for logic (e.g., setting an object's height)
        transform.position = new Vector3(transform.position.x, curveValue, transform.position.z);
    }

    void OnDestroy()
    {

    }


}
