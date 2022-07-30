// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ValidatableNumericEntry.cs
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
   using System;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Converters;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Validations;
   using Xamarin.Forms;

   public interface IValidatableNumericEntry_Forms : IValidatableEntry_Forms
   { }

   public class ValidatableNumericEntry_Forms : ValidatableEntry_Forms, IValidatableNumericEntry_Forms
   {
      private readonly ICanBeValid_PI _validator;

      public ValidatableNumericEntry_Forms(
         double?                                        entryFontSize,
         string                                         stringFormat,
         INumericEntryValidationBehavior_Forms validator,
         Keyboard                                       keyboard        = default,
         bool                                           asleepInitially = false
      )
         : base(entryFontSize: entryFontSize, keyboard: keyboard, asleepInitially: asleepInitially)
      {
         _validator = validator;

         Converter =
            new StringToNumericConverter
            {
               StringFormat = stringFormat,
               NumericType = validator.IsNotNullOrDefault()
                  ? validator.NumericType
                  : ValidationUtils_PI.DOUBLE_NUMERIC_TYPE,
            };

         if (!IsConstructing)
         {
            RecreateAllViewsBindingsAndStyles().AndReturnToCallingContext();
         }
      }

      protected override bool DerivedViewIsFocused => false;

      protected override bool UserHasEnteredValidContent =>
         _validator is INumericEntryValidationBehavior_Forms validatorAsNumeric
            ? validatorAsNumeric.PrepareTextForEditing(EditableEntry.Text).IsNotEmpty()
            : EditableEntry.Text.IsNotEmpty();

      private static Func<string, object> DoubleFunc(string valueEntered)
      {
         return str =>
                {
                   if (valueEntered.IsNotEmpty() && double.TryParse(valueEntered, out var valueAsDouble))
                   {
                      return valueAsDouble;
                   }

                   return null;
                };
      }

      private static Func<string, object> IntFunc(string valueEntered)
      {
         return str =>
                {
                   if (valueEntered.IsNotEmpty() && int.TryParse(valueEntered, out var valueAsInt))
                   {
                      return valueAsInt;
                   }

                   return null;
                };
      }

      private static Func<string, object> LongFunc(string valueEntered)
      {
         return str =>
                {
                   if (valueEntered.IsNotEmpty() && long.TryParse(valueEntered, out var valueAsLong))
                   {
                      return valueAsLong;
                   }

                   return null;
                };
      }

      private static Func<string, object> NullableDoubleFunc(string valueEntered)
      {
         return str =>
                {
                   if (valueEntered.IsNotEmpty() && double.TryParse(valueEntered, out var valueAsDouble))
                   {
                      return valueAsDouble as double?;
                   }

                   return null;
                };
      }

      private static Func<string, object> NullableIntFunc(string valueEntered)
      {
         return str =>
                {
                   if (valueEntered.IsNotEmpty() && int.TryParse(valueEntered, out var valueAsInt))
                   {
                      return valueAsInt as int?;
                   }

                   return null;
                };
      }

      private static Func<string, object> NullableLongFunc(string valueEntered)
      {
         return str =>
                {
                   if (valueEntered.IsNotEmpty() && long.TryParse(valueEntered, out var valueAsLong))
                   {
                      return valueAsLong as long?;
                   }

                   return null;
                };
      }
   }
}