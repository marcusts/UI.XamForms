// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=EmailEntryValidatorBehavior_Forms.cs
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
   using Com.MarcusTS.PlatformIndependentShared.Common.Behaviors;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;

   public interface IEmailEntryValidatorBehavior_Forms : IEntryValidationBehavior_Forms
   { }

   /// <summary>
   /// Class EmailEntryValidatorBehavior. Implements the <see cref="EntryValidationBehavior_Forms" />
   /// </summary>
   /// <seealso cref="EntryValidationBehavior_Forms" />
   public class EmailEntryValidatorBehavior_Forms : EntryValidationBehavior_Forms,
      IEmailEntryValidatorBehavior_Forms
   {
      // Can have a period as long as it is not at the start or end of either the local part or the domain.
      /// <summary>At sign</summary>
      private const char AT_SIGN = '@';

      // private const string REG_EX_VALID_CHARS = "^[a–zA–Z0-9!#$%&‘*+/=?^_`{|}~.-]*$";
      /// <summary>The reg ex valid chars</summary>
      private const string REG_EX_VALID_CHARS = "^[a-zA-Z0-9.]*$";

      protected override IIsValidCondition_PI[] GetValidationConditions()
      {
         // The only condition is that the email be properly fprmed
         var retConditions = new List<IIsValidCondition_PI>
                             {
                                new IsValidCondition_PI
                                {
                                   RuleDescription = ValidationUtils_PI.IS_LEGAL_EMAIL,
                                   IsValidFunc     = (targetObj, compareObj) => IsAValidEmail(),
                                },
                             };

         return retConditions.ToArray();
      }

      protected override string IllegalCharFilter(
         IValidationBehaviorBase_Forms behavior,
         string                                 newText,
         string                                 originalText,
         out bool                               isOutsideOfRange)
      {
         return EmailIllegalCharFunc(base.IllegalCharFilter(behavior, newText, originalText, out isOutsideOfRange),
            out isOutsideOfRange);
      }

      private static string EmailIllegalCharFunc(string newText, out bool isOutsideOfRange)
      {
         isOutsideOfRange = false;

         // Overall: too much complexity for easy management; will just focus on completely illegal characters, spaces,
         // etc. But as the user types, we have to allow partially accurate values so the user can complete their work.
         var retStr      = string.Empty;
         var atSignFound = false;

         foreach (var c in newText)
         {
            if (!atSignFound && (c == AT_SIGN))
            {
               retStr      += c;
               atSignFound =  true;
            }
            else if (c.ToString().IsNonNullRegexMatch(REG_EX_VALID_CHARS))
            {
               retStr += c;
            }
         }

         return retStr;
      }

      private bool IsAValidEmail()
      {
         return CurrentText.IsAValidEmailAddress();
      }
   }
}