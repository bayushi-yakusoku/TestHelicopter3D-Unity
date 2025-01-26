using System;
using UnityEngine;


/*
 * Part of using Roslyn analyzers.
 * This class should raise a warning during compilation:
 * https://docs.unity3d.com/6000.0/Documentation/Manual/install-existing-analyzer.html
 */
public class RethrowError : MonoBehaviour
{
    void Update()
    {
        try
        {
            DoSomethingInteresting();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw e;
        }
    }

    private void DoSomethingInteresting()
    {
        throw new System.NotImplementedException();
    }
}
