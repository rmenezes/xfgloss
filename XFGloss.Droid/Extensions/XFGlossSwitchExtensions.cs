﻿/*
 * Copyright (C) 2016 Ansuria Solutions LLC & Tommy Baggett: 
 * http://github.com/tbaggett
 * http://twitter.com/tbaggett
 * http://tommyb.com
 * http://ansuria.com
 * 
 * The MIT License (MIT) see GitHub For more information
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.V4.Graphics.Drawable;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XFGloss.Droid.Utils;
using AColor = Android.Graphics.Color;
using ASwitch = Android.Widget.Switch;
using ASwitchCompat = Android.Support.V7.Widget.SwitchCompat;

namespace XFGloss.Droid.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="T:Xamarin.Forms.Switch"/> control to apply the
	/// <see cref="T:Xamarin.Forms.Color"/> values to an Android Switch control
	/// </summary>
	public static class XFGlossSwitchExtensions
	{
		/// <summary>
		/// An extension method that applies all of the current properties defined by the passed
		/// <see cref="T:XFGloss.ISwitchGloss"/> interface implementation to the Android Switch control
		/// </summary>
		/// <param name="control">Control.</param>
		/// <param name="properties">Properties.</param>
		/// <param name="propertyName">Property name.</param>
		public static void UpdateColorProperty(this Android.Widget.Switch control, ISwitchGloss properties, string propertyName)
		{
			ApplyColorProperty(control, properties, propertyName);
		}

		/// <summary>
		/// An extension method that applies all of the current properties defined by the passed
		/// <see cref="T:XFGloss.ISwitchGloss"/> interface implementation to the Android SwitchCompat control
		/// </summary>
		/// <param name="control">Control.</param>
		/// <param name="properties">Properties.</param>
		/// <param name="propertyName">Property name.</param>
		public static void UpdateColorProperty(this Android.Support.V7.Widget.SwitchCompat control,
											   ISwitchGloss properties,
											   string propertyName)
		{
			// Use the internal ApplyColorProperty method in XFGlossSwitchExtensions to handle our color updates
			XFGlossSwitchExtensions.ApplyColorProperty(control, properties, propertyName);
		}

		const string appCompatWarning = "XFGloss: Android control tinting isn't supported prior to Android API 23" +
										" (Marshmallow) unless you're using the Android AppCompat library, which " +
										"provides support back to API 16 (JellyBean).";

		/// <summary>
		/// Internal method used to do the work on behalf of the UpdateColorProperty extension method for both
		/// XFGlossSwitchExtensions and XFGlossSwitchCompatExtensions
		/// </summary>
		/// <param name="control">Control.</param>
		/// <param name="properties">Properties.</param>
		/// <param name="propertyName">Property name.</param>
		/// <typeparam name="TControl">The 1st type parameter.</typeparam>
		static void ApplyColorProperty<TControl>(TControl control, ISwitchGloss properties, string propertyName)
		{
			// We have to create a multiple state color list to set both the "off" and "on" (checked/unchecked)
			// states of the switch control.

			bool isSwitch = Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M && 
	                        control is ASwitch;

			bool isSwitchCompat = !isSwitch &&
								  XFGloss.Droid.Library.UsingAppCompat &&
								  control is ASwitchCompat;

			Android.Content.Context controlContext = (isSwitch) ? (control as ASwitch).Context :
													 (isSwitchCompat) ? (control as ASwitchCompat).Context :
													 null;

			int[][] states = new int[2][];
			int[] colors = new int[2];

			if (propertyName == null ||
			    propertyName == SwitchGloss.TintColorProperty.PropertyName ||
			    propertyName == SwitchGloss.OnTintColorProperty.PropertyName)
			{
				var tintColor = properties.TintColor;
				var onTintColor = properties.OnTintColor;

				// Clamp the track tint colors to 30% opacity - API 24 automatically does this. AppCompat doesn't.
				if (isSwitchCompat)
				{
					if (tintColor != Color.Default)
					{
						tintColor = new Color(tintColor.R, tintColor.G, tintColor.B, 0.3);
					}
					if (onTintColor != Color.Default)
					{
						onTintColor = new Color(onTintColor.R, onTintColor.G, onTintColor.B, 0.3);
					}
				}

				states[0] = new int[] { -Android.Resource.Attribute.StateChecked };
				colors[0] = (tintColor != Color.Default) ?
							tintColor.ToAndroid() :
							new AColor(ThemeUtil.ColorControlNormal(controlContext, new AColor(175, 175, 175, 77)));

				states[1] = new int[] { Android.Resource.Attribute.StateChecked };
				colors[1] = (onTintColor != Color.Default) ?
							onTintColor.ToAndroid() :
							new AColor(ThemeUtil.ColorControlActivated(controlContext, new AColor(252, 69, 125, 77)));

				var colorList = new ColorStateList(states, colors);

				if (isSwitch)
				{
					(control as ASwitch).TrackTintList = colorList;
				}
				else if (isSwitchCompat)
				{
					DrawableCompat.SetTintList((control as ASwitchCompat).TrackDrawable, colorList);
				}
				else
				{
					Console.WriteLine(appCompatWarning);
				}
			}

			if (propertyName == null ||
			    propertyName == SwitchGloss.ThumbTintColorProperty.PropertyName ||
			    propertyName == SwitchGloss.ThumbOnTintColorProperty.PropertyName)
			{
				var thumbTintColor = properties.ThumbTintColor;
				var thumbOnTintColor = properties.ThumbOnTintColor;

				states[0] = new int[] { -Android.Resource.Attribute.StateChecked };
				colors[0] = (thumbTintColor != Color.Default) ?
							thumbTintColor.ToAndroid() :
							// Default thumb color...
							// Xamarin.Android doesn't have the needed ColorSwitchThumbNormal ID defined yet
							new AColor(175, 175, 175, 255);

				states[1] = new int[] { Android.Resource.Attribute.StateChecked };
				colors[1] = (thumbOnTintColor != Color.Default) ?
							thumbOnTintColor.ToAndroid() :
							new AColor(ThemeUtil.ColorControlActivated(controlContext, 
				                                                       // Default Thumb On color
				                                                       new AColor(252, 69, 125, 255)));

				var colorList = new ColorStateList(states, colors);

				if (isSwitch)
				{
					(control as ASwitch).ThumbTintList = colorList;
				}
				else if (isSwitchCompat)
				{
					DrawableCompat.SetTintList((control as ASwitchCompat).ThumbDrawable, colorList);
				}
				else
				{
					Console.WriteLine(appCompatWarning);
				}
			}
		}
	}
}