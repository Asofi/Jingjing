using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;
using System.Linq;

[System.Serializable]
public class BrickProperties
{
    public BrickTypes Type;
    public Transform Prefab;
    public int dropRarity;
}

public enum BrickTypes
{
    EMPTY,
    COMMON,
    SPINNER,
    SOLID,
    STICKY,
    BONUS
}

public class Spawner : MonoBehaviour {

    [Header("Grid Setup")]
    public int RowNum;
    public int ColNum;
    public float XOffset;
    public float YOffset;
    public float Width;
    public float Height;

    [Space]
    [Header("Blocks Steup")]
    public List<BrickProperties> Bricks = new List<BrickProperties>();
    public Transform BrickContainer;
    public Transform CommonBrickPrefab;
    public Transform SpinnerBrickPrefab;
    public Transform SolidBrickPrefab;
    public Transform StickyBrickPrefab;
    public Transform BonusBrickPrefab;

    [Space]
    [Header("Game Setup")]
    public int Level = 1;
    public float BricksSpeed = 1;
    public float SpeedStep = 0.001f;
    private float curBricksSpeed;
    #region Probabilities
    [Space]
    [Header("Probability Setup")]
    [Header("Spawn Chance")]
    [Range(0, 1)]
    public float SpawnBrickChance;
    [Header("Bonus")]
    [Range(0, 100)]
    public int BonusBrickMinWeidth;
    [Range(0, 1)]
    public float BonusBrickMaxWeidth;
    #endregion


    private void Awake()
    {
        Field.Set(transform.position.x, transform.position.y, RowNum, ColNum, XOffset, YOffset, Width, Height);

    }

    private void Start()
    {
        EventManager.OnGameStart += EventManager_OnGameStart;
        EventManager.OnGameOver += EventManager_OnGameOver;
        EventManager.OnSecondChance += EventManager_OnSecondChance;
        curBricksSpeed = BricksSpeed;
    }

    private void EventManager_OnSecondChance()
    {
        print("Start sec");
        StartCoroutine(MoveBricksUp());
    }

    private void EventManager_OnGameOver()
    {
        StopCoroutine(movingBricks);
        StartCoroutine(MoveBricksUp());
    }

    private void EventManager_OnGameStart()
    {
        StopAllCoroutines();
        Level = 1;
        curBricksSpeed = BricksSpeed;
        BrickContainer.position = Vector2.zero;
        AddRows();
        //AddRows();
        if (movingBricks != null)
            StopCoroutine(movingBricks);
        movingBricks = MovingBricks();
        StartCoroutine(movingBricks);
    }

