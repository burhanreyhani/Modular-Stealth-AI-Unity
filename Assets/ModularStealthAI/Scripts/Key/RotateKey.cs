using UnityEngine;

public class RotateKey : MonoBehaviour
{
    GameObject key;

    void Start()
    {
        key = GetComponent<GameObject>();
    }

    void Update()
    {
        transform.Rotate(0, 120f * Time.deltaTime, 120f * Time.deltaTime);
    }
}
