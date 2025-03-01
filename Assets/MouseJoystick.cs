using UnityEngine;
using UnityEngine.InputSystem;

public class MouseJoystick : MonoBehaviour
{
    public float smoothTime = 1;
    public float sensitivity = 1;

    PlayerInput input;
    InputBinding mouseMask;
    Vector2 vel;
    Vector2 prev;

    void Awake()
    {
        input = new PlayerInput();
        mouseMask = new InputBinding { path = "<Mouse>/delta" };
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.Aim.ApplyParameterOverride("DeltaPerSecond:sensitivity", sensitivity, mouseMask);
    }

    void OnDisable()
    {
        input.Disable();
    }

    // https://powered-up-games.com/blog/wordpress/archives/404
    static float SmoothDamp(float from, float to, ref float vel, float smoothTime, float deltaTime)
    {
        float omega = 2f / smoothTime;
        float x = omega * deltaTime;
        float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
        float change = from - to;
        float temp = (vel + omega * change) * deltaTime;
        vel = (vel - omega * temp) * exp; // Equation 5
        return to + (change + temp) * exp; // Equation 4
    }

    static Vector2 SmoothDamp(Vector2 from, Vector2 to, ref Vector2 vel, float smoothTime, float deltaTime)
    {
        return new Vector2(
            SmoothDamp(from.x, to.x, ref vel.x, smoothTime, deltaTime),
            SmoothDamp(from.y, to.y, ref vel.y, smoothTime, deltaTime)
        );
    }

    void Update()
    {
        Vector2 aim = input.Player.Aim.ReadValue<Vector2>();
        float dt = Time.deltaTime;
        Vector2 smoothed = SmoothDamp(prev, aim, ref vel, smoothTime, dt);
        if (smoothed.sqrMagnitude > 1)
        {
            smoothed.Normalize();
        }
        prev = smoothed;
        transform.position = Vector3.right * smoothed.x + Vector3.up * smoothed.y;
    }
}
