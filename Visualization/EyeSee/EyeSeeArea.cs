/*
 * MIT License
 *
 * Copyright (c) 2017 Uwe Gruenefeld, Dag Ennenga, Dana Hsiao
 * University of Oldenburg (GERMANY)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using Toolkit;
using Visualization.Core;
using UnityEngine;

using System;
using System.Collections.Generic;

#pragma warning disable 162

namespace Visualization.EyeSee
{
	/*
	 * EyeSeeArea
	 */
	public class EyeSeeArea : CoreArea
	{
		private GameObject innerBoundary;
		private GameObject[] outerBoundary;

		public Vector2 innerBoundarySize { get; private set; }
		public Vector2 outerBoundarySize { get; private set; }

		private Dictionary<int, GameObject[]> verticalHelpLines;
		private Dictionary<int, GameObject[]> horizontalHelpLines;

		public EyeSeeArea(Vector3 position, Vector2 distance) : base (position, distance) {}

		public override void Init()
		{
			this.area.name = "EyeSeeArea";

			this.verticalHelpLines = new Dictionary<int, GameObject[]>();
			this.horizontalHelpLines = new Dictionary<int, GameObject[]>();

			this.outerBoundarySize = this.OuterBoundarySize();
			this.innerBoundarySize = this.InnerBoundarySize(this.outerBoundarySize);
		}

		public override void Update()
		{
			this.DrawInnerBoundary ();
			this.DrawOuterBoundary ();
			this.DrawVerticalHelplines ();
			this.DrawHorizontalHelplines ();
			this.DrawVerticalZeroline ();
			this.DrawHorizontalZeroline ();

			base.Update ();

			if (EyeSee.ROLL)
			{
				Vector3 rotation = this.area.transform.rotation.eulerAngles;
				this.area.transform.rotation = Quaternion.Euler (rotation.x, rotation.y, 0);
			}
		}

		public override Vector3 Gaze()
		{
			#pragma warning disable 472

			if (this.innerBoundary == null || base.position == null)
				return base.Gaze ();

			#pragma warning restore 472

			return new Vector3 (0, this.innerBoundary.transform.localPosition.y, base.position.z).normalized;
		}

		#region BOUNDARIES

		public float Compress(EnumCompression compression, float value, float maximum)
		{
			switch (compression)
			{
			case EnumCompression.LINEAR:
				break;
			case EnumCompression.SQUARE_ROOT:
				float factor = 1f / 2048f;
				value = value < 0 ? -(Mathf.Pow (Mathf.Abs (value) + 1, factor) - 1) : Mathf.Pow (Mathf.Abs (value) + 1, factor) - 1;
				value /= Mathf.Pow (maximum + 1, factor) - 1;
				value *= maximum;
				break;
			}
			return value;
		}

		private CoreFOV InnerFOV()
		{
			switch (EyeSee.INNER_FOV_REPRESENTATION)
			{
			case EnumFOV.SCREEN:
				return AbstractToolkit.Toolkit().Screen ();
			case EnumFOV.VIEW:
				return AbstractToolkit.Toolkit().View ();
			default:
				return EyeSee.INNER_FOV_REPRESENTATION_CUSTOM;
			}
		}

		private CoreFOV OuterFOV()
		{
			switch (EyeSee.OUTER_FOV)
			{
			case EnumFOV.SCREEN:
				return AbstractToolkit.Toolkit().Screen ();
			case EnumFOV.VIEW:
				return AbstractToolkit.Toolkit().View ();
			default:
				return EyeSee.OUTER_FOV_CUSTOM;
			}
		}

		private void DrawInnerBoundary()
		{
			if (this.innerBoundary == null)
				this.innerBoundary = CoreUtilities.GameObject ("InnerBoundary", Vector3.zero, this.area.transform);

			if (this.InnerFOV().isEllipse)
				CoreUtilities.AddEllipse (this.innerBoundary, this.innerBoundarySize);
			else
				CoreUtilities.AddRectangle (this.innerBoundary, this.innerBoundarySize, EyeSee.INNER_LINE_WIDTH);

			CoreUtilities.SetColor (this.innerBoundary, EyeSee.INNER_LINE_COLOR);

			if(EyeSee.PITCH)
				this.Locate ();
		}

		private void DrawOuterBoundary()
		{
			if (this.outerBoundary == null)
			{
				this.outerBoundary = new GameObject[2];
				this.outerBoundary[0] = CoreUtilities.GameObject("OuterBoundaryA", Vector3.zero, this.area.transform);
				this.outerBoundary[1] = CoreUtilities.GameObject("OuterBoundaryB", Vector3.zero, this.area.transform);
			}

			if (!this.DrawEllipse (this.outerBoundary [0], this.outerBoundarySize, true))
				this.DrawEllipse (this.outerBoundary [1], this.outerBoundarySize, false);
		    else
				CoreUtilities.SetSegments (this.outerBoundary [1], 0);

			CoreUtilities.SetColor (this.outerBoundary[0], EyeSee.OUTER_LINE_COLOR);
			CoreUtilities.SetColor (this.outerBoundary[1], EyeSee.OUTER_LINE_COLOR);
		}

		private void Locate()
		{
			Vector3 position = this.innerBoundary.transform.localPosition;
			float rotation = 0;

			//rotation = CoreUtilities.GetRotation ().eulerAngles.x;
			rotation = AbstractToolkit.Toolkit().Camera().transform.rotation.eulerAngles.x;

			if (rotation > 180)
			{
				rotation = 360 - rotation;
				rotation = Mathf.Min (90, rotation);
			} else
				rotation = -Mathf.Min (90, rotation);

			position.y = (this.outerBoundarySize.y / 10f) * (rotation / 9);
			this.innerBoundary.transform.localPosition = position;
		}

		private Vector2 OuterBoundarySize()
		{
			Vector2 outerBoundarySize = new Vector2();
			Vector2 degreeFOV = this.OuterFOV().degrees;

			outerBoundarySize.x = CalcuateSize (degreeFOV.x);
			outerBoundarySize.y = CalcuateSize (degreeFOV.y);
			return outerBoundarySize;
		}

		private Vector2 InnerBoundarySize(Vector2 outerBoundarySize)
		{
			Vector2 innerBoundarySize = new Vector2();
			Vector2 degreeFOV = this.InnerFOV().degrees;

			innerBoundarySize.x = (outerBoundarySize.x / 360) * degreeFOV.x;
			innerBoundarySize.y = (outerBoundarySize.y / 180) * degreeFOV.y;

			// Portrait camera
			if (degreeFOV.x < degreeFOV.y)
				innerBoundarySize.y = innerBoundarySize.x / 2;

			return new Vector2(
				this.Compress(EyeSee.COMPRESS_X, innerBoundarySize.x, this.outerBoundarySize.x),
				this.Compress(EyeSee.COMPRESS_Y, innerBoundarySize.y, this.outerBoundarySize.y)
			);
		}

		private float CalcuateSize(float degree)
		{
			return Mathf.Tan ((degree * Mathf.Deg2Rad) / 2) * this.position.z;
		}

		private bool OnInner(Vector2 point)
		{
			point = new Vector2 (point.x, point.y - innerBoundary.transform.localPosition.y);

			// Returns true if on inner and false if not
			if (!this.InnerFOV ().isEllipse)
				return ((point.y >= -innerBoundarySize.y) && (point.y <= innerBoundarySize.y)) &&
				((point.x >= -innerBoundarySize.x) && (point.x <= innerBoundarySize.x));
			else
				return ((Mathf.Pow (point.x, 2) / Mathf.Pow (innerBoundarySize.x, 2)) +
					(Mathf.Pow (point.y, 2) / Mathf.Pow (innerBoundarySize.y, 2))) <= 1;
		}

		#endregion

		#region HELPLINES

		private void DrawVerticalHelplines()
		{
			int step = EyeSee.HELPLINE_VERTICAL;
			int i = 1;
			while (step < (EyeSee.OUTER_SIZE.x / 2))
			{
				if (!this.verticalHelpLines.ContainsKey (i) || this.verticalHelpLines [i] == null)
					this.AddVerticalHelpline (i);

				GameObject[] helpline = this.verticalHelpLines [i];

				// Calculate size
				float ratio = ((float)180 / step);
				Vector2 size = outerBoundarySize;
				size.x = (size.x / ratio );
				size.x = this.Compress (EyeSee.COMPRESS_X, size.x, this.outerBoundarySize.x);

				if (!this.DrawEllipse (helpline [0], size, true))
					this.DrawEllipse (helpline [1], size, false);
				else
					CoreUtilities.SetSegments (helpline [1], 0);

				CoreUtilities.SetColor (helpline [0].GetComponent<LineRenderer>(), EyeSee.HELPLINE_COLOR, true);
				CoreUtilities.SetColor (helpline [1].GetComponent<LineRenderer>(), EyeSee.HELPLINE_COLOR, true);

				step += EyeSee.HELPLINE_VERTICAL;
				i++;
			}
		}

		private void DrawHorizontalHelplines()
		{
			float yPositionDistance = 0;
			int i = 1;
			int step = EyeSee.HELPLINE_HORIZONTAL;
			while (step < EyeSee.OUTER_SIZE.y / 2)
			{
				if (!this.horizontalHelpLines.ContainsKey (i) || this.horizontalHelpLines [i] == null)
					this.AddHorizontalHelpline (i);

				GameObject[] helpline = this.horizontalHelpLines [i];

				Vector2 start = Vector2.zero;
				float ratio = (float)90 / step;
				yPositionDistance = outerBoundarySize.y / (ratio);
				yPositionDistance = this.Compress (EyeSee.COMPRESS_Y, yPositionDistance, this.outerBoundarySize.y);
				start.y = yPositionDistance * Mathf.Pow (-1, i);

				start.x = (outerBoundarySize.x / outerBoundarySize.y)
					* Mathf.Sqrt (Mathf.Pow (outerBoundarySize.y, 2) - Mathf.Pow (start.y, 2));
				Vector2 end = new Vector2 (-start.x, start.y);
				if (!this.DrawLine (helpline [0], start, end))
					this.DrawLine (helpline [1], end, start);
				else
					CoreUtilities.SetSegments (helpline [1], 0);

				CoreUtilities.SetColor (helpline [0].GetComponent<LineRenderer> (), EyeSee.HELPLINE_COLOR, true);
				CoreUtilities.SetColor (helpline [1].GetComponent<LineRenderer> (), EyeSee.HELPLINE_COLOR, true);

				if (i % 2 == 0)
					step += EyeSee.HELPLINE_HORIZONTAL;

				i++;
			}
		}

		private void DrawVerticalZeroline()
		{
			if (!EyeSee.ZEROLINE_VERTICAL)
				return;

			GameObject[] zeroline = null;
			if (!this.verticalHelpLines.ContainsKey (0) || this.verticalHelpLines [0] == null)
				zeroline = this.AddVerticalHelpline (0);
			else
				zeroline = this.verticalHelpLines [0];

			Vector2 start = new Vector2 (0, this.outerBoundarySize.y);
			Vector2 end = new Vector2 (0, -this.outerBoundarySize.y);

			if (!this.DrawLine(zeroline[0], start, end))
				this.DrawLine(zeroline[1], end, start);
			else
				CoreUtilities.SetSegments (zeroline [1], 0);

			CoreUtilities.SetColor (zeroline [0].GetComponent<LineRenderer>(), EyeSee.ZEROLINE_COLOR, true);
			CoreUtilities.SetColor (zeroline [1].GetComponent<LineRenderer>(), EyeSee.ZEROLINE_COLOR, true);
		}

		private void DrawHorizontalZeroline()
		{
			if (!EyeSee.ZEROLINE_HORIZONTAL)
				return;

			GameObject[] zeroline = null;
			if (!this.horizontalHelpLines.ContainsKey (0) || this.horizontalHelpLines [0] == null)
				zeroline = this.AddHorizontalHelpline (0);
			else
				zeroline = this.horizontalHelpLines [0];

			Vector2 start = new Vector2 (this.outerBoundarySize.x, 0);
			Vector2 end = new Vector2 (-this.outerBoundarySize.x, 0);

			if (!this.DrawLine(zeroline[0], start, end))
				this.DrawLine(zeroline[1], end, start);
			else
				CoreUtilities.SetSegments (zeroline [1], 0);

			CoreUtilities.SetColor (zeroline [0].GetComponent<LineRenderer>(), EyeSee.ZEROLINE_COLOR, true);
			CoreUtilities.SetColor (zeroline [1].GetComponent<LineRenderer>(), EyeSee.ZEROLINE_COLOR, true);
		}

		private bool DrawEllipse(GameObject gameObject, Vector2 radius, bool direction, float width = EyeSee.HELPLINE_WIDTH)
		{
			LineRenderer lineRenderer = CoreUtilities.AddLineRenderer (gameObject, width);
			lineRenderer.loop = false;

			int count = (int)((radius.x + radius.y) * Mathf.PI * 3);
			CoreUtilities.SetSegments(lineRenderer, count);
			float angle = 90;
			float step = (360f / count);
			int runs = 0;
			for (int i = 0; i <= count; i++)
			{
				if (runs >= count)
				{
					CoreUtilities.SetSegments(lineRenderer, Mathf.Max(0, i - 1));
					return false;
				}
				runs++;

				Vector2 ellipsePoint = new Vector2(
					Mathf.Sin (Mathf.Deg2Rad * angle) * radius.x,
					Mathf.Cos (Mathf.Deg2Rad * angle) * radius.y);

				if(this.OnInner(ellipsePoint))
				{
					if(i == 0)
					{
						angle = direction ? angle + step : angle - step;
						i--;
						continue;
					}
					CoreUtilities.SetSegments(lineRenderer, Mathf.Max(0, i - 1));
					return false;
				}
				lineRenderer.SetPosition (i, ellipsePoint);
				angle = direction ? angle + step : angle - step;
			}

			return true;
		}

		private bool DrawLine(GameObject gameObject, Vector2 start, Vector2 end, float width = EyeSee.HELPLINE_WIDTH)
		{
			LineRenderer lineRenderer = CoreUtilities.AddLineRenderer (gameObject, width);

			int count = (int)(Vector2.Distance(start, end) * 3);
			CoreUtilities.SetSegments(lineRenderer, count);

			Vector2 step = (end - start) / count;

			for(int i = 0; i <= count; i++)
			{
				Vector2 linePoint = start + (step * i);
				if(this.OnInner(linePoint))
				{
					if(i == 0)
					{
						lineRenderer.positionCount = 0;
						return false;
					}

					CoreUtilities.SetSegments(lineRenderer, Mathf.Max(0, i - 1));

					return false;
				}
				lineRenderer.SetPosition(i, linePoint);
			}

			return true;
		}

		private GameObject[] AddVerticalHelpline(int i)
		{
			GameObject[] helpline = this.AddHelpline(i, "Vertical");
			this.verticalHelpLines.Add (i, helpline);
			return helpline;
		}

		private GameObject[] AddHorizontalHelpline(int i)
		{
			GameObject[] helpline = this.AddHelpline(i, "Horizontal");
			this.horizontalHelpLines.Add (i, helpline);
			return helpline;
		}

		private GameObject[] AddHelpline(int i, string name)
		{
			GameObject helplineA = CoreUtilities.GameObject (name + "Helpline" + i + "a", Vector3.zero, this.area.transform);
			GameObject helplineB = CoreUtilities.GameObject (name + "Helpline" + i + "b", Vector3.zero, this.area.transform);
			return new GameObject[]{ helplineA, helplineB };
		}

		#endregion
	}
}

#pragma warning restore 162
