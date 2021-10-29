// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=SimpleImageLabelButton_Forms.cs
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

// #define SHOW_BACK_COLORS

namespace Com.MarcusTS.UI.XamForms.Views.Controls
{
   using System;
   using Com.MarcusTS.PlatformIndependentShared.Annotations;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Xamarin.Forms;

   /// <summary>
   /// Interface ISimpleImageLabelButton_Forms
   /// Implements the <see cref="ITriStateImageLabelButton_Forms" />
   /// </summary>
   /// <seealso cref="ITriStateImageLabelButton_Forms" />
   public interface ISimpleImageLabelButton_Forms : ITriStateImageLabelButton_Forms
   {
      /// <summary>
      /// Gets or sets the color of the button back.
      /// </summary>
      /// <value>The color of the button back.</value>
      Color ButtonBackColor { get; set; }

      /// <summary>
      /// Gets or sets the color of the button border.
      /// </summary>
      /// <value>The color of the button border.</value>
      Color ButtonBorderColor { get; set; }

      /// <summary>
      /// Gets or sets the width of the button border.
      /// </summary>
      /// <value>The width of the button border.</value>
      float ButtonBorderWidth { get; set; }

      /// <summary>
      /// Gets or sets the image file path.
      /// </summary>
      /// <value>The image file path.</value>
      string ImageFilePath { get; set; }

      /// <summary>
      /// Gets or sets the label font attributes.
      /// </summary>
      /// <value>The label font attributes.</value>
      FontAttributes LabelFontAttributes { get; set; }

      /// <summary>
      /// Gets or sets the size of the label font.
      /// </summary>
      /// <value>The size of the label font.</value>
      double LabelFontSize { get; set; }

      /// <summary>
      /// Gets or sets the label text.
      /// </summary>
      /// <value>The label text.</value>
      string LabelText { get; set; }

      /// <summary>
      /// Gets or sets the color of the label text.
      /// </summary>
      /// <value>The color of the label text.</value>
      Color LabelTextColor { get; set; }
   }

   /// <summary>
   /// A button that can contain either an image and/or a label.
   /// This button is not selectable or toggle-able.
   /// Implements the
   /// <see cref="Com.MarcusTS.UI.XamForms.Views.Controls.TriStateImageLabelButton_Forms" />
   /// Implements the <see cref="ISimpleImageLabelButton_Forms" />
   /// </summary>
   /// <seealso cref="Com.MarcusTS.UI.XamForms.Views.Controls.TriStateImageLabelButton_Forms" />
   /// <seealso cref="ISimpleImageLabelButton_Forms" />
   public class SimpleImageLabelButton_Forms : TriStateImageLabelButton_Forms, ISimpleImageLabelButton_Forms
   {
      /// <summary>
      /// The button back color property
      /// </summary>
      public static readonly BindableProperty ButtonBackColorProperty =
         CreateSimpleImageLabelButtonBindableProperty
         (
            nameof( ButtonBackColor ),
            default( Color ),
            BindingMode.OneWay,
            (
               simpleButton,
               oldVal,
               newVal
            ) =>
            {
               simpleButton.ResetButtonStyle();
            }
         );

      /// <summary>
      /// The button border color property
      /// </summary>
      public static readonly BindableProperty ButtonBorderColorProperty =
         CreateSimpleImageLabelButtonBindableProperty
         (
            nameof( ButtonBorderColor ),
            default( Color ),
            BindingMode.OneWay,
            (
               simpleButton,
               oldVal,
               newVal
            ) =>
            {
               simpleButton.ResetButtonStyle();
            }
         );

      /// <summary>
      /// The button border width property
      /// </summary>
      public static readonly BindableProperty ButtonBorderWidthProperty =
         CreateSimpleImageLabelButtonBindableProperty
         (
            nameof( ButtonBorderWidth ),
            default( float ),
            BindingMode.OneWay,
            (
               simpleButton,
               oldVal,
               newVal
            ) =>
            {
               simpleButton.ResetButtonStyle();
            }
         );

