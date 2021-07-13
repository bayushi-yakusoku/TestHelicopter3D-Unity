using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory
{
    public Trajectory(Vector3 origin, Vector3 target, Vector3 gravity, float hight = 6, float nbDots = 20)
    {
        Origin = origin;
        Target = target;
        Gravity = gravity;
        Hight = hight;
        NbDots = nbDots;
    }

    public Trajectory(Vector3 origin, Vector3 target, float high = 6, float nbDots = 20)
        : this(origin, target, Physics.gravity, high, nbDots) { }

    Vector3 origin;
    public Vector3 Origin
    {
        get => origin;
        set
        {
            origin = value;
            UpdateTrajectoryData();
        }
    }


    Vector3 target;
    public Vector3 Target
    {
        get => target;
        set
        {
            target = value;
            UpdateTrajectoryData();
        }
    }

    Vector3 gravity;
    public Vector3 Gravity
    {
        get => gravity;
        set
        {
            gravity = value;
            UpdateTrajectoryData();
        }
    }


    float hight;
    public float Hight
    {
        get => hight;
        set
        {
            hight = value;
            UpdateTrajectoryData();
        }
    }

    public float NbDots { get; set; }

    // Read Only:
    Vector3 velocity;
    public Vector3 Velocity
    {
        get => velocity;
        private set => velocity = value;
    }

    // Read Only:
    float timeToTarget;
    public float TimeToTarget
    {
        get => timeToTarget;
        private set => timeToTarget = value;
    }

    /*
     * Using SUVAT Equations:
     *  1) S = U * T + (A * T²) / 2 -> On the vertical axis Uy = Sqrt(-2 * Ay * Sy)
     *  
     *  2) On the others axis (without acceleration), we have:
     *      Ux,z = Sx,z / (sqrt(-2 * Sy / Ay) + sqrt(2 * (H - Sy) / Ay))
     *      
     *  Better explain here:
     *  https://www.youtube.com/watch?v=IvT8hjy6q4o
     */
    void UpdateTrajectoryData()
    {
        float targetHight = Target.y - Origin.y;

        Vector3 planXZ = new Vector3(
            Target.x - Origin.x,
            0,
            Target.z - Origin.z);

        float trajectorySummit = Target.y + Hight;

        float velocityY = Mathf.Sqrt(-2 * Gravity.y * trajectorySummit);

        timeToTarget = Mathf.Sqrt(-2 * trajectorySummit / Gravity.y) + 
            Mathf.Sqrt(2 * (targetHight - trajectorySummit) / Gravity.y);

        planXZ /= TimeToTarget;

        velocity = new Vector3(planXZ.x, velocityY, planXZ.z);

        Debug.Log($"Velocity: {Velocity} Time: {TimeToTarget}");
    }

    /*
     * Using SUVAT Equation:
     *  S = U * T + (A * T²) / 2
     */
    public void DrawTrajectory()
    {
        float deltaTime = timeToTarget / (float)NbDots;

        Vector3 previousDot = Origin;

        for (int index = 1; index <= NbDots; index++)
        {
            float time = index * deltaTime;

            Vector3 nextDot = Origin + (velocity * time +
                time * time * Gravity / 2);

            Debug.DrawLine(previousDot, nextDot);

            previousDot = nextDot;
        }
    }
}
