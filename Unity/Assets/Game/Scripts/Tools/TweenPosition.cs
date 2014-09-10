using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

public class TweenPosition : Tweener
{
	public Vector3 from;
	public Vector3 to;

	Transform mTrans;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }
	public Vector3 position { get { return cachedTransform.localPosition; } set { cachedTransform.localPosition = value; } }

	protected override void OnUpdate (float factor, bool isFinished) { cachedTransform.localPosition = from * (1f - factor) + to * factor; }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenPosition Begin (GameObject go, float duration, Vector3 pos)
	{
		TweenPosition comp = Tweener.Begin<TweenPosition>(go, duration);
		comp.from = comp.position;
		comp.to = pos;

		if (duration <= 0f)
		{
			comp.position = pos;
			comp.enabled = false;
		}
		return comp;
	}
}
