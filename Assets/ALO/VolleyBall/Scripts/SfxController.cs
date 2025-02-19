using UnityEngine;

public class SfxController : MonoBehaviour {

    [SerializeField] AudioClip[] ReceptionSfx;
    [SerializeField] AudioClip[] SetSfx;
    [SerializeField] AudioClip[] SpikeSfx;
    [SerializeField] AudioClip[] BounceFloorSfx;


    // Make it Singleton:
    public static SfxController Singleton { get; private set; }

    private void Awake() {
        // Singleton simple implementation:
        if (Singleton != null) {
            Debug.LogWarning(this + ": There is more than one SfxController instance... Destroying this one...");
            Destroy(this.gameObject);
        }

        Singleton = this;
    }

    // Play a random clip from the ReceptionSfx array at the given position:
    public void PlayReceptionSfx(Vector3 position) {
        if (ReceptionSfx.Length == 0) {
            Debug.LogWarning(this + ": No ReceptionSfx clips found...");
            return;
        }
        AudioSource.PlayClipAtPoint(ReceptionSfx[Random.Range(0, ReceptionSfx.Length)], position);
    }

    // Play a random clip from the PassSfx array at the given position:
    public void PlaySetSfx(Vector3 position) {
        if (SetSfx.Length == 0) {
            Debug.LogWarning(this + ": No PassSfx clips found...");
            return;
        }
        AudioSource.PlayClipAtPoint(SetSfx[Random.Range(0, SetSfx.Length)], position);
    }

    // Play a random clip from the SpikeSfx array at the given position:
    public void PlaySpikeSfx(Vector3 position) {
        if (SpikeSfx.Length == 0) {
            Debug.LogWarning(this + ": No SpikeSfx clips found...");
            return;
        }
        AudioSource.PlayClipAtPoint(SpikeSfx[Random.Range(0, SpikeSfx.Length)], position);
    }

    // Play a random clip from the BounceFloorSfx array at the given position:
    public void PlayBounceFloorSfx(Vector3 position) {
        if (BounceFloorSfx.Length == 0) {
            Debug.LogWarning(this + ": No BounceFloorSfx clips found...");
            return;
        }
        AudioSource.PlayClipAtPoint(BounceFloorSfx[Random.Range(0, BounceFloorSfx.Length)], position);
    }

}
