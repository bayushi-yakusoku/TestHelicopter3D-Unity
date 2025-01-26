using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrediktPhysic {
    const string prediktSceneName = "PrediktScene";

    Scene source;
    Scene prediktScene;
    PhysicsScene prediktPhysicScene;

    public PrediktPhysic(Scene source) {
        this.source = source;

        prediktScene = SceneManager.CreateScene(
            prediktSceneName,
            new CreateSceneParameters(LocalPhysicsMode.Physics3D));

        prediktPhysicScene = prediktScene.GetPhysicsScene();

        CopystaticObjects();

        Debug.Log($"PrediktPhysic: End of initialization");
    }

    readonly Dictionary<string, GameObject> staticObjects = new();

    void CopystaticObjects() {
        Dictionary<string, GameObject> tmpList = new();

        foreach (GameObject root in source.GetRootGameObjects()) {
            foreach (Collider child in root.GetComponentsInChildren<Collider>()) {
                if (child.gameObject.isStatic)
                    tmpList[child.gameObject.name] = child.gameObject;
            }
        }

        foreach (KeyValuePair<string, GameObject> kvp in tmpList) {
            GameObject tmp = MonoBehaviour.Instantiate(
                kvp.Value,
                kvp.Value.transform.position,
                kvp.Value.transform.rotation
            );

            foreach (Renderer renderer in tmp.GetComponentsInChildren<Renderer>()) {
                renderer.enabled = false;
            }

            SceneManager.MoveGameObjectToScene(tmp, prediktScene);

            staticObjects[tmp.name] = tmp;
        }
    }

    GameObject mobile;
    Rigidbody mobileRigidBody;
    Vector3 initialPosition;
    public void AddMobile(GameObject obj, Vector3 position) {
        mobile = MonoBehaviour.Instantiate(obj);

        foreach (Renderer renderer in mobile.GetComponentsInChildren<Renderer>()) {
            renderer.enabled = false;
        }

        mobileRigidBody = mobile.GetComponent<Rigidbody>();

        SceneManager.MoveGameObjectToScene(mobile, prediktScene);

        mobileRigidBody.useGravity = true;

        initialPosition = position;
    }

    void ResetScene() {
        mobile.transform.SetPositionAndRotation(initialPosition, Quaternion.identity);

        mobileRigidBody.angularVelocity = Vector3.zero;
        mobileRigidBody.linearVelocity = Vector3.zero;
    }


    public List<Vector3> Predikt(Vector3 force, int iterate = 20) {
        ResetScene();

        List<Vector3> result = new();

        mobileRigidBody.AddForce(force, ForceMode.Impulse);

        for (int step = 0; step < iterate; step++) {
            prediktPhysicScene.Simulate(Time.fixedDeltaTime);

            result.Add(mobile.transform.position);
        }

        return result;
    }
}
