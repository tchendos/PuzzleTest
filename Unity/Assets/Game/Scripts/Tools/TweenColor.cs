using UnityEngine;

/// <summary>
/// Tween the object's color.
/// </summary>

public class TweenColor : Tweener
{
	public Color from = Color.white;
	public Color to = Color.white;

	SpriteRenderer mSprite;
	Material mMat;

	/// <summary>
	/// Current color.
	/// </summary>

	public Color color
	{
		get
		{
			if (mSprite != null) return mSprite.color;
			if (mMat != null) return mMat.color;
			return Color.black;
		}
		set
		{
			if (mMat != null) mMat.color = value;

			if (mSprite != null)
				mSprite.color = value;
		}
	}

	/// <summary>
	/// Find all needed components.
	/// </summary>

	void Awake ()
	{
		Renderer ren = renderer;
		if (ren != null) 
		{
			if (ren.material.HasProperty("_Color"))
				mMat = ren.material;

			SpriteRenderer sRen = ren as SpriteRenderer;
			if (sRen != null)
				mSprite = sRen;
		}
	}

	/// <summary>
	/// Interpolate and update the color.
	/// </summary>

	protected override void OnUpdate(float factor, bool isFinished) { color = Color.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenColor Begin (GameObject go, float duration, Color color)
	{
		TweenColor comp = Tweener.Begin<TweenColor>(go, duration);
		comp.from = comp.color;
		comp.to = color;

		if (duration <= 0f)
		{
			comp.color = color;
			comp.enabled = false;
		}
		return comp;
	}

	public static void BeginOnChildren(GameObject go, float duration, Color color, Tweener.Method method)
	{
		if (go.activeSelf && go.renderer != null)
		{
			TweenColor tweenColor = Begin(go, duration, color);
			tweenColor.method = method;
			if (duration == 0)
				tweenColor.color = color;
		}
		
		for(int i = 0; i < go.transform.childCount; i++)
			BeginOnChildren(go.transform.GetChild(i).gameObject, duration, color, method);
	}
}
