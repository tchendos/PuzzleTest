using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * Class managing all the visual gems. It handles all the animation.
 */
public class Visual
{
	Gem[,] _grid;
	Cache _cache;
	ArtData _artData;

	public Visual(Cache cache, ArtData artData)
	{
		_cache = cache;
		_artData = artData;
		_grid = new Gem[ArtData.FIELD_SIZE_X, ArtData.FIELD_SIZE_Y];
	}

	/**
	 * Generates visual representation of the game state in logic.
	 */
	public void Generate(Logic logic)
	{
		GameObject prefab = _artData.GemBasePrefab;
		prefab.SetActive(true);

		GameObject go;
		Gem gem;
		Vector2i gridPos;
		for(gridPos.x = 0; gridPos.x < ArtData.FIELD_SIZE_X; gridPos.x++)
		{
			for(gridPos.y = 0; gridPos.y < ArtData.FIELD_SIZE_Y; gridPos.y++)
			{
				go = GameObject.Instantiate(prefab) as GameObject;
				go.name = "gemInst" + gridPos.x + "_" + gridPos.y;
				go.transform.parent = _artData.FieldRoot.transform;
				go.transform.localScale = Vector3.one;

				gem = go.GetComponent<Gem>();
				gem.Init(_cache, gridPos, logic.GetGemType(gridPos));
				_grid[gridPos.x, gridPos.y] = gem;
			}
		}

		prefab.SetActive(false);
	}

	public bool IsMoving(Vector2i pos)
	{
		return _grid[pos.x, pos.y].IsMoving();
	}

	/**
	 * Detects, whether some gem fell into its place and marks it as still.
	 */
	public bool AnyGemInPlace()
	{
		bool ret = false;
		Vector2i pos;
		for(pos.x = 0; pos.x < ArtData.FIELD_SIZE_X; pos.x++)
		{
			for(pos.y = 0; pos.y < ArtData.FIELD_SIZE_Y; pos.y++)
			{
				if (_grid[pos.x, pos.y].InPlace())
				{
					_grid[pos.x, pos.y].SetMoving(false);
					ret = true;
				}
			}
		}
		return ret;
	}

	/**
	 * Fills mask with gems not currently moving.
	 */
	public void FillInPlaceMask(bool[,] inPlace)
	{
		Vector2i pos;
		for(pos.x = 0; pos.x < ArtData.FIELD_SIZE_X; pos.x++)
		{
			for(pos.y = 0; pos.y < ArtData.FIELD_SIZE_Y; pos.y++)
			{
				inPlace[pos.x, pos.y] = !_grid[pos.x, pos.y].IsMoving();
			}
		}
	}

	/**
	 * Animates gem disappearing.
	 */
	public void Collect(bool[,] matched)
	{
		float time = _artData.MatchFadeTime;

		Vector2i pos;
		for(pos.x = 0; pos.x < ArtData.FIELD_SIZE_X; pos.x++)
		{
			for(pos.y = 0; pos.y < ArtData.FIELD_SIZE_Y; pos.y++)
			{
				if (matched[pos.x, pos.y])
				{
					_grid[pos.x, pos.y].FadeOut(time);
				}
			}
		}
	}

	/**
	 * Applies changes from game logic passed in the form of simple array with counts of
	 * grids to fall for each gem.
	 */
	public void ApplyLogicUpdate(Logic logic, int[,] gemUpdates)
	{
		Vector2i pos;
		int dist;
		for(pos.x = 0; pos.x < ArtData.FIELD_SIZE_X; pos.x++)
		{
			for(pos.y = ArtData.FIELD_SIZE_Y - 1; pos.y >= 0; pos.y--)
			{
				dist = gemUpdates[pos.x, pos.y];
				if (dist > 0)
				{
					Gem gem = _grid[pos.x, pos.y];
					if (dist <= pos.y)
					{
						_grid[pos.x, pos.y] = _grid[pos.x, pos.y - dist];
						_grid[pos.x, pos.y - dist] = gem;
						gem = _grid[pos.x, pos.y];
					}
					else 
					{
						Gem bottomGem = null;
						if ((pos.y + 1) < ArtData.FIELD_SIZE_Y)
							bottomGem = _grid[pos.x, pos.y + 1];
						gem.SetPosition(bottomGem, pos + Vector2i.up * dist);
						gem.SetGemType(logic.GetGemType(pos));
					}
					gem.MoveTo(pos, _artData.CollapseFieldVelocity);
					gem.SetMoving(true);
				}
			}
		}
	}

	/**
	 * Animates swapping of the gems. 
	 * Sets the moving flag of the gems, so the game loop can detect when 
	 * the animation is finished and start to collapse the gem field.
	 */
	public void SwapGems(Vector2i newPos, Vector2i lastPos)
	{
		float velocity = _artData.SwapGemsVelocity;
		
		Gem newGem = _grid[newPos.x, newPos.y];
		Gem oldGem = _grid[lastPos.x, lastPos.y];

		_grid[newPos.x, newPos.y] = oldGem;
		oldGem.MoveTo(newPos, velocity);
		oldGem.SetMoving(true);
		_grid[lastPos.x, lastPos.y] = newGem;
		newGem.MoveTo(lastPos, velocity);
		newGem.SetMoving(true);
	}

	/**
	 * Animates invalid gem swap. It is blocking operation.
	 */
	public IEnumerator SwapGemsFail(Vector2i newPos, Vector2i lastPos)
	{
		float velocity = _artData.SwapGemsFailVelocity;
		
		_grid[newPos.x, newPos.y].MoveTo(lastPos, velocity);
		float time = _grid[lastPos.x, lastPos.y].MoveTo(newPos, velocity);
		
		yield return new WaitForSeconds(time);
		
		_grid[newPos.x, newPos.y].MoveTo(newPos, velocity);
		time = _grid[lastPos.x, lastPos.y].MoveTo(lastPos, velocity);

		yield return new WaitForSeconds(time);
	}
}
