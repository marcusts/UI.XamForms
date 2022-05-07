// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=OneWayConverter.cs
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

   public abstract class OneWayConverter<FromT, ToT> : IValueConverter
   {
      public readonly ToT FailedDefaultValue = default;

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is FromT valueAsFromT)
         {
            return Convert(valueAsFromT, parameter);
         }

         return FailedDefaultValue;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         ErrorUtils.ThrowArgumentError(nameof(OneWayConverter<FromT, ToT>) + ": two-way bindings not supported.");

         return FailedDefaultValue;
      }

      protected abstract ToT Convert(FromT value, object parameter);

      public object ConvertEasily(FromT value)
      {
         return Convert(value, null);
      }
   }
}