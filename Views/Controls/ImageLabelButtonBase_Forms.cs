// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=ImageLabelButtonBase_Forms.cs
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
// #define FORCE_CURRENT_STYLE_COPY

namespace Com.MarcusTS.UI.XamForms.Views.Controls
{
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Diagnostics;
   using System.Linq;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Subviews;
   using Xamarin.Essentials;
   using Xamarin.Forms;

   /// <summary>
   /// Interface IImageLabelButton_Forms Implements the <see cref="System.IDisposable" /> Implements the
   /// <see
   ///    cref="System.ComponentModel.INotifyPropertyChanged" />
   /// </summary>
   /// <seealso cref="System.IDisposable" />
   /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
   public interface IImageLabelButton_Forms :
      IShapeView_Forms,
      IHaveButtonState_PI,
      IDisposable,
      INotifyPropertyChanged
   {
      /// <summary>Gets or sets a value indicating whether [animate button].</summary>
      /// <value><c>true</c> if [animate button]; otherwise, <c>false</c>.</value>
      bool AnimateButton { get; set; }

      /// <summary>Gets or sets the button command.</summary>
      /// <value>The button command.</value>
      Command ButtonCommand { get; set; }

      /// <summary>Gets or sets the name of the button command binding.</summary>
      /// <value>The name of the button command binding.</value>
      string ButtonCommandBindingName { get; set; }

      /// <summary>Gets or sets the button command converter.</summary>
      /// <value>The button command converter.</value>
      IValueConverter ButtonCommandConverter { get; set; }

      /// <summary>Gets or sets the button command converter parameter.</summary>
      /// <value>The button command converter parameter.</value>
      object ButtonCommandConverterParameter { get; set; }

      /// <summary>Gets or sets the button command source.</summary>
      /// <value>The button command source.</value>
      object ButtonCommandSource { get; set; }

      /// <summary>Gets or sets the button command string format.</summary>
      /// <value>The button command string format.</value>
      string ButtonCommandStringFormat { get; set; }

      /// <summary>Gets or sets the button image.</summary>
      /// <value>The button image.</value>
      Image ButtonImage { get; set; }

      /// <summary>Gets or sets the button label.</summary>
      /// <value>The button label.</value>
      Label ButtonLabel { get; set; }

      bool CannotTap { get; set; }

      bool CanTapOnDisabled { get; set; }

      /// <summary>Gets or sets the current style.</summary>
      /// <value>The current style.</value>
      IImageLabelButtonStyle_Forms CurrentStyle { get; set; }

      double ImageHeight { get; set; }

      LayoutOptions ImageHorizontalAlign { get; set; }

      IResponsiveTasks ImageLabelButtonPressedTask { get; set; }

      /// <summary>Gets the image label button styles.</summary>
      /// <value>The image label button styles.</value>
      IList<IImageLabelButtonStyle_Forms> ImageLabelButtonStyles { get; }

      Thickness     ImageMargin        { get; set; }
      LayoutOptions ImageVerticalAlign { get; set; }

      double ImageWidth { get; set; }

      /// <summary>Gets or sets a value indicating whether [include haptic feedback].</summary>
      /// <value><c>true</c> if [include haptic feedback]; otherwise, <c>false</c>.</value>
      bool IncludeHapticFeedback { get; set; }

      IResponsiveTasks IsEnabledChangedTask { get; set; }

      /// <summary>Gets a value indicating whether this instance is selected.</summary>
      /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
      bool IsSelected { get; }

      /// <summary>Gets or sets the selection style.</summary>
      /// <value>The selection style.</value>
      ImageLabelButtonSelectionStyles SelectionStyle { get; set; }

      /// <summary>Gets a value indicating whether [update button text from style].</summary>
      /// <value><c>true</c> if [update button text from style]; otherwise, <c>false</c>.</value>
      bool UpdateButtonTextFromStyle { get; }
   }

   /// <summary>
   /// A button that can contain either an image and/or a label. Implements the <see cref="ShapeView_Forms" />
   /// Implements the
   /// <see cref="IImageLabelButton_Forms" />
   /// </summary>
   /// <seealso cref="ShapeView_Forms" />
   /// <seealso cref="IImageLabelButton_Forms" />
   public abstract class ImageLabelButtonBase_Forms : ShapeView_Forms, IImageLabelButton_Forms
   {
      /// <summary>The animate button property</summary>
      public static readonly BindableProperty AnimateButtonProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( AnimateButton ),
            true
         );

