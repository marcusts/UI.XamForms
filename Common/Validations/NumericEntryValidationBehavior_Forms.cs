// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=NumericEntryValidationBehavior_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Common.Validations
{
   using System.Collections.Generic;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Behaviors;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;

   public interface INumericEntryValidationBehavior_Forms : 
      IEntryValidationBehavior_Forms,
      IMinAndMaxNumberProperties_PI,
      IHaveMinAndMaxNumbers_Setters_PI
   { }

   public class NumericEntryValidationBehavior_Forms : EntryValidationBehavior_Forms,
      INumericEntryValidationBehavior_Forms
   {
      private int    _charsToRightOfDecimal;
      private double _maxDecimalNumber;
      private double _minDecimalNumber;
      private string _numericType;

      public int CharsToRightOfDecimal
      {
         get => _charsToRightOfDecimal;

         set =>
            SetCharsToRightOfDecimal( value ).FireAndFuhgetAboutIt();
      }

      public double MaxDecimalNumber
      {
         get => _maxDecimalNumber;

         set =>
            SetMaxDecimalNumber(value).FireAndFuhgetAboutIt();
      }

      public double MinDecimalNumber
      {
         get => _minDecimalNumber;

         set =>
            SetMinDecimalNumber(value).FireAndFuhgetAboutIt();
      }

      public string NumericType
      {
         get => _numericType;

         set =>
            SetNumericType(value).FireAndFuhgetAboutIt();
      }

      public override string PrepareTextForEditing(string entryText, bool firstFocused = false)
      {
         return entryText.PrepareTextForEditing_PI(firstFocused, NumericType, StringFormat, CharsToRightOfDecimal);
      }

      public async Task SetCharsToRightOfDecimal(int charsToRightOfDecimal)
      {
         if (_charsToRightOfDecimal != charsToRightOfDecimal)
         {
            _charsToRightOfDecimal = charsToRightOfDecimal;
            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      public async Task SetMaxDecimalNumber(double maxNum)
      {
         if (_maxDecimalNumber.IsDifferentThan(maxNum))
         {
            _maxDecimalNumber = _maxDecimalNumber.IsLessThan(MaxDecimalNumber) ? MaxDecimalNumber : maxNum;

            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      public async Task SetMinDecimalNumber(double minNum)
      {
         if (_minDecimalNumber.IsDifferentThan(minNum))
         {
            _minDecimalNumber = _minDecimalNumber.IsGreaterThan(MinDecimalNumber) ? MinDecimalNumber : minNum;

            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      public async Task SetNumericType(string numType)
      {
         if (_numericType.IsDifferentThan(numType))
         {
            _numericType = numType;

            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      /// <remarks>
      /// We can only evaluate the unmasked text (otherwise all of our tests fail).
      /// </remarks>
      protected override string IllegalCharFilter(IValidationBehaviorBase_Forms behavior,
                                                  string                              changedText,
                                                  string                              originalText,
                                                  out bool                            isOutsideOfRange)
      {
         var retStr =
            CallAnalyzeChangedNumericText
            (
               changedText,
               originalText,
               out var isLessThanMin,
               out var isGreaterThanMax,
               out var isValidNumber
            );

         isOutsideOfRange = isLessThanMin || isGreaterThanMax;

         return retStr;
      }

      private string CallAnalyzeChangedNumericText
      (
         string   changedText,
         string   originalText,
         out bool isLessThanMin,
         out bool isGreaterThanMax,
         out bool isValidNumber
      )
      {
         return ValidationUtils_PI.AnalyzeChangedNumericText
         (
            changedText,
            originalText,
            NumericType,
            Mask,
            MaskPositions,
            MinDecimalNumber,
            MaxDecimalNumber,
            CharsToRightOfDecimal,
            out isLessThanMin,
            out isGreaterThanMax,
            out isValidNumber
         );
      }

      protected override IIsValidCondition_PI[] GetValidationConditions()
      {
         // The only conditions are that this is a number and that it is within the designated range
         var retConditions =
            new List<IIsValidCondition_PI>
            {
               new IsValidCondition_PI
               {
                  RuleDescription = "Is a valid number",
                  IsValidFunc =
                     (targetObj, compareObj) =>
                     {
                        CallAnalyzeChangedNumericText(
                           CurrentText, 
                           ValidationUtils_PI.IMPOSSIBLE_TEXT, 
                           out var isLessThanMin,
                           out var isGreaterThanMax,
                           out var isValidNumber);

                        return isValidNumber;
                     },
               },
            };

         if (MinDecimalNumber.IsANumberGreaterThanZero())
         {
            retConditions.Add(
               new IsValidCondition_PI
               {
                  RuleDescription = "Is greater than or equal to " +
                     MinDecimalNumber.ToString(CharsToRightOfDecimal.GetGenericNumberFormat()),
                  IsValidFunc =
                     (targetObj, compareObj) =>
                     {
                        CallAnalyzeChangedNumericText(
                           CurrentText, 
                           ValidationUtils_PI.IMPOSSIBLE_TEXT, 
                           out var isLessThanMin,
                           out var isGreaterThanMax,
                           out var isValidNumber);

                        return !isLessThanMin;
                     },
               });
         }

         if (MaxDecimalNumber.IsANumberGreaterThanZero())
         {
            retConditions.Add(
               new IsValidCondition_PI
               {
                  RuleDescription = "Is less than or equal to " +
                     MinDecimalNumber.ToString(CharsToRightOfDecimal.GetGenericNumberFormat()),
                  IsValidFunc =
                     (targetObj, compareObj) =>
                     {
                        CallAnalyzeChangedNumericText(
                           CurrentText, 
                           ValidationUtils_PI.IMPOSSIBLE_TEXT, 
                           out var isLessThanMin,
                           out var isGreaterThanMax,
                           out var isValidNumber);

                        return !isGreaterThanMax;
                     },
               });
         }

         return retConditions.ToArray();
      }
   }
}