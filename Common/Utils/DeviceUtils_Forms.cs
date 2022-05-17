﻿// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=DeviceUtils_Forms.cs
// company="Marcus Technical Services, Inc.">
// </copyright>
// 
// MIT License
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *********************************************************************************

namespace Com.MarcusTS.UI.XamForms.Common.Utils
{
    using Xamarin.Essentials;

    public class DeviceUtils_Forms
    {
        /// <summary>The ios top margin</summary>
        public const float IOS_TOP_MARGIN = 40;

        /// <summary>Determines whether this instance is ios.</summary>
        /// <returns><c>true</c> if this instance is ios; otherwise, <c>false</c>.</returns>
        public static bool IsIos()
        {
            return DeviceInfo.Platform == DevicePlatform.iOS;
        }

        /// <summary>Gets the starting y for runtime platform.</summary>
        /// <returns>System.Single.</returns>
        public static double GetStartingYForRuntimePlatform()
        {
            return IsIos() ? IOS_TOP_MARGIN : 0;
        }
    }
}