using UnityEngine;

public class PlayerDied : MonoBehaviour
{
    public LayerMask enemyLayer;

    public bool doPlayerDie = false;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (((1 << hit.gameObject.layer) & enemyLayer) != 0)
        {
            Debug.Log("You Died!");
            doPlayerDie = true;
            Time.timeScale = 0f;
        }
    }
}
