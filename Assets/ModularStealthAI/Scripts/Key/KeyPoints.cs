using UnityEngine;

public class KeyPoints : MonoBehaviour
{
    public Transform[] keyPoints;
    public Transform key;

    int keyLocation;

    void Start()
    {
        keyLocation = Random.Range(0, keyPoints.Length);
        key.transform.position = keyPoints[keyLocation].transform.position;
    }

    void OnDrawGizmos()
    {
        if (keyPoints == null || keyPoints.Length == 0)
        {
            return;
        }

        foreach (var point in keyPoints)
        {
            if (point == null)
            {
                continue;
            }  

            Gizmos.color = new Color(0, 0, 1, 1f);
            Gizmos.DrawSphere(point.position, 0.3f);
        }       
    }
}
