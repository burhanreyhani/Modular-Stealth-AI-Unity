using UnityEngine;

public class EscapePoint : MonoBehaviour
{
    const string PLAYER = "Player";

    public bool doPlayerWin = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PLAYER))
        {
            return;
        }

        ObtainKey key = other.GetComponent<ObtainKey>();

        if (key != null && key.hasKey)
        {
            doPlayerWin = true;
            Debug.Log("YOU WIN");
            Time.timeScale = 0f;
        }
        else
        {
            Debug.Log("Need key!");
        }
            
    }
}
