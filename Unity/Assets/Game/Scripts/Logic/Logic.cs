using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * Class contains game state featuring gem grid and score. 
 * 
 * It is able to perform operations like generating gems on the grid,
 * swapping of two gems, detecting matched gems and collapsing the gem 
 * grid.
 * 
 * It also counts score, although in very basic fashion (each matched gem 
 * increases score by 1)
 */
public class Logic
{
	int _score;
	int[,] _grid;
	
	LevelData _levelData;

	public Logic(LevelData levelData)
	{
		_levelData = levelData;
		_score = 0;
		_grid = new int[ArtData.FIELD_SIZE_X,ArtData.FIELD_SIZE_Y];
	}

	public int GetGemType(Vector2i pos) { return _grid[pos.x, pos.y]; }
	public int GetScore() { return _score; }

	static public bool Valid(Vector2i pos)
	{
		return (pos.x >= 0 && pos.x < ArtData.FIELD_SIZE_X &&
		        pos.y >= 0 && pos.y < ArtData.FIELD_SIZE_Y);
	}

	static int GenerateGemType(HashSet<int> set)
	{
		int rnd = UnityEngine.Random.Range (0, set.Count);
		foreach(int index in set)
		{
			if (rnd == 0)
			{
				return index;
			}
			rnd--;
		}
		return -1;
	}

	public void Generate()
	{
		bool[,] mask = new bool[ArtData.FIELD_SIZE_X, ArtData.FIELD_SIZE_Y];
		Vector2i gridPos;
		for(gridPos.x = 0; gridPos.x < ArtData.FIELD_SIZE_X; gridPos.x++)
		{
			for(gridPos.y = 0; gridPos.y < ArtData.FIELD_SIZE_Y; gridPos.y++)
			{
				_grid[gridPos.x, gridPos.y] = GenerateGemType(_levelData.GetStartGemIndices());
				mask[gridPos.x, gridPos.y] = true;
			}
		}

		bool[,] matched = new bool[ArtData.FIELD_SIZE_X, ArtData.FIELD_SIZE_Y];
		bool any;
		do
		{
			any = CheckMatches(mask, matched);

			if (any)
				Collapse(matched);

		} while (any);
	}

	public bool IsValidSwap(bool[,] mask, Vector2i newPos, Vector2i lastPos)
	{
		SwapGems(newPos, lastPos);
		bool[,] matched = new bool[ArtData.FIELD_SIZE_X, ArtData.FIELD_SIZE_Y];
		bool any = CheckMatches(mask, matched);
		SwapGems(newPos, lastPos);

		return any;
	}

	public void SwapGems(Vector2i newPos, Vector2i lastPos)
	{
		int newGem = _grid[newPos.x, newPos.y];
		_grid[newPos.x, newPos.y] = _grid[lastPos.x, lastPos.y];
		_grid[lastPos.x, lastPos.y] = newGem;
	}

	/**
	 * Detects three or more adjacent gems of the same color and returns matches 
	 * as a two dimensional boolean array. It ignores gems marked in mask.
	 */
	public bool CheckMatches(bool[,] mask, bool[,] matches)
	{
		Vector2i gridPos;
		bool any = false;
		for(gridPos.x = 0; gridPos.x < ArtData.FIELD_SIZE_X; gridPos.x++)
		{
			for(gridPos.y = 0; gridPos.y < ArtData.FIELD_SIZE_Y; gridPos.y++)
			{
				if (mask[gridPos.x, gridPos.y])
				{
					int type = _grid[gridPos.x, gridPos.y];
					if ((gridPos.x + 2) < ArtData.FIELD_SIZE_X &&
					    mask[gridPos.x + 1, gridPos.y] && CheckMatch(type, _grid[gridPos.x + 1, gridPos.y]) &&
					    mask[gridPos.x + 2, gridPos.y] && CheckMatch(type, _grid[gridPos.x + 2, gridPos.y]))
					{
						matches[gridPos.x, gridPos.y] = true;
						matches[gridPos.x + 1, gridPos.y] = true;
						matches[gridPos.x + 2, gridPos.y] = true;
						any = true;
					}
					if ((gridPos.y + 2) < ArtData.FIELD_SIZE_Y &&
					    mask[gridPos.x, gridPos.y + 1] && CheckMatch(type, _grid[gridPos.x, gridPos.y + 1]) &&
					    mask[gridPos.x, gridPos.y + 2] && CheckMatch(type, _grid[gridPos.x, gridPos.y + 2]))
					{
						matches[gridPos.x, gridPos.y] = true;
						matches[gridPos.x, gridPos.y + 1] = true;
						matches[gridPos.x, gridPos.y + 2] = true;
						any = true;
					}
				}
			}
		}
		
		return any;
	}

	/**
	 * Collapses gems based on the matches marked in the input array.
	 * It generates updates array for visualisation.
	 */
	public int[,] Collapse(bool[,] matched)
	{
		int[,] gemUpdates = new int[ArtData.FIELD_SIZE_X,ArtData.FIELD_SIZE_Y];
		int x, y, yy, ctr;
		for(x = 0; x < ArtData.FIELD_SIZE_X; x++)
		{
			ctr = 0;
			for(y = ArtData.FIELD_SIZE_Y - 1; y >= 0; y--)
			{
				if (matched[x,y])
				{
					_score++;

					for(yy = y; yy > 0; yy--)
					{
						_grid[x, yy] = _grid[x, yy - 1];
						matched[x, yy] = matched[x, yy - 1];
					}

					_grid[x, 0] = GenerateGemType(_levelData.GetBasicGemIndices());
					matched[x, 0] = false;

					gemUpdates[x, y] = ++ctr;

					y++;
				}
				else gemUpdates[x, y] = ctr;
			}
		}

		return gemUpdates;
	}

	public void Log()
	{
		if (Application.isEditor)
		{
			string str = "";
			for(int y = 0; y < ArtData.FIELD_SIZE_Y; y++)
			{
				for(int x = 0; x < ArtData.FIELD_SIZE_X; x++)
				{
					LevelData.GemTypeInfo gi = _levelData.GetGemTypeInfo(_grid[x,y]);
					str += gi.data.ToString() + " ";
				}
				str += "\n";
			}
			Debug.Log(str);
		}
	}

	bool CheckMatch(int type1, int type2)
	{
		LevelData.GemTypeInfo ti1 = _levelData.GetGemTypeInfo(type1);
		LevelData.GemTypeInfo ti2 = _levelData.GetGemTypeInfo(type2);
		
		if (ti1.colored && ti2.colored && ti1.data == ti2.data)
			return true;
		
		return false;
	}
}