      /// <summary>
      /// The image file path property
      /// </summary>
      public static readonly BindableProperty ImageFilePathProperty =
         CreateSimpleImageLabelButtonBindableProperty
         (
            nameof( ImageFilePath ),
            default( string ),
            BindingMode.OneWay,
            (
               simpleButton,
               oldVal,
               newVal
            ) =>
            {
               simpleButton.ImageDeselectedFilePath = newVal;
               simpleButton.ResetButtonStyle();
            },
            ( simpleButton, value ) => simpleButton.ImageDeselectedFilePath );

      /// <summary>
      /// The label font attributes property
      /// </summary>
      public static readonly BindableProperty LabelFontAttributesProperty =
         CreateSimpleImageLabelButtonBindableProperty
         (
            nameof( LabelFontAttributes ),
            default( FontAttributes ),
            BindingMode.OneWay,
            (
               simpleButton,
               oldVal,
               newVal
            ) =>
            {
               simpleButton.ResetLabelStyle();
            }
         );

      /// <summary>
      /// The label font size property
      /// </summary>
      public static readonly BindableProperty LabelFontSizeProperty =
         CreateSimpleImageLabelButtonBindableProperty
         (
            nameof( LabelFontSize ),
            default( double ),
            BindingMode.OneWay,
            (
               simpleButton,
               oldVal,
               newVal
            ) =>
            {
               simpleButton.ResetLabelStyle();
            }
         );

      /// <summary>
      /// The label text color property
      /// </summary>
      public static readonly BindableProperty LabelTextColorProperty =
         CreateSimpleImageLabelButtonBindableProperty
         (
            nameof( LabelTextColor ),
            default( Color ),
            BindingMode.OneWay,
            (
               simpleButton,
               oldVal,
               newVal
            ) =>
            {
               simpleButton.ResetLabelStyle();
            }
         );

      /// <summary>
      /// The label text property
      /// </summary>
      public static readonly BindableProperty LabelTextProperty =
         CreateSimpleImageLabelButtonBindableProperty
         (
            nameof( LabelText ),
            default( string ),
            BindingMode.OneWay,
            (
               simpleButton,
               oldVal,
               newVal
            ) =>
            {
               simpleButton.ResetLabelStyle();
            }
         );

      /// <summary>
      /// Initializes a new instance of the <see cref="SimpleImageLabelButton_Forms" /> class.
      /// </summary>
      public SimpleImageLabelButton_Forms
      (
         double?            labelWidth         = default,
         double?            labelHeight        = default,
         [CanBeNull] string fontFamilyOverride = ""
      )
      {
         CanSelect             = false;
         ButtonToggleSelection = false;
         ButtonLabel           = UIUtils_Forms.GetSimpleLabel();

         if ( labelWidth.HasValue )
         {
            ButtonLabel.WidthRequest = labelWidth.GetValueOrDefault();
         }

         if ( labelHeight.HasValue )
         {
            ButtonLabel.HeightRequest = labelHeight.GetValueOrDefault();
         }

         if ( fontFamilyOverride.IsNotNullOrDefault() )
         {
            ButtonLabel.FontFamily = fontFamilyOverride;
         }

#if SHOW_BACK_COLORS
         //ButtonLabel.WidthRequest = 200;
         //ButtonLabel.HeightRequest = 50;
         ButtonLabel.BackgroundColor = Color.Yellow;
#endif

         SetAllStyles();
      }

      /// <summary>
      /// Gets a value indicating whether this instance is disabled.
      /// </summary>
      /// <value><c>true</c> if this instance is disabled; otherwise, <c>false</c>.</value>
      protected override bool IsDisabled => false;

      /// <summary>
      /// Gets or sets the color of the button back.
      /// </summary>
      /// <value>The color of the button back.</value>
      public Color ButtonBackColor
      {
         get => (Color)GetValue( ButtonBackColorProperty );
         set => SetValue( ButtonBackColorProperty, value );
      }

      /// <summary>
      /// Gets or sets the color of the button border.
      /// </summary>
      /// <value>The color of the button border.</value>
      public Color ButtonBorderColor
      {
         get => (Color)GetValue( ButtonBorderColorProperty );
         set => SetValue( ButtonBorderColorProperty, value );
      }