      /// <summary>The button command binding name property</summary>
      public static readonly BindableProperty ButtonCommandBindingNameProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ButtonCommandBindingName ),
            default( string )
           ,
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.SetUpCompleteButtonCommandBinding();
            }
         );

      /// <summary>The button command converter parameter property</summary>
      public static readonly BindableProperty ButtonCommandConverterParameterProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ButtonCommandConverterParameter ),
            default( object )
           ,
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.SetUpCompleteButtonCommandBinding();
            }
         );

      /// <summary>The button command converter property</summary>
      public static readonly BindableProperty ButtonCommandConverterProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ButtonCommandConverter ),
            default( IValueConverter )
           ,
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.SetUpCompleteButtonCommandBinding();
            }
         );

      /// <summary>The button command property</summary>
      public static readonly BindableProperty ButtonCommandProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ButtonCommand ),
            default( Command )
           ,
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.RemoveButtonCommandEventListener();

               if ( imageLabelButton.ButtonCommand != null )
               {
                  // NOTE Cannot avoid improper TPL root
                  imageLabelButton.ButtonCommand.CanExecuteChanged +=
                     imageLabelButton.HandleButtonCommandCanExecuteChanged;

                  // Force-fire the initial state
                  imageLabelButton.ButtonCommand.ChangeCanExecute();

                  imageLabelButton.OnButtonCommandCreated();
               }
            }
         );

      /// <summary>The button command converter property</summary>
      public static readonly BindableProperty ButtonCommandSourceProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ButtonCommandSource ),
            default( IValueConverter )
           ,
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.SetUpCompleteButtonCommandBinding();
            }
         );

      /// <summary>The button command string format property</summary>
      public static readonly BindableProperty ButtonCommandStringFormatProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ButtonCommandStringFormat ),
            default( string )
           ,
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.SetUpCompleteButtonCommandBinding();
            }
         );

      /// <summary>The button image property</summary>
      public static readonly BindableProperty ButtonImageProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ButtonImage ),
            default( Image ),
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.ResetImageAndLabel( imageLabelButton.ImageHorizontalAlign, imageLabelButton.ImageVerticalAlign );

#if SHOW_BACK_COLORS
               newVal.BackgroundColor = Color.Cyan;
#endif
            }
         );

      /// <summary>The button label property</summary>
      public static readonly BindableProperty ButtonLabelProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ButtonLabel ),
            default( Label ),
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               if ( oldVal.IsNotNullOrDefault() )
               {
                  oldVal.PropertyChanged -= imageLabelButton.HandleLabelPropertyChanged;
               }

               imageLabelButton.ResetImageAndLabel( newVal.HorizontalOptions, newVal.VerticalOptions );

               if ( newVal.IsNotNullOrDefault() )
               {
                  newVal.PropertyChanged += imageLabelButton.HandleLabelPropertyChanged;
               }

#if SHOW_BACK_COLORS
               newVal.BackgroundColor = Color.GreenYellow;
