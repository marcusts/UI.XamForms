// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=EditorOrEntryValidationBehaviorBase_Forms.cs
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
   using System.Text;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Behaviors;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Forms;

   public interface IEditorOrEntryValidationBehaviorBase_Forms : IValidationBehaviorBase_Forms,
                                                                          ICommonEditorValidationProps_PI,
                                                                          ICommonEditorValidationProps_Setters_PI
   {
      string OriginalText { get; set; }

      string PrepareTextForEditing(string entryText, bool firstFocused = false);
   }

   public abstract class EditorOrEntryValidationBehaviorBase_Forms : ValidationBehaviorBase_Forms,
                                                                              IEditorOrEntryValidationBehaviorBase_Forms
   {
      private string _excludedChars;
      private bool   _ignoreTextChanged;
      private int    _maxLength;
      private int    _minLength;
      private string _originalText;
      private int    _textMustChange;

      protected abstract string CurrentText { get; set; }

      protected string LastValidatedText { get; private set; }

      public string ExcludedChars
      {
         get => _excludedChars;
         set =>
            SetExcludedChars(value).FireAndFuhgetAboutIt();
      }

      public int MaxLength
      {
         get => _maxLength;
         set =>
            SetMaxLength(value).FireAndFuhgetAboutIt();
      }

      public int MinLength
      {
         get => _minLength;
         set =>
            SetMinLength(value).FireAndFuhgetAboutIt();
      }

      public string OriginalText
      {
         get => _originalText;
         set =>
            SetOriginalText(value).FireAndFuhgetAboutIt();
      }

      public int TextMustChange
      {
         get => _textMustChange;
         set =>
            SetTextMustChange(value).FireAndFuhgetAboutIt();
      }

      public virtual string PrepareTextForEditing(string entryText, bool firstFocused = false)
      {
         return entryText;
      }

      public async Task SetExcludedChars(string excludedChars)
      {
         if (_excludedChars != excludedChars)
         {
            _excludedChars = excludedChars;
            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      public async Task SetMaxLength(int maxLength)
      {
         if (_maxLength != maxLength)
         {
            _maxLength = maxLength;
            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      public async Task SetMinLength(int minLength)
      {
         if (_minLength != minLength)
         {
            _minLength = minLength;

            if (_maxLength < minLength)
            {
               // INVALIDATE THIS
               _maxLength = ViewModelCustomAttribute_Static_PI.UNSET_INT;
            }

            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      public async Task SetOriginalText(string originalText)
      {
         if (_originalText != originalText)
         {
            _originalText = originalText;
            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      public async Task SetTextMustChange(int textMustChange)
      {
         if (_textMustChange != textMustChange)
         {
            _textMustChange = textMustChange;
            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      protected virtual Task AfterTextAssignment()
      {
         return Task.CompletedTask;
      }

      protected virtual Task<string> ConsiderMasking(string preparedNewText)
      {
         return Task.FromResult(preparedNewText);
      }

      protected override IIsValidCondition_PI[] GetValidationConditions()
      {
         var retConditions = new List<IIsValidCondition_PI>();

         if (MinLength.IsANumberGreaterThanZero())
         {
            retConditions.Add(
            new IsValidCondition_PI
            {
               RuleDescription = "At least " + MinLength + " character(s)",
               IsValidFunc =
                  (targetObj, compareObj) => CurrentText?.Length >= MinLength
            });
         }

         if (MaxLength.IsANumberGreaterThanZero())
         {
            retConditions.Add(
            new IsValidCondition_PI
            {
               RuleDescription = "No more then " + MaxLength + " character(s)",
               IsValidFunc     = (targetObj, compareObj) => !IsLongerThanMaxLength(MaxLength, CurrentText)
            });
         }

         if (TextMustChange.IsTrue() && OriginalText.IsNotEmpty())
         {
            retConditions.Add(
            new IsValidCondition_PI
            {
               RuleDescription = "Text must change",
               IsValidFunc =
                  (targetObj, compareObj) => CurrentText.IsDifferentThan(OriginalText)
            });
         }

         return retConditions.ToArray();
      }

      protected virtual string IllegalCharFilter(
         IValidationBehaviorBase_Forms behavior,
         string                              newText,
         string                              originalText,
         out bool                            isOutsideOfRange)
      {
         isOutsideOfRange = false;

         // Works for max; not helpful for min -- the user can back-space until the string is empty.
         if (IsLongerThanMaxLength(MaxLength, newText))
         {
            return originalText;
         }

         if (ExcludedChars.IsNotEmpty())
         {
            var finalTextStrBuilder = new StringBuilder();

            foreach (var ch in newText)
            {
               if (!ExcludedChars.Contains(ch.ToString()))
               {
                  finalTextStrBuilder.Append(ch);
               }
            }

            newText = finalTextStrBuilder.ToString();
         }

         // ELSE success
         return newText;
      }

      protected virtual bool IsLongerThanMaxLength(int maxLength, string newText)
      {
         return (maxLength > 0) && newText.IsNotEmpty() && (newText.Length > maxLength);
      }

      protected virtual void OnTextChanged
      (
         object               sender,
         TextChangedEventArgs e
      )
      {
         if (_ignoreTextChanged)
         {
            return;
         }

         ValidateText(e.NewTextValue, e.OldTextValue).FireAndFuhgetAboutIt();
      }

      /// <remarks>All stripping is repeated here for safety</remarks>
      protected async Task ValidateText(string newText, string oldText)
      {
         if (BoundHost.IsNullOrDefault() || !IsFocused || newText.IsSameAs(LastValidatedText))
         {
            return;
         }

         var preparedNewText = PrepareTextForEditing(newText);
         var preparedOldText = PrepareTextForEditing(oldText);

         // This is another "success" case, since there is nothing to do.
         if (preparedNewText.IsSameAs(LastValidatedText))
         {
            return;
         }

         // Allow the deriving class to filter out illegal characters
         var filteredText = IllegalCharFilter(this, preparedNewText, preparedOldText, out var isOutsideOfRange);

         // Add masking after validation
         preparedNewText = await ConsiderMasking(filteredText).WithoutChangingContext();

         if (CurrentText.IsDifferentThan(preparedNewText))
         {
            _ignoreTextChanged = true;
            CurrentText        = preparedNewText;
            _ignoreTextChanged = false;
         }

         await AfterTextAssignment().WithoutChangingContext();

         await Task.Delay(100).WithoutChangingContext();

         await RevalidateAllConditions().WithoutChangingContext();

         LastValidatedText = preparedNewText;
      }
   }
}