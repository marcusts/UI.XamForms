// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=RoundedContentView.cs
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

namespace Com.MarcusTS.UI.XamForms.Views.Controls
{
   using Xamarin.Forms;

   public interface IRoundedContentView
   {
      /// <summary>
      /// The background color of the rounded ContentView
      /// </summary>
      Color FillColor { get; set; }

      /// <summary>
      /// The Corner Radius of the ContentView
      /// Defaults to 0 which is not rounded.
      /// </summary>
      double CornerRadius { get; set; }

      /// <summary>
      /// If set to true, the ContentView will be circular
      /// This means the edges will be rounded as much as necessary
      /// to form a circle
      /// </summary>
      bool Circle { get; set; }

      bool   HasShadow   { get; set; }
      Color  BorderColor { get; set; }
      double BorderWidth { get; set; }
   }

   public class RoundedContentView : ContentView, IRoundedContentView
   {
      /// <summary>
      /// The background color of the rounded ContentView
      /// </summary>
      public static readonly BindableProperty FillColorProperty = BindableProperty.Create(
         "FillColor",
         typeof( Color ),
         typeof( RoundedContentView ),
         Color.White
      );

      public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
         "CornerRadius",
         typeof( double ),
         typeof( RoundedContentView ),
         0d
      );

      public static readonly BindableProperty CircleProperty = BindableProperty.Create(
         "Circle",
         typeof( bool ),
         typeof( RoundedContentView ),
         false
      );

      public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(
         "HasShadow",
         typeof( bool ),
         typeof( RoundedContentView ),
         false
      );

      public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
         "BorderColor",
         typeof( Color ),
         typeof( RoundedContentView ),
         Color.Transparent
      );

      public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(
         "BorderWidth",
         typeof( double ),
         typeof( RoundedContentView )
      );

      public RoundedContentView()
      {
         BackgroundColor = Color.Transparent;
      }

      /// <summary>
      /// The background color of the rounded ContentView
      /// </summary>
      public Color FillColor
      {
         get => (Color)GetValue( FillColorProperty );
         set => SetValue( FillColorProperty, value );
      }

      /// <summary>
      /// The Corner Radius of the ContentView
      /// Defaults to 0 which is not rounded.
      /// </summary>
      public double CornerRadius
      {
         get => (double)GetValue( CornerRadiusProperty );
         set => SetValue( CornerRadiusProperty, value );
      }

      /// <summary>
      /// If set to true, the ContentView will be circular
      /// This means the edges will be rounded as much as necessary
      /// to form a circle
      /// </summary>
      public bool Circle
      {
         get => (bool)GetValue( CircleProperty );
         set => SetValue( CircleProperty, value );
      }

      public bool HasShadow
      {
         get => (bool)GetValue( HasShadowProperty );
         set => SetValue( HasShadowProperty, value );
      }

      public Color BorderColor
      {
         get => (Color)GetValue( BorderColorProperty );
         set => SetValue( BorderColorProperty, value );
      }

      public double BorderWidth
      {
         get => (double)GetValue( BorderWidthProperty );
         set => SetValue( BorderWidthProperty, value );
      }

      public static void Init()
      { }
   }
}