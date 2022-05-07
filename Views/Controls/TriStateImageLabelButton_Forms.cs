// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=TriStateImageLabelButton.cs
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
   using System.Collections.Generic;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Forms;

   /// <summary>
   /// Interface ITriStateImageLabelButton_Forms Implements the
   /// <see
   ///    cref="IImageLabelButton_Forms" />
   /// </summary>
   /// <seealso cref="IImageLabelButton_Forms" />
   public interface ITriStateImageLabelButton_Forms : IImageLabelButton_Forms
   {
      // Can deselect when the selection group is 0.
      bool AllowCoercedDeselection { get; set; }

      /// <summary>Gets or sets the button deselected style.</summary>
      /// <value>The button deselected style.</value>
      Style ButtonDeselectedStyle { get; set; }

      /// <summary>Gets or sets the button disabled style.</summary>
      /// <value>The button disabled style.</value>
      Style ButtonDisabledStyle { get; set; }

      /// <summary>Gets or sets the button selected style.</summary>
      /// <value>The button selected style.</value>
      Style ButtonSelectedStyle { get; set; }

      /// <summary>Gets or sets a value indicating whether [button toggle selection].</summary>
      /// <value><c>true</c> if [button toggle selection]; otherwise, <c>false</c>.</value>
      bool ButtonToggleSelection { get; set; }

      /// <summary>Gets or sets a value indicating whether this instance can disable.</summary>
      /// <value><c>true</c> if this instance can disable; otherwise, <c>false</c>.</value>
      bool CanDisable { get; set; }

      /// <summary>Gets or sets a value indicating whether this instance can select.</summary>
      /// <value><c>true</c> if this instance can select; otherwise, <c>false</c>.</value>
      bool CanSelect { get; set; }

      /// <summary>Gets or sets a value indicating whether [get image from resource].</summary>
      /// <value><c>true</c> if [get image from resource]; otherwise, <c>false</c>.</value>
      bool GetImageFromResource { get; set; }

      /// <summary>Gets or sets the image deselected file path.</summary>
      /// <value>The image deselected file path.</value>
      string ImageDeselectedFilePath { get; set; }

      /// <summary>Gets or sets the image disabled file path.</summary>
      /// <value>The image disabled file path.</value>
      string ImageDisabledFilePath { get; set; }

      /// <summary>Gets or sets the type of the image resource class.</summary>
      /// <value>The type of the image resource class.</value>
      Type ImageResourceClassType { get; set; }

      /// <summary>Gets or sets the image selected file path.</summary>
      /// <value>The image selected file path.</value>
      string ImageSelectedFilePath { get; set; }

      bool IsCompleted { get; set; }

      /// <summary>Gets or sets the label deselected style.</summary>
      /// <value>The label deselected style.</value>
      Style LabelDeselectedStyle { get; set; }

      /// <summary>Gets or sets the label disabled style.</summary>
      /// <value>The label disabled style.</value>
      Style LabelDisabledStyle { get; set; }

      /// <summary>Gets or sets the label selected style.</summary>
      /// <value>The label selected style.</value>
      Style LabelSelectedStyle { get; set; }

      void ForceOnIsEnabledChanged();
   }

   /// <summary>
   /// A button that can contain either an image and/or a label. Implements the
   /// <see
   ///    cref="ImageLabelButtonBase_Forms" />
   /// Implements the
   /// <see
   ///    cref="ITriStateImageLabelButton_Forms" />
   /// </summary>
   /// <seealso cref="ImageLabelButtonBase_Forms" />
   /// <seealso cref="ITriStateImageLabelButton_Forms" />
   public class TriStateImageLabelButton_Forms : ImageLabelButtonBase_Forms, ITriStateImageLabelButton_Forms
   {
      public const string DESELECTED_BUTTON_STATE = "Deselected";
      public const string DISABLED_BUTTON_STATE   = "Disabled";
      public const string SELECTED_BUTTON_STATE   = "Selected";

      /// <summary>The button deselected style property</summary>
      public static readonly BindableProperty ButtonDeselectedStyleProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(ButtonDeselectedStyle),
            default(Style),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      /// <summary>The disabled button style property</summary>
      public static readonly BindableProperty ButtonDisabledStyleProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(ButtonDisabledStyle),
            default(Style),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      /// <summary>The selected button style property</summary>
      public static readonly BindableProperty ButtonSelectedStyleProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(ButtonSelectedStyle),
            default(Style),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      public static readonly BindableProperty ButtonToggleSelectionProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(ButtonToggleSelection),
            default(bool),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.ResetSelectionStyle();
            }
         );

      public static readonly BindableProperty CanDisableProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(CanDisable),
            default(bool),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               // Corner case -- get off of disabled if illegal
               if (viewButton.IsDisabled && !viewButton.CanDisable)
               {
                  viewButton.ButtonState = DESELECTED_BUTTON_STATE;
               }

               viewButton.OnIsEnabledChanged();
               viewButton.ResetSelectionStyle();
            }
         );

      /// <summary>The can select property</summary>
      public static readonly BindableProperty CanSelectProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(CanSelect),
            default(bool),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.ResetSelectionStyle();

               // Corner case
               if (viewButton.IsSelected && !viewButton.CanSelect)
               {
                  viewButton.ButtonState = DESELECTED_BUTTON_STATE;
               }
            }
         );

      /// <summary>The get image from resource property</summary>
      public static readonly BindableProperty GetImageFromResourceProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(GetImageFromResource),
            default(bool),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      /// <summary>The image deselected file path property</summary>
      public static readonly BindableProperty ImageDeselectedFilePathProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(ImageDeselectedFilePath),
            default(string),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      /// <summary>The image disabled file path property</summary>
      public static readonly BindableProperty ImageDisabledFilePathProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(ImageDisabledFilePath),
            default(string),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      /// <summary>The image resource class type property</summary>
      public static readonly BindableProperty ImageResourceClassTypeProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(ImageResourceClassType),
            default(Type),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      /// <summary>The image selected file path property</summary>
      public static readonly BindableProperty ImageSelectedFilePathProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(ImageSelectedFilePath),
            default(string),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      public static readonly BindableProperty IsCompletedProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(IsCompleted),
            default(bool),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               // If not selected, change the button state to disabled is that is legal
               if (viewButton.CanDisable && !viewButton.IsSelected)
               {
                  viewButton.ButtonState = DISABLED_BUTTON_STATE;
               }
            }
         );

      /// <summary>The label deselected style property</summary>
      public static readonly BindableProperty LabelDeselectedStyleProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(LabelDeselectedStyle),
            default(Style),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      /// <summary>The label disabled style property</summary>
      public static readonly BindableProperty LabelDisabledStyleProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(LabelDisabledStyle),
            default(Style),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      /// <summary>The label selected style property</summary>
      public static readonly BindableProperty LabelSelectedStyleProperty =
         CreateImageLabelButtonBindableProperty
         (
            nameof(LabelSelectedStyle),
            default(Style),
            BindingMode.OneWay,
            (
               viewButton,
               oldVal,
               newVal
            ) =>
            {
               viewButton.CreateOrRefreshImageLabelButtonStyles();
            }
         );

      /// <summary>The image label button styles</summary>
      private IList<IImageLabelButtonStyle_Forms> _imageLabelButtonStyles;

      /// <summary>Gets a value indicating whether this instance is disabled.</summary>
      /// <value><c>true</c> if this instance is disabled; otherwise, <c>false</c>.</value>
      protected override bool IsDisabled => ButtonState.IsSameAs(DISABLED_BUTTON_STATE);

      public bool AllowCoercedDeselection { get; set; }

      /// <summary>Gets or sets the button deselected style.</summary>
      /// <value>The button deselected style.</value>
      public Style ButtonDeselectedStyle
      {
         get => (Style)GetValue(ButtonDeselectedStyleProperty);
         set => SetValue(ButtonDeselectedStyleProperty, value);
      }

      /// <summary>Gets or sets the button disabled style.</summary>
      /// <value>The button disabled style.</value>
      public Style ButtonDisabledStyle
      {
         get => (Style)GetValue(ButtonDisabledStyleProperty);
         set => SetValue(ButtonDisabledStyleProperty, value);
      }

      /// <summary>Gets or sets the button selected style.</summary>
      /// <value>The button selected style.</value>
      public Style ButtonSelectedStyle
      {
         get => (Style)GetValue(ButtonSelectedStyleProperty);
         set => SetValue(ButtonSelectedStyleProperty, value);
      }

      /// <summary>Gets or sets a value indicating whether [button toggle selection].</summary>
      /// <value><c>true</c> if [button toggle selection]; otherwise, <c>false</c>.</value>
      public bool ButtonToggleSelection
      {
         get => (bool)GetValue(ButtonToggleSelectionProperty);
         set => SetValue(ButtonToggleSelectionProperty, value);
      }

      /// <summary>Gets or sets a value indicating whether this instance can disable.</summary>
      /// <value><c>true</c> if this instance can disable; otherwise, <c>false</c>.</value>
      public bool CanDisable
      {
         get => (bool)GetValue(CanDisableProperty);
         set => SetValue(CanDisableProperty, value);
      }

      /// <summary>Gets or sets a value indicating whether this instance can select.</summary>
      /// <value><c>true</c> if this instance can select; otherwise, <c>false</c>.</value>
      public bool CanSelect
      {
         get => (bool)GetValue(CanSelectProperty);
         set => SetValue(CanSelectProperty, value);
      }

      /// <summary>Gets or sets a value indicating whether [get image from resource].</summary>
      /// <value><c>true</c> if [get image from resource]; otherwise, <c>false</c>.</value>
      public bool GetImageFromResource
      {
         get => (bool)GetValue(GetImageFromResourceProperty);
         set => SetValue(GetImageFromResourceProperty, value);
      }

      /// <summary>Gets or sets the image deselected file path.</summary>
      /// <value>The image deselected file path.</value>
      public string ImageDeselectedFilePath
      {
         get => (string)GetValue(ImageDeselectedFilePathProperty);
         set => SetValue(ImageDeselectedFilePathProperty, value);
      }

      /// <summary>Gets or sets the image disabled file path.</summary>
      /// <value>The image disabled file path.</value>
      public string ImageDisabledFilePath
      {
         get => (string)GetValue(ImageDisabledFilePathProperty);
         set => SetValue(ImageDisabledFilePathProperty, value);
      }

      /// <summary>Gets the image label button styles.</summary>
      /// <value>The image label button styles.</value>
      public override IList<IImageLabelButtonStyle_Forms> ImageLabelButtonStyles => _imageLabelButtonStyles;

      /// <summary>Gets or sets the type of the image resource class.</summary>
      /// <value>The type of the image resource class.</value>
      public Type ImageResourceClassType
      {
         get => (Type)GetValue(ImageResourceClassTypeProperty);
         set => SetValue(ImageResourceClassTypeProperty, value);
      }

      /// <summary>Gets or sets the image selected file path.</summary>
      /// <value>The image selected file path.</value>
      public string ImageSelectedFilePath
      {
         get => (string)GetValue(ImageSelectedFilePathProperty);
         set => SetValue(ImageSelectedFilePathProperty, value);
      }

      public bool IsCompleted
      {
         get => (bool)GetValue(IsCompletedProperty);
         set => SetValue(IsCompletedProperty, value);
      }

      /// <summary>Gets a value indicating whether this instance is selected.</summary>
      /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
      public override bool IsSelected => ButtonState.IsSameAs(SELECTED_BUTTON_STATE);

      /// <summary>Gets or sets the label deselected style.</summary>
      /// <value>The label deselected style.</value>
      public Style LabelDeselectedStyle
      {
         get => (Style)GetValue(LabelDeselectedStyleProperty);
         set => SetValue(LabelDeselectedStyleProperty, value);
      }

      /// <summary>Gets or sets the label disabled style.</summary>
      /// <value>The label disabled style.</value>
      public Style LabelDisabledStyle
      {
         get => (Style)GetValue(LabelDisabledStyleProperty);
         set => SetValue(LabelDisabledStyleProperty, value);
      }

      /// <summary>Gets or sets the label selected style.</summary>
      /// <value>The label selected style.</value>
      public Style LabelSelectedStyle
      {
         get => (Style)GetValue(LabelSelectedStyleProperty);
         set => SetValue(LabelSelectedStyleProperty, value);
      }

      public void ForceOnIsEnabledChanged()
      {
         OnIsEnabledChanged();
      }

      /// <summary>Creates the image label button bindable property.</summary>
      /// <typeparam name="PropertyTypeT">The type of the property type t.</typeparam>
      /// <param name="localPropName">Name of the local property.</param>
      /// <param name="defaultVal">The default value.</param>
      /// <param name="bindingMode">The binding mode.</param>
      /// <param name="callbackAction">The callback action.</param>
      /// <returns>BindableProperty.</returns>
      public static BindableProperty CreateImageLabelButtonBindableProperty<PropertyTypeT>
      (
         string                                                                  localPropName,
         PropertyTypeT                                                           defaultVal     = default,
         BindingMode                                                             bindingMode    = BindingMode.OneWay,
         Action<TriStateImageLabelButton_Forms, PropertyTypeT, PropertyTypeT> callbackAction = null
      )
      {
         return BindableUtils_Forms.CreateBindableProperty(localPropName, defaultVal, bindingMode, callbackAction);
      }

      /// <summary>Deselects this instance.</summary>
      protected override void Deselect()
      {
         if (IsCompleted && CanDisable)
         {
            // "Disabled" is the same as "completed"
            ButtonState = DISABLED_BUTTON_STATE;
         }
         else
         {
            ButtonState = DESELECTED_BUTTON_STATE;
         }
      }

      protected override IImageLabelButtonStyle_Forms LastCheckBeforeAssigningStyle(IImageLabelButtonStyle_Forms style)
      {
         if (IsCompleted && CanDisable && style.InternalButtonState.IsSameAs(DESELECTED_BUTTON_STATE))
         {
            if (ButtonStateIndexFound(DISABLED_BUTTON_STATE, out var disabledStyleIdx))
            {
               return ImageLabelButtonStyles[disabledStyleIdx];
            }
         }

         // ELSE let the base do the work
         return base.LastCheckBeforeAssigningStyle(style);
      }

      /// <summary>Called when [button command created].</summary>
      protected override void OnButtonCommandCreated()
      {
         CreateOrRefreshImageLabelButtonStyles();
      }

      protected override void OnIsEnabledChanged()
      {
         if (!IsEnabled && CanDisable)
         {
            ButtonState = DISABLED_BUTTON_STATE;
         }
         else if (IsEnabled)
         {
            ButtonState = IsSelected ? SELECTED_BUTTON_STATE : DESELECTED_BUTTON_STATE;
         }

         ResetSelectionStyle();
      }

      /// <summary>Assigns the styles if not null.</summary>
      /// <param name="imageLabelButtonStyle">The image label button style.</param>
      /// <param name="buttonStyle">The button style.</param>
      /// <param name="imageFilePath">The image file path.</param>
      /// <param name="labelStyle">The label style.</param>
      private void AssignStylesIfNotNull
      (
         IImageLabelButtonStyle_Forms imageLabelButtonStyle,
         Style                 buttonStyle,
         string                imageFilePath,
         Style                 labelStyle
      )
      {
         if (imageLabelButtonStyle == null)
         {
            return;
         }

         imageLabelButtonStyle.ButtonStyle            = buttonStyle;
         imageLabelButtonStyle.ImageFilePath          = imageFilePath;
         imageLabelButtonStyle.GetImageFromResource   = GetImageFromResource;
         imageLabelButtonStyle.ImageResourceClassType = ImageResourceClassType;
         imageLabelButtonStyle.LabelStyle             = labelStyle;
      }

      /// <summary>Creates the or refresh image label button styles.</summary>
      /// <param name="forceCreate">if set to <c>true</c> [force create].</param>
      private void CreateOrRefreshImageLabelButtonStyles(bool forceCreate = false)
      {
         IImageLabelButtonStyle_Forms deselectedStyle = default;
         var                                   selectedStyle   = new ImageLabelButtonStyle_Forms();
         var                                   disabledStyle   = new ImageLabelButtonStyle_Forms();

         _imageLabelButtonStyles = new List<IImageLabelButtonStyle_Forms>();

         deselectedStyle = new ImageLabelButtonStyle_Forms
                                   {
                                      InternalButtonState    = DESELECTED_BUTTON_STATE,
                                      GetImageFromResource   = GetImageFromResource,
                                      ImageResourceClassType = ImageResourceClassType,
                                   };

         ImageLabelButtonStyles.Add(deselectedStyle);

         if (CanSelect || ButtonToggleSelection)
         {
            selectedStyle = new ImageLabelButtonStyle_Forms
                            {
                               InternalButtonState    = SELECTED_BUTTON_STATE,
                               GetImageFromResource   = GetImageFromResource,
                               ImageResourceClassType = ImageResourceClassType,
                            };

            ImageLabelButtonStyles.Add(selectedStyle);
         }

         if (CanDisable)
         {
            disabledStyle = new ImageLabelButtonStyle_Forms
                            {
                               InternalButtonState    = DISABLED_BUTTON_STATE,
                               GetImageFromResource   = GetImageFromResource,
                               ImageResourceClassType = ImageResourceClassType,
                            };

            ImageLabelButtonStyles.Add(disabledStyle);
         }

         // Now update these from our proprietary styles ON refresh, there will be no creation, so this step is
         // necessary.
         AssignStylesIfNotNull(deselectedStyle, ButtonDeselectedStyle, ImageDeselectedFilePath, LabelDeselectedStyle);
         AssignStylesIfNotNull(selectedStyle,   ButtonSelectedStyle,   ImageSelectedFilePath,   LabelSelectedStyle);
         AssignStylesIfNotNull(disabledStyle,   ButtonDisabledStyle,   ImageDisabledFilePath,   LabelDisabledStyle);

         FixNullButtonState();

         // Update the variable "CurrentStyle" with these changes
         UpdateCurrentStyleFromButtonState(ButtonState);

         SetAllStyles();
      }

      private void FixNullButtonState()
      {
         if (ButtonState.IsEmpty() && (ImageLabelButtonStyles.Count > 0))
         {
            ButtonState = DESELECTED_BUTTON_STATE;
         }
      }

      /// <summary>Resets the selection style.</summary>
      private void ResetSelectionStyle()
      {
         SelectionStyle = CanSelect
            ? ButtonToggleSelection
               ? ImageLabelButtonSelectionStyles.ToggleSelectionAsFirstTwoStyles
               : ImageLabelButtonSelectionStyles.SelectionButNoToggleAsFirstTwoStyles
            : ImageLabelButtonSelectionStyles.NoSelection;

         CreateOrRefreshImageLabelButtonStyles(true);
      }
   }
}