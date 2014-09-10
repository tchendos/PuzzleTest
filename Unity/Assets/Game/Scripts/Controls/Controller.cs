using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * Class handling user input. It transforms position on screen to grid and detects 
 * either swipe gesture or tapping on two adjacent gems and triggers Simulation.Swap.
 */
public class Controller:MonoBehaviour
{
	Camera _camera;
	GameObject _fieldCenter;
	Vector2 _fieldLT;
	Vector2 _fieldRB;

	int _state;

	bool _dragging;
	Vector3 _dragStartWorld;
	Vector2i _dragStartGrid;
	Vector2i _lastTapGrid;
	bool _lastTapGridValid = false;

	Simulation _simulation;

	public void Init(Camera camera, GameObject fieldCenter, Simulation simulation) 
	{
		_camera = camera; _fieldCenter = fieldCenter; _simulation = simulation;
	}

	Vector3 ScreenToWorld(Vector3 position)
	{
		Vector3 wPos = _camera.ScreenToWorldPoint(position);
		return _fieldCenter.transform.worldToLocalMatrix.MultiplyPoint(wPos);
	}

	Vector2i WorldToGrid(Vector3 position)
	{
		return new Vector2i((int)((position.x - _fieldLT.x) / ArtData.FIELD_GRID_X),
		                    ArtData.FIELD_SIZE_Y - 1 - (int)((position.y - _fieldLT.y) / ArtData.FIELD_GRID_Y));
	}

	void Awake()
	{
		_fieldLT = new Vector2(-ArtData.FIELD_GRID_X * ArtData.FIELD_SIZE_X / 2,
		                       -ArtData.FIELD_GRID_Y * ArtData.FIELD_SIZE_Y / 2);
		_fieldRB = new Vector2(ArtData.FIELD_GRID_X * ArtData.FIELD_SIZE_X / 2,
		                       ArtData.FIELD_GRID_Y * ArtData.FIELD_SIZE_Y / 2);
	}

	IEnumerator Swap(Vector2i newPos, Vector2i lastPos)
	{
		_state++;
		yield return StartCoroutine(_simulation.Swap(newPos, lastPos));
		_state--;
		_dragging = false;
	}

	void Update()
	{
		if (_camera == null || _state > 0)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			_dragStartWorld = ScreenToWorld(Input.mousePosition);
			if (_dragStartWorld.x >= _fieldLT.x && _dragStartWorld.y >= _fieldLT.y &&
			    _dragStartWorld.x < _fieldRB.x && _dragStartWorld.y < _fieldRB.y)
			{
				_dragStartGrid = WorldToGrid(_dragStartWorld);
				_dragging = true;
			}
			else _dragging = false;
		}
		else if (Input.GetMouseButton(0) && _dragging)
		{
			Vector3 delta = ScreenToWorld(Input.mousePosition) - _dragStartWorld;
			if (Mathf.Abs(delta.x) > (ArtData.FIELD_GRID_X / 2) ||
			    Mathf.Abs(delta.y) > (ArtData.FIELD_GRID_Y / 2))
			{
				Vector2i newPos;
				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
				{
					newPos.x = delta.x < 0 ? (_dragStartGrid.x - 1) : (_dragStartGrid.x + 1);
					newPos.y =_dragStartGrid.y;
				}
				else 
				{
					newPos.x = _dragStartGrid.x;
					newPos.y = delta.y < 0 ? (_dragStartGrid.y + 1) : (_dragStartGrid.y - 1);
				}
				if (Logic.Valid(newPos))
				{
					_dragging = false;
					StartCoroutine(Swap(newPos, _dragStartGrid));
				}
			}
		}
		
		if (!Input.GetMouseButton(0) && _dragging)
		{
			if (_lastTapGridValid &&
			    _dragStartGrid.x == _lastTapGrid.x && Math.Abs(_dragStartGrid.y - _lastTapGrid.y) == 1 ||
			    _dragStartGrid.y == _lastTapGrid.y && Math.Abs(_dragStartGrid.x - _lastTapGrid.x) == 1)
			{
				StartCoroutine(Swap(_dragStartGrid, _lastTapGrid));
				_lastTapGridValid = false;
			}
			else
			{
				_lastTapGrid = _dragStartGrid;
				_lastTapGridValid = true;
			}

			_dragging = false;
		}
	}
}
