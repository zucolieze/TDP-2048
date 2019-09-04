using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static int gridWidth = 4, gridHeigth = 4;
    public static Transform[,] grid = new Transform[gridWidth, gridHeigth];
    public static NotATile[,] previousGrid = new NotATile[gridWidth, gridHeigth];

    public Canvas gameOverCanvas;

    public Text gameScoreText;
    public Text bestScoreText;
    public int score = 0;
    public int previousScore = 0;
    //tiene que ser publico porque Unity no puede verlos sino

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("bestScore", 0);//Reinicia el bestScore
        GenerateNewTile(2);
        UpdateBestScore();

    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckGameOver())
        {
            CheckUserInput();
        }
        else
        {
            SaveBestScore();
            UpdateScore();
            gameOverCanvas.gameObject.SetActive(true);
        }
    }


    void CheckUserInput()
    {
        bool up = Input.GetKeyDown(KeyCode.UpArrow),
             down = Input.GetKeyDown(KeyCode.DownArrow),
             left = Input.GetKeyDown(KeyCode.LeftArrow),
             right = Input.GetKeyDown(KeyCode.RightArrow);

        if (down || up || left || right)
        {
            storePreviousTiles();
            mergeTiles();

            //Mueve todas las Tiles en un direccion
            if (down)
            {
                MoveAllTiles(Vector2.down);
            }
            if (up)
            {
                MoveAllTiles(Vector2.up);
            }
            if (left)
            {
                MoveAllTiles(Vector2.left);
            }
            if (right)
            {
                MoveAllTiles(Vector2.right);
            }
        }
    }

    private void storePreviousTiles()
    {
        previousScore = score;
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeigth; j++)
            {
                Transform tempTile = grid[i, j];
                previousGrid[i, j] = null;
                if (tempTile != null)
                {
                    NotATile notATile = new NotATile();
                    notATile.setLocation(tempTile.localPosition);
                    notATile.setValue(tempTile.GetComponent<tile>().tileValue);
                    previousGrid[i, j] = notATile;
                    //se guardan todas las posiciones de las Tiles para poder volver atras
                }
            }
        }
    }

    void UpdateScore()
    {
        gameScoreText.text = score.ToString("000000000");
    }

    void UpdateBestScore()
    {
        bestScoreText.text = PlayerPrefs.GetInt("bestScore").ToString();
    }

    void SaveBestScore()
    {
        int oldScore = PlayerPrefs.GetInt("bestScore");

        if (score > oldScore)
        {
            PlayerPrefs.SetInt("bestScore", score);
        }

    }

    bool CheckGameOver()
    {

        if (transform.childCount < gridWidth * gridHeigth)
        {
            //Checkea que no termino el juego si todavia hay un espacio vacio
            return false;
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeigth; y++)
            {
                Transform currentTile = grid[x, y];
                Transform tileArriba = null;
                Transform tileAbajo = null;

                if (y != 0)
                    tileAbajo = grid[x, y - 1];
                if (x != gridWidth - 1)
                    tileArriba = grid[x + 1, y];

                if (tileArriba != null)
                {
                    if (currentTile.GetComponent<tile>().tileValue == tileArriba.GetComponent<tile>().tileValue)
                    {//Checkea si hay una Tile arriba arriba
                        return false;//quedan movimentos para hacer
                    }

                }
                if (tileAbajo != null)
                {
                    if (currentTile.GetComponent<tile>().tileValue == tileAbajo.GetComponent<tile>().tileValue)
                    {//Checkea si hay una Tile arriba abajo
                        return false;//quedan movimentos para hacer
                    }
                }
            }
        }
        //se termino el juego
        return true;
    }

    //Este metodo solo mueve una Tile
    bool MoveTile(Transform tile, Vector2 direc)
    {
        Vector2 startPos = tile.localPosition;
        while (true)
        {
            tile.transform.localPosition += (Vector3)direc;
            Vector2 pos = tile.transform.localPosition;

            if (CheckIsInsideGrid(pos))
            {//si esta dentro actualiza y es valido
                if (CheckPosition(pos))
                {
                    UpdateGrid();
                }
                else
                {
                    if (!CheckAndCombineTiles(tile))
                    {
                        tile.transform.localPosition += -(Vector3)direc;
                        if (tile.transform.localPosition == (Vector3)startPos)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            else
            { //mueve la ficha para el lado contrario si no esta en el grid
                tile.transform.localPosition += -(Vector3)direc;
                if (tile.transform.localPosition == (Vector3)startPos)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }



    //Mueve tosa las fichas posibles para una direccion
    void MoveAllTiles(Vector2 direc)
    {
        int cantTilesMoved = 0;
        if (direc == Vector2.left)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeigth; y++)
                {
                    if (grid[x, y] != null)
                    {
                        if (MoveTile(grid[x, y], direc))
                        {
                            cantTilesMoved++;
                        }
                    }
                }
            }
        }
        if (direc == Vector2.right)
        {
            for (int x = gridWidth - 1; x >= 0; x--)
            {
                for (int y = 0; y < gridHeigth; y++)
                {
                    if (grid[x, y] != null)
                    {
                        if (MoveTile(grid[x, y], direc))
                        {
                            cantTilesMoved++;
                        }
                    }
                }
            }
        }
        if (direc == Vector2.down)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeigth; y++)
                {
                    if (grid[x, y] != null)
                    {
                        if (MoveTile(grid[x, y], direc))
                        {
                            cantTilesMoved++;
                        }
                    }
                }
            }
        }
        if (direc == Vector2.up)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = gridHeigth - 1; y >= 0; y--)
                {
                    if (grid[x, y] != null)
                    {
                        if (MoveTile(grid[x, y], direc))
                        {
                            cantTilesMoved++;
                        }
                    }
                }
            }
        }
        //generar nueva Tile si se logro hacer un movimiento
        if (cantTilesMoved != 0)
        {
            GenerateNewTile(1);
        }
    }

    bool CheckAndCombineTiles(Transform movingTile)
    {
        Vector2 pos = movingTile.transform.localPosition;
        Transform collidingTile = grid[(int)pos.x, (int)pos.y]; //Tile que se superpone para cambiar por otra mayor

        int movingTileValue = movingTile.GetComponent<tile>().tileValue;
        int collidingTileValue = collidingTile.GetComponent<tile>().tileValue;

        if (movingTileValue == collidingTileValue &&
            !movingTile.GetComponent<tile>().merged &&
             !collidingTile.GetComponent<tile>().merged)
        {
            Destroy(movingTile.gameObject);
            Destroy(collidingTile.gameObject);
            grid[(int)pos.x, (int)pos.y] = null;
            string newTileName = "tile_" + movingTileValue * 2; //cambia la ficha por una mayor
            GameObject newTile = (GameObject)Instantiate(Resources.Load(newTileName, typeof(GameObject)), pos, Quaternion.identity);
            newTile.transform.parent = transform;
            newTile.GetComponent<tile>().merged = true;
            UpdateGrid();
            //actualizar score por la colision de fichas
            score += movingTileValue * 2;
            UpdateScore();

            return true;
        }
        else
        {
            return false;
        }

    }

    void GenerateNewTile(int cantTiles)
    {
        for (int i = 0; i < cantTiles; ++i)
        {
            Vector2 locationForNewTile = GetRandomLocationForNewTile();
            string tile = "tile_2";
            float chanceOfTwo = UnityEngine.Random.Range(0f, 1f);
            if (chanceOfTwo > 0.85f)
            {
                tile = "tile_4";
            }
            GameObject newTile = (GameObject)Instantiate(Resources.Load(tile, typeof(GameObject)), locationForNewTile, Quaternion.identity);
            newTile.transform.parent = transform;
        }
        UpdateGrid();
    }


    void UpdateGrid() //actualizar todas las Tiles
    {
        for (int y = 0; y < gridWidth; ++y)
        {
            for (int x = 0; x < gridHeigth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform tile in transform)
        {
            Vector2 v = new Vector2(Mathf.Round(tile.position.x), Mathf.Round(tile.position.y));
            grid[(int)v.x, (int)v.y] = tile;
        }
    }

    /// <summary>
    /// Posiciones random para los nuevos numeros
    /// </summary>
    /// <returns> Vector2 </returns>
    Vector2 GetRandomLocationForNewTile()
    {
        List<int> x = new List<int>();
        List<int> y = new List<int>();

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeigth; j++)
            {
                if (grid[i, j] == null)
                {
                    x.Add(i);
                    y.Add(j);
                }
            }
        }

        int randIndex = UnityEngine.Random.Range(0, x.Count);
        int randX = x.ElementAt(randIndex);
        int randY = y.ElementAt(randIndex);
        //Nueva localizacion para la Tile nueva
        return new Vector2(randX, randY);
    }

    bool CheckIsInsideGrid(Vector2 pos)
    {
        if (pos.x >= 0 && pos.x <= gridWidth - 1 && pos.y >= 0 && pos.y <= gridHeigth - 1)
        {
            return true;
        }
        return false;
    }

    bool CheckPosition(Vector2 pos)
    {
        if (grid[(int)pos.x, (int)pos.y] == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void mergeTiles()
    {
        foreach (Transform t in transform)
        {
            t.GetComponent<tile>().merged = false;
        }
    }

    //---------------BUTTONS

    /// <summary>
    /// Restart the game, create a new grid
    /// </summary>
    void playAgain()
    {
        grid = new Transform[gridWidth, gridHeigth];
        score = 0;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform t in transform)
        {
            children.Add(t.gameObject);
        }
        children.ForEach(t => DestroyImmediate(t));
        gameOverCanvas.gameObject.SetActive(false);
        UpdateScore();
        UpdateBestScore();
        GenerateNewTile(2);
    }

    public void Undo()
    {
        score = previousScore;
        UpdateScore();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeigth; j++)
            {
                grid[i, j] = null;
                NotATile notAtile = previousGrid[i, j];

                if (notAtile != null)
                {
                    int tileValue = notAtile.getValue();
                    string newTileName = "tile_" + tileValue;

                    GameObject newTile = (GameObject)Instantiate(Resources.Load(newTileName, typeof(GameObject)), notAtile.getLocation(), Quaternion.identity);
                    newTile.transform.parent = transform;
                    grid[i, j] = newTile.transform;
                }
            }
        }
    }
}