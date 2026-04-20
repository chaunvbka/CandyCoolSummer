#pragma warning disable IDE0130

using UnityEngine;

[CreateAssetMenu(fileName = "ApplicationSettings", menuName = "Texell/ApplicationSettings")]
public class ApplicationSettings : ScriptableObject
{
    public bool Active = true;
    public bool DebugMode = true;
    public bool ShowFPS = false;
}

