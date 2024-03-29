﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidbody;
    AudioSource audio;
    bool collisionEnabled = true;

    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 50f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    [SerializeField] float levelLoadDelay = 2f;

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
        if (Debug.isDebugBuild)
        {
            RespondToDebugKey();
        }
        
    }

    private void RespondToDebugKey()
    {
        if (Input.GetKeyDown("l"))
        {
            LoadNextScene();
        }else if (Input.GetKeyDown("c"))
        {
            collisionEnabled = !collisionEnabled;
        }
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
            rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
            if (!audio.isPlaying)
                audio.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
        else
        {
            audio.Stop();
            mainEngineParticles.Stop();
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
        if(currentState != State.Alive || !collisionEnabled)
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
                audio.Stop();
                audio.PlayOneShot(success);
                successParticles.Play();
                Invoke("LoadNextScene", levelLoadDelay);
                break;
            default:
                currentState = State.Dying;
                audio.Stop();
                audio.PlayOneShot(death);
                deathParticles.Play();
                Invoke("Dying", levelLoadDelay);
                break;
        }
    }

    private void Dying()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex+1)%SceneManager.sceneCountInBuildSettings);
    }
}
