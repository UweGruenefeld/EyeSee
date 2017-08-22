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
using Visualization.Studies;
using Toolkit;
using UnityEngine;

using System;
using System.Collections.Generic;

namespace Visualization.Core
{
	/*
	 * CoreArea
	 */
	public class CoreArea
	{
		public const float AREA_SCALE_X = 4f;
		public const float AREA_SCALE_Y = 4f;
		public static readonly Color AREA_COLOR = Color.black;

		public GameObject area { get; private set; }
		public List<CoreProxy> proxies { get; private set; }
		public Vector3 position { get; private set; }
		public Vector2 distance { get; private set; }

		protected GameObject parent { get; private set; }
		protected bool visible;

		public CoreArea(Vector3 position, Vector2 distance)
		{
			this.position = position;
			this.distance = distance;

			this.area = CoreUtilities.GameObject (
				"Area",
				position,
				AbstractToolkit.Toolkit().Root().transform
			);
			this.area.AddComponent<SphereCollider> ().radius = 1;

			this.proxies = new List<CoreProxy> ();

			this.visible = false;
		}

		public virtual void Init()
		{
			CoreUtilities.AddEllipse (this.area, new Vector2(CoreArea.AREA_SCALE_X, CoreArea.AREA_SCALE_Y));
			CoreUtilities.SetColor (this.area, CoreArea.AREA_COLOR);
		}

		public virtual Vector3 Gaze()
		{
			return new Vector3 (0, 0, 1);
		}

		public virtual void UpdateProxies(EnumLimitation limit)
		{
			// Check if proxies are active
			foreach(CoreProxy proxy in this.proxies)
			{
				switch (limit) {
				case EnumLimitation.NO_LIMITATION:
					{
						proxy.SetVisible (true);
						break;
					}
				case EnumLimitation.OFF_SCREEN:
					{
						if (AbstractToolkit.Toolkit().OutOfFOV (EnumFOV.SCREEN, proxy.coreObject.position))
							proxy.SetVisible (true);
						else
							proxy.SetVisible (false);
						break;
					}
				case EnumLimitation.OUT_OF_VIEW:
					{
						if (AbstractToolkit.Toolkit().OutOfFOV (EnumFOV.VIEW, proxy.coreObject.position))
							proxy.SetVisible (true);
						else
							proxy.SetVisible (false);
						break;
					}
				}
			}
		}

		public virtual void Update()
		{
			this.area.transform.LookAt(this.area.transform.position +
				AbstractToolkit.Toolkit().Camera().transform.rotation * Vector3.forward,
				AbstractToolkit.Toolkit().Camera().transform.rotation * Vector3.up);

			foreach (CoreProxy proxy in this.proxies)
			{
				if (!this.visible)
					proxy.SetVisible (false);

				proxy.Update ();
			}
		}

		public virtual void Destroy()
		{
			foreach (CoreProxy proxy in this.proxies)
				proxy.Destroy ();

			CoreUtilities.Destroy (this.area);
		}

		public void SetVisible(bool visible)
		{
			this.visible = visible;
			CoreUtilities.SetVisible (this.area, visible);
		}

		public bool IsVisible()
		{
			return this.visible;
		}

		public CoreProxy AddProxy(CoreObject coreObject, CoreTechnique coreTechnique)
		{
			CoreProxy coreProxy = (CoreProxy)Activator.CreateInstance (coreTechnique.GetProxy(), this, coreObject);
			this.proxies.Add (coreProxy);
			return coreProxy;
		}

		public void RemoveProxy(CoreObject coreObject)
		{
			CoreProxy coreProxy = null;
			foreach(CoreProxy proxy in this.proxies)
				if(proxy.coreObject == coreObject)
				{
					coreProxy = proxy;
					break;
				}

			if(coreProxy != null)
				this.proxies.Remove (coreProxy);
		}
	}
}