      /// <summary>
      /// Gets or sets the width of the button border.
      /// </summary>
      /// <value>The width of the button border.</value>
      public float ButtonBorderWidth
      {
         get => (float)GetValue( ButtonBorderWidthProperty );
         set => SetValue( ButtonBorderWidthProperty, value );
      }

      /// <summary>
      /// Gets or sets the image file path.
      /// </summary>
      /// <value>The image file path.</value>
      public string ImageFilePath
      {
         get => (string)GetValue( ImageFilePathProperty );
         set => SetValue( ImageFilePathProperty, value );
      }

      /// <summary>
      /// Gets a value indicating whether this instance is selected.
      /// </summary>
      /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
      public override bool IsSelected => false;

      /// <summary>
      /// Gets or sets the label font attributes.
      /// </summary>
      /// <value>The label font attributes.</value>
      public FontAttributes LabelFontAttributes
      {
         get => (FontAttributes)GetValue( LabelFontAttributesProperty );
         set => SetValue( LabelFontAttributesProperty, value );
      }

      /// <summary>
      /// Gets or sets the size of the label font.
      /// </summary>
      /// <value>The size of the label font.</value>
      public double LabelFontSize
      {
         get => (double)GetValue( LabelFontSizeProperty );
         set => SetValue( LabelFontSizeProperty, value );
      }

      /// <summary>
      /// Gets or sets the label text.
      /// </summary>
      /// <value>The label text.</value>
      public string LabelText
      {
         get => (string)GetValue( LabelTextProperty );
         set => SetValue( LabelTextProperty, value );
      }

      /// <summary>
      /// Gets or sets the color of the label text.
      /// </summary>
      /// <value>The color of the label text.</value>
      public Color LabelTextColor
      {
         get => (Color)GetValue( LabelTextColorProperty );
         set => SetValue( LabelTextColorProperty, value );
      }

      /// <summary>
      /// Images  supported through bindings.
      /// </summary>
      public static ISimpleImageLabelButton_Forms CreateCompleteSimpleImageLabelButton
      (
         bool                            animate                   = true,
         Color?                          backColor                 = default,
         [CanBeNull] object              bindingContext            = default,
         [CanBeNull] IValueConverter     bindingConverter          = null,
         [CanBeNull] object              bindingConverterParameter = null,
         BindingMode                     bindingMode               = BindingMode.OneWay,
         [CanBeNull] string              bindingPropertyName       = "",
         object                          bindingSource             = default,
         Color?                          borderColor               = default,
         double                          borderWidth               = 0.0,
         bool                            cannotTap                 = true,
         FontAttributes                  fontAttributes            = FontAttributes.None,
         [CanBeNull] string              fontFamily                = "",
         double?                         fontSize                  = default,
         bool                            getImageFromResource      = true,
         double?                         heightRequest             = default,
         LayoutOptions?                  horizontalOptions         = default,
         TextAlignment                   horizontalTextAlignment   = TextAlignment.Center,
         [CanBeNull] string              imageFilePath             = "",
         [CanBeNull] Type                imageResourceClassType    = default,
         bool                            includeHapticFeedback     = true,
         LineBreakMode                   lineBreakMode             = LineBreakMode.TailTruncation,
         ImageLabelButtonSelectionStyles selectionStyle            = ImageLabelButtonSelectionStyles.NoSelection,
         [CanBeNull] string              stringFormat              = "",
         [CanBeNull] string              text                      = "",
         Color?                          textColor                 = default,
         bool                            toggleSelection           = false,
         double                          uniformMargin             = 0.0,
         double                          uniformPadding            = 0.0,
         LayoutOptions?                  verticalOptions           = default,
         TextAlignment                   verticalTextAlignment     = TextAlignment.Center,
         double?                         widthRequest              = default
      )
      {
         var button =
            CreateSimpleImageLabelButton
            (
               text,
               textColor,
               fontSize,
               widthRequest,
               heightRequest,
               bindingContext,
               backColor,
               borderColor,
               horizontalOptions,
               verticalOptions,
               fontFamily,
               getImageFromResource,
               imageFilePath,
               imageResourceClassType,
               borderWidth,
               fontAttributes,
               "",
               animate,
               includeHapticFeedback
            );

         // Fix misc. stuff
         button.CannotTap             = cannotTap;
         button.ButtonToggleSelection = toggleSelection;
         button.SelectionStyle        = selectionStyle;

         if ( uniformMargin.IsANumberGreaterThanZero() )
         {
            ( (SimpleImageLabelButton_Forms)button ).Margin = uniformMargin;
         }

         if ( uniformPadding.IsANumberGreaterThanZero() )
         {
            ( (SimpleImageLabelButton_Forms)button ).Padding = uniformPadding;
         }

         button.ButtonLabel.HorizontalTextAlignment = horizontalTextAlignment;
         button.ButtonLabel.HorizontalOptions       = horizontalOptions ?? button.ButtonLabel.HorizontalTextAlignment.ToLayoutOptions();
         button.ButtonLabel.VerticalTextAlignment   = verticalTextAlignment;
         button.ButtonLabel.VerticalOptions         = verticalOptions ?? button.ButtonLabel.VerticalTextAlignment.ToLayoutOptions();
         button.ButtonLabel.LineBreakMode           = lineBreakMode;

         button.ButtonLabel.ConsiderBindings
         (
            bindingPropertyName,
            bindingSource,
            stringFormat,
            bindingMode,
            bindingConverter,
            bindingConverterParameter );

         return button;
      }

