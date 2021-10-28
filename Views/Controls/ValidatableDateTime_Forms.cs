// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=ValidatableDateTime_Forms.cs
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
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Validations;
   using Xamarin.Forms;

   /// <summary>
   /// Interface IValidatableDateTime Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
   /// </summary>
   /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
   public interface IValidatableDateTime_Forms : IValidatableViewBase_Forms
   {
      IResponsiveTasks NullableResultChangedTask { get; set; }
      DatePicker       EditableDatePicker        { get; }
      DateTime?        NullableResult            { get; set; }
   }

   /// <summary>
   /// A UI element that includes an DateTime surrounded by a border. Implements the <see cref="Xamarin.Forms.Grid" />
   /// Implements the <see cref="IValidatableDateTime_Forms" />
   /// </summary>
   /// <seealso cref="Xamarin.Forms.Grid" />
   /// <seealso cref="IValidatableDateTime_Forms" />
   public class ValidatableDateTime_Forms : ValidatableViewBase_Forms, IValidatableDateTime_Forms
   {
      public static readonly BindableProperty NullableResultProperty =
         CreateValidatableViewBindableProperty
         (
            nameof(NullableResult),
            default(DateTime?),
            BindingMode.TwoWay,
            (
               view,
               oldVal,
               newVal
            ) =>
            {
               view.EditableDatePicker.Date = newVal.GetValueOrDefault();
               view.NullableResultChangedTask.RunAllTasksUsingDefaults(newVal);
            }
         );

      private readonly double?    _fontSize;
      private          DatePicker _editableDateTime;

      public ValidatableDateTime_Forms
      (
         bool                                emptyAllowed            = false,
         double?                             fontSize                = null,
         bool                                returnNonNullableResult = false,
         IValidationBehaviorBase_Forms validator               = null,
         string                              recordName              = "",
         bool                                asleepInitially         = false
      )
         : base
         (
            returnNonNullableResult ? DatePicker.DateProperty : NullableResultProperty,
            asleepInitially: asleepInitially
         )
      {
         _fontSize = fontSize;

         Validator = validator ??
            new DatePickerValidationBehavior_Forms
            { RecordName = recordName, EmptyAllowed = emptyAllowed, };

         if (!IsConstructing)
         {
            RecreateAllViewsBindingsAndStyles().FireAndFuhgetAboutIt();
         }
      }

      protected override bool DerivedViewIsFocused => false;

      public override View EditableView => EditableDatePicker;

      protected override View EditableViewContainer => EditableDatePicker;

      protected override bool UserHasEnteredValidContent => EditableDatePicker.Date.IsNotEmpty();

      public IResponsiveTasks NullableResultChangedTask { get; set; } = new ResponsiveTasks(1);

      public DatePicker EditableDatePicker
      {
         get
         {
            if (_editableDateTime.IsNullOrDefault())
            {
               _editableDateTime = new DatePicker
                                   {
                                      FontSize = _fontSize ?? UIConst_Forms.EDITABLE_VIEW_FONT_SIZE,
                                   };

               _editableDateTime.DateSelected +=
                  (sender, args) =>
                  {
                     NullableResult = _editableDateTime.Date;


                     CallRevalidate().FireAndFuhgetAboutIt();
                  };
            }

            return _editableDateTime;
         }
      }

      public DateTime? NullableResult
      {
         get => (DateTime?)GetValue(NullableResultProperty);
         set => SetValue(NullableResultProperty, value);
      }

      public static BindableProperty CreateValidatableViewBindableProperty<PropertyTypeT>
      (
         string                                                             localPropName,
         PropertyTypeT                                                      defaultVal     = default,
         BindingMode                                                        bindingMode    = BindingMode.OneWay,
         Action<ValidatableDateTime_Forms, PropertyTypeT, PropertyTypeT> callbackAction = null
      )
      {
         return BindableUtils_Forms.CreateBindableProperty(localPropName, defaultVal, bindingMode, callbackAction);
      }

      protected override void OnBindingContextChanged()
      {
         base.OnBindingContextChanged();

         NullableResult = EditableDatePicker?.Date;
      }
   }
}