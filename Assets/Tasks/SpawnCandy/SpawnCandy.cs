using System.Collections.Generic;
using Texell.CandyCoolSummer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class SpawnCandy : MonoBehaviour
{

    public InputAction mouseAction;
    readonly List<int> list1 = new() { 1, 2, 3, 4 };
    List<int> List1 => list1;
    List<int> list2;


    void Start()
    {
        list2 = List1;
        list2.Remove(2);
        list2.Add(8);
        foreach (var i in list2)
        {
            Debug.Log("List2==i: " + i);
        }

        foreach (var i in list1)
        {
            Debug.Log("List1==i: " + i);
        }
    }

    void Update()
    {

    }

    void OnDestroy()
    {

    }


}
