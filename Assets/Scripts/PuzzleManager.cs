using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private Transform emptySpace;
    [SerializeField] private Tile[] tiles;
    private Camera _camera;
    private int emptySpaceIndex = 8;

    public AudioClip hitSound;
    private AudioSource audioSource;

    private void Start()
    {
        _camera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        //Shuffle();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Hit: " + hit.transform.name);

                audioSource.PlayOneShot(hitSound);

                if (Vector2.Distance(emptySpace.position, hit.transform.position) < 4)
                {
                    Vector2 lastEmptySpacePosition = emptySpace.position;
                    Tile thisTile = hit.transform.GetComponent<Tile>();
                    emptySpace.position = thisTile.targetPosition;
                    thisTile.targetPosition = lastEmptySpacePosition;

                    int tileIndex = findIndex(thisTile);
                    tiles[emptySpaceIndex] = tiles[tileIndex];
                    tiles[tileIndex] = null;
                    emptySpaceIndex = tileIndex;

                    // Periksa kondisi menang
                    CheckWinCondition();
                }
            }
            else
            {
                Debug.Log("No hit");
            }
        }
    }

    public void Shuffle()
    {
        if (tiles[tiles.Length - 1] != null && emptySpaceIndex != tiles.Length - 1)
        {
            var lastEmptyPosition = emptySpace.position;
            tiles[tiles.Length - 1].targetPosition = emptySpace.position;
            emptySpace.position = lastEmptyPosition;

            tiles[emptySpaceIndex] = tiles[tiles.Length - 1];
            tiles[tiles.Length - 1] = null;
            emptySpaceIndex = tiles.Length - 1;
        }

        int inversions;
        do
        {
            for (int i = 0; i < tiles.Length - 1; i++)
            {
                int randomIndex = Random.Range(0, tiles.Length - 1);
                if (tiles[i] != null && tiles[randomIndex] != null)
                {
                    var lastPosition = tiles[i].targetPosition;
                    tiles[i].targetPosition = tiles[randomIndex].targetPosition;
                    tiles[randomIndex].targetPosition = lastPosition;

                    var temp = tiles[i];
                    tiles[i] = tiles[randomIndex];
                    tiles[randomIndex] = temp;
                }
            }

            inversions = GetInversions();
        } while (inversions % 2 != 0);
    }

    int GetInversions()
    {
        int inversionsSum = 0;
        for (int i = 0; i < tiles.Length; i++)
        {
            int thisTileInvertion = 0;
            for (int j = i; j < tiles.Length; j++)
            {
                if (tiles[j] != null)
                {
                    if (tiles[i].number > tiles[j].number)
                    {
                        thisTileInvertion++;
                    }
                }
            }
            inversionsSum += thisTileInvertion;
        }
        return inversionsSum;
    }

    public int findIndex(Tile ts)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
            {
                if (tiles[i] == ts)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    private void CheckWinCondition()
    {
        bool isSolved = true;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
            {
                if (tiles[i].targetPosition != tiles[i].correctPosition)
                {
                    isSolved = false; 
                    break;
                }
            }
        }

        if (isSolved)
        {
            Debug.Log("You Won!"); 
            FindObjectOfType<GameManager>().EndGame(); 
        }
    }
}
