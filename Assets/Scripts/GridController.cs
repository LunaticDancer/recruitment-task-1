using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public static GridController Instance; // this is a singleton

	[System.Serializable]
	private class GridDimensions
	{
		public int x;
		public int y;
		public GridDimensions(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	[SerializeField] GridCell gridCellPrefab;

	private GridDimensions _gridDimensions;
	private Vector2 _gridBottomLeftCorner;
	private GridCell[,] _grid;

	public GridCell[,] Grid { get => _grid; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		// the first things to happen in the whole program
		LoadGridDimensions();
		// set the camera size to the bigger of the two size values
		CameraController.Instance.SetMaxSize((_gridDimensions.x > _gridDimensions.y) ? _gridDimensions.x / 2f : _gridDimensions.y / 2f);
		_gridBottomLeftCorner = CalculateBottomLeftCorner(_gridDimensions.x, _gridDimensions.y);
		InitializeGrid();
		Spawner.Instance.SnapOnDrop();
	}

	private void InitializeGrid()
	{
		_grid = new GridCell[_gridDimensions.x, _gridDimensions.y];
		for (int i = 0; i < _gridDimensions.x; i++)
		{
			for (int j = 0; j < _gridDimensions.y; j++)
			{
				_grid[i, j] = Instantiate(gridCellPrefab, _gridBottomLeftCorner + new Vector2(i, j), Quaternion.identity, transform);
				_grid[i, j].Init(CheckIfGray(i, j), i, j);
			}
		}
	}

	private bool CheckIfGray(int x, int y)
	{
		if (y % 2 == 0)
		{
			return (x % 2) == 1;
		}
		else
		{
			return (x % 2) == 0;
		}
	}

	private void LoadGridDimensions()
	{
		string path = Application.dataPath + "/GridSettings.json";
		if (System.IO.File.Exists(path))
		{
			string loadedString = System.IO.File.ReadAllText(path);
			_gridDimensions = JsonUtility.FromJson<GridDimensions>(loadedString);
		}
		if (_gridDimensions == null)
		{
			_gridDimensions = new GridDimensions(3, 3);
		}
	}

	private Vector2 CalculateBottomLeftCorner(float x, float y)
	{
		return new Vector2(0.5f-(x / 2), 0.5f-(y / 2));
	}

	public GridCell GetFirstEmptyCell()
	{
		for (int i = 0; i < _gridDimensions.x; i++)
		{
			for (int j = 0; j < _gridDimensions.y; j++)
			{
				if (!_grid[i, j].IsBlocked && _grid[i, j].Content == GridCell.CellContentTypes.EMPTY)
				{
					return _grid[i, j];
				}
			}
		}
		return null;
	}

	public void ClearGrid()
	{
		List<GridCell> cellsToClear = new List<GridCell>();
		for (int i = 0; i < _gridDimensions.x; i++)
		{
			for (int j = 0; j < _gridDimensions.y; j++)
			{
				if (_grid[i, j].Content != GridCell.CellContentTypes.EMPTY)
				{
					if (CheckForAdjacentMatch(_grid[i, j].Content, _grid[i, j].GridPosition))
					{
						cellsToClear.Add(_grid[i, j]);
					}
				}
			}
		}

		foreach (GridCell cell in cellsToClear)
		{
			cell.RemoveDot();
		}

		Spawner.Instance.ResetSpiral();
	}

	private bool CheckForAdjacentMatch(GridCell.CellContentTypes type, Vector2Int cellCoords)
	{
		if (cellCoords.x > 0)
		{
			if (_grid[cellCoords.x - 1, cellCoords.y].Content == type)
			{
				return true;
			}
		}
		if (cellCoords.y > 0)
		{
			if (_grid[cellCoords.x, cellCoords.y - 1].Content == type)
			{
				return true;
			}
		}
		if (cellCoords.x < _grid.GetLength(0) - 2)
		{
			if (_grid[cellCoords.x + 1, cellCoords.y].Content == type)
			{
				return true;
			}
		}
		if (cellCoords.y < _grid.GetLength(1) - 2)
		{
			if (_grid[cellCoords.x, cellCoords.y + 1].Content == type)
			{
				return true;
			}
		}
		return false;
	}
}
