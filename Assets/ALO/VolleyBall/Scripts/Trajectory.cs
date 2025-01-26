using System.Collections.Generic;
using UnityEngine;

public class Trajectory {
    public Trajectory(Vector3 origin, Vector3 target, Vector3 gravity, float hight = 6, float nbDots = 20) {
        Origin = origin;
        Target = target;
        Gravity = gravity;
        Hight = hight;
        NbDots = nbDots;

        invalidated = true;
    }

    public Trajectory(Vector3 origin, Vector3 target, float high = 6, float nbDots = 20)
        : this(origin, target, Physics.gravity, high, nbDots) { }

    Vector3 origin;
    public Vector3 Origin {
        get => origin;
        set {
            if (origin == value)
                return;

            origin = value;
            invalidated = true;
        }
    }


    Vector3 target;
    public Vector3 Target {
        get => target;
        set {
            target = value;
            invalidated = true;
        }
    }

    Vector3 gravity;
    public Vector3 Gravity {
        get => gravity;
        set {
            gravity = value;
            invalidated = true;
        }
    }


    float hight;
    public float Hight {
        get => hight;
        set {
            hight = value;
            invalidated = true;
        }
    }

    public float NbDots { get; set; }

    bool invalidated = true;

    // Read Only:
    Vector3 velocity;
    public Vector3 Velocity {
        get {
            if (invalidated)
                UpdateTrajectoryData();

            return velocity;
        }

        private set => velocity = value;
    }

    // Read Only:
    float timeToTarget;
    public float TimeToTarget {
        get {
            if (invalidated)
                UpdateTrajectoryData();

            return timeToTarget;
        }

        private set => timeToTarget = value;
    }

    public void UpdateProperties(float hight, float gravity, float nbDots) {
        Hight = hight;
        Gravity = Vector3.up * gravity;
        NbDots = nbDots;
    }

    /*
     * Using SUVAT Equations:
     *  1) S = U * T + (A * T²) / 2 -> On the vertical axis Uy = Sqrt(-2 * Ay * Sy)
     *  
     *  2) On the others axis (without acceleration), we have:
     *      Ux,z = Sx,z / (sqrt(-2 * Sy / Ay) + sqrt(2 * (H - Sy) / Ay))
     *  
     *  S: Distance - metres (m)
     *  U: Initial velocity - metres per second (ms−1)
     *  V: Instant Velocity - metres per second (ms−1)
     *  A: Acceleration - metres per second per second (ms−2)
     *  T: Time - seconds (s)
     *      
     *  Better explain here:
     *  https://www.youtube.com/watch?v=IvT8hjy6q4o
     */
    void UpdateTrajectoryData() {
        float targetHight = Target.y - Origin.y;

        Vector3 planXZ = new(
                Target.x - Origin.x,
                0,
                Target.z - Origin.z
            );

        float trajectorySummit = Target.y + Hight;

        float velocityY = Mathf.Sqrt(-2 * Gravity.y * trajectorySummit);

        timeToTarget = Mathf.Sqrt(-2 * trajectorySummit / Gravity.y) +
            Mathf.Sqrt(2 * (targetHight - trajectorySummit) / Gravity.y);

        planXZ /= timeToTarget;

        velocity = new Vector3(planXZ.x, velocityY, planXZ.z);

        invalidated = false;

        UpdateListDots();

        //Debug.Log(this + $": Trajectory Updated! Velocity: {velocity} Time: {timeToTarget}");
    }

    List<Vector3> listDots;
    public List<Vector3> ListDots {
        private set => listDots = value;

        get {
            if (invalidated)
                UpdateTrajectoryData();

            return listDots;
        }
    }

    /*
     * Using SUVAT Equation:
     *  S = U * T + (A * T²) / 2
     */
    void UpdateListDots() {
        ListDots = new List<Vector3>();

        float deltaTime = timeToTarget / (float)NbDots;

        for (int index = 1; index <= NbDots; index++) {
            float time = index * deltaTime;

            Vector3 nextDot = Origin + (velocity * time +
                time * time * Gravity / 2);

            ListDots.Add(nextDot);
        }
    }

    public void DrawTrajectory() {
        if (invalidated)
            UpdateTrajectoryData();

        Vector3 previousDot = Origin;

        foreach (Vector3 nextDot in ListDots) {
            Debug.DrawLine(previousDot, nextDot);

            previousDot = nextDot;
        }
    }
}
