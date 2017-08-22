/*
 * MIT License
 *
 * Copyright (c) 2017 Uwe Grünefeld
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
using UnityEngine;

namespace Visualization.Core
{
	/*
	 * CoreObject
	 */
	public class CoreObject
	{
		public const string NAME = "CoreObject";
		public static readonly Color SELECTED = Color.green;
		public static readonly Color UNSELECTED = Color.blue;

		public Vector3 position { get; private set; }
		public GameObject gameObject { get; private set; }

		public CoreObject(Vector3 position)
		{
			this.position = position;

			this.gameObject = UnityEngine.GameObject.CreatePrimitive (PrimitiveType.Cube);
			this.gameObject.name = CoreObject.NAME;
			this.gameObject.transform.position = position;
			this.gameObject.AddComponent<BoxCollider> ();

			this.Visible (false);
			this.Select (false);
		}

		public void Select(bool select)
		{
			if(select)
				CoreUtilities.SetColor (this.gameObject, CoreObject.SELECTED);
			else
				CoreUtilities.SetColor (this.gameObject, CoreObject.UNSELECTED);
		}

		public void Visible(bool visible)
		{
			CoreUtilities.SetVisible (this.gameObject, visible);
		}

		public void Destroy()
		{
			if (gameObject != null)
				CoreUtilities.Destroy (gameObject);
		}
	}
}
