using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class PlayerController1 : MonoBehaviour
{
    public ShipStats ship;

    public Text currentSpeed;
    public Text currentHeading;
    public Text antiSlideActive;
    public Text antiTumbleActive;
    public Text accelerationMode;

    private bool antiTumble = true;
    private bool antiSlide = true;
    private bool spacebreak = false;
    private bool velocityTranslation = false;


    private Rigidbody rb;

    void Start()
    {
        Debug.Log("started");
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Debug.Log(Input.GetAxis("Jump"));
        TrackToggles();
    }

    void FixedUpdate()
    {
        RotateCraft();
        //AccelerateCraft();
        UpdateText();
    }

    private void TrackToggles()
    {
        if (Input.GetButtonDown("antiDrift")) antiTumble = !antiTumble;
        if (Input.GetButtonDown("antiSlide")) antiSlide = !antiSlide;
        if (Input.GetButtonDown("velocityControl")) velocityTranslation = !velocityTranslation;

        if (Input.GetButton("spacebreak")) spacebreak = true;
        else spacebreak = false;
    }


    private void UpdateText()
    {
        Vector3 lspd = transform.InverseTransformVector(rb.velocity);
        currentSpeed.text = "Speed: " + rb.velocity.magnitude + " x:" + lspd.x + " y:" + lspd.y + " z:" + lspd.z;
        currentHeading.text = "Heading: " + rb.velocity.normalized.ToString ();
        antiSlideActive.text = "AntiSlide: " + antiSlide;
        antiTumbleActive.text = "AntiTumble: " + antiTumble;
        if(velocityTranslation) { accelerationMode.text = "Velocity mode"; }
        else { accelerationMode.text = "Acceleration mode"; }
        
    }

  
    void RotateCraft()
    {
        Vector3 rotation = transform.InverseTransformDirection(rb.angularVelocity);

        if (Math.Abs(Input.GetAxis("Yaw")) > .01)
        {
            rb.AddRelativeTorque(Vector3.up * ship.yaw * Input.GetAxis("Yaw"));
            //torqueDir = torqueDir  +  Vector3.up * yaw * Input.GetAxis("Horizontal") + Vector3.left * pitch * Input.GetAxis("Vertical");

        } else if (spacebreak || antiTumble)
        {
            if (rotation.y <= .1 && rotation.y >= -0.1)
            {
                rotation.y = 0;
                Debug.Log("cancel y");
            }
            else if (rotation.y > .1)
            {
                rb.AddRelativeTorque(Vector3.up * ship.yaw * -1);
            }
            else if (rotation.y < -0.1)
            {
                rb.AddRelativeTorque(Vector3.up * ship.yaw);
            }
        }

        if (Math.Abs(Input.GetAxis("Roll")) > .01)
        {
            rb.AddRelativeTorque(Vector3.forward * -ship.roll * Input.GetAxis("Roll"));

        }
        else if (spacebreak || antiTumble)
        {
            if (rotation.z <= .1 && rotation.z >= -0.1)
            {
                rotation.z = 0;
                Debug.Log("cancel y");
            }
            else if (rotation.z > .1)
            {
                rb.AddRelativeTorque(Vector3.forward * ship.roll * -1);
            }
            else if (rotation.z < -0.1)
            {
                rb.AddRelativeTorque(Vector3.forward * ship.roll);
            }
        }


        if (Math.Abs(Input.GetAxis("Pitch")) > .01)
        {
            rb.AddRelativeTorque(Vector3.right * -ship.pitch * Input.GetAxis("Pitch"));
        }
        else if (spacebreak || antiTumble)
        {
            if (rotation.x <= .1 && rotation.x >= -0.1)
            {
                rotation.x = 0;
                Debug.Log("cancel x");
            }
            else if (rotation.x > .1)
            {
                rb.AddRelativeTorque(Vector3.right * ship.yaw * -1);
            }
            else if (rotation.x < -0.1)
            {
                rb.AddRelativeTorque(Vector3.right * ship.yaw);
            }
        }

        rb.angularVelocity = transform.TransformVector(rotation);
    }

    void AccelerateCraft()
    {
        Vector3 translation = transform.InverseTransformVector(rb.velocity);
        bool powerslide = false;
        if (velocityTranslation) velocityAcceleration(ref translation, ref powerslide);
        else thrustAcceleration(ref translation, ref powerslide);

        if (spacebreak || antiSlide || powerslide)
        {
            if (translation.y < .1 && translation.y > -.1)
            {
                translation.y = 0;
                rb.velocity = transform.TransformVector(translation);
            }
            else if (translation.y <= -0.1) rb.AddRelativeForce(Vector3.up * ship.acceleration);
            else if (translation.y >=  0.1) rb.AddRelativeForce(Vector3.down * ship.acceleration);

            if (translation.x < .1 && translation.x > -.1)
            {
                translation.x = 0;
                rb.velocity = transform.TransformVector(translation);
            }
            else if (translation.x <= -0.1) rb.AddRelativeForce(Vector3.right * ship.acceleration);
            else if (translation.x >=  0.1) rb.AddRelativeForce(Vector3.left * ship.acceleration);
        }

        rb.velocity = transform.TransformVector(translation);
    }

    private void velocityAcceleration(ref Vector3 translation, ref bool powerslide)
    {
        float speedReqest = Input.GetAxis("Jump") * ship.maxSpeed;
        if (translation.z > speedReqest)
        {
            rb.AddRelativeForce(Vector3.back * ship.acceleration);
        }
        else 
        {

            rb.AddRelativeForce(Vector3.forward * ship.acceleration);
        }
        powerslide = true;
    }

    private void thrustAcceleration(ref Vector3 translation, ref bool powerslide)
    {
        if (Math.Abs(Input.GetAxis("Jump")) > .1)
        {
            if (translation.z >= ship.maxSpeed)
            {
                Debug.Log("halt accel");
            }
            else if (translation.magnitude >= ship.maxSpeed)
            {
                powerslide = true;
                rb.AddRelativeForce(Input.GetAxis("Jump") * ship.acceleration * Vector3.forward);
            }
            else { rb.AddRelativeForce(Input.GetAxis("Jump") * ship.acceleration * Vector3.forward); }

        }
        else if (spacebreak || antiSlide)
        {

            if (translation.z < .1 && translation.z > -.1)
            {
                translation.z = 0;

            }
            else if (translation.z <= -0.1) rb.AddRelativeForce(Vector3.forward * ship.acceleration);
            else if (translation.z >= .1 && spacebreak) rb.AddRelativeForce(Vector3.back * ship.acceleration);
        }
    }
}