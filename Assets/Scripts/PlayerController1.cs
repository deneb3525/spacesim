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
    bool vJoyEnabled = false;
    public Text speedText;
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
        if (Input.GetButtonDown("enableVJoy")) vJoyEnabled = !vJoyEnabled;
     }


    private void UpdateText()
    {
        Vector3 velocity = transform.InverseTransformDirection(rb.velocity).normalized;
        speedText.text = "Speed: " + rb.velocity.magnitude;
        Vector3 v3 = new Vector3(-velocity.x, -velocity.y, 0) * ship.rudderStr;
        //text2.text = "pitch:" + Input.GetAxis("Pitch") + " yaw:" + Input.GetAxis("Yaw") + "roll: " + Input.GetAxis("Roll") + " thrust:" + Input.GetAxis("thrust");


        //text3.text = transform.InverseTransformDirection(rb.velocity).normalized.ToString();
        //text4.text = rb.velocity.ToString();

    }

    void AccelCraft()
    {
        // apply acceleration
        rb.AddRelativeForce(Vector3.forward * Input.GetAxis("thrust") * ship.thrust * Time.deltaTime);
        
        // rudder v3
        if (Input.GetAxis("thrust") >= 0)
        {
            Vector3 velocity = transform.InverseTransformDirection(rb.velocity).normalized;
            float angle = Vector3.Angle(Vector3.forward, velocity);


            text4.text = "angle: " + angle;
            if (angle > .1f)
            {
                String velocString = velocity.ToString();
                if (velocity.z < .0001)
                {
                    velocity.z = .0001f;
                }
                float planeIntercept = velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z;
                float zIntercept = (planeIntercept / velocity.z) *.999f;
                float deadband;
                if(angle < 5)
                {
                    deadband = angle / 5;
                } else
                {
                    deadband = 1;
                }

                Vector3 rudderVector = (new Vector3(0, 0, zIntercept) - velocity).normalized;

                text3.text = "veloc: " + velocString + " adjveloc: " + velocity + " cvloc: " + rudderVector + " angle: " + angle + "magnatude: " + (ship.rudderStr * Time.deltaTime * (100 + angle * 5) / 100 * deadband);

                rb.AddRelativeForce(rudderVector.normalized * ship.rudderStr * Time.deltaTime*(100 + angle*5)/100 *deadband);
            }
        }
        
        /*
        // rudder v4

        Vector3 velocity = transform.InverseTransformDirection(rb.velocity);
        float angle = Vector3.Angle(velocity, Vector3.forward);
        text3.text = " angle: " + angle;
        text4.text = "velocty: " + velocity.ToString();
        if (angle < 90+1)
        {
            rb.velocity = Vector3.RotateTowards(rb.velocity, transform.TransformDirection(Vector3.forward), 1 * Time.deltaTime * (angle +10)/1000 * ship.maxroll, 0);
        } else
        {
            Vector3 midpointTarget = new Vector3(velocity.x, velocity.y, 0);
            rb.velocity = Vector3.RotateTowards(rb.velocity, transform.TransformDirection(midpointTarget), 1 * Time.deltaTime * (angle + 10) / 100 * ship.maxroll, 100);
        }
     */
        
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

        float yawTarget = Input.GetAxis("Yaw"); // * ship.maxRotation;
        float yawPresent = rotation.y / ship.yawMax;
        float delta = pidSettings.yPid.Update(yawTarget, yawPresent, Time.deltaTime);

        rb.AddRelativeTorque(Vector3.up * delta * ship.yawAccel * Time.deltaTime);
    }

    void VelocPitch()
    {
        Vector3 rotation = transform.InverseTransformDirection(rb.angularVelocity);

        float pitchTarget = -Input.GetAxis("Pitch");// * ship.maxRotation; -1 to +1
        float pitchPresent = rotation.x / ship.pitchMax; // -1 to +1
        float delta = pidSettings.xPid.Update(pitchTarget, pitchPresent, Time.deltaTime);
        text2.text = "deltapitch: " + delta + "rtq" + (Vector3.left * -delta * ship.pitchAccel * Time.deltaTime).ToString() ;
       // if(delta == 0 && Input.GetAxis("Pitch") == 0)
        {
         //   rb.
        }
        rb.AddRelativeTorque(Vector3.left * -delta * ship.pitchAccel * Time.deltaTime);
    }

    void VelocRoll()
    {
        Vector3 rotation = transform.InverseTransformDirection(rb.angularVelocity);

        float rollTarget = -Input.GetAxis("Roll");// * ship.maxRotation;
        float rollPresent = rotation.z / ship.rollMax;
        float delta = pidSettings.zPid.Update(rollTarget, rollPresent, Time.deltaTime);

        rb.AddRelativeTorque(Vector3.back * -delta * ship.rollAccel);


    }

}