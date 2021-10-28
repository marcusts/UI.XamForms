// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=IntToDoubleTwoWayConverter.cs
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

namespace Com.MarcusTS.UI.XamForms.Common.Converters
{
   using System;
   using System.Globalization;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Forms;

   /// <summary>
   /// Class IntToDoubleTwoWayConverter.
   /// Implements the <see cref="Xamarin.Forms.IValueConverter" />
   /// </summary>
   /// <seealso cref="Xamarin.Forms.IValueConverter" />
   public class IntToDoubleTwoWayConverter : IValueConverter
   {
      /// <summary>
      /// The instance
      /// </summary>
      public static readonly IntToDoubleTwoWayConverter INSTANCE = new IntToDoubleTwoWayConverter();

      // From int (view model) to double (view)
      /// <summary>
      /// Implement this method to convert <paramref name="value" /> to <paramref name="targetType" /> by using
      /// <paramref name="parameter" /> and <paramref name="culture" />.
      /// </summary>
      /// <param name="value">The value to convert.</param>
      /// <param name="targetType">The type to which to convert the value.</param>
      /// <param name="parameter">A parameter to use during the conversion.</param>
      /// <param name="culture">The culture to use during the conversion.</param>
      /// <returns>To be added.</returns>
      /// <remarks>To be added.</remarks>
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is int valueAsInt)
         {
            return (double) valueAsInt;
         }

         return default;
      }

      // From double (view) to int (view model)
      /// <summary>
      /// Converts the back.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <param name="targetType">Type of the target.</param>
      /// <param name="parameter">The parameter.</param>
      /// <param name="culture">The culture.</param>
      /// <returns>System.Object.</returns>
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is double valueAsDouble)
         {
            return valueAsDouble.ToRoundedInt();
         }

         return default;
      }
   }
}