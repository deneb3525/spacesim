using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class PlayerController1 : MonoBehaviour
{
    public ShipStats ship;
    public PidSettings pidSettings;
    
    bool capRotation = true;
    bool accelRotation = false;
    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;





    private Rigidbody rb;

    void Start()
    {
        Debug.Log("started");
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 9;
    }

    private void Update()
    {
        TrackToggles();
        UpdateText();
    }

    void FixedUpdate()
    {
        RotateCraft();
        AccelCraft();
        UpdateText();
    }

    private void TrackToggles()
    {
        if (Input.GetButtonDown("capRotation")) capRotation = !capRotation;
        if (Input.GetButtonDown("accelRotation")) accelRotation = !accelRotation;
     }


    private void UpdateText()
    {
        Vector3 velocity = transform.InverseTransformDirection(rb.velocity).normalized;
        text1.text = rb.angularVelocity.ToString();
        Vector3 v3 = new Vector3(-velocity.x, -velocity.y, 0) * ship.rudderStr;
        text2.text = v3.ToString();
        
        text3.text = transform.InverseTransformDirection(rb.velocity).normalized.ToString();
        text4.text = rb.velocity.ToString();

    }

    void AccelCraft()
    {
        // apply acceleration
        rb.AddRelativeForce(Vector3.forward * Input.GetAxis("thrust") * ship.thrust);

        // apply rudder
        Vector3 velocity = transform.InverseTransformDirection(rb.velocity).normalized;
        rb.AddRelativeForce(new Vector3(-velocity.x, -velocity.y, 0) * ship.rudderStr);
        rb.AddRelativeForce(new Vector3(0, 0, Mathf.Sqrt(Mathf.Pow(velocity.x,  2) + Mathf.Pow(velocity.y , 2)) * ship.rudderStr));
        // apply drag ??
    }


    void RotateCraft()
    {

        if (accelRotation)
        {
            AccelPitch();
            AccelYaw();
        } else
        {
            VelocPitch();
            VelocYaw();
            VelocRoll();
        }
        

        
    }

    void AccelPitch()
    {

        Vector3 rotation = transform.InverseTransformDirection(rb.angularVelocity);


        float pitchTarget = Input.GetAxis("Pitch");
        float pitchAcceleration;

        if (pitchTarget > 0 && -rotation.x >= ship.pitchMax && capRotation)
        {
            pitchAcceleration = 0;
        }
        else if (pitchTarget < 0 && (rotation.x >= ship.pitchMax) && capRotation)
        {
            pitchAcceleration = 0;
        }
        else
        {
            pitchAcceleration = pitchTarget;
        }


        rb.AddRelativeTorque(Vector3.left * ship.pitchAccel * pitchAcceleration);
    }

    void AccelYaw()
    {

        Vector3 rotation = transform.InverseTransformDirection(rb.angularVelocity);


        float yawTarget = Input.GetAxis("Yaw");
        float yawAcceleration;

        if (yawTarget > 0 && -rotation.x >= ship.yawMax && capRotation)
        {
            yawAcceleration = 0;
        }
        else if (yawTarget < 0 && (rotation.x >= ship.yawMax) && capRotation)
        {
            yawAcceleration = 0;
        }
        else
        {
            yawAcceleration = yawTarget;
        }


        rb.AddRelativeTorque(Vector3.up * ship.yawAccel * yawAcceleration);
    }





    void VelocYaw()
    {
        Vector3 rotation = transform.InverseTransformDirection(rb.angularVelocity);

        float yawTarget = Input.GetAxis("Yaw");// * ship.maxRotation;
        float yawPresent = rotation.y / ship.yawMax;
        float delta = pidSettings.yPid.Update(yawTarget, yawPresent, Time.deltaTime);

        if (delta > 1) delta = 1; else if (delta < -1) delta = -1;

        rb.AddRelativeTorque(Vector3.up * delta * ship.yawAccel);
    }

    void VelocPitch()
    {
        Vector3 rotation = transform.InverseTransformDirection(rb.angularVelocity);

        float pitchTarget = -Input.GetAxis("Pitch");// * ship.maxRotation; -1 to +1
        float pitchPresent = rotation.x / ship.pitchMax; // -1 to +1
        float delta = pidSettings.xPid.Update(pitchTarget, pitchPresent, Time.deltaTime);

        //if (delta > 1) delta = 1; else if (delta < -1) delta = -1;

        text4.text = "pid out" + delta + "   target" + pitchTarget + "    present" + pitchPresent + "   deltatime" + Time.deltaTime;
        rb.AddRelativeTorque(Vector3.left * -delta * ship.pitchAccel * Time.deltaTime);
    }

    void VelocRoll()
    {
        Vector3 rotation = transform.InverseTransformDirection(rb.angularVelocity);

        float rollTarget = -Input.GetAxis("Roll");// * ship.maxRotation;
        float rollPresent = rotation.z / ship.rollMax;
        float delta = pidSettings.zPid.Update(rollTarget, rollPresent, Time.deltaTime);

        if (delta > 1) delta = 1; else if (delta < -1) delta = -1;

        text4.text = "pid out" + delta + "   target" + rollTarget + "    present" + rollPresent + "   deltatime" + Time.deltaTime;
        rb.AddRelativeTorque(Vector3.back * -delta * ship.rollAccel);
    }

}