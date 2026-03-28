using UnityEngine;

public class VerticalMovelLoop : MonoBehaviour
{
    public float amplitude = 2f;   // how high/low relative to initial position
    public float speed = 1f;       // speed of up/down movement

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // Mathf.PingPong returns a value that goes back and forth between 0 and amplitude
        // basically when value hit 2 (amplitude), return back to 0 and repeat
        float yOffset = Mathf.PingPong(Time.time * speed, amplitude * 2) - amplitude;
        transform.position = initialPosition + new Vector3(0, yOffset, 0);
    }
}