#endif
            }
         );

      public static readonly BindableProperty ButtonStateProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ButtonState ),
            default( string ),
            BindingMode.TwoWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               if ( newVal == oldVal )
               {
                  // Nothing to do
                  return;
               }

               // Already done before entering this method:
               // _buttonState = value;

               // IMPORTANT Restarting the TPL root
               MainThread.BeginInvokeOnMainThread(

                  // ReSharper disable once AsyncVoidLambda
                  () =>
                  {
                     imageLabelButton.AfterButtonStateChanged().FireAndFuhgetAboutIt();
                     imageLabelButton.BroadcastIfSelected().FireAndFuhgetAboutIt();
                     imageLabelButton.UpdateCurrentStyleFromButtonState( newVal );
                     imageLabelButton.ButtonStateChangedTask.RunAllTasksUsingDefaults( newVal ).FireAndFuhgetAboutIt();
                  }
               );
            }
         );

      public static readonly BindableProperty CannotTapProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( CannotTap ),
            default( bool )
         );

      public static readonly BindableProperty CanTapOnDisabledProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( CanTapOnDisabled ),
            default( bool )
         );

      /// <summary>The current style property</summary>
      public static readonly BindableProperty CurrentStyleProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( CurrentStyle ),
            default( IImageLabelButtonStyle_Forms ),
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               if ( _currentStylePropChangedEntered )
               {
                  // CRITICAL
                  imageLabelButton.SetAllStyles();
                  return;
               }

               _currentStylePropChangedEntered = true;

               try
               {
                  newVal = imageLabelButton.LastCheckBeforeAssigningStyle( newVal );

                  if ( newVal.IsNotNullOrDefault() && oldVal.IsNotNullOrDefault() &&
                       oldVal.IsAnEqualReferenceTo( newVal ) )
                  {
                     // No change
                     return;
                  }

                  if ( imageLabelButton.CurrentStyle.IsNotAnEqualReferenceTo( newVal ) )
                  {
                     imageLabelButton.CurrentStyle = newVal;
                  }

                  imageLabelButton.ButtonState = imageLabelButton.CurrentStyle.InternalButtonState;
                  imageLabelButton.UpdateButtonText();
                  imageLabelButton.SetAllStyles();
               }
               finally
               {
                  _currentStylePropChangedEntered = false;
               }
            },
            ( viewButton, newVal ) =>
            {
               if ( _currentStyleCoerceEntered )
               {
                  return newVal;
               }

               _currentStyleCoerceEntered = true;

               try
               {
                  if ( newVal.IsNullOrDefault() && viewButton.CurrentStyle.IsNullOrDefault() &&
                       viewButton.ImageLabelButtonStyles.IsNotEmpty() )
                  {
                     viewButton.CurrentStyle = viewButton.ImageLabelButtonStyles[ 0 ];
                  }

                  // ELSE
                  return newVal;
               }
               finally
               {
                  _currentStyleCoerceEntered = false;
               }
            }
         );

      /// <summary>The image height property</summary>
      public static readonly BindableProperty ImageHeightProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ImageHeight ),
            default( double )
         );

      public static readonly BindableProperty ImageHorizontalAlignProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ImageHorizontalAlign ),
            LayoutOptions.Center,
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.ResetImageAndLabel( imageLabelButton.ImageHorizontalAlign, imageLabelButton.ImageVerticalAlign );
            }
         );

      public static readonly BindableProperty ImageMarginProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ImageMargin ),
            default( Thickness ),
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.CallRecreateImageSafely();
            }
         );

      public static readonly BindableProperty ImageVerticalAlignProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ImageVerticalAlign ),
            LayoutOptions.Center,
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.ResetImageAndLabel( imageLabelButton.ImageHorizontalAlign, imageLabelButton.ImageVerticalAlign );
            }
         );

      public static readonly BindableProperty ImageWidthProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( ImageWidth ),
            default( double )
         );

      /// <summary>The include haptic feedback property</summary>
      public static readonly BindableProperty IncludeHapticFeedbackProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( IncludeHapticFeedback ),
            true
         );

      /// <summary>The selection group property</summary>
      public static readonly BindableProperty SelectionGroupProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( SelectionGroup ),
            default( int )
           ,
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.BroadcastIfSelected().FireAndFuhgetAboutIt();
            }
         );

      /// <summary>The selection style property</summary>
      public static readonly BindableProperty SelectionStyleProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( SelectionStyle ),
            default( ImageLabelButtonSelectionStyles ),
            BindingMode.OneWay,
            (
               imageLabelButton,
               oldVal,
               newVal
            ) =>
            {
               imageLabelButton.SetAllStyles();
            }
         );

      public static readonly BindableProperty UpdateButtonTextFromStyleProperty =
         CreateImageLabelButtonBaseBindableProperty
         (
            nameof( UpdateButtonTextFromStyle ),
            default( bool )
         );

      /// <summary>Occurs when [i am selected static].</summary>
      protected static readonly IResponsiveTasks IAmSelectedStaticTask = new ResponsiveTasks( 1 );

      private static bool _currentStyleCoerceEntered;

      private static bool _currentStylePropChangedEntered;

      /// <summary>The layout</summary>
      private readonly Grid _masterGrid = UIUtils_Forms.GetExpandingGrid();

      /// <summary>The tap gesture</summary>
      private readonly TapGestureRecognizer _tapGesture = new TapGestureRecognizer();

      //private bool _isInstantiating;

      //private double _lastButtonCornerRadiusFactor;

      /// <summary>The tapped listener entered</summary>
      private volatile bool _tappedListenerEntered;

      //private double _lastButtonCornerRadiusFixed;

      /// <summary>Initializes a new instance of the <see cref="ImageLabelButtonBase_Forms" /> class.</summary>
      protected ImageLabelButtonBase_Forms()
      {
         CallStartup();
      }

      /// <summary>Gets a value indicating whether this instance is disabled.</summary>
      /// <value><c>true</c> if this instance is disabled; otherwise, <c>false</c>.</value>
      protected abstract bool IsDisabled { get; }

      /// <summary>Gets or sets a value indicating whether [animate button].</summary>
      /// <value><c>true</c> if [animate button]; otherwise, <c>false</c>.</value>
      public bool AnimateButton
      {
         get => (bool)GetValue( AnimateButtonProperty );
         set => SetValue( AnimateButtonProperty, value );
      }

      /// <summary>Gets or sets the button command.</summary>
      /// <value>The button command.</value>
      public Command ButtonCommand
      {
         get => (Command)GetValue( ButtonCommandProperty );
         set => SetValue( ButtonCommandProperty, value );
      }

      /// <summary>Gets or sets the name of the button command binding.</summary>
      /// <value>The name of the button command binding.</value>
      public string ButtonCommandBindingName
      {
         get => (string)GetValue( ButtonCommandBindingNameProperty );
         set => SetValue( ButtonCommandBindingNameProperty, value );
      }

      /// <summary>Gets or sets the button command converter.</summary>
      /// <value>The button command converter.</value>
      public IValueConverter ButtonCommandConverter
      {
         get => (IValueConverter)GetValue( ButtonCommandConverterProperty );
         set => SetValue( ButtonCommandConverterProperty, value );
      }

      /// <summary>Gets or sets the button command converter parameter.</summary>
      /// <value>The button command converter parameter.</value>
      public object ButtonCommandConverterParameter
      {
         get => (IValueConverter)GetValue( ButtonCommandConverterParameterProperty );
         set => SetValue( ButtonCommandConverterParameterProperty, value );
      }

      /// <summary>Gets or sets the button command source.</summary>
      /// <value>The button command source.</value>
      public object ButtonCommandSource
      {
         get => (IValueConverter)GetValue( ButtonCommandSourceProperty );
         set => SetValue( ButtonCommandSourceProperty, value );
      }

      /// <summary>Gets or sets the button command string format.</summary>
      /// <value>The button command string format.</value>
      public string ButtonCommandStringFormat
      {
         get => (string)GetValue( ButtonCommandStringFormatProperty );
         set => SetValue( ButtonCommandStringFormatProperty, value );
      }

      /// <summary>Gets or sets the button image.</summary>
      /// <value>The button image.</value>
      public Image ButtonImage
      {
         get => (Image)GetValue( ButtonImageProperty );
         set => SetValue( ButtonImageProperty, value );
      }

      ///// <summary>Gets or sets the button corner radius fixed.</summary>
      ///// <value>The button corner radius fixed.</value>
      //public double? ButtonCornerRadiusFixed
      //{
      //   get => (double?)GetValue(ButtonCornerRadiusFixedProperty);
      //   set => SetValue(ButtonCornerRadiusFixedProperty, value);
      //}
      /// <summary>Gets or sets the button label.</summary>
      /// <value>The button label.</value>
      public Label ButtonLabel
      {
         get => (Label)GetValue( ButtonLabelProperty );
         set => SetValue( ButtonLabelProperty, value );
      }

      ///// <summary>Gets or sets the button corner radius factor.</summary>
      ///// <value>The button corner radius factor.</value>
      //public double? ButtonCornerRadiusFactor
      //{
      //   get => (double?)GetValue(ButtonCornerRadiusFactorProperty);
      //   set => SetValue(ButtonCornerRadiusFactorProperty, value);
      //}
      /// <summary>Gets or sets the state of the button.</summary>
      /// <value>The state of the button.</value>
      public string ButtonState
      {
         get => (string)GetValue( ButtonStateProperty );
         set => SetValue( ButtonStateProperty, value );
      }

      public IResponsiveTasks ButtonStateChangedTask { get; set; } = new ResponsiveTasks( 1 );

      public bool CannotTap
      {
         get => (bool)GetValue( CannotTapProperty );
         set => SetValue( CannotTapProperty, value );
      }

      public bool CanTapOnDisabled
      {
         get => (bool)GetValue( CanTapOnDisabledProperty );
         set => SetValue( CanTapOnDisabledProperty, value );
      }

      /// <summary>Gets or sets the current style.</summary>
      /// <value>The current style.</value>
      public IImageLabelButtonStyle_Forms CurrentStyle
      {
         get => (IImageLabelButtonStyle_Forms)GetValue( CurrentStyleProperty );
         set => SetValue( CurrentStyleProperty, value );
      }

      public double ImageHeight
      {
         get => (double)GetValue( ImageHeightProperty );
         set => SetValue( ImageHeightProperty, value );
      }

      public LayoutOptions ImageHorizontalAlign
      {
         get => (LayoutOptions)GetValue( ImageHorizontalAlignProperty );
         set => SetValue( ImageHorizontalAlignProperty, value );
      }

      public IResponsiveTasks ImageLabelButtonPressedTask { get; set; } = new ResponsiveTasks();

      /// <summary>Gets the image label button styles.</summary>
      /// <value>The image label button styles.</value>
      public abstract IList<IImageLabelButtonStyle_Forms> ImageLabelButtonStyles { get; }

      public Thickness ImageMargin
      {
         get => (Thickness)GetValue( ImageMarginProperty );
         set => SetValue( ImageMarginProperty, value );
      }

      public LayoutOptions ImageVerticalAlign
      {
         get => (LayoutOptions)GetValue( ImageVerticalAlignProperty );
         set => SetValue( ImageVerticalAlignProperty, value );
      }

      public double ImageWidth
      {
         get => (double)GetValue( ImageWidthProperty );
         set => SetValue( ImageWidthProperty, value );
      }

      /// <summary>Gets or sets a value indicating whether [include haptic feedback].</summary>
      /// <value><c>true</c> if [include haptic feedback]; otherwise, <c>false</c>.</value>
      public bool IncludeHapticFeedback
      {
         get => (bool)GetValue( IncludeHapticFeedbackProperty );
         set => SetValue( IncludeHapticFeedbackProperty, value );
      }

      public IResponsiveTasks IsEnabledChangedTask { get; set; } = new ResponsiveTasks( 1 );

      /// <summary>Gets a value indicating whether this instance is selected.</summary>
      /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
      public abstract bool IsSelected { get; }

      /// <summary>Gets or sets the selection group.</summary>
      /// <value>The selection group.</value>
      public int SelectionGroup
      {
         get => (int)GetValue( SelectionGroupProperty );
         set => SetValue( SelectionGroupProperty, value );
      }

      /// <summary>Gets or sets the selection style.</summary>
      /// <value>The selection style.</value>
      public ImageLabelButtonSelectionStyles SelectionStyle
      {
         get => (ImageLabelButtonSelectionStyles)GetValue( SelectionStyleProperty );
         set => SetValue( SelectionStyleProperty, value );
      }

      /// <summary>Gets a value indicating whether [update button text from style].</summary>
      /// <value><c>true</c> if [update button text from style]; otherwise, <c>false</c>.</value>
      public bool UpdateButtonTextFromStyle
      {
         get => (bool)GetValue( UpdateButtonTextFromStyleProperty );
         set => SetValue( UpdateButtonTextFromStyleProperty, value );
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
         ReleaseUnmanagedResources();
         GC.SuppressFinalize( this );
      }

      /// <summary>Creates the button style.</summary>
      /// <param name="backColor">Color of the back.</param>
      /// <param name="borderThickness">Width of the border.</param>
      /// <param name="borderColor">Color of the border.</param>
      /// <param name="cornerRadius"></param>
      /// <returns>Style.</returns>
      public static Style CreateButtonStyle
      (
         Color?  backColor       = null,
         float?  borderThickness = null,
         Color?  borderColor     = default,
         double? cornerRadius    = null
      )
      {
         var newStyle = new Style( typeof( ImageLabelButtonBase_Forms ) );
         newStyle.SetRoundedCornerContentViewStyle( backColor, cornerRadius, borderColor, borderThickness );
         return newStyle;
      }

      /// <summary>Creates the toggle image label button bindable property.</summary>
      /// <typeparam name="PropertyTypeT">The type of the property type t.</typeparam>
      /// <param name="localPropName">Name of the local property.</param>
      /// <param name="defaultVal">The default value.</param>
      /// <param name="bindingMode">The binding mode.</param>
      /// <param name="callbackAction">The callback action.</param>
      /// <param name="coerceValueDelegate"></param>
      /// <returns>BindableProperty.</returns>
      public static BindableProperty CreateImageLabelButtonBaseBindableProperty<PropertyTypeT>
      (
         string                                                           localPropName,
         PropertyTypeT                                                    defaultVal          = default,
         BindingMode                                                      bindingMode         = BindingMode.OneWay,
         Action<ImageLabelButtonBase_Forms, PropertyTypeT, PropertyTypeT> callbackAction      = default,
         Func<ImageLabelButtonBase_Forms, PropertyTypeT, PropertyTypeT>   coerceValueDelegate = default
      )
      {
         return BindableUtils_Forms.CreateBindableProperty( localPropName, defaultVal, bindingMode, callbackAction,
            coerceValueDelegate );
      }

      /// <summary>Creates the label style.</summary>
      /// <param name="textColor">Color of the text.</param>
      /// <param name="fontSize">Size of the font.</param>
      /// <param name="fontAttributes">The font attributes.</param>
      /// <returns>Style.</returns>
      public static Style CreateLabelStyle
      (
         Color?          textColor      = null,
         double?         fontSize       = null,
         FontAttributes? fontAttributes = null
      )
      {
         var newStyle = new Style( typeof( Label ) );

         if ( textColor != null )
         {
            newStyle.Setters.Add( new Setter { Property = Label.TextColorProperty, Value = textColor, } );
         }

         // The label is always transparent
         newStyle.Setters.Add( new Setter { Property = BackgroundColorProperty, Value = Color.Transparent, } );

         if ( fontSize.IsANumberGreaterThanZero() )
         {
            newStyle.Setters.Add( new Setter { Property = Label.FontSizeProperty, Value = fontSize, } );
         }

         if ( fontAttributes != null )
         {
            newStyle.Setters.Add( new Setter { Property = Label.FontAttributesProperty, Value = fontAttributes, } );
         }

         return newStyle;
      }

      /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
      /// <param name="disposing">
      /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
      /// </param>
      public void Dispose( bool disposing )
      {
         ReleaseUnmanagedResources();
         if ( disposing )
         { }
      }

      protected virtual Task AfterButtonStateChanged()
      {
         return Task.CompletedTask;
      }

      protected bool ButtonStateIndexFound( string buttonStateToFind, out int styleIdx )
      {
         styleIdx = -1;

         if ( ImageLabelButtonStyles.IsEmpty() || buttonStateToFind.IsEmpty() )
         {
            return false;
         }

         var foundStyle =
            ImageLabelButtonStyles.FirstOrDefault( style => style.InternalButtonState.IsSameAs( buttonStateToFind ) );
         styleIdx = ImageLabelButtonStyles.IndexOf( foundStyle );

         // Should never occur due to constraints set up at this class's constructor
         if ( ( styleIdx < 0 ) || ( ImageLabelButtonStyles.Count < styleIdx ) )
         {
            return false;
         }

         return true;
      }

      /// <summary>Calls the recreate image safely.</summary>
      protected void CallRecreateImageSafely()
      {
         ThreadHelper.ConsiderBeginInvokeActionOnMainThread(
            () =>
            {
               try
               {
                  RecreateImage();
               }
               catch ( Exception )
               {
                  Debug.WriteLine( nameof( ImageLabelButtonBase_Forms ) + ": " + nameof( CallRecreateImageSafely ) +
                                   ": COULD NOT CREATE AN IMAGE" );
               }
            } );
      }

      /// <summary>Deselects this instance.</summary>
      protected abstract void Deselect();

      protected void HandleTapGestureTapped
      (
         object    sender,
         EventArgs e
      )
      {
         if ( _tappedListenerEntered              ||
              CannotTap                           ||
              ( IsDisabled && !CanTapOnDisabled ) ||
              ImageLabelButtonStyles.IsEmpty()    ||
              (
                 IsAToggleButton
                 &&
                 (
                     CurrentStyle.IsNullOrDefault() ||
                     CurrentStyle.InternalButtonState.IsEmpty() 
                 )
              )
            )
         {
            return;
         }

         _tappedListenerEntered = true;

         if ( IsAToggleButton )
         {
            ToggleCurrentStyle();
         }

         MainThread.BeginInvokeOnMainThread(
            // IMPORTANT Async required for delay
            // ReSharper disable once AsyncVoidLambda
            async
            () =>
            {
               // If a command exists, fire it and reset our selected status to false; otherwise, leave the selected state as
               // it is.
               await this.AddAnimationAndHapticFeedback( AnimateButton, IncludeHapticFeedback ).WithoutChangingContext();

               await Task.Delay( UIConst_PI.AFTER_BUTTON_PRESS_DELAY_MILLISECONDS ).WithoutChangingContext();

               if ( ButtonCommand.IsNotNullOrDefault() )
               {
                  ButtonCommand.Execute( this );
               }

               await ImageLabelButtonPressedTask.RunAllTasksUsingDefaults().WithoutChangingContext();
               _tappedListenerEntered = false;
            } );
      }

      protected virtual IImageLabelButtonStyle_Forms LastCheckBeforeAssigningStyle( IImageLabelButtonStyle_Forms value )
      {
         return value;
      }

      /// <summary>Called when [button command created].</summary>
      protected virtual void OnButtonCommandCreated()
      { }

      protected virtual void OnIsEnabledChanged()
      { }

      /// <summary>Sets all styles.</summary>
      protected void SetAllStyles()
      {
         SetButtonStyle();
         SetLabelStyle();
         CallRecreateImageSafely();
      }

      /// <summary>Sets the button style.</summary>
      protected void SetButtonStyle()
      {
         if ( !CurrentStyleIndexFound( out var styleIdx ) )
         {
            return;
         }

         // Should never occur due to constraints set up at this class's constructor
         if ( ( styleIdx < 0 ) || ( ImageLabelButtonStyles.Count < styleIdx ) )
         {
            return;
         }

         var newStyle = ImageLabelButtonStyles[ styleIdx ].ButtonStyle;

         // The button is the shape view ("this")
         Style = newStyle;

         // This library is not working well with styles, so forcing all settings manually
         this.ForceStyle( newStyle );
      }

      /// <summary>Sets the current style by button text.</summary>
      /// <param name="buttonText">The button text.</param>
      protected void SetCurrentStyleByButtonText( string buttonText )
      {
         if ( ImageLabelButtonStyles.IsEmpty() )
         {
            return;
         }

         var foundStyle = ImageLabelButtonStyles.FirstOrDefault( s => s.ButtonText.IsSameAs( buttonText ) );
         if ( foundStyle.IsNotNullOrDefault() )
         {
            CurrentStyle = foundStyle;
         }
      }

      /// <summary>Sets the label style.</summary>
      protected void SetLabelStyle()
      {
         if ( ButtonLabel.IsNullOrDefault() || !CurrentStyleIndexFound( out var styleIdx ) )
         {
            return;
         }

         var newStyle = ImageLabelButtonStyles[ styleIdx ].LabelStyle;

         ButtonLabel.Style = newStyle;

         // This library is not working well with styles, so forcing all settings manually
         ButtonLabel.ForceStyle( newStyle );

         ButtonLabel.BackgroundColor = Color.Transparent;
      }

      /// <summary>Starts up.</summary>
      protected virtual void StartUp()
      {
         //_isInstantiating = true;

         IAmSelectedStaticTask.AddIfNotAlreadyThere( this, HandleStaticSelectionChanges );

         GestureRecognizers.Add( _tapGesture );

         // NOTE Cannot avoid improper TPL root
         _tapGesture.Tapped += HandleTapGestureTapped;

         // Applies to the base control only
         InputTransparent = false;

         this.SetContentSafelyAndAwaitAllBranchingTasks( _masterGrid ).FireAndFuhgetAboutIt();

         //_isInstantiating = false;
      }

      /// <summary>Converts toggle current style.</summary>
      protected virtual void ToggleCurrentStyle()
      {
         // Corner case: cannot manually deselect if selected and if the SelectionGroup is set
         if ( IsSelected && ( SelectionGroup > 0 ) )
         {
            return;
         }

         switch ( SelectionStyle )
         {
            case ImageLabelButtonSelectionStyles.NoSelection:
            case ImageLabelButtonSelectionStyles.SelectionButNoToggleAsFirstTwoStyles:
               break;

            case ImageLabelButtonSelectionStyles.ToggleSelectionAsFirstTwoStyles:

               // Toggle between ButtonStates[0] and ButtonStates[1]
               CurrentStyle = ImageLabelButtonStyles.Count >= 2
                  ? CurrentStyle.InternalButtonState.IsSameAs(
                     ImageLabelButtonStyles[ 0 ]
                       .InternalButtonState )
                     ? ImageLabelButtonStyles[ 1 ]
                     : ImageLabelButtonStyles[ 0 ]
                  : ImageLabelButtonStyles.IsNotEmpty()
                     ? ImageLabelButtonStyles[ 0 ]
                     : default;
               break;

            case ImageLabelButtonSelectionStyles.ToggleSelectionThroughAllStyles:

               // Find the current button state; Increment it; If beyond the end of the button states, go back to 0.
               var foundStyle =
                  ImageLabelButtonStyles.FirstOrDefault(
                     style =>
                        style.InternalButtonState.IsSameAs( CurrentStyle
                          .InternalButtonState ) );
               var buttonStateIdx = ImageLabelButtonStyles.IndexOf( foundStyle );
               if ( buttonStateIdx < 0 )
               {
                  CurrentStyle = ImageLabelButtonStyles.IsNotEmpty() ? ImageLabelButtonStyles[ 0 ] : default;
               }
               else
               {
                  buttonStateIdx++;

                  // ReSharper disable once PossibleNullReferenceException
                  CurrentStyle = ImageLabelButtonStyles.Count <= buttonStateIdx
                     ? ImageLabelButtonStyles[ 0 ]
                     : ImageLabelButtonStyles[ buttonStateIdx ];
               }

               break;
         }
      }

      protected void UpdateCurrentStyleFromButtonState( string currentState )
      {
         if ( ImageLabelButtonStyles.IsEmpty() )
         {
            return;
         }

         var newStyle = ImageLabelButtonStyles.FirstOrDefault( s => s.InternalButtonState.IsSameAs( currentState ) );

         if ( newStyle.IsNotNullOrDefault() )
         {
            CurrentStyle = newStyle;

#if FORCE_CURRENT_STYLE_COPY
            CurrentStyle.ButtonStyle = newStyle.ButtonStyle;
            CurrentStyle.ButtonText = newStyle.ButtonText;
            CurrentStyle.GetImageFromResource = newStyle.GetImageFromResource;
            CurrentStyle.ImageFilePath = newStyle.ImageFilePath;
            CurrentStyle.ImageResourceClassType = newStyle.ImageResourceClassType;
            CurrentStyle.LabelStyle = newStyle.LabelStyle;
#endif
         }
      }

      /// <summary>Broadcasts if selected.</summary>
      private async Task BroadcastIfSelected()
      {
         if ( ( SelectionGroup > 0 ) && IsSelected )
         {
            // Raise a static task to notify others in this selection group that they should be *deselected*
            await IAmSelectedStaticTask.RunAllTasksUsingDefaults( this ).WithoutChangingContext();
         }
      }

      /// <summary>Calls the startup.</summary>
      private void CallStartup()
      {
         PostBindingContextTasks.AddIfNotAlreadyThere( this, HandlePostBindingContextTasks );
         StartUp();
      }

      /// <remarks>
      /// This method only tries to resolve simple issues of image ad button placement:
      /// * One is on the left
      /// * One is on the right
      /// It manages *Center* alignments (the button and label are in conflcit, or overlay one another) by simply assigning both
      /// to a single grid.
      /// </remarks>
      private void ResetImageAndLabel( LayoutOptions horizontalAlign, LayoutOptions verticalAlign )
      {
         // Start with nothing
         _masterGrid.ClearCompletely();

         // Set the grid rows and columns *only* for the position of a label in relatin to one another
         var finishedOK = false;

         // Position both in relation to one another
         if ( ButtonLabel.IsNotNullOrDefault() && ButtonImage.IsNotNullOrDefault() )
         {
            if ( ( ButtonLabel.HorizontalOptions.Alignment == LayoutAlignment.Start ) &&
                 ( ImageHorizontalAlign.Alignment          == LayoutAlignment.End ) )
            {
               _masterGrid.AddStarRow();
               _masterGrid.AddStarColumn();
               _masterGrid.AddAutoColumn();
               _masterGrid.AddAutoColumn();
               _masterGrid.AddStarColumn();

               _masterGrid.AddAndSetRowsAndColumns( ButtonLabel, 0, 1 );
               _masterGrid.AddAndSetRowsAndColumns( ButtonImage, 0, 2 );

               finishedOK = true;
            }
            else if
            ( ( ImageHorizontalAlign.Alignment          == LayoutAlignment.Start ) &&
              ( ButtonLabel.HorizontalOptions.Alignment == LayoutAlignment.End ) )
            {
               _masterGrid.AddStarRow();
               _masterGrid.AddStarColumn();
               _masterGrid.AddStarColumn();

               _masterGrid.AddAndSetRowsAndColumns( ButtonLabel, 0, 2 );
               _masterGrid.AddAndSetRowsAndColumns( ButtonImage, 0, 1 );

               finishedOK = true;
            }
            else if ( ( ButtonLabel.VerticalOptions.Alignment == LayoutAlignment.Start ) &&
                      ( ImageVerticalAlign.Alignment          == LayoutAlignment.End ) )
            {
               _masterGrid.AddStarColumn();
               _masterGrid.AddStarRow();
               _masterGrid.AddAutoRow();
               _masterGrid.AddAutoRow();
               _masterGrid.AddStarRow();

               _masterGrid.AddAndSetRowsAndColumns( ButtonLabel, 1, 0 );
               _masterGrid.AddAndSetRowsAndColumns( ButtonImage, 2, 0 );

               finishedOK = true;
            }
            else if
            ( ( ImageVerticalAlign.Alignment          == LayoutAlignment.Start ) &&
              ( ButtonLabel.VerticalOptions.Alignment == LayoutAlignment.End ) )
            {
               _masterGrid.AddStarColumn();
               _masterGrid.AddStarRow();
               _masterGrid.AddAutoRow();
               _masterGrid.AddAutoRow();
               _masterGrid.AddStarRow();

               _masterGrid.AddAndSetRowsAndColumns( ButtonLabel, 2, 0 );
               _masterGrid.AddAndSetRowsAndColumns( ButtonImage, 1, 0 );

               finishedOK = true;
            }
         }

         // Stop-gap; just put both in the same space
         if ( !finishedOK )
         {
            // Add one grid and column; add the view
            _masterGrid.AddStarColumn();
            _masterGrid.AddStarRow();

            if ( ButtonImage.IsNotNullOrDefault() )
            {
               _masterGrid.AddAndSetRowsAndColumns( ButtonImage, 0, 0 );
            }

            if ( ButtonLabel.IsNotNullOrDefault() )
            {
               _masterGrid.AddAndSetRowsAndColumns( ButtonLabel, 0, 0 );
            }
         }

         if ( ButtonLabel.IsNotNullOrDefault() )
         {
            // *** The label is always on top ***
            _masterGrid.RaiseChild( ButtonLabel );
         }
      }

      /// <summary>Currents the style index found.</summary>
      /// <param name="styleIdx">Index of the style.</param>
      /// <returns><c>true</c> if the button state was found, otherwise <c>false</c> .</returns>
      private bool CurrentStyleIndexFound( out int styleIdx )
      {
         return ButtonStateIndexFound( CurrentStyle?.InternalButtonState, out styleIdx );
      }

      private void HandleButtonCommandCanExecuteChanged
      (
         object    sender,
         EventArgs e
      )
      {
         var newCanExecute = sender is Command senderAsCommand && senderAsCommand.CanExecute( this );

         if ( IsEnabled != newCanExecute )
         {
            MainThread.BeginInvokeOnMainThread(
               () =>
               {
                  IsEnabled = newCanExecute;
                  OnIsEnabledChanged();
                  IsEnabledChangedTask.RunAllTasksUsingDefaults( IsEnabled ).FireAndFuhgetAboutIt();
               } );
         }
      }

      private void HandleLabelPropertyChanged( object sender, PropertyChangedEventArgs propertyChangedEventArgs )
      {
         if ( propertyChangedEventArgs.PropertyName.IsSameAs( nameof( HorizontalOptions ) ) ||
              propertyChangedEventArgs.PropertyName.IsSameAs( nameof( VerticalOptions ) ) )
         {
            ResetImageAndLabel( ButtonLabel.HorizontalOptions, ButtonLabel.VerticalOptions );
         }
      }

      private Task HandlePostBindingContextTasks( IResponsiveTaskParams paramDict )
      {
         if ( ButtonLabel.IsNotNullOrDefault() )
         {
            ButtonLabel.BindingContext = BindingContext;
         }

         if ( ButtonImage.IsNotNullOrDefault() )
         {
            ButtonImage.BindingContext = BindingContext;
         }

         return Task.CompletedTask;
      }

      private bool IsAToggleButton =>
         ( SelectionStyle == ImageLabelButtonSelectionStyles.ToggleSelectionAsFirstTwoStyles ) ||
         ( SelectionStyle == ImageLabelButtonSelectionStyles.ToggleSelectionThroughAllStyles );

      /// <summary>Handles the static selection changes.</summary>
      private Task HandleStaticSelectionChanges( IResponsiveTaskParams paramDict )
      {
         var button = paramDict.GetTypeSafeValue<IImageLabelButton_Forms>( 0 );

         // Do not recur onto our own broadcast; also only respond to the same selection group.
         if (
               button.IsNotNullOrDefault()
               &&
               ( button.SelectionGroup == SelectionGroup )
               &&
               !ReferenceEquals( button, this )
               &&
               IsAToggleButton 
               && 
               button.IsSelected
            )
         {
            Deselect();
         }

         return Task.CompletedTask;
      }

      /// <summary>Recreates the image.</summary>
      private void RecreateImage()
      {
         if ( !CurrentStyleIndexFound( out var styleIdx ) )
         {
            return;
         }

         var imageFileName = ImageLabelButtonStyles[ styleIdx ].ImageFilePath;

         if ( imageFileName.IsEmpty() )
         {
            return;
         }

         if ( CurrentStyle.GetImageFromResource && CurrentStyle.ImageResourceClassType.IsNullOrDefault() )
         {
            return;
         }

         ButtonImage =
            UIUtils_Forms.GetImage(
               imageFileName,
               ImageWidth,
               ImageHeight,
               Aspect.AspectFit,
               ImageHorizontalAlign,
               ImageVerticalAlign,
               ImageMargin,
               CurrentStyle.GetImageFromResource,
               CurrentStyle.ImageResourceClassType );
      }

      /// <summary>Releases the unmanaged resources.</summary>
      private void ReleaseUnmanagedResources()
      {
         // Global static, so remove the handler
         IAmSelectedStaticTask.RemoveIfThere( this, HandleStaticSelectionChanges );

         _tapGesture.Tapped -= HandleTapGestureTapped;

         RemoveButtonCommandEventListener();
      }

      /// <summary>Removes the button command listener.</summary>
      private void RemoveButtonCommandEventListener()
      {
         if ( ButtonCommand != null )
         {
            ButtonCommand.CanExecuteChanged -= HandleButtonCommandCanExecuteChanged;
         }
      }

      /// <summary>Sets up complete button command binding.</summary>
      private void SetUpCompleteButtonCommandBinding()
      {
         if ( ButtonCommandBindingName.IsEmpty() )
         {
            RemoveBinding( ButtonCommandProperty );
         }
         else
         {
            // NOTE Extremely reactive code below; see ValidatableViewBase.CreateBindings
            if ( ButtonCommandConverter.IsNotNullOrDefault() )
            {
               if ( ButtonCommandSource.IsNotNullOrDefault() )
               {
                  this.SetUpBinding( ButtonCommandProperty, ButtonCommandBindingName, converter: ButtonCommandConverter,
                     converterParameter: ButtonCommandConverterParameter, source: ButtonCommandSource );
               }
               else
               {
                  this.SetUpBinding( ButtonCommandProperty, ButtonCommandBindingName, converter: ButtonCommandConverter,
                     converterParameter: ButtonCommandConverterParameter );
               }
            }
            else
            {
               if ( ButtonCommandSource.IsNotNullOrDefault() )
               {
                  this.SetUpBinding( ButtonCommandProperty, ButtonCommandBindingName, source: ButtonCommandSource );
               }
               else
               {
                  this.SetUpBinding( ButtonCommandProperty, ButtonCommandBindingName );
               }
            }
         }
      }

      /// <summary>Updates the button text.</summary>
      private void UpdateButtonText()
      {
         if ( !UpdateButtonTextFromStyle || ( ButtonLabel == null ) || CurrentStyle.IsNullOrDefault() )
         {
            return;
         }

         ButtonLabel.Text = CurrentStyle.ButtonText;
      }

      /// <summary>Finalizes an instance of the <see cref="ImageLabelButtonBase_Forms" /> class.</summary>
      ~ImageLabelButtonBase_Forms()
      {
         ReleaseUnmanagedResources();
      }
   }
}