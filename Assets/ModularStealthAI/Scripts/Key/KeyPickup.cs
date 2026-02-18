using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    const string PLAYER = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER))
        {
            ObtainKey key = other.GetComponent<ObtainKey>();

            if (key != null)
            {
                key.GetKey();
                Destroy(gameObject); // kendini yok eder
            }
        }
    }
}
