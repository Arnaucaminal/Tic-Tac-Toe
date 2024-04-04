using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundItself : MonoBehaviour
{
    public float rotateSpeed; // Velocidad de rotación en grados por segundo
    private bool hasToRotate = false;
    private float timer_rotating = 0;
    private float timer_waiting = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasToRotate == true)
        {
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
            timer_rotating += Time.deltaTime;
            if (timer_rotating > 2.0f)
            {
                hasToRotate = false;
                timer_waiting = 0.0f;
            }
        }
        else
        {
            timer_waiting += Time.deltaTime;
            if (timer_waiting > 1.0f)
            {
                hasToRotate = true;
                timer_rotating = 0.0f;
            }
        }
    }
}
