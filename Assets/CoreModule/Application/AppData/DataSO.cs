#pragma warning disable IDE0130


using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Texell/AppData/Data", order = 1)]
public class Data : ScriptableObject
{
    public string AppName;
    public string[] Arrays;
}
