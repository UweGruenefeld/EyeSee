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
#define VUFORIA

using Visualization.Core;
using UnityEngine;
using UnityEngine.VR;

#if VUFORIA
using Vuforia;
#endif

using System;

namespace Toolkit
{
	public class VuforiaToolkit : AbstractToolkit
	{
		public override void Awake()
		{
			AbstractToolkit.toolkit = this;
			base.Awake();

			// VR Settings
			VRSettings.enabled = false;

			#if VUFORIA

			VuforiaBehaviour vuforiaBehaviour = this.GetComponent<VuforiaBehaviour>();
			vuforiaBehaviour.enabled = false;

			// Initialize Vuforia
			VuforiaARController vuforiaARController = VuforiaARController.Instance;
			vuforiaARController.RegisterVuforiaStartedCallback (() => WaitInit());

			VuforiaRuntime.Instance.InitVuforia ();
			vuforiaBehaviour.enabled = true;

			#else

			Debug.LogError("Please import Vuforia 6.2.10 as Unity package and uncomment \"#undef VUFORIA\" in VuforiaToolkit.cs");

			#endif
		}

		private void WaitInit()
		{
			#if VUFORIA

			MixedRealityController.Instance.SetMode(MixedRealityController.Mode.ROTATIONAL_VIEWER_AR);
			CameraDevice.Instance.SelectVideoMode(CameraDevice.CameraDeviceMode.MODE_OPTIMIZE_SPEED);

			this.initialized = true;

			#endif
		}

		public override CoreFOV Screen()
		{
			#if VUFORIA

			Vector2 result = CameraDevice.Instance.GetCameraFieldOfViewRads ();

			#else

			Vector2 result = Vector2.zero;

			#endif

			result.x *= Mathf.Rad2Deg;
			result.y *= Mathf.Rad2Deg;

			return new CoreFOV (false, result);
		}

		public override CoreFOV View()
		{
			return this.Screen ();
		}
	}
}
