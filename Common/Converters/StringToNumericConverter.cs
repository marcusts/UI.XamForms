// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=StringToNumericConverter.cs
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

#define FORCE_NULLABLE_DOUBLE

namespace Com.MarcusTS.UI.XamForms.Common.Converters
{
   using System;
   using System.Globalization;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Forms;

   public class StringToNumericConverter : IValueConverter
   {
      
#if !FORCE_NULLABLE_DOUBLE
      public Func<string, object> ConvertBackFunc { get; set; }
#endif
      
      public string               StringFormat    { get; set; }
      public string               NumericType  { get; set; }

      // From numeric to string (for editing)
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return ConvertValueToString(value);
      }

      // From string to numeric (for the view model)
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
#if FORCE_NULLABLE_DOUBLE
         var valueEntered = ConvertValueToString(value);
         if (valueEntered.IsNotEmpty() && double.TryParse(valueEntered, out var valueAsDouble))
         {
            return valueAsDouble as double?;
         }
#else
         if (ConvertBackFunc.IsNotNullOrDefault())
         {
            return ConvertBackFunc(ConvertValueToString(value));
         }
#endif

         return default;
      }

      private string ConvertValueToString(object value)
      {
         return value.IsNullOrDefault()
            ? ""
            : ValidationUtils_PI.StripStringFormatCharacters(value.ToString(), StringFormat, NumericType);
      }
   }
}