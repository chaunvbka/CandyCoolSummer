using System.Collections.Generic;
using Texell.CandyCoolSummer;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnCandy : MonoBehaviour
{
    Hello hello1;
    Hello hello2 = Hello.Instance;

    void Start()
    {
        hello1 = new();
    }

    void OnDestroy()
    {
        hello1.Dispose();
        hello1 = null;
    }


}
