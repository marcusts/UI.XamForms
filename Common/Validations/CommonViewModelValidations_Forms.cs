// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=CommonViewModelValidations_Forms.cs
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
   using System.Runtime.CompilerServices;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;

   /// <summary>
   /// Because all inputs are two-way, that is implied (so not controlled here or at <see cref="ViewModelValidationAttribute_PI"/>.
   /// </summary>
   public static class CommonViewModelValidations_Forms
   {
      public const string CONFIRM_PASSWORD_PLACEHOLDER_TEXT = "Confirm " + PASSWORD_PLACEHOLDER_TEXT;
      public const string FORBIDDEN_CHARS                   = "!@#$%^&";
      public const int    MUST_BE_NON_EMPTY_MIN_LENGTH      = 1;
      public const string PASSWORD_PLACEHOLDER_TEXT         = "Password";
      public const int    PHONE_EDIT_LEN_INCLUDING_MASK     = 14;
      public const string PHONE_NUMBER_FORMAT               = "{0:(###) ###-####}";
      public const string PHONE_NUMBER_MASK                 = "(XXX) XXX-XXXX";
      public const string PHONE_PLACEHOLDER_TEXT            = "Phone";
      public const string USER_NAME_PLACEHOLDER_TEXT        = "User Name";

      public class DateTimeValidatableTwoWayNonEmptyViewModelValidationAttribute :
         ValidatableTwoWayNonEmptyViewModelValidationAttribute
      {
         public DateTimeValidatableTwoWayNonEmptyViewModelValidationAttribute(int                       displayOrder,
                                                                              [CallerMemberName] string propName = "")
            : base(displayOrder, propName)
         {
            InputTypeStr = ValidationUtils_PI.NULLABLE_DATE_TIME_INPUT_TYPE;
         }
      }

      public class NonEmptyViewModelValidationAttribute : ViewModelValidationAttribute_PI
      {
         public NonEmptyViewModelValidationAttribute(int displayOrder, [CallerMemberName] string propName = "")
            : base(displayOrder, viewModelPropertyName: propName)
         {
            MinLength = MUST_BE_NON_EMPTY_MIN_LENGTH;
         }
      }

      public class
         PasswordValidatableTwoWayNonEmptyViewModelValidationAttribute :
            ValidatableTwoWayNonEmptyViewModelValidationAttribute
      {
         public PasswordValidatableTwoWayNonEmptyViewModelValidationAttribute(
            int displayOrder, [CallerMemberName] string propName = "")
            : base(displayOrder, propName)
         {
            CanUnmaskPassword = ViewModelCustomAttribute_Static_PI.TRUE_BOOL;
            IsPassword        = ViewModelCustomAttribute_Static_PI.TRUE_BOOL;
            MinLength         = ValidationUtils_PI.DEFAULT_MIN_CHARACTER_COUNT;
            MaxLength         = ValidationUtils_PI.DEFAULT_MAX_CHARACTER_COUNT;
            PlaceholderText   = PASSWORD_PLACEHOLDER_TEXT;
            ValidatorType     = typeof(PasswordEntryValidationBehavior_Forms);
         }
      }

      public class
         PhoneValidatableTwoWayNonEmptyViewModelValidationAttribute :
            ValidatableTwoWayNonEmptyViewModelValidationAttribute
      {
         public PhoneValidatableTwoWayNonEmptyViewModelValidationAttribute(
            int displayOrder, [CallerMemberName] string propName = "")
            : base(displayOrder, propName)
         {
            PlaceholderText = PHONE_PLACEHOLDER_TEXT;
            MinLength       = PHONE_EDIT_LEN_INCLUDING_MASK;
            MaxLength       = PHONE_EDIT_LEN_INCLUDING_MASK;
            Mask            = PHONE_NUMBER_MASK;
            StringFormat    = PHONE_NUMBER_FORMAT;
            ValidatorType   = typeof(PhoneEntryValidatorBehavior_Forms);
         }
      }

      public class TwoWayNonEmptyViewModelValidationAttribute : NonEmptyViewModelValidationAttribute
      {
         public TwoWayNonEmptyViewModelValidationAttribute(int displayOrder, [CallerMemberName] string propName = "")
            : base(displayOrder, propName)
         { }
      }

      public class
         UserNameValidatableTwoWayNonEmptyValidationAttribute : ValidatableTwoWayNonEmptyViewModelValidationAttribute
      {
         public UserNameValidatableTwoWayNonEmptyValidationAttribute(int                       displayOrder,
                                                                     [CallerMemberName] string propName = "")
            : base(displayOrder, propName)
         {
            PlaceholderText = USER_NAME_PLACEHOLDER_TEXT;
         }
      }

      public class ValidatableTwoWayNonEmptyViewModelValidationAttribute : TwoWayNonEmptyViewModelValidationAttribute
      {
         public ValidatableTwoWayNonEmptyViewModelValidationAttribute(int                       displayOrder,
                                                                      [CallerMemberName] string propName = "")
            : base(displayOrder, propName)
         {
            ShowValidationErrors = ViewModelCustomAttribute_Static_PI.TRUE_BOOL;
         }
      }
   }
}