// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=UIConst_Forms.cs
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
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Xamarin.Forms;

   public static class UIConst_Forms
   {
      public static readonly Keyboard STANDARD_KEYBOARD = Keyboard.Create(UIConst_PI.STANDARD_KEYBOARD_NUMBER);

      public static readonly double MARGIN_SPACING_SINGLE_FACTOR = 10.0.AdjustForOsAndDevice();

      public static readonly Thickness DEFAULT_STACK_LAYOUT_MARGIN =
         new Thickness(MARGIN_SPACING_SINGLE_FACTOR);

      public static readonly double EDITABLE_VIEW_FONT_SIZE = Device.GetNamedSize(NamedSize.Small, typeof(View));

      public static readonly Easing DEFAULT_EASING = Easing.Linear;

      public static readonly double DEFAULT_ENTRY_FONT_SIZE =
         NamedSize.Small.ToFormsNamedSize().AdjustForOsAndDevice(1.1);

      public static readonly  double SPACER_HEIGHT           = 2.0.AdjustForOsAndDevice();

      public static readonly  Color HEADER_AND_KEY_LINE_COLOR           = Color.FromRgb(102, 153, 204);
   }
}