    IEnumerator movingBricks;
    IEnumerator MovingBricks()
    {
        while (true)
        {
            BrickContainer.Translate(Vector2.down * curBricksSpeed * Time.deltaTime);
            if (BrickContainer.position.y < -Field.CellHeight * Level * 2)
            {
                //print("Add rows. Level - " + Level + "\n Time: " + Time.timeSinceLevelLoad);
                AddRows();
                Level++;
                curBricksSpeed += SpeedStep;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator MoveBricksUp()
    {
        BrickCounter.isSecondChance = true;
        var _startPos = BrickContainer.position;
        while (BrickCounter.BrickCount > 0)
        {
            BrickContainer.Translate(Vector2.down * -BricksSpeed * 60 * Time.unscaledDeltaTime);
            yield return new WaitForEndOfFrame();
        }
        BrickCounter.isSecondChance = false;
        EventManager.Candycallipce();
        BrickContainer.position = _startPos;
        
    }

    /// <summary>
    /// Добавление двух рядов
    /// </summary>
    void AddRows()
    {
        Field.CleanFreeArray();
        for (int j = 0; j <= Field.Row; j++)
        {
            for (int i = 0; i <= Field.Cols; i++)
            {
                BrickTypes type = GetBrick((j > 0) ? false : true);
                AddElement(type, i, j);
            }
        }
    }

    BrickTypes GetBrick(bool includeBonus)
    {
        float brickChance = Random.value;
        BrickTypes brickType = BrickTypes.EMPTY;

        if(brickChance > SpawnBrickChance)
        {
           brickType = BrickTypes.EMPTY;
        }
        else
        {
            int itemWeight = 0;

            for (int i = 0; i < Bricks.Count; i++)
            {
                itemWeight += Bricks[i].dropRarity;
            }
            //print("Item Weight: " + itemWeight);

            int randomValue = Random.Range(0, itemWeight);

            for (int i = 0; i < (includeBonus ? Bricks.Count : Bricks.Count-1); i++)
            {
                if(randomValue <= Bricks[i].dropRarity)
                {
                    brickType =  Bricks[i].Type;
                    if (!includeBonus)
                        return brickType;

                    if (Bricks[i].Type != BrickTypes.BONUS)
                        Bricks.Last().dropRarity += 15;
                    else
                        Bricks.Last().dropRarity = BonusBrickMinWeidth;
                    return brickType;
                }
                randomValue -= Bricks[i].dropRarity;

            }
        }
        return brickType;
    }

    /// <summary>
    /// Проверка на возможность установки кирпича
    /// </summary>
    /// <param name="startCol">Стартовый столбец</param>
    /// <param name="startRow">Стартовый ряд</param>
    /// <param name="endCol">Конечный столбец</param>
    /// <param name="endRow">Конечный ряд</param>
    /// <returns>Свободно ли место под новый кирпич</returns>
    bool canPlace(int startCol, int startRow, int endCol, int endRow)
    {
        bool canPlace = true;
        if (endCol > Field.Cols-1 || endRow > Field.Row-1)
            return false;
        for (int i = startCol; i <= endCol; i++)
        {
            for (int j = startRow; j <= endRow; j++)
            {
                canPlace = Field.isFree[i, j];
                if (!canPlace)
                {
                    //print(string.Format("You can't place brick here. Something already at {1}:{2}", i, j));
                    return false;
                }
            }
        }

        return canPlace;
    }

    /// <summary>
    /// Добавить элемент на сцену
    /// </summary>
    /// <param name="brickType">Тип элемента</param>
    /// <param name="startCol">Стартовый столбец</param>
    /// <param name="startRow">Стартовый ряд</param>
    void AddElement(BrickTypes brickType, int startCol, int startRow)
    {
        int endCol = startCol;
        int endRow = startRow;
        Transform obj = null;

        switch (brickType)
        {
            case BrickTypes.EMPTY:
                break;
            case BrickTypes.COMMON:
                endCol = startCol + 1;
                if (canPlace(startCol, startRow, endCol, endRow))
                {
                    //AddElement(CommonBrickPrefab, new Vector2(startCol, startRow), new Vector2(endCol, endRow));
                    obj = EZ_PoolManager.Spawn(CommonBrickPrefab, Field.GetTriangulatedPos(new Vector2(startCol, startRow), new Vector2(endCol, endRow)), Quaternion.identity);
                }
                break;

            case BrickTypes.SOLID:
                endCol = startCol + 1;
                if (canPlace(startCol, startRow, endCol, endRow))
                    //AddElement(SolidBrickPrefab, new Vector2(startCol, startRow), new Vector2(endCol, endRow));
                    obj = EZ_PoolManager.Spawn(SolidBrickPrefab, Field.GetTriangulatedPos(new Vector2(startCol, startRow), new Vector2(endCol, endRow)), Quaternion.identity);
                break;
            case BrickTypes.STICKY:
                endCol = startCol + 1;
                if (canPlace(startCol, startRow, endCol, endRow))
                    //AddElement(StickyBrickPrefab, new Vector2(startCol, startRow), new Vector2(endCol, endRow));
                    obj = EZ_PoolManager.Spawn(StickyBrickPrefab, Field.GetTriangulatedPos(new Vector2(startCol, startRow), new Vector2(endCol, endRow)), Quaternion.identity);
                break;

            case BrickTypes.SPINNER:
                endCol = startCol + 2;
                if (canPlace(startCol, startRow, endCol, endRow))
                    //AddElement(SpinnerBrickPrefab, new Vector2(startCol, startRow), new Vector2(endCol, endRow));
                    obj = EZ_PoolManager.Spawn(SpinnerBrickPrefab, Field.GetTriangulatedPos(new Vector2(startCol, startRow), new Vector2(endCol, endRow)), Quaternion.identity);
                break;

            case BrickTypes.BONUS:
                if (startRow != 0)
                    return;
                endCol = startCol + 1;
                endRow = 1;
                if (canPlace(startCol, startRow, endCol, endRow))
                    //AddElement(BonusBrickPrefab, new Vector2(startCol, startRow), new Vector2(endCol, endRow));
                    obj = EZ_PoolManager.Spawn(BonusBrickPrefab, Field.GetTriangulatedPos(new Vector2(startCol, startRow), new Vector2(endCol, endRow)), Quaternion.identity);
                break;
        }

        if (obj != null)
        {
            for (int i = startCol; i <= endCol; i++)
            {
                for (int j = startRow; j <= endRow; j++)
                {
                    Field.isFree[i, j] = false;
                }
            }

            obj.SetParent(BrickContainer, true);
            //print("Spawned: " + brickType + " at " + startCol + " " + startRow);
        }

    }
}
