using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoves : MonoBehaviour
{

    [SerializeField] float duration = 3;
    [SerializeField] AnimationCurve speedCurve;

    float elapsedTime = 0;

    Vector3 startingPos = Vector3.zero;

    bool destinationReached = true;

    Vector3 finalPos;
    public Vector3 FinalPos
    {
        get => finalPos;
        set
        {
            if (finalPos == value)
                return;

            finalPos = value;

            startingPos = transform.position;
            elapsedTime = 0;
            destinationReached = false;
        }
        
    }

    private void Update()
    {
        if (destinationReached == true)
            return;

        elapsedTime += Time.deltaTime;

        float percentage = elapsedTime / duration;

        Vector3 newPos = Vector3.Lerp(startingPos, finalPos, speedCurve.Evaluate(percentage));
        
        Vector3 oldPos = transform.position;
        oldPos.x = newPos.x;
        
        transform.position = oldPos;

        if (transform.position == FinalPos)
            destinationReached = true;
    }
}
