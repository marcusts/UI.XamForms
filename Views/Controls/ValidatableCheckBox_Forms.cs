// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ValidatableCheckBox.cs
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
   using System.Threading.Tasks;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Forms;

   /// <summary>
   ///    Interface IValidatableCheckBox_Forms Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
   /// </summary>
   /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
   public interface IValidatableCheckBox_Forms : IValidatableViewBase_Forms
   {
      CustomCheckBox_Forms EditableCheckBox { get; }
   }

   /// <summary>
   ///    A UI element that includes an CheckBox surrounded by a border. Implements the <see cref="Xamarin.Forms.Grid" />
   ///    Implements the <see cref="IValidatableCheckBox_Forms" />
   /// </summary>
   /// <seealso cref="Xamarin.Forms.Grid" />
   /// <seealso cref="IValidatableCheckBox_Forms" />
   public class ValidatableCheckBox_Forms : ValidatableViewBase_Forms, IValidatableCheckBox_Forms
   {
      private CustomCheckBox_Forms _editableCheckBox;

      public ValidatableCheckBox_Forms(bool asleepInitially = false)
         : base
         (
            CustomCheckBox_Forms.IsCheckedProperty,
            asleepInitially: asleepInitially
         )
      {
         if (!IsConstructing)
         {
            
            RecreateAllViewsBindingsAndStyles().FireAndFuhgetAboutIt();
         }
      }

      protected override bool DerivedViewIsFocused => false;

      public override View EditableView => EditableCheckBox;

      protected override View EditableViewContainer => EditableCheckBox;

      // Checked or not, the content is always valid
      protected override bool UserHasEnteredValidContent => true;

      public CustomCheckBox_Forms EditableCheckBox
      {
         get
         {
            if (_editableCheckBox.IsNullOrDefault())
            {
               _editableCheckBox = new CustomCheckBox_Forms { BackgroundColor = Color.Transparent, Margin = 7.5 };

               _editableCheckBox.IsCheckedChangedTask.AddIfNotAlreadyThere(this, HandleIsCheckedChanged);
            }

            return _editableCheckBox;
         }
      }

      private async Task HandleIsCheckedChanged(IResponsiveTaskParams paramDict)
      {
         
         await CallRevalidate().AndReturnToCallingContext();
         await ResetPlaceholderPosition().AndReturnToCallingContext();
      }
   }
}