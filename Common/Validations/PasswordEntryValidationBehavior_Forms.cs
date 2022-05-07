// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=PasswordEntryValidationBehavior_Forms.cs
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
   using System.Linq;
   using Com.MarcusTS.PlatformIndependentShared.Common.Behaviors;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;

   public interface IPasswordEntryValidationBehavior_Forms : IEntryValidationBehavior_Forms,
      IPasswordProperties_PI
   { }

   public class PasswordEntryValidationBehavior_Forms : EntryValidationBehavior_Forms,
      IPasswordEntryValidationBehavior_Forms
   {
      public int MaxCharacterCount        { get; set; } = ValidationUtils_PI.DEFAULT_MAX_CHARACTER_COUNT;
      public int MaxRepeatChars           { get; set; } = ValidationUtils_PI.DEFAULT_MAX_REPEAT_CHARS;
      public int MinCapitalCharacterCount { get; set; } = ValidationUtils_PI.DEFAULT_MIN_CAPITAL_CHARACTER_COUNT;
      public int MinCharacterCount        { get; set; } = ValidationUtils_PI.DEFAULT_MIN_CHARACTER_COUNT;
      public int MinLowCaseCharacterCount { get; set; } = ValidationUtils_PI.DEFAULT_MIN_LOW_CASE_CHARACTER_COUNT;
      public int MinNumericCharacterCount { get; set; } = ValidationUtils_PI.DEFAULT_MIN_NUMERIC_CHARACTER_COUNT;
      public int MinSpecialCharacterCount { get; set; } = ValidationUtils_PI.DEFAULT_MIN_SPECIAL_CHARACTER_COUNT;

      protected override IIsValidCondition_PI[] GetValidationConditions()
      {
         // Use the base as well
         var retConditions = base.GetValidationConditions().ToList();

         if (MinCapitalCharacterCount.IsANumberGreaterThanZero())
         {
            retConditions.Add(
               new IsValidCondition_PI
               {
                  RuleDescription = "At least " + MinCapitalCharacterCount + " capital character(s)",
                  IsValidFunc =
                     (targetObj, compareObj) =>
                        CurrentText.IsNotEmpty() && (CurrentText.Count(char.IsUpper) >= MinCapitalCharacterCount),
               });
         }

         if (MinLowCaseCharacterCount.IsANumberGreaterThanZero())
         {
            retConditions.Add(
               new IsValidCondition_PI
               {
                  RuleDescription =
                     "Has at least " + MinLowCaseCharacterCount + " lower-case character(s)",
                  IsValidFunc =
                     (targetObj, compareObj) =>
                        CurrentText.IsNotEmpty() && (CurrentText.Count(char.IsLower) >= MinLowCaseCharacterCount),
               });
         }

         if (MinNumericCharacterCount.IsANumberGreaterThanZero())
         {
            retConditions.Add(
               new IsValidCondition_PI
               {
                  RuleDescription =
                     "Has at least " + MinNumericCharacterCount + " numeric character(s)",
                  IsValidFunc =
                     (targetObj, compareObj) =>
                        CurrentText.IsNotEmpty() && (CurrentText.Count(char.IsNumber) >= MinNumericCharacterCount),
               });
         }

         if (MinSpecialCharacterCount.IsANumberGreaterThanZero())
         {
            retConditions.Add(
               new IsValidCondition_PI
               {
                  RuleDescription =
                     "Has at least " + MinSpecialCharacterCount + " special character(s)",
                  IsValidFunc =
                     (targetObj, compareObj) =>
                        CurrentText.IsNotEmpty() &&
                        ((CurrentText.Length                  -
                              CurrentText.Count(char.IsUpper) -
                              CurrentText.Count(char.IsLower) -
                              CurrentText.Count(char.IsNumber))
                        >= MinSpecialCharacterCount),
               });
         }

         if (MaxRepeatChars.IsANumberGreaterThanZero())
         {
            retConditions.Add(
               new IsValidCondition_PI
               {
                  RuleDescription =
                     "Has no more than " + MaxRepeatChars + " repeated character(s) in sequence",
                  IsValidFunc =
                     (targetObj, compareObj) =>
                        CurrentText.IsEmpty() || CurrentText.DoesNotRepeatCharacters(MaxRepeatChars),
               });
         }

         return retConditions.ToArray();
      }
   }
}