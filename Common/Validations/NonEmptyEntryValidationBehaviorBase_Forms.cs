
// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=NonEmptyEntryValidationBehaviorBase.cs
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

   public interface INonEmptyEntryValidationBehaviorBase_Forms : IValidationBehaviorBase_Forms
   {
      bool   EmptyAllowed { get; set; }
      bool   IsNumeric    { get; set; }
      string RecordName   { get; set; }
   }

   public abstract class NonEmptyEntryValidationBehaviorBase_Forms : ValidationBehaviorBase_Forms,
                                                                              INonEmptyEntryValidationBehaviorBase_Forms
   {
      private bool   _emptyAllowed;
      private bool   _isNumeric;
      private string _recordName;

      public bool EmptyAllowed
      {
         get => _emptyAllowed;
         set
         {
            if (_emptyAllowed != value)
            {
               _emptyAllowed = value;
               ResetAndRevalidateAllConditions().FireAndFuhgetAboutIt();
            }
         }
      }

      public bool IsNumeric
      {
         get => _isNumeric;
         set
         {
            if (_isNumeric != value)
            {
               _isNumeric = value;
               ResetAndRevalidateAllConditions().FireAndFuhgetAboutIt();
            }
         }
      }

      public string RecordName
      {
         get => _recordName;
         set
         {
            if (_recordName.IsDifferentThan(value))
            {
               _recordName = value;
               ResetAndRevalidateAllConditions().FireAndFuhgetAboutIt();
            }
         }
      }

      protected override IIsValidCondition_PI[] GetValidationConditions()
      {
         // The only condition is that the item not be empty
         var retConditions = new List<IIsValidCondition_PI>
         {
            new IsValidCondition_PI
            {
               RuleDescription = "Is a valid " + (RecordName.IsNotEmpty() ? RecordName : "record"),
               IsValidFunc =
                  (targetObj, compareObj) =>
                     EmptyAllowed ||
                     IsNotEmpty(targetObj)
            }
         };

         return retConditions.ToArray();
      }

      private bool IsNotEmpty(object targetObj)
      {
         var isNotEmpty =
            (targetObj is string && targetObj.ToString().IsNotEmpty()) ||
            (!(targetObj is string) && ((!IsNumeric && targetObj.IsNotNullOrDefault()) ||
                                        (IsNumeric  && targetObj.IsANumberGreaterThanZero())));

         return isNotEmpty;
      }
   }
}