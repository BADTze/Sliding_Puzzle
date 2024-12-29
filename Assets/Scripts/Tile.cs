using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3 targetPosition; 
    public Vector3 correctPosition;
    public int number;
    public bool inRightPlace = false;

    private void Awake()
    {
        correctPosition = transform.position; // Posisi awal adalah posisi benar
        targetPosition = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.05f);
        if (Vector3.Distance(transform.position, correctPosition) < 0.05f)
        {
            inRightPlace = true;
        }
        else
        {
            inRightPlace = false;
        }
    }
}
