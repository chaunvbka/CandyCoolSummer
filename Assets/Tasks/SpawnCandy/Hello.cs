using UnityEngine;

public class Hello
{
    private bool _dispose = false;
    private static Hello s_Instance;
    public static Hello Instance => s_Instance;

    public Hello()
    {
        if (s_Instance != null)
        {
            Debug.LogError("Hello instance already exists. Cannot create a new one.");
            return;
        }
        s_Instance = this;
    }

    public void Dispose()
    {
        if (_dispose) return;
        _dispose = true;

        s_Instance = null;
    }
}
