using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PidSettings : MonoBehaviour
{

    public PID xPid = new PID(100.0f, 0, 0, -1, 1);
    public PID yPid = new PID(100.0f, 0, 0, -1, 1);
    public PID zPid = new PID(100.0f, 0, 0, -1, 1);

    public PID throttlePid = new PID(100.0f, 0, 0, -1, 1);

}
