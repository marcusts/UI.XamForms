// *********************************************************************************
// <copyright file=ImageLabelButtonStyle_Forms.cs company="Marcus Technical Services, Inc.">
//     Copyright @2019 Marcus Technical Services, Inc.
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
   using System;
   using Xamarin.Forms;

   /// <summary>
   ///    Interface IImageLabelButtonStyle_Forms
   /// </summary>
   public interface IImageLabelButtonStyle_Forms
   {
      /// <summary>
      ///    Gets or sets the button style.
      /// </summary>
      /// <value>The button style.</value>
      Style ButtonStyle { get; set; }

      /// <summary>
      ///    Gets or sets the button text.
      /// </summary>
      /// <value>The button text.</value>
      string ButtonText { get; set; }

      /// <summary>
      ///    Gets or sets a value indicating whether [get image from resource].
      /// </summary>
      /// <value><c>true</c> if [get image from resource]; otherwise, <c>false</c>.</value>
      bool GetImageFromResource { get; set; }

      /// <summary>
      ///    Gets or sets the image file path.
      /// </summary>
      /// <value>The image file path.</value>
      string ImageFilePath { get; set; }

      /// <summary>
      ///    Gets or sets the type of the image resource class.
      /// </summary>
      /// <value>The type of the image resource class.</value>
      Type ImageResourceClassType { get; set; }

      /// <summary>
      ///    Gets or sets the state of the internal button.
      /// </summary>
      /// <value>The state of the internal button.</value>
      string InternalButtonState { get; set; }

      /// <summary>
      ///    Gets or sets the label style.
      /// </summary>
      /// <value>The label style.</value>
      Style LabelStyle { get; set; }
   }

   public class ImageLabelButtonStyle_Forms : IImageLabelButtonStyle_Forms
   {
      /// <summary>
      ///    Gets or sets the button style.
      /// </summary>
      /// <value>The button style.</value>
      public Style ButtonStyle { get; set; }

      /// <summary>
      ///    Gets or sets the button text.
      /// </summary>
      /// <value>The button text.</value>
      public string ButtonText { get; set; }
      
      /// <summary>
      ///    Gets or sets a value indicating whether [get image from resource].
      /// </summary>
      /// <value><c>true</c> if [get image from resource]; otherwise, <c>false</c>.</value>
      public bool GetImageFromResource { get; set; }

      /// <summary>
      ///    Gets or sets the image file path.
      /// </summary>
      /// <value>The image file path.</value>
      public string ImageFilePath { get; set; }

      /// <summary>
      ///    Gets or sets the type of the image resource class.
      /// </summary>
      /// <value>The type of the image resource class.</value>
      public Type ImageResourceClassType { get; set; }

      /// <summary>
      ///    Gets or sets the state of the internal button.
      /// </summary>
      /// <value>The state of the internal button.</value>
      public string InternalButtonState { get; set; }

      /// <summary>
      ///    Gets or sets the label style.
      /// </summary>
      /// <value>The label style.</value>
      public Style LabelStyle { get; set; }
   }
}