// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=EntryValidationBehavior_Forms.cs
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
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Xamarin.Forms;

   public interface IEntryValidationBehavior_Forms : IEditorValidationBehaviorBase_Forms,
      IEntryValidationBehaviorProperties_PI,
      ICommonEntryValidationProps_Setters_PI
   {
      string UnmaskedText { get; }
   }

   public class EntryValidationBehavior_Forms : EditorOrEntryValidationBehaviorBase_Forms,
      IEntryValidationBehavior_Forms
   {
      private int    _doNotForceMaskInitially;
      private string _mask;
      private string _stringFormat;

      protected IDictionary<int, char> MaskPositions { get; set; }

      protected override string CurrentText
      {
         get => (BoundHost as Entry)?.Text;
         set => ((Entry) BoundHost).Text = value;
      }

      public int DoNotForceMaskInitially
      {
         get => _doNotForceMaskInitially;
         set =>
            SetDoNotForceMaskInitially(value).FireAndFuhgetAboutIt();
      }

      public string Mask
      {
         get => _mask;
         set =>
            SetMask(value).FireAndFuhgetAboutIt();
      }

      public string StringFormat
      {
         get => _stringFormat;
         set =>
            SetStringFormat(value).FireAndFuhgetAboutIt();
      }

      public string UnmaskedText { get; private set; }

      public async Task SetDoNotForceMaskInitially(int doNotForceMaskInitially)
      {
         if (_doNotForceMaskInitially != doNotForceMaskInitially)
         {
            _doNotForceMaskInitially = doNotForceMaskInitially;
            await ConsiderDoNotForceMaskInitially().WithoutChangingContext();
            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      public async Task SetMask(string mask)
      {
         if (_mask.IsDifferentThan(mask))
         {
            _mask = mask;

            CreateMaskPositions();

            if (_mask.IsNotEmpty())
            {
               MinLength = _mask.Length;
               MaxLength = _mask.Length;

               // Can't mask with numeric keyboard, etc.
               if (CurrentText.IsNotNullOrDefault())
               {
                  ((Entry) BoundHost).Keyboard = UIConst_Forms.STANDARD_KEYBOARD;
               }
            }

            await ResetAndRevalidateAllConditions().WithoutChangingContext();
         }
      }

      public async Task SetStringFormat(string stringFormat)
      {
         if (_stringFormat.IsDifferentThan(stringFormat))
         {
            {
               _stringFormat = stringFormat;
               await ResetAndRevalidateAllConditions().WithoutChangingContext();
            }
         }
      }

      public string StripMaskFromText(string text)
      {
         return text.StripMaskFromText_PI(Mask, MaskPositions);
      }

      protected override async Task AfterAttached(VisualElement bindableAsVisualElement)
      {
         await base.AfterAttached(bindableAsVisualElement).WithoutChangingContext();

         if (bindableAsVisualElement is Entry bindableAsEntry)
         {
            bindableAsEntry.TextChanged += OnTextChanged;
         }

         await ConsiderDoNotForceMaskInitially().WithoutChangingContext();
      }

      protected override async Task AfterTextAssignment()
      {
         await base.AfterTextAssignment().WithoutChangingContext();

         SetUnmaskedText();
      }

      protected override async Task AfterUnattached(VisualElement bindableAsVisualElement)
      {
         await base.AfterAttached(bindableAsVisualElement).WithoutChangingContext();

         if (bindableAsVisualElement is Entry bindableAsEntry)
         {
            bindableAsEntry.TextChanged -= OnTextChanged;
         }
      }

      protected override Task<string> ConsiderMasking(string preparedNewText)
      {
         return preparedNewText.ConsiderMasking_PI(MaskPositions, LastValidatedText, MaxLength);
      }

      protected override (object, object) GetValuesFromView()
      {
         var retText = (BoundHost as Entry)?.Text;
         return (retText, default);

         // Returns NULL every other time
         // return (CurrentText, default);
      }

      private async Task ConsiderDoNotForceMaskInitially()
      {
         if (DoNotForceMaskInitially.IsFalse())
         {
            await ValidateText(CurrentText, CurrentText).WithoutChangingContext();
         }

         // Might be redundant in some cases but must occur.
         SetUnmaskedText();
      }

      private void CreateMaskPositions()
      {
         MaskPositions = Mask.CreateMaskPositions_PI();
      }

      private void SetUnmaskedText()
      {
         UnmaskedText = StripMaskFromText(CurrentText);
      }
   }
}