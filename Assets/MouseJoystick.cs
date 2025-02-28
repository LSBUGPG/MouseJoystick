using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MouseJoystick : MonoBehaviour
{
    class Filter
    {
        const float alpha = 0.98f;
        float previousValue;

        public float FilterValue(float value, float dt)
        {
            float newValue = Mathf.Lerp(previousValue, value, Mathf.Exp(-alpha * dt));
            previousValue = value;
            return newValue;
        }
    }

    PlayerInput input;
    Filter xFilter = new Filter();
    Filter yFilter = new Filter();

    List<float> data = new ();
    List<float> filtered = new ();
    List<float> time = new ();

    void Awake()
    {
        input = new PlayerInput();
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();

        using (StreamWriter writer = new StreamWriter("Output.csv"))
        {
            writer.WriteLine("Value, Filtered Value, Time");
            for (int i = 0; i < data.Count; i++)
            {
                writer.WriteLine($"{data[i]}, {filtered[i]}, {time[i]}");
            }
        }
    }

    void Update()
    {
        Vector2 aim = input.Player.Aim.ReadValue<Vector2>();
        float dt = Time.deltaTime;
        float x = Mathf.Clamp(xFilter.FilterValue(aim.x, dt), -1, 1);
        data.Add(aim.x);
        filtered.Add(x);
        time.Add(Time.time);
        float y = Mathf.Clamp(yFilter.FilterValue(aim.y, dt), -1, 1);
        transform.position = Vector3.right * x + Vector3.up * y;
    }
}
