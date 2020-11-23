using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class LevelManager : MonoBehaviour
{
    public static LevelManager levelManagerInstance;

    public Pathfinder pathFinder;

    private int n = 0;
    private int m = 0;

    private int numberOfSolidObstacles = 0;
    private int numberOfDestructibleObstacles = 0;

    public int numberOfWaves = 2;
    private float waveDuration = 10;
    private int numberOfEnemiesPerWave = 2;

    public float currentWaveDuration = 0;
    public int currentWaveIndex = 0;

    private List<Field> enemySpawnPoints = new List<Field>();

    private bool hasGameEnded;

    public GameObject floorTile;
    public GameObject wallTile;
    public GameObject destructibleWall;
    public GameObject borderWall;
    public PlayerBase playerBase;
    public PlayerBase playerBaseGameObject;
    public Player player;
    public Player playerGameObject;
    public GameObject enemy;

    public Vector2Int enemyPositionInMatrix;
    public int deadEnemiesCount;

    public Vector2Int playerBasePosition = Vector2Int.zero;

    public Field[,] fields;

    public GameObject winningMusic;

    // Initiate level 
    private void Awake()
    {
        levelManagerInstance = this;

        Time.timeScale = 1;
        hasGameEnded = false;
        deadEnemiesCount = 0;

        LoadSaveData();

        playerBasePosition.x = n / 2;
        playerBasePosition.y = m / 2;

        fields = new Field[n, m];

        InitFields();
        GenerateFloor();
        ChooseEnemySpawnPoints();
        GenerateBase();
        InitPlayer();

        pathFinder = new Pathfinder(fields, n, m);

        GenerateObsticles();
        GenerateBorderWall();
    }

    void LoadSaveData()
    {
        SaveData saveData = SaveData.GetSaveData();

        n = saveData.n;
        m = saveData.m;

        numberOfSolidObstacles = saveData.numberOfSolidObstacles;
        numberOfDestructibleObstacles = saveData.numberOfDestructibleObstacles;

        numberOfWaves = saveData.numberOfWaves;
        waveDuration = saveData.waveDuration;
        numberOfEnemiesPerWave = saveData.numberOfEnemiesPerWave;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentWaveDuration = 0;
        currentWaveIndex = 0;
        GenerateWave();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0.1f && HasGameEnded())
        {
            Time.timeScale -= Time.deltaTime;

            if(Time.timeScale <= 0.1f)
            {
                MenuManager.menuManagerInstance.OpenPage(PAGE_NAME.PAUSE_MENU);
            }
        }

        if(currentWaveIndex < numberOfWaves)
        {
            if (currentWaveDuration <= 0)
            {
                GenerateWave();
            }
            else
            {
                currentWaveDuration -= Time.deltaTime;
            }
        }
    }

    public bool IsAlive(Entity entity)
    {
        return entity != null && entity.health > 0;
    }

    public bool HasGameEnded()
    {
        if (!hasGameEnded)
        {
            hasGameEnded = !IsAlive(playerGameObject) || !IsAlive(playerBaseGameObject) || HasPlayerWon();
            
            if(hasGameEnded)
                winningMusic.SetActive(false);
        }
        return hasGameEnded;
    }

    public bool HasPlayerWon()
    {
        return numberOfEnemiesPerWave * numberOfWaves <= deadEnemiesCount;
    }

    private void GenerateWave()
    {
        currentWaveDuration = waveDuration;
        currentWaveIndex++;

        foreach(Field field in enemySpawnPoints)
        {
            Instantiate(enemy, field.GetWorldCoordinates(), Quaternion.identity);
        }
    }

    private void ChooseEnemySpawnPoints()
    {
        int numberOfSpawnPoints = numberOfEnemiesPerWave;
        List<Field> availableFields = new List<Field>();

        for(int i = 0; i < n; i++)
        {
            availableFields.Add(fields[i, 0]);
            availableFields.Add(fields[i, m - 1]);
        }

        for (int i = 0; i < m; i++)
        {
            availableFields.Add(fields[0, i]);
            availableFields.Add(fields[n - 1, i]);
        }

        while (numberOfSpawnPoints > 0 && availableFields.Count > 0)
        {
            int enemyPositionIndex = Random.Range(0, availableFields.Count);
            enemySpawnPoints.Add(availableFields[enemyPositionIndex]);

            availableFields.RemoveAt(enemyPositionIndex);
            numberOfSpawnPoints--;
        }
    }

    private void InitPlayer()
    {
        playerGameObject = Instantiate(player, playerBaseGameObject.transform.position, Quaternion.identity);
    }

    private void InitFields()
    {
        fields = new Field[n, m];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                fields[i, j] = new Field();
                fields[i, j].position = new Vector2Int(i, j);
            }
        }
    }

    private void GenerateFloor()
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                fields[i, j].field = Instantiate(floorTile, new Vector2(i, j), Quaternion.identity, this.transform);
                fields[i, j].field.GetComponent<SpriteRenderer>().sortingOrder = 0;
                fields[i, j].fieldType = Field.FieldType.Floor;
            }
        }
    }

    private void GenerateBase()
    {
        Field playerBaseField = fields[playerBasePosition.x, playerBasePosition.y];

        playerBaseField.field = Instantiate(playerBase, playerBaseField.field.transform.position, Quaternion.identity, this.transform).gameObject;
        playerBaseGameObject = playerBaseField.field.GetComponent<PlayerBase>();
        playerBaseField.field.GetComponent<SpriteRenderer>().sortingOrder = 1;
        playerBaseField.fieldType = Field.FieldType.Base;
    }

    private void GenerateObsticles()
    {
        List<int> avaliableFields = new List<int>(m * n);
        for (int i = 0; i < m * n; i++)
            avaliableFields.Add(i);

        Vector2 basePosition = new Vector2(LevelManager.levelManagerInstance.playerBaseGameObject.transform.position.x, LevelManager.levelManagerInstance.playerBaseGameObject.transform.position.y);

        while ((numberOfSolidObstacles > 0 || numberOfDestructibleObstacles > 0) && avaliableFields.Count > 0)
        {
            int randomField = Random.Range(0, avaliableFields.Count);

            int i = avaliableFields[randomField] / n;
            int j = avaliableFields[randomField] % m;

            float distanceToBase = Vector2.Distance(new Vector2(i, j), playerBasePosition);

            if(fields[i, j].fieldType == Field.FieldType.Floor && fields[i, j].position != enemyPositionInMatrix && distanceToBase > 3)
            {
                fields[i, j].fieldType = Field.FieldType.Obstacle;

                bool enemiesHavePath = true;
                for(int k = 0; k < enemySpawnPoints.Count; k++)
                {
                    if((enemySpawnPoints[k].position.x == i && enemySpawnPoints[k].position.y == j)
                        || (numberOfSolidObstacles <= 0 && pathFinder.GetPath(enemySpawnPoints[k].position, new Vector2Int(playerBasePosition.x, playerBasePosition.y)).Count == 0))
                    {
                        enemiesHavePath = false;
                        break;
                    }
                }

                if(enemiesHavePath)
                {
                    if(numberOfSolidObstacles > 0)
                    {
                        fields[i, j].field = Instantiate(wallTile, fields[i, j].field.transform.position, Quaternion.identity, this.transform);
                        fields[i, j].fieldType = Field.FieldType.Obstacle;
                        numberOfSolidObstacles--;
                    }
                    else
                    {
                        fields[i, j].field = Instantiate(destructibleWall, fields[i, j].field.transform.position, Quaternion.identity, this.transform);
                        fields[i, j].fieldType = Field.FieldType.DestructibleWall;
                        numberOfDestructibleObstacles--;
                    }
                }
                else
                {
                    fields[i, j].fieldType = Field.FieldType.Floor;
                }
            }

            avaliableFields.RemoveAt(randomField);
        }
    }

    private void GenerateBorderWall()
    {
        for (int i = -1; i <= n; i++)
        {
            Instantiate(borderWall, new Vector2(i, -1), Quaternion.identity, this.transform);
            Instantiate(borderWall, new Vector2(i, m), Quaternion.identity, this.transform);
        }

        for (int i = 0; i <= m; i++)
        {
            Instantiate(borderWall, new Vector2(-1, i), Quaternion.identity, this.transform);
            Instantiate(borderWall, new Vector2(n, i), Quaternion.identity, this.transform);
        }
    }
}
