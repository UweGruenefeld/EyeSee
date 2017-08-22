/*
 * MIT License
 *
 * Copyright (c) 2017 Uwe Gruenefeld, Dag Ennenga
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
using System.Collections;


#pragma warning disable 162

namespace Visualization.EyeSee
{
	/*
	 * EyeSeeProxy
	 */
	public class EyeSeeProxy : CoreProxy
	{
		public EyeSeeProxy(CoreArea coreArea, CoreObject coreObject) : base(coreArea, coreObject)
		{
			CoreUtilities.AddCircle (base.coreProxy);
			base.coreProxy.transform.localRotation = Quaternion.Euler(0, 180, 0);
		}

		protected override void UpdateColor()
		{
			if (!EyeSee.DISTANCE_COLOR || this.selected)
			{
				base.UpdateColor ();
				return;
			}

			float distance = this.relativeDistance ();
			float inverted = 1 - distance;

			CoreUtilities.SetColor (this.coreProxy, EyeSee.DISTANCE_MIN_COLOR * inverted + EyeSee.DISTANCE_MAX_COLOR * distance);
		}

		protected override void UpdatePosition()
		{
			if (base.coreProxy == null)
				return;

			// Transform object position to camera space
			Matrix4x4 matrix = Matrix4x4.TRS (
				AbstractToolkit.Toolkit().Camera().transform.position,
				Quaternion.Euler (
					0,
					AbstractToolkit.Toolkit().Camera().transform.rotation.eulerAngles.y,
					0
				),
				Vector3.one
			);
			Vector3 objectToCamera = matrix.inverse.MultiplyPoint(this.coreObject.position);

			// Theta = y-angle of object position
			float theta = Mathf.Min((EyeSee.OUTER_SIZE.y / 2), Vector3.Angle (objectToCamera, new Vector3 (objectToCamera.x, 0, objectToCamera.z)));

			// Phi = x-angle of object position
			float phi = Mathf.Min((EyeSee.OUTER_SIZE.x / 2), Vector3.Angle (new Vector3(0, 0, 1), new Vector3 (objectToCamera.x, 0, objectToCamera.z)));

			Vector2 position = new Vector2 (phi, theta);

			// Specify algebraic sign
			if (objectToCamera.x < 0)
				position.x *= -1;
			if (objectToCamera.y < 0)
				position.y *= -1;

			Vector2 outerBoundarySize = ((EyeSeeArea)this.coreArea).outerBoundarySize;

			// Calculate position
			float yValue = Mathf.Min(position.y / (EyeSee.OUTER_SIZE.y / 2), 1) * outerBoundarySize.y;
			float limit = Mathf.Sqrt (Mathf.Pow (outerBoundarySize.y, 2) - Mathf.Pow (yValue, 2)) * (outerBoundarySize.x / outerBoundarySize.y);
			float xValue = Mathf.Min (position.x / (EyeSee.OUTER_SIZE.x / 2), 1) * limit;

			// Calculate compression
			position = new Vector2(
				((EyeSeeArea)this.coreArea).Compress (EyeSee.COMPRESS_X, xValue, limit),
				((EyeSeeArea)this.coreArea).Compress (EyeSee.COMPRESS_Y, yValue, outerBoundarySize.y)
			);

			this.coreProxy.transform.localPosition = new Vector3(position.x, position.y, -this.z);
	    }

		protected override void UpdateScale()
		{
			Vector2 outerBoundarySize = ((EyeSeeArea)this.coreArea).outerBoundarySize;
			float factor = Mathf.Min(outerBoundarySize.x, outerBoundarySize.y) / 30f;

			if (!EyeSee.DISTANCE_RESIZE)
			{
				this.coreProxy.transform.localScale = new Vector3 (EyeSee.PROXY_SIZE * factor, EyeSee.PROXY_SIZE * factor, 1);
				return;
			}

			float distance = this.relativeDistance ();
			distance *= EyeSee.DISTANCE_MAX_RESIZE * factor - EyeSee.DISTANCE_MIN_RESIZE * factor;
			distance += EyeSee.PROXY_SIZE * factor;

			this.coreProxy.transform.localScale = new Vector3 (distance, distance, 1);
		}

		protected override void UpdateRotation() {}

		private float relativeDistance()
		{
			float distance = this.coreObject.position.magnitude;
			distance = Mathf.Max (this.coreArea.distance.x, Mathf.Min(this.coreArea.distance.y, distance));
			distance -= this.coreArea.distance.x;
			distance /= this.coreArea.distance.y - this.coreArea.distance.x;
			return distance;
		}
	}
}

#pragma warning restore 162
