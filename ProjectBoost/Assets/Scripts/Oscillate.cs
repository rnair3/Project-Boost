using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillate : MonoBehaviour
{
    [SerializeField] Vector3 movement = new Vector3(10, 10, 10);
    [SerializeField] float period = 2f;

    float movementFactor;
    private Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(period <= Mathf.Epsilon)
        {
            return;
        }
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2f;

        float rawSine = Mathf.Sin(cycles * tau);
        float movementFactor = rawSine / 2f + 0.5f;

        Vector3 offset = movementFactor * movement;
        transform.position = startingPos + offset;
    }
}
