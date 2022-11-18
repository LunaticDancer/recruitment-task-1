using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance; // singleton

	[SerializeField] Camera _camera;
	private float _minSize = 0.1f;
	private float _maxSize = 3;
	private bool _isDraggingSpawner = false;
	private Vector3 prevMousePosition; 

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (_isDraggingSpawner)
			{
				Spawner.Instance.transform.position += _camera.ScreenToWorldPoint(Input.mousePosition) - prevMousePosition;
			}
			else
			{
				transform.position -= _camera.ScreenToWorldPoint(Input.mousePosition) - prevMousePosition;
			}
		}
		else
		{
			if (_isDraggingSpawner)
			{
				Spawner.Instance.SnapOnDrop();
				_isDraggingSpawner = false;
			}
		}
		prevMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
	}

	public void SetMaxSize(float value)
	{
		_maxSize = value;
		_camera.orthographicSize = value;
	}

	public void ChangeZoom(float percentage)
	{
		_camera.orthographicSize = Mathf.Lerp(_minSize, _maxSize, percentage);
	}

	public void StartDraggingSpawner()
	{
		_isDraggingSpawner = true;
	}
}