      /// <summary>
      /// Creates the simple image label button.
      /// </summary>
      /// <param name="labelText">The label text.</param>
      /// <param name="labelTextColor">Color of the label text.</param>
      /// <param name="labelFontSize">Size of the label font.</param>
      /// <param name="widthRequest">The width request.</param>
      /// <param name="heightRequest">The height request.</param>
      /// <param name="bindingContext">The binding context.</param>
      /// <param name="buttonBackColor">Color of the button back.</param>
      /// <param name="buttonBorderColor">Color of the button border.</param>
      /// <param name="horizontalOptions">The horizontal options.</param>
      /// <param name="verticalOptions">The vertical options.</param>
      /// <param name="fontFamily"></param>
      /// <param name="getImageFromResource"></param>
      /// <param name="imageFilePath">The image file path.</param>
      /// <param name="imageResourceClassType"></param>
      /// <param name="buttonBorderWidth">Width of the button border.</param>
      /// <param name="labelFontAttributes">The label font attributes.</param>
      /// <param name="buttonCommandBindingName">Name of the button command binding.</param>
      /// <param name="animateButton"></param>
      /// <param name="includeHapticFeedback"></param>
      /// <returns>ISimpleImageLabelButton_Forms.</returns>
      public static ISimpleImageLabelButton_Forms CreateSimpleImageLabelButton
      (
         [CanBeNull] string labelText                = "",
         Color?             labelTextColor           = default,
         double?            labelFontSize            = default,
         double?            widthRequest             = default,
         double?            heightRequest            = default,
         [CanBeNull] object bindingContext           = default,
         Color?             buttonBackColor          = default,
         Color?             buttonBorderColor        = default,
         LayoutOptions?     horizontalOptions        = default,
         LayoutOptions?     verticalOptions          = default,
         [CanBeNull] string fontFamily               = "",
         bool               getImageFromResource     = true,
         [CanBeNull] string imageFilePath            = "",
         [CanBeNull] Type   imageResourceClassType   = default,
         double             buttonBorderWidth        = 0.0,
         FontAttributes     labelFontAttributes      = FontAttributes.None,
         [CanBeNull] string buttonCommandBindingName = "",
         bool               animateButton            = true,
         bool               includeHapticFeedback    = true
      )
      {
         var newSimpleImageLabelButton =
            new SimpleImageLabelButton_Forms( widthRequest, heightRequest, fontFamily )
            {
               LabelText      = labelText,
               LabelTextColor = labelTextColor ?? Color.Transparent,
               LabelFontSize = labelFontSize ??
                               Device.GetNamedSize( NamedSize.Small, typeof( Label ) ).AdjustForOsAndDevice(),
               ButtonBackColor          = buttonBackColor   ?? Color.Transparent,
               ButtonBorderColor        = buttonBorderColor ?? Color.Transparent,
               GetImageFromResource     = getImageFromResource,
               ImageResourceClassType   = imageResourceClassType,
               ImageFilePath            = imageFilePath,
               ButtonBorderWidth        = (float)buttonBorderWidth,
               LabelFontAttributes      = labelFontAttributes,
               ButtonCommandBindingName = buttonCommandBindingName,
            };

         if ( bindingContext.IsNotNullOrDefault() )
         {
            newSimpleImageLabelButton.SetBindingContextSafelyAndAwaitAllBranchingTasks( bindingContext ).FireAndFuhgetAboutIt();
         }

         if ( newSimpleImageLabelButton.ButtonLabel != null )
         {
            newSimpleImageLabelButton.ButtonLabel.BindingContext = bindingContext;
         }

         if ( newSimpleImageLabelButton.ButtonImage != null )
         {
            newSimpleImageLabelButton.ButtonImage.BindingContext = bindingContext;
         }

         newSimpleImageLabelButton.AnimateButton         = animateButton;
         newSimpleImageLabelButton.IncludeHapticFeedback = includeHapticFeedback;

         if ( widthRequest.GetValueOrDefault().IsANumberGreaterThanZero() )
         {
            newSimpleImageLabelButton.WidthRequest = widthRequest.GetValueOrDefault();
         }

         if ( heightRequest.GetValueOrDefault().IsANumberGreaterThanZero() )
         {
            newSimpleImageLabelButton.HeightRequest = heightRequest.GetValueOrDefault();
         }

         if ( horizontalOptions.HasValue )
         {
            newSimpleImageLabelButton.HorizontalOptions = horizontalOptions.GetValueOrDefault();
         }

         if ( verticalOptions.HasValue )
         {
            newSimpleImageLabelButton.VerticalOptions = verticalOptions.GetValueOrDefault();
         }

         return newSimpleImageLabelButton;
      }

