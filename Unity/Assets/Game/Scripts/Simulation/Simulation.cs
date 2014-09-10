using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * Connects user input with game logic and visualisation.
 */
public class Simulation:MonoBehaviour
{
	Logic _logic;
	Visual _visual;
	BurnTimer _timer;
	bool _ignoreSwipe;

	public void Init(Logic logic, Visual visual, BurnTimer timer)
	{
		_logic = logic;
		_visual = visual;
		_timer = timer;

		_logic.Generate();
		_visual.Generate(_logic);
		StartCoroutine(TimeOut(_timer.GetTimeOut()));

		StartCoroutine(Process());
	}

	/**
	 * Checks, whether two gems are not moving and detects, whether swapping the 
	 * gems will generate a match. 
	 * Depending on the result it swaps the gems and lets the simulation to perform 
	 * collapsing or just animates invalid swap.
	 */
	public IEnumerator Swap(Vector2i newPos, Vector2i lastPos)
	{
		if (_ignoreSwipe)
			yield break;

		if (!_visual.IsMoving(newPos) &&
		    !_visual.IsMoving(lastPos))
		{
			bool[,] mask = new bool[ArtData.FIELD_SIZE_X,ArtData.FIELD_SIZE_Y];
			_visual.FillInPlaceMask(mask);
			if (_logic.IsValidSwap(mask, newPos, lastPos))
			{
				_logic.SwapGems(newPos, lastPos);
				_visual.SwapGems(newPos, lastPos);
			}
			else 
			{
				yield return StartCoroutine(_visual.SwapGemsFail(newPos, lastPos));
			}
		}
	}

	/**
	 * Main game loop.
	 */
	IEnumerator Process()
	{
		bool[,] mask = new bool[ArtData.FIELD_SIZE_X,ArtData.FIELD_SIZE_Y];
		bool[,] matched = new bool[ArtData.FIELD_SIZE_X,ArtData.FIELD_SIZE_Y];

		do
		{
			// any gem fell into its place?
			if (_visual.AnyGemInPlace())
			{
				// crate mask of gems that are not moving
				// only these will be used for detecting matches
				_visual.FillInPlaceMask(mask);
				// checks matched gems
				bool any = _logic.CheckMatches(mask, matched);

				if (any)
				{
					// animates (shrinks / disappears) all the matched gems
					_visual.Collect(matched);

					// collapses gems and generates new ones
					int[,] gemUpdates = _logic.Collapse(matched);
					_logic.Log();

					// applies changes made in logic to visual - animates falling of the gems
					_visual.ApplyLogicUpdate(_logic, gemUpdates);
				}
			}
			yield return null;
		} while (true);
	}

	IEnumerator TimeOut(float time)
	{
		yield return new WaitForSeconds(time);
		
		_ignoreSwipe = true;
		yield return StartCoroutine(_timer.Boom(_logic.GetScore()));
	}
}
