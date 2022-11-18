using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

	[SerializeField] private Dot[] dotPrefabs;
	private GridCell _currentCell;
	private bool _isSpawning;
	private Vector2Int _currentSpiralPoint;
	private int _spawnDirection; // 0 is up, then it's clockwise
	private int _currentDistance;
	private int _maxDistance;
	private bool _hasTakenFirstTurn; // the plan is for the spiral arm to get longer by 1 every 2 turns
	private bool _isCompletelyOutOfBounds;


	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (_isSpawning) // this is synonymous with "Is the Spawn button held"
		{
			SpawnFrame();
		}
	}

	private void SpawnFrame() // the goal of this method is to spawn a single dot or cease functioning if there's no more space left
	{
		if (_isCompletelyOutOfBounds)
		{
			return;
		}
		bool hasSpawned = false;
		while (!hasSpawned)
		{
			SpiralStep();
			if (_isCompletelyOutOfBounds)
			{
				return;
			}
			if (IsSpiralPointInBounds())
			{
				if (!GridController.Instance.Grid[_currentSpiralPoint.x, _currentSpiralPoint.y].IsBlocked && GridController.Instance.Grid[_currentSpiralPoint.x, _currentSpiralPoint.y].Content == GridCell.CellContentTypes.EMPTY)
				{
					Dot newDot = Instantiate(dotPrefabs[Random.Range(0, dotPrefabs.Length)], transform.position, 
						Quaternion.identity);
					newDot.StartMoving(GridController.Instance.Grid[_currentSpiralPoint.x, _currentSpiralPoint.y]);
					hasSpawned = true;
				}
			}
		}
	}

	private bool IsSpiralPointInBounds()
	{
		if (_currentSpiralPoint.x >= 0)
		{
			if (_currentSpiralPoint.x < GridController.Instance.Grid.GetLength(0))
			{
				if (_currentSpiralPoint.y >= 0)
				{
					if (_currentSpiralPoint.y < GridController.Instance.Grid.GetLength(1))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void EndOfSpiralCheck()
	{
		bool isDoneVertically = false;
		bool isDoneHorizontally = false;
		Vector2Int midpoints = new Vector2Int(GridController.Instance.Grid.GetLength(0) / 2, GridController.Instance.Grid.GetLength(0) / 2);
		if (_currentCell.GridPosition.x < midpoints.x)
		{
			if (_currentSpiralPoint.x > GridController.Instance.Grid.GetLength(0))
			{
				isDoneHorizontally = true;
			}
		}
		else
		{
			if (_currentSpiralPoint.x < 0)
			{
				isDoneHorizontally = true;
			}
		}

		if (_currentCell.GridPosition.y < midpoints.y)
		{
			if (_currentSpiralPoint.y > GridController.Instance.Grid.GetLength(1))
			{
				isDoneVertically = true;
			}
		}
		else
		{
			if (_currentSpiralPoint.y < 0)
			{
				isDoneVertically = true;
			}
		}

		if (isDoneHorizontally && isDoneVertically)
		{
			_isCompletelyOutOfBounds = true;
		}
	}

	private void SpiralStep()
	{
		switch (_spawnDirection)
		{
			case 0:
				_currentSpiralPoint += Vector2Int.up;
				break;
			case 1:
				_currentSpiralPoint += Vector2Int.right;
				break;
			case 2:
				_currentSpiralPoint += Vector2Int.down;
				break;
			case 3:
				_currentSpiralPoint += Vector2Int.left;
				break;
		}
		_currentDistance++;
		if (_currentDistance >= _maxDistance)
		{
			TakeTurn();
		}
	}

	private void TakeTurn()
	{
		_spawnDirection = (_spawnDirection + 1) % 4; // cycle around in values 0, 1, 2, 3, 0...
		_currentDistance = 0;
		if (_hasTakenFirstTurn)
		{
			_hasTakenFirstTurn = false;
			_maxDistance++;
		}
		else
		{
			_hasTakenFirstTurn = true;
		}
		EndOfSpiralCheck();
	}

	public void SnapOnDrop()
	{
		_currentCell = FindNearestValidCell();
		transform.position = _currentCell.transform.position;
		ResetSpiral();
	}

	public GridCell FindNearestValidCell()
	{
		GridCell result = GridController.Instance.GetFirstEmptyCell();
		float closestDistance = Vector2.Distance(transform.position, result.transform.position);


		for (int i = result.GridPosition.x; i < GridController.Instance.Grid.GetLength(0); i++)
		{
			for (int j = 0; j < GridController.Instance.Grid.GetLength(1); j++)
			{
				if (!GridController.Instance.Grid[i, j].IsBlocked && GridController.Instance.Grid[i, j].Content == GridCell.CellContentTypes.EMPTY)
				{
					float distanceCache = Vector2.Distance(transform.position, GridController.Instance.Grid[i, j].transform.position);
					if (distanceCache < closestDistance)
					{
						closestDistance = distanceCache;
						result = GridController.Instance.Grid[i, j];
					}
				}
			}
		}
		return result;
	}

	public void ToggleSpawning(bool value)
	{
		_isSpawning = value;
	}

	public void ResetSpiral()
	{
		_spawnDirection = 0;
		_currentDistance = 0;
		_maxDistance = 1;
		_hasTakenFirstTurn = false;
		_currentSpiralPoint = _currentCell.GridPosition;
		_isCompletelyOutOfBounds = false;
	}

	private void OnMouseDown()
	{
		CameraController.Instance.StartDraggingSpawner();
	}
}
