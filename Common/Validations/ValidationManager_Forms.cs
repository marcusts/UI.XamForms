// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=ValidationManager_Forms.cs
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
   using System;
   using System.Collections;
   using System.Linq;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Controls;
   using Xamarin.Forms;

   public interface IValidationManager_Forms
   {
      IEntryValidationBehavior_Forms ChooseEntryValidator(Type   attributeValidatorType,
                                                          string numericType);

      Task<(IValidatableView_Forms, ICanBeValid_PI, int)> CreateValidatableEditorsForAttribute
      (
         IHaveValidationViewModelHelper      viewModel,
         IViewModelValidationAttribute_PI attribute,
         double                              itemHeight,
         double                              itemWidth,
         double                              fontSize,
         int                                 tabIndex
      );
   }

   public class ValidationManager_Forms : IValidationManager_Forms
   {
      private static readonly Thickness EXTRA_TOP_SPACE_MARGIN = new Thickness(0, 10.0.AdjustForOsAndDevice(), 0, 0);
      
      public virtual IEntryValidationBehavior_Forms ChooseEntryValidator(Type   attributeValidatorType,
                                                                         string numericType)
      {
         IEntryValidationBehavior_Forms retEntryValidationBehavior = new EntryValidationBehavior_Forms();

         if (attributeValidatorType == typeof(PasswordEntryValidationBehavior_Forms))
         {
            retEntryValidationBehavior = new PasswordEntryValidationBehavior_Forms();
         }
         else if (attributeValidatorType == typeof(PhoneEntryValidatorBehavior_Forms))
         {
            retEntryValidationBehavior = new PhoneEntryValidatorBehavior_Forms();
         }
         else if (attributeValidatorType == typeof(ComparisonEntryValidatorBehavior_Forms))
         {
            retEntryValidationBehavior = new ComparisonEntryValidatorBehavior_Forms();
         }
         else if (attributeValidatorType == typeof(EmailEntryValidatorBehavior_Forms))
         {
            retEntryValidationBehavior = new EmailEntryValidatorBehavior_Forms();
         }
         else if (attributeValidatorType == typeof(NumericEntryValidationBehavior_Forms))
         {
            retEntryValidationBehavior = new NumericEntryValidationBehavior_Forms();
         }
         //else if (attributeValidatorType == typeof(EntryValidationBehavior_Forms))
         //{
         //   retEntryValidationBehavior = new EntryValidationBehavior_Forms();
         //}
         else if
         (
            numericType.IsSameAs(ValidationUtils_PI.DOUBLE_NUMERIC_TYPE)          ||
            numericType.IsSameAs(ValidationUtils_PI.INT_NUMERIC_TYPE)             ||
            numericType.IsSameAs(ValidationUtils_PI.LONG_NUMERIC_TYPE)            ||
            numericType.IsSameAs(ValidationUtils_PI.NULLABLE_DOUBLE_NUMERIC_TYPE) ||
            numericType.IsSameAs(ValidationUtils_PI.NULLABLE_INT_NUMERIC_TYPE)    ||
            numericType.IsSameAs(ValidationUtils_PI.NULLABLE_LONG_NUMERIC_TYPE)
         )
         {
            retEntryValidationBehavior = new NumericEntryValidationBehavior_Forms();
         }

         return retEntryValidationBehavior;
      }

      public virtual async Task<(IValidatableView_Forms, ICanBeValid_PI, int)>
         CreateValidatableEditorsForAttribute
         (
            IHaveValidationViewModelHelper      viewModel,
            IViewModelValidationAttribute_PI attribute,
            double                              itemHeight,
            double                              itemWidth,
            double                              fontSize,
            int                                 tabIndex
         )
      {
         ICanBeValid_PI retValidator;
         var            nextTabIndex = tabIndex;

         if (viewModel.IsNullOrDefault())
         {
            return default;
         }

         var keyboard = UIUtils_Forms.GetKeyboardFromString(attribute.KeyboardName);

         switch (attribute.InputTypeStr)
         {
            case ValidationUtils_PI.STATE_INPUT_TYPE:
               return await SetUpValidatablePicker(ValidationUtils_PI.STATIC_STATES, nextTabIndex)
                 .WithoutChangingContext();

            case ValidationUtils_PI.MONTH_INPUT_TYPE:
            {
               return await SetUpValidatablePicker(ValidationUtils_PI.STATIC_MONTHS, nextTabIndex)
                 .WithoutChangingContext();
            }

            case ValidationUtils_PI.EXPIRATION_YEAR_INPUT_TYPE:
               return await SetUpValidatablePicker(ValidationUtils_PI.STATIC_EXPIRATION_YEARS, nextTabIndex)
                 .WithoutChangingContext();

            case ValidationUtils_PI.DATE_TIME_INPUT_TYPE:
            case ValidationUtils_PI.NULLABLE_DATE_TIME_INPUT_TYPE:
            {
               var dateTimePicker =
                  new ValidatableDateTime_Forms(
                     !attribute.MinLength.IsANumberGreaterThanZero(),
                     asleepInitially: true,
                     returnNonNullableResult: attribute.InputTypeStr.IsSameAs(ValidationUtils_PI.DATE_TIME_INPUT_TYPE)
                  );

               dateTimePicker.CopyCommmonPropertiesFromAttribute_Forms(attribute);
               retValidator = ValidatorFromView(dateTimePicker);
               SetUpValidator(retValidator as IValidationBehaviorBase_Forms);
               await dateTimePicker.StopConstructionAndRefresh().WithoutChangingContext();

               nextTabIndex =
                  dateTimePicker.EditableDatePicker.AssignInternalDateTimeProperties(itemHeight,
                     fontSize,
                     nextTabIndex);

               dateTimePicker.SetBorderViewHeightIfNecessary(itemWidth, itemHeight);
               dateTimePicker.Margin = EXTRA_TOP_SPACE_MARGIN;
               
               return (dateTimePicker, retValidator, nextTabIndex);
            }

            case ValidationUtils_PI.CHECK_BOX_INPUT_TYPE:
            {
               var checkBoxPicker = new ValidatableCheckBox_Forms(true);

               checkBoxPicker.CopyCommmonPropertiesFromAttribute_Forms(attribute);
               retValidator                = ValidatorFromView(checkBoxPicker);
               await checkBoxPicker.StopConstructionAndRefresh().WithoutChangingContext();
               
               nextTabIndex = 
                  checkBoxPicker.EditableCheckBox.AssignInternalViewProperties(itemHeight, nextTabIndex);

               checkBoxPicker.SetBorderViewHeightIfNecessary(itemWidth, itemHeight);
               checkBoxPicker.Margin = EXTRA_TOP_SPACE_MARGIN;
               
               return (checkBoxPicker, retValidator, nextTabIndex);
            }

            case ValidationUtils_PI.TEXT_INPUT_TYPE:
            {
               var validator =
                  ChooseEntryValidator(attribute.ValidatorType, attribute.NumericType);

               IValidatableEntry_Forms entry;

               if (attribute.NumericType.IsSameAs(ValidationUtils_PI.NO_NUMERIC_TYPE))
               {
                  entry =
                     new ValidatableEntry_Forms
                     (
                        fontSize,
                        attribute.IsPassword.IsTrue(),
                        keyboard,
                        true
                     )
                     {
                        Validator = validator,
                     };
               }
               else
               {
                  entry =
                     new ValidatableNumericEntry_Forms(
                        fontSize,
                        attribute.StringFormat,
                        validator as INumericEntryValidationBehavior_Forms,
                        Keyboard.Numeric, true);
               }

               entry.CopyCommmonPropertiesFromAttribute_Forms(attribute);
               retValidator = validator;
               SetUpValidator(validator);
               
               ((View) entry).WidthRequest = itemWidth;

               await entry.StopConstructionAndRefresh().WithoutChangingContext();

               nextTabIndex =
                  entry.EditableEntry.AssignInternalEntryProperties(itemHeight, fontSize, nextTabIndex);

               entry.SetBorderViewHeightIfNecessary(itemWidth, itemHeight);
               
               return (entry, retValidator, nextTabIndex);
            }

            default:
               return default;
         }

         // -------------------------------------------------------------------------------------------------------
         // PRIVATE METHODS
         // -------------------------------------------------------------------------------------------------------
         async Task<(IValidatableView_Forms, ICanBeValid_PI, int)> SetUpValidatablePicker(
            IList items, int nextTabIdx)
         {
            var picker =
               new ValidatablePicker_Forms(items, !attribute.MinLength.IsANumberGreaterThanZero(), fontSize,
                  asleepInitially: true);

            picker.CopyCommmonPropertiesFromAttribute_Forms(attribute);

            retValidator = ValidatorFromView(picker);

            SetUpValidator(retValidator as IValidationBehaviorBase_Forms);

            await picker.StopConstructionAndRefresh().WithoutChangingContext();

            nextTabIdx =
               picker.EditablePicker.AssignInternalPickerProperties(itemHeight, fontSize, nextTabIdx);
            
            picker.SetBorderViewHeightIfNecessary(itemWidth, itemHeight);
            picker.Margin = EXTRA_TOP_SPACE_MARGIN;

            return (picker, retValidator, nextTabIdx);
         }

         async Task HandleValidatorIsValidChangedTask(IResponsiveTaskParams paramDict)
         {
            await viewModel.ValidationHelper.RevalidateBehaviors(true).WithoutChangingContext();
         }

         void SetUpValidator<T>(T validator)
            where T : class, IValidationBehaviorBase_Forms
         {
            if (validator.IsNotNullOrDefault())
            {
               validator.IsValidChangedTask.AddIfNotAlreadyThere(viewModel, HandleValidatorIsValidChangedTask);

               validator.CopyCommmonPropertiesFromAttribute_Forms(attribute);
            }
         }
      }

      public static ICanBeValid_PI ValidatorFromView(View view)
      {
         return view is IValidatableView_Forms viewAsValidatable
            ? viewAsValidatable.Validator
            : view.Behaviors?.FirstOrDefault(b => b is ICanBeValid_PI) as ICanBeValid_PI;
      }
   }
}