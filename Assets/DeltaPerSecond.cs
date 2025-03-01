using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class DeltaPerSecond : InputProcessor<Vector2>
{
    #if UNITY_EDITOR
    static DeltaPerSecond()
    {
        Initialize();
    }
    #endif

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<DeltaPerSecond>();
    }

    [Tooltip("Number to scale delta pixels per second.")]
    public float sensitivity = 0.001f;

    public override Vector2 Process(Vector2 value, InputControl control)
    {
        float dt = Time.unscaledDeltaTime;
        if (dt > 0)
        {
            float scale = sensitivity / dt;
            value *= scale;
        }
        return value;
    }
}
