// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ValidatableEnumPicker_Forms.cs
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
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Forms;

   /// <summary>
   /// Interface IValidatableEnumPicker_Forms Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
   /// </summary>
   /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
   public interface IValidatableEnumPicker_Forms : IValidatableViewBase_Forms
   {
      EnumPicker_Forms EditableEnumPicker { get; }
   }

   /// <summary>
   /// A UI element that includes an EnumPicker surrounded by a border. Implements the <see cref="Xamarin.Forms.Grid" />
   /// Implements the <see cref="IValidatableEnumPicker_Forms" />
   /// </summary>
   /// <seealso cref="Xamarin.Forms.Grid" />
   /// <seealso cref="IValidatableEnumPicker_Forms" />
   public class ValidatableEnumPicker_Forms : ValidatableViewBase_Forms, IValidatableEnumPicker_Forms
   {
      private readonly Type                      _enumType;
      private readonly double?                   _fontSize;
      private readonly string                    _viewModelPropertyName;
      private          EnumPicker_Forms _editableEnumPicker;

      public ValidatableEnumPicker_Forms
      (
         Type    enumType,
         string  viewModelPropertyName,
         double? fontSize        = null,
         bool    asleepInitially = false
      )
         : base(Picker.SelectedItemProperty, asleepInitially: asleepInitially)
      {
         _fontSize              = fontSize;
         _enumType              = enumType;
         _viewModelPropertyName = viewModelPropertyName;

         if (!IsConstructing)
         {
            RecreateAllViewsBindingsAndStyles().FireAndFuhgetAboutIt();
         }
      }

      protected override bool DerivedViewIsFocused => false;

      public override View EditableView => EditableEnumPicker;

      protected override View EditableViewContainer => EditableEnumPicker;

      protected override bool UserHasEnteredValidContent => EditableEnumPicker.SelectedItem.IsNotNullOrDefault();

      public EnumPicker_Forms EditableEnumPicker
      {
         get
         {
            if (_editableEnumPicker.IsNullOrDefault() && _enumType.IsNotNullOrDefault())
            {
               _editableEnumPicker = new EnumPicker_Forms(_enumType, "", _viewModelPropertyName)
                                     {
                                        FontSize        = _fontSize ?? UIConst_Forms.EDITABLE_VIEW_FONT_SIZE,
                                        BackgroundColor = Color.Transparent,
                                     };

               _editableEnumPicker.SelectedIndexChanged +=
                  async (sender, args) => { await CallRevalidate().AndReturnToCallingContext(); };
            }

            return _editableEnumPicker;
         }
      }
   }
}