      /// <summary>
      /// Creates the simple image label button bindable property.
      /// </summary>
      /// <typeparam name="PropertyTypeT">The type of the property type t.</typeparam>
      /// <param name="localPropName">Name of the local property.</param>
      /// <param name="defaultVal">The default value.</param>
      /// <param name="bindingMode">The binding mode.</param>
      /// <param name="callbackAction">The callback action.</param>
      /// <param name="coerceValueDelegate"></param>
      /// <returns>BindableProperty.</returns>
      public static BindableProperty CreateSimpleImageLabelButtonBindableProperty<PropertyTypeT>
      (
         string                                                             localPropName,
         PropertyTypeT                                                      defaultVal          = default,
         BindingMode                                                        bindingMode         = BindingMode.OneWay,
         Action<SimpleImageLabelButton_Forms, PropertyTypeT, PropertyTypeT> callbackAction      = null,
         Func<SimpleImageLabelButton_Forms, PropertyTypeT, PropertyTypeT>   coerceValueDelegate = default
      )
      {
         return BindableUtils_Forms.CreateBindableProperty( localPropName, defaultVal, bindingMode, callbackAction,
            coerceValueDelegate );
      }

      /// <summary>
      /// Deselects this instance.
      /// </summary>
      protected override void Deselect()
      {
         // Do nothing
      }

      /// <summary>
      /// Resets the button style.
      /// </summary>
      private void ResetButtonStyle()
      {
         ButtonDeselectedStyle = CreateButtonStyle( ButtonBackColor, ButtonBorderWidth, ButtonBorderColor );
      }

      /// <summary>
      /// Resets the label style.
      /// </summary>
      private void ResetLabelStyle()
      {
         // These might not be set yet
         if ( ButtonLabel.IsNotNullOrDefault() )
         {
            ButtonLabel.Text           = LabelText;
            ButtonLabel.FontSize       = LabelFontSize;
            ButtonLabel.FontAttributes = LabelFontAttributes;
            ButtonLabel.TextColor      = LabelTextColor;
         }

         LabelDeselectedStyle = CreateLabelStyle( LabelTextColor, LabelFontSize, LabelFontAttributes );
      }
   }
}