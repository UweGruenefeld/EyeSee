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
using Visualization.Core;
using Toolkit;
using UnityEngine;

using System;


namespace Visualization.EyeSee
{
	/*
	 * EyeSee
	 */
	public class EyeSee : CoreTechnique
	{
		public const bool 				ROLL = false;
		public const bool 				PITCH = true;
		public const bool 				YAW = true;

		public const EnumCompression 	COMPRESS_X = EnumCompression.SQUARE_ROOT;
		public const EnumCompression	COMPRESS_Y = EnumCompression.SQUARE_ROOT;

		public const EnumFOV 			INNER_FOV_REPRESENTATION = EnumFOV.VIEW;
		public static CoreFOV			INNER_FOV_REPRESENTATION_CUSTOM = new CoreFOV (true, new Vector2 (180, 90));
		public const float 				INNER_LINE_WIDTH = 0.06f;
		public static Color 			INNER_LINE_COLOR = Color.white;

		public static Vector2			OUTER_SIZE = new Vector2 (360, 180);
		public const EnumFOV 			OUTER_FOV = EnumFOV.SCREEN;
		public static CoreFOV 		OUTER_FOV_CUSTOM = new CoreFOV(false, new Vector2(10, 10));
		public const float 				OUTER_LINE_WIDTH = 0.06f;
		public static Color 			OUTER_LINE_COLOR = Color.white;

		public const bool 				ZEROLINE_VERTICAL = true;
		public const bool 				ZEROLINE_HORIZONTAL = true;
		public const float 				ZEROLINE_WIDTH = 0.06f;
		public static Color 			ZEROLINE_COLOR = Color.white;

		public const int 					HELPLINE_VERTICAL = 45;
		public const int					HELPLINE_HORIZONTAL = 45;
		public const float 				HELPLINE_WIDTH = 0.06f;
		public static Color 			HELPLINE_COLOR = Color.white;

		public const float 				PROXY_SIZE = 5f;

		public const bool 				DISTANCE_COLOR = true;
		public static Color 			DISTANCE_MIN_COLOR = Color.red;
		public static Color 			DISTANCE_MAX_COLOR = Color.blue;
		public const bool 				DISTANCE_RESIZE = false;
		public const float 				DISTANCE_MIN_RESIZE = 1f;
		public const float 				DISTANCE_MAX_RESIZE = 10f;

		public override Type GetArea()
		{
			return typeof(EyeSeeArea);
		}

		public override Type GetProxy()
		{
			return typeof(EyeSeeProxy);
		}

		public override bool HasRotationVisualization()
		{
			return true;
		}

		public override bool HasDistanceVisualization()
		{
			return true;
		}
	}
}
