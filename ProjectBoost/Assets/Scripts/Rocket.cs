using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidbody;
    AudioSource audio;

    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 50f;

    enum State { Alive, Dying, Transcending};
    State currentState = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();        
    }

    private void ProcessInput()
    {
        if(currentState == State.Alive)
        {
            Thrust();
            RotateShip();
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidbody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audio.isPlaying)
                audio.Play();
        }
        else
        {
            audio.Stop();
        }
    }

    private void RotateShip()
    {
        rigidbody.freezeRotation = true;    //take manual control of rotation
        float rotation = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotation);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotation);
        }
        rigidbody.freezeRotation = false;   //resume physics control of rotation
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(currentState != State.Alive)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Fuel":
                break;
            case "Finish":
                currentState = State.Transcending;
                Invoke("LoadNextScene", 1f);
                break;
            default:
                currentState = State.Dying;
                Invoke("Dying", 1f);
                break;
        }
    }

    private static void Dying()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
