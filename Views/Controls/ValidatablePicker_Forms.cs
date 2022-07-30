// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ValidatablePicker_Forms.cs
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
   using System.Collections;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Validations;
   using Xamarin.Forms;

   /// <summary>
   /// Interface IValidatablePicker Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
   /// </summary>
   /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
   public interface IValidatablePicker_Forms : IValidatableViewBase_Forms
   {
      Picker EditablePicker { get; }
   }

   /// <summary>
   /// A UI element that includes an Picker surrounded by a border. Implements the <see cref="Xamarin.Forms.Grid" />
   /// Implements the <see cref="IValidatablePicker_Forms" />
   /// </summary>
   /// <seealso cref="Xamarin.Forms.Grid" />
   /// <seealso cref="IValidatablePicker_Forms" />
   public class ValidatablePicker_Forms : ValidatableViewBase_Forms, IValidatablePicker_Forms
   {
      private readonly double? _fontSize;
      private readonly IList   _items;
      private          Picker  _editablePicker;

      public ValidatablePicker_Forms
      (
         IList                         items,
         bool                          emptyAllowed    = false,
         double?                       fontSize        = null,
         IValidationBehaviorBase_Forms validator       = null,
         string                        recordName      = "",
         bool                          asleepInitially = false
      )
         : base
         (
            Picker.SelectedItemProperty,
            asleepInitially: asleepInitially
         )
      {
         BackgroundColor = Color.Transparent;
         _fontSize       = fontSize;
         _items          = items;

         if (EditablePicker.IsNotNullOrDefault())
         {
            EditablePicker.ItemsSource = _items;
         }

         Validator = validator ??
            new PickerValidationBehavior_Forms
            { RecordName = recordName, EmptyAllowed = emptyAllowed, };

         if (!IsConstructing)
         {
            RecreateAllViewsBindingsAndStyles().FireAndFuhgetAboutIt();
         }
      }

      protected override bool DerivedViewIsFocused  => false;
      protected override View EditableViewContainer => EditablePicker;

      protected override bool UserHasEnteredValidContent => EditablePicker.SelectedItem.IsNotNullOrDefault();

      public Picker EditablePicker
      {
         get
         {
            if (_editablePicker.IsNullOrDefault())
            {
               _editablePicker =
                  new Picker
                  {
                     FontSize        = _fontSize ?? UIConst_Forms.EDITABLE_VIEW_FONT_SIZE,
                     ItemsSource     = _items,
                     BackgroundColor = Color.Transparent,
                  };

               _editablePicker.SelectedIndexChanged +=
                  async
                     (sender, args) =>
                  {
                     await CallRevalidate().AndReturnToCallingContext();
                     await ResetPlaceholderPosition().AndReturnToCallingContext();
                  };
            }

            return _editablePicker;
         }
      }

      public override View EditableView => EditablePicker;
   }
}