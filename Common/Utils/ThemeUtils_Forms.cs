// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ThemeUtils_XFS.cs
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
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Forms;

   public static class ThemeUtils_Forms
   {
      public const            string MAIN_STAGE_THEME_COLOR_HEX = "#00008B";
      public static readonly  Color  MAIN_STAGE_THEME_COLOR     = Color.DarkBlue;
      public static readonly  Color  DARK_THEME_COLOR           = Color.FromRgb(0, 48,  87);
      public static readonly  Color  LIGHT_THEME_COLOR          = Color.FromRgb(0, 153, 216);
      private static readonly Easing FADE_IN_EASING             = Easing.CubicIn;
      private static readonly uint FADE_TIME = 1500;

      public static async Task FadeIntoColor(this VisualElement view, Color color, uint? fadeTime = null)
      {
         view.Opacity         = UIConst_PI.NOT_VISIBLE_OPACITY;
         view.BackgroundColor = color;
         await view.FadeTo(UIConst_PI.VISIBLE_OPACITY, fadeTime ?? FADE_TIME, FADE_IN_EASING).WithoutChangingContext();
      }
   }
}