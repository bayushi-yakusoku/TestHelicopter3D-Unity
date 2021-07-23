using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrediktPhysic
{
    string prediktSceneName = "PrediktScene";

    Scene source;
    Scene prediktScene;
    PhysicsScene prediktPhysicScene;

    public PrediktPhysic(Scene source)
    {
        this.source = source;

        prediktScene = SceneManager.CreateScene(
            prediktSceneName, 
            new CreateSceneParameters(LocalPhysicsMode.Physics3D));

        prediktPhysicScene = prediktScene.GetPhysicsScene();

        CopystaticObjects();

        Debug.Log($"PrediktPhysic: End of initialization");
    }
    
    Dictionary<string, GameObject> staticObjects = new Dictionary<string, GameObject>();

    void CopystaticObjects()
    {
        Dictionary<string, GameObject> tmpList = new Dictionary<string, GameObject>();

        foreach (GameObject root in source.GetRootGameObjects())
        {
            foreach (Collider child in root.GetComponentsInChildren<Collider>())
            {
                if (child.gameObject.isStatic)
                    tmpList[child.gameObject.name] = child.gameObject;
            }
        }

        foreach (KeyValuePair<string, GameObject> kvp in tmpList)
        {
            GameObject tmp = MonoBehaviour.Instantiate(kvp.Value);

            foreach (Renderer renderer in tmp.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }

            SceneManager.MoveGameObjectToScene(tmp, prediktScene);

            staticObjects[tmp.name] = tmp;
        }
    }

    GameObject mobile;
    Rigidbody mobileRigidBody;
    Vector3 initialPosition;
    public void AddMobile(GameObject obj, Vector3 position)
    {
        mobile = MonoBehaviour.Instantiate(obj);

        foreach (Renderer renderer in mobile.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        mobileRigidBody = mobile.GetComponent<Rigidbody>();

        SceneManager.MoveGameObjectToScene(mobile, prediktScene);
        
        mobileRigidBody.useGravity = true;

        initialPosition = position;
    }

    void ResetScene()
    {
        mobile.transform.position = initialPosition;
        mobile.transform.rotation = Quaternion.identity;

        mobileRigidBody.angularVelocity = Vector3.zero;
        mobileRigidBody.velocity = Vector3.zero;
    }


    public List<Vector3> Predikt(Vector3 force, int iterate = 20)
    {
        ResetScene();

        List<Vector3> result = new List<Vector3>();

        mobileRigidBody.AddForce(force, ForceMode.Impulse);

        for (int step = 0; step < iterate; step++)
        {
            prediktPhysicScene.Simulate(Time.fixedDeltaTime);

            result.Add(mobile.transform.position);
        }

        return result;
    }
}
