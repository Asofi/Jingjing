using UnityEngine;
using EZ_Pooling;

/// <summary>
/// Field implementation
/// </summary>

public static class Field {
	
	/// Amount of rows
	public static int Row { private set; get; }	
	/// Amount of columns
	public static int Cols { private set; get;} 	
	/// Cell width
	public static float CellWidth { private set; get; }
    public static float CellHeight { private set; get; }

    static float upStartPoint;		///< Y start point
	static Vector2 count;			///< Full amount
	static Vector2 offset;			///< Offsets between enemies
	static Vector2 size;			///< Sizes of enemies
	static float leftStartPoint;	///< Calculated left start point
    static public bool[,] isFree;          ///< Is cell is free

    /// <summary>
    /// Set params of Field building
    /// </summary>
    /// <param name="x"></param> Start position by x
    /// <param name="y"></param> Start position by y
    /// <param name="rowNum"></param> Number of rows
    /// <param name="colNum"></param> Number of columns	
    /// <param name="xOffset"></param> Space between cells by x axis
    /// <param name="yOffset"></param> Space between cells by y axis
    /// <param name="width"></param> Width of cell
    /// <param name="height"></param> Height of cell
    public static void Set(float x, float y, int rowNum, int colNum, float xOffset, float yOffset, float width, float height) {
		upStartPoint = y;
		count = new Vector2(colNum, rowNum);
		offset = new Vector2(xOffset, yOffset);
		size = new Vector2(width, height);
        isFree = new bool[colNum, rowNum];
		leftStartPoint = x - count.x * (offset.x + size.x) / 2 + (offset.x / 2.0f);
		Row = rowNum;
		Cols = colNum;
		CellWidth = width;
        CellHeight = height;

        CleanFreeArray();

    }

    public static void CleanFreeArray()
    {
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Cols; j++)
                isFree[j, i] = true;
        }
    }

	/// <summary>
	/// Set params of Field building
	/// </summary>
	/// <param name="pos"></param> Start position of Field
	/// <param name="amount"></param> Number of cells where x is columns and y is rows
	/// <param name="offset"></param> Space between cells
	/// <param name="size"></param>	Size of cell
	public static void Set(Vector2 pos, Vector2 amount, Vector2 offset, Vector2 size) {
		Set(pos.x, pos.y, (int)amount.y, (int)amount.x, offset.x, offset.y, size.x, size.y);
	}

	/// <summary>
	/// Get Position from field space to world space
	/// </summary>
	/// <param name="col"></param> Column
	/// <param name="row"></param> Row
	/// <returns></returns> Position in world
	public static Vector2 FieldToWorldPos(int col, int row = 1) {
		Vector2 point;
		point.x = (leftStartPoint + size.x * col + offset.x * col) + (size.x / 2);
		point.y = upStartPoint - size.y * row - offset.y * row - (size.y / 2);
		return point;
	}

    /// <summary>
    /// Get Position from field space to world space
    /// </summary>
    /// <param name="fieldPos">Position in field space</param> 
    /// <returns>Position in world</returns> 
    public static Vector2 FieldToWorldPos(Vector2 fieldPos) {
		return FieldToWorldPos((int)fieldPos.x, (int)fieldPos.y);
	}

	/// <summary>
	/// Get Object with Collider2D from Field position
	/// </summary>
	/// <param name="col"></param> Column
	/// <param name="row"></param> Row
	/// <returns></returns> Collider of object
	public static Collider2D ObjectFromFieldPos(int col, int row) {
		Vector2 pos = FieldToWorldPos(col, row);
		return Physics2D.BoxCast(pos, Vector2.one * 0.1f, 0, Vector2.zero).collider;
	}

	/// <summary>
	/// Get Object with Collider2D from Field position
	/// </summary>
	/// <param name="fieldpos"></param> Position of object in Field Space
	/// <returns></returns> Collider of object
	public static Collider2D ObjectFromFieldPos(Vector2 fieldpos) {
		return ObjectFromFieldPos((int)fieldpos.x, (int)fieldpos.y);
	}

	/// <summary>
	/// Get Position from world space to field space
	/// </summary>
	/// <param name="pos"></param> Position in world space
	/// <returns></returns> Position in field space
	public static Vector2 WorldToFieldPos(Vector2 pos) {
		int col = Mathf.RoundToInt((pos.x - (size.x / 2.0f) - leftStartPoint) / (size.x + offset.x));
		int row = Mathf.RoundToInt((upStartPoint - (pos.y + (size.y / 2.0f))) / (size.y + offset.y));
		return new Vector2(col, row);
	}

	/// <summary>
	/// Get points to show field
	/// </summary>
	/// <returns></returns> Array of points
	public static Vector2[,] GetFieldDrawPoints() {
		Vector2[,] points = new Vector2[(int)count.x * (int)count.y, 4];
		for (int i = 0; i < count.x; i++) {
			for (int j = 0; j < count.y; j++) {
				Vector2 pointLeftUp;
				Vector2 pointLeftDown;
				Vector2 pointRightUp;
				Vector2 pointRightDown;

				pointLeftUp.x = leftStartPoint + size.x * i + offset.x * i;
				pointLeftUp.y = upStartPoint - size.y * j - offset.y * j;

				pointLeftDown.x = pointLeftUp.x;
				pointLeftDown.y = pointLeftUp.y - size.y;

				pointRightUp.x = leftStartPoint + size.x + size.x * i + offset.x * i;
				pointRightUp.y = upStartPoint - size.y * j - offset.y * j;

				pointRightDown.x = pointRightUp.x;
				pointRightDown.y = pointRightUp.y - size.y;

				points[i + j * (int)count.x,0] = pointLeftUp;
				points[i + j * (int)count.x,1] = pointLeftDown;
				points[i + j * (int)count.x,2] = pointRightUp;
				points[i + j * (int)count.x,3] = pointRightDown;
			}
 		}
		return points;
	}

	/// <summary>
	/// Just draw field to debug
	/// </summary>
	public static void DebugDraw() {
		Vector2[,] points = GetFieldDrawPoints();
		for (int i = 0; i < points.Length / 4; i++) {
			Debug.DrawLine(points[i,0], points[i,1]);
			Debug.DrawLine(points[i,1], points[i,3]);
			Debug.DrawLine(points[i,2], points[i,3]);
			Debug.DrawLine(points[i,2], points[i,0]);
		}
	}

    public static Vector2 GetTriangulatedPos(Vector2 start, Vector3 end)
    {
        Vector2 worldStart = FieldToWorldPos(start);
        Vector2 worldEnd = FieldToWorldPos(end);

        float newX = (worldStart.x + worldEnd.x) / 2;
        float newY = (worldStart.y + worldEnd.y) / 2;
        return new Vector2(newX, newY);
    }
}
