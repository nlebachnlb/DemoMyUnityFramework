using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PuzzleController : MonoBehaviour
{
    [HideInInspector]
    public PuzzleGenerator map;
    //public ParticleSystem dustTrail;

    public float moveSpeed;
    public bool canMove = true;

    bool canRestart = true;


    public int start_x = 5;
    public int start_y = 5;
    public int x;
    public int y;

    int _x, _y;

    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    public Vector3 destination;


    private Vector2 moveInput;

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.instance.OnGameOver += OnGameOver;
    }

    // Update is called once per frame
    void Update()
    {

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && canMove)
        {
            AttemptMove("Up");
        }
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && canMove)
        {
            AttemptMove("Down");
        }
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && canMove)
        {
            AttemptMove("Left");
        }
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && canMove)
        {
            AttemptMove("Right");
        }       

        if ((Vector2)destination != map.CoordToWorldPosition(x, y))
        {
            if (x <= 0)
                x = 0;
            if (x >= map.columns)
                x = map.columns;

            if (y <= 0)
                y = 0;
            if (y >= map.rows - 1)
                y = map.rows - 1;

            destination = map.CoordToWorldPosition(x, y);
        }

        if (transform.position != destination)
        {
            //if (GameManager.instance.gameOver)
            //{
            //    return;
            //}
            canMove = false;
            transform.position = Vector2.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }
        else
        {
            canMove = true;
        }

        if (Mathf.Abs((transform.position - destination).magnitude) < 1f && !canMove)
        {
            Camera.main.transform.DOComplete();
            Camera.main.transform.DOShakePosition(.1f, .2f, 5, 90, false, true);
        }

        if (transform.position == destination && !canMove)
        {
            //udioManager.instance.PlaySFX("Hit Wall", 1f);
        }

        if (canRestart)
        {
            //AudioManager.instance.PlaySFX("Restart", 1f);
            //map.GenerateMap();
            //StartCoroutine(RestartCooldown());
        }
    }

    void AttemptMove(string direction)
    {
        if (!canMove)
            return;
        canMove = false;

        switch (direction)
        {
            case "Up":
                while (map.grid[x, Mathf.Min(map.rows - 1, y + 1)] == 0 && y < map.rows - 1)
                {
                    y++;
                }
                break;
            case "Down":
                while (map.grid[x, Mathf.Max(0, y - 1)] == 0 && y > 0)
                {
                    y--;
                }
                break;
            case "Left":
                while (map.grid[Mathf.Max(0, x - 1), y] == 0 && x > 0)
                {
                    x--;
                }
                break;
            case "Right":
                while (map.grid[Mathf.Min(map.columns - 1, x + 1), y] == 0 && x < map.columns - 1)
                {
                    x++;
                }
                break;
        }

        Debug.Log("Stop: " + map.stop[x, y]);
    }

    public void SetPosition(int x, int y)
    {
        //dustTrail.Stop();
        //dustTrail.Play();
        this.x = x;
        this.y = y;
        transform.position = map.CoordToPosition(x, y);
        destination = map.CoordToWorldPosition(x, y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            if (!map.hasComplete)
                map.OnCompletePuzzle();
        }        
    }

    void OnGameOver()
    {
        this.gameObject.SetActive(false);
    }


    IEnumerator RestartCooldown()
    {
        canRestart = false;
        yield return new WaitForSeconds(1f);
        canRestart = true;
    }
}
