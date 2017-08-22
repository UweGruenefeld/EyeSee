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
using Visualization.Core;
using UnityEngine;
using UnityEngine.VR;

using System;

namespace Toolkit
{
	public class OculusToolkit : AbstractToolkit
	{
		public override void Awake()
		{
			AbstractToolkit.toolkit = this;
			base.Awake();

			// VR Settings
			VRSettings.enabled = true;

			// Check supported devices
			bool oculusSupport = false;
			foreach (String device in VRSettings.supportedDevices)
			{
				if (device.ToLower ().Contains ("oculus"))
				{
					oculusSupport = true;
					break;
				}
			}
			if (!oculusSupport)
			{
				Debug.LogError ("It is required to add \"Oculus\" as a Virtual Reality SDK in PlayerSettings," +
					"to enable \"Virtual Reality Supported\" and to switch the platform in the build settings to \"PC, Mac and Linux Standalone\".");
				AbstractToolkit.Close ();
			}

			this.initialized = true;
		}

		public override CoreFOV Screen()
		{
			return new CoreFOV (false, new Vector2 (110f, 80f));
		}

		public override CoreFOV View()
		{
			return Screen();
		}
	}
}
