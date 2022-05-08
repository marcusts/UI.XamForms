// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=KeyboardEffect_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Common.Devices
{
   using System.Linq;
   using Xamarin.Forms;

   /// <summary>Set up Bindable Properties for KeyboardEnableEffect_Forms</summary>
   public static class KeyboardEffect_Forms
   {
      /// <summary>Bindable property to Enable keyboard</summary>
      public static readonly BindableProperty EnableKeyboardProperty =
         BindableProperty.Create("EnableKeyboard", typeof(bool), typeof(KeyboardEffect_Forms), false,
            propertyChanged: OnEnableKeyboardChanged);

      /// <summary>Bindable property to focus control</summary>
      public static readonly BindableProperty RequestFocusProperty =
         BindableProperty.Create("RequestFocus", typeof(bool), typeof(KeyboardEffect_Forms), false);

      /// <summary>Get EnableKeyboard value</summary>
      /// <param name="view"></param>
      /// <returns></returns>
      public static bool GetEnableKeyboard(BindableObject view)
      {
         return (bool)view.GetValue(EnableKeyboardProperty);
      }

      /// <summary>Get RequestFocus value</summary>
      /// <param name="view"></param>
      /// <returns></returns>
      public static bool GetRequestFocus(BindableObject view)
      {
         return (bool)view.GetValue(RequestFocusProperty);
      }

      /// <summary>Set EnableKeyboard Value</summary>
      /// <param name="view"></param>
      /// <param name="value"></param>
      public static void SetEnableKeyboard(BindableObject view, bool value)
      {
         view.SetValue(EnableKeyboardProperty, value);
      }

      /// <summary>Set RequestFocus Value</summary>
      /// <param name="view"></param>
      /// <param name="value"></param>
      public static void SetRequestFocus(BindableObject view, bool value)
      {
         view.SetValue(RequestFocusProperty, value);
      }

      private static void OnEnableKeyboardChanged(BindableObject bindable, object oldValue, object newValue)
      {
         if (!(bindable is View view))
         {
            return;
         }

         var enableKeyboard = (bool)newValue;

         if (enableKeyboard)
         {
            var toRemove = view.Effects.FirstOrDefault(e => e is KeyboardEnableEffect_Forms);
            if (toRemove != null)
            {
               view.Effects.Remove(toRemove);
            }
         }
         else
         {
            view.Effects.Add(new KeyboardEnableEffect_Forms());
         }
      }
   }
}