/*
 * MIT License
 *
 * Copyright (c) 2017 Uwe Grünefeld, Dana Hsiao
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
using Visualization.Core;
using UnityEngine;

using System;

namespace Toolkit
{
	public abstract class AbstractToolkit : MonoBehaviour
	{
		public bool initialized { get; protected set; }
		protected static AbstractToolkit toolkit = null;

		public static AbstractToolkit Toolkit()
		{
			if (AbstractToolkit.toolkit == null)
			{
				Debug.LogError ("Please add a camera prefab to the scene.");
				AbstractToolkit.Close ();
			}
			return AbstractToolkit.toolkit;
		}

		public static void Close()
		{
			#if UNITY_EDITOR

			UnityEditor.EditorApplication.isPlaying = false;

			#else

			Application.Quit ();

			#endif
		}

		public abstract CoreFOV Screen();
		public abstract CoreFOV View();

		public virtual void Awake()
		{
			this.initialized = false;
		}

		public virtual GameObject Root()
		{
			return this.gameObject;
		}

		public virtual Camera Camera()
		{
			return UnityEngine.Camera.main;
		}

		public virtual bool OutOfFOV(EnumFOV fov, Vector3 point)
		{
			switch(fov)
			{
				case EnumFOV.VIEW:
					return this.OutOfFOV (this.View (), point);
				case EnumFOV.SCREEN:
					return this.OutOfFOV (this.Screen (), point);
				case EnumFOV.CUSTOM:
					return false;
			}
			return false;
		}

		public virtual bool OutOfFOV(CoreFOV fov, Vector3 point)
		{
			Vector3 transformedPoint = this.Camera().transform.InverseTransformPoint (point);

			if (transformedPoint.z < 0)
				return true;

			Vector2 fieldOfView = fov.ToRadians();
			float maxYCoordinate = transformedPoint.z * Mathf.Tan (fieldOfView.y / 2f);
			float maxXCoordinate = (fieldOfView.x / fieldOfView.y) * maxYCoordinate;

			if(fov.isEllipse)
				if ((Mathf.Pow(transformedPoint.x, 2) / Mathf.Pow(maxXCoordinate, 2)) +
					(Mathf.Pow(transformedPoint.y, 2) / Mathf.Pow(maxYCoordinate, 2)) <= 1)
					return false;
			else
				if (transformedPoint.y >= -maxXCoordinate && transformedPoint.y <= maxYCoordinate &&
					transformedPoint.x >= -maxXCoordinate && transformedPoint.x <= maxXCoordinate)
					return false;

			return true;
		}
	}
}
