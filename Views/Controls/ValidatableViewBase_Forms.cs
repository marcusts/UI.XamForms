// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ValidatableViewBase_Forms.cs
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

// #define SET_IMAGE_SOURCE_ON_MAIN_THREAD
// #define VALIDATE_ON_MAIN_THREAD

#define HIDE_VALIDATORS

namespace Com.MarcusTS.UI.XamForms.Views.Controls
{
   using System;
   using System.ComponentModel;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Behaviors;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Images;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Validations;
   using Com.MarcusTS.UI.XamForms.Views.Subviews;
   using Xamarin.Forms;

   public interface IValidatableViewBase_Forms : IValidatableView_Forms, IViewValidationBehaviorProperties_PI
   {
      bool        HideValidationsWhenAllValid { get; set; }
      StackLayout ValidationsList             { get; }
   }

   public abstract class ValidatableViewBase_Forms : Grid, IValidatableViewBase_Forms
   {
      // unset
      public const double DEFAULT_GRID_SINGLE_PADDING = 8;

      public const FontAttributes DEFAULT_INSTRUCTIONS_FONT_ATTRIBUTES =
         FontAttributes.Italic | FontAttributes.Bold;

      public const double DEFAULT_INSTRUCTIONS_HEIGHT                 = 30;
      public const double DEFAULT_INSTRUCTIONS_LABEL_FONT_SIZE_FACTOR = 2 / 3d;

      public const FontAttributes DEFAULT_INVALID_FONT_ATTRIBUTES =
         FontAttributes.Bold | FontAttributes.Italic;

      public const double DEFAULT_PLACEHOLDER_HEIGHT                 = 20;
      public const double DEFAULT_PLACEHOLDER_INSET                  = 8;
      public const double DEFAULT_PLACEHOLDER_LABEL_FONT_SIZE_FACTOR = 2 / 3d;
      public const double DEFAULT_PLACEHOLDER_LABEL_SIDE_MARGIN      = 8;
      public const double DEFAULT_PLACEHOLDER_TOP_MARGIN_ADJUSTMENT  = -4;
      public const double DEFAULT_VALIDATION_FONT_SIZE_FACTOR        = 2 / 3d;

      private static readonly Color DEFAULT_BORDER_VIEW_BORDER_COLOR = Color.Black;
      private static readonly Color DEFAULT_INVALID_TEXT_COLOR       = Color.Red;

      private static readonly Color DEFAULT_PLACEHOLDER_BACK_COLOR = Color.White;

      private static readonly Color  DEFAULT_PLACEHOLDER_TEXT_COLOR = ThemeUtils_Forms.MAIN_STAGE_THEME_COLOR;
      private static readonly double VALIDATION_IMAGE_BOTTOM_MARGIN = 2.0.AdjustForOsAndDevice();
      private static readonly double VALIDATION_IMAGE_RIGHT_MARGIN  = 4.0.AdjustForOsAndDevice();

      public static readonly double DEFAULT_BORDER_VIEW_BORDER_WIDTH =
         ( DeviceUtils_PI.IsIos() ? 1.75 : 2.5 ).AdjustForOsAndDevice();

      public static readonly Color DEFAULT_INSTRUCTIONS_TEXT_COLOR = Color.Purple;
      public static readonly Color DEFAULT_VALID_TEXT_COLOR        = Color.Black;

      public static readonly BindableProperty BorderViewBorderColorProperty =
         CreateValidatableViewBindablePropertyAndRespondByRefreshingStyles
         (
            nameof( BorderViewBorderColor ),
            DEFAULT_BORDER_VIEW_BORDER_COLOR
         );

      public static readonly BindableProperty BorderViewBorderWidthProperty =
         CreateValidatableViewBindablePropertyAndRespondByRefreshingStyles
         (
            nameof( BorderViewBorderWidth ),
            DEFAULT_BORDER_VIEW_BORDER_WIDTH
         );

      public static readonly BindableProperty BorderViewCornerRadiusFactorProperty =
         CreateValidatableViewBindablePropertyAndRespondByRefreshingStyles
         (
            nameof( BorderViewCornerRadiusFactor ),
            (double)UIConst_PI.DEFAULT_CORNER_RADIUS_FACTOR
         );

      public static readonly BindableProperty BorderViewHeightProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( BorderViewHeight ),
            default( double )
         );

      public static readonly BindableProperty BoundModeProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( BoundMode ),
            BindingMode.TwoWay
         );

      public static readonly BindableProperty BoundPropertyProperty =
         CreateValidatableViewBindableProperty
         (
            nameof( BoundProperty ),
            default( BindableProperty )
         );

      public static readonly double CHECK_WIDTH_HEIGHT = 12.0.AdjustForOsAndDevice();

      public static readonly BindableProperty ConverterParameterProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( ConverterParameter ),
            default( object )
         );

      public static readonly BindableProperty ConverterProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( Converter ),
            default( IValueConverter )
         );

      // No impact on anything
      public static readonly BindableProperty FontFamilyOverrideProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( FontFamilyOverride ),
            UIUtils_Forms.DefaultFontFamily()
         );

      public static readonly BindableProperty GridSinglePaddingProperty =
         CreateValidatableViewBindablePropertyAndRespondByRefreshingStyles
         (
            nameof( GridSinglePadding ),
            DEFAULT_GRID_SINGLE_PADDING
         );

      public static readonly BindableProperty InstructionsFontAttributesProperty =
         CreateValidatableViewBindablePropertyAndRespondByRefreshingStyles
         (
            nameof( InstructionsFontAttributes ),
            DEFAULT_INSTRUCTIONS_FONT_ATTRIBUTES
         );

      public static readonly BindableProperty InstructionsHeightProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( InstructionsHeight ),
            DEFAULT_INSTRUCTIONS_HEIGHT
         );

      public static readonly BindableProperty InstructionsLabelFontSizeFactorProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( InstructionsLabelFontSizeFactor ),
            DEFAULT_INSTRUCTIONS_LABEL_FONT_SIZE_FACTOR
         );

      public static readonly BindableProperty InstructionsTextColorProperty =
         CreateValidatableViewBindableProperty
         (
            nameof( InstructionsTextColor ),
            DEFAULT_INSTRUCTIONS_TEXT_COLOR
         );

      // No impact on anything
      public static readonly BindableProperty InstructionsTextProperty =
         CreateValidatableViewBindableProperty
         (
            nameof( InstructionsText ),
            default( string ),
            BindingMode.TwoWay,
            ( tasks, s, args ) => { tasks.HandleCurrentInstructionsChanged(); }
         );

      public static readonly BindableProperty InvalidBorderViewStyleProperty =
         CreateValidatableViewBindableProperty
         (
            nameof( InvalidBorderViewStyle ),
            default( Style )
         );

      public static readonly BindableProperty InvalidFontAttributesProperty =
         CreateValidatableViewBindablePropertyAndRespondByRefreshingStyles
         (
            nameof( InvalidFontAttributes ),
            DEFAULT_INVALID_FONT_ATTRIBUTES
         );

      public static readonly BindableProperty InvalidInstructionsStyleProperty =
         CreateValidatableViewBindableProperty
         (
            nameof( InvalidInstructionsStyle ),
            default( Style )
         );

      public static readonly BindableProperty InvalidPlaceholderStyleProperty =
         CreateValidatableViewBindableProperty
         (
            nameof( InvalidPlaceholderStyle ),
            default( Style )
         );

      public static readonly BindableProperty InvalidTextColorProperty =
         CreateValidatableViewBindablePropertyAndRespondByRefreshingStyles
         (
            nameof( InvalidTextColor ),
            DEFAULT_INVALID_TEXT_COLOR
         );

      public static readonly BindableProperty PlaceholderBackColorProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( PlaceholderBackColor ),
            DEFAULT_PLACEHOLDER_BACK_COLOR
         );

      public static readonly BindableProperty PlaceholderHeightProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( PlaceholderHeight ),
            DEFAULT_PLACEHOLDER_HEIGHT
         );

      public static readonly BindableProperty PlaceholderInsetProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( PlaceholderInset ),
            DEFAULT_PLACEHOLDER_INSET
         );

      public static readonly BindableProperty PlaceholderLabelFontSizeFactorProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( PlaceholderLabelFontSizeFactor ),
            DEFAULT_PLACEHOLDER_LABEL_FONT_SIZE_FACTOR
         );

      public static readonly BindableProperty PlaceholderLabelSideMarginProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( PlaceholderLabelSideMargin ),
            DEFAULT_PLACEHOLDER_LABEL_SIDE_MARGIN
         );

      public static readonly BindableProperty PlaceholderTextColorProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( PlaceholderTextColor ),
            DEFAULT_PLACEHOLDER_TEXT_COLOR
         );

      public static readonly BindableProperty PlaceholderTextProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( PlaceholderText ),
            default( string )
         );

      public static readonly BindableProperty PlaceholderTopMarginAdjustmentProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( PlaceholderTopMarginAdjustment ),
            DEFAULT_PLACEHOLDER_TOP_MARGIN_ADJUSTMENT
         );

      public static readonly BindableProperty ShowInstructionsProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( ShowInstructions ),
            default( int )
         );

      public static readonly BindableProperty ShowValidationErrorsProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( ShowValidationErrors ),
            default( int )
         );

      public static readonly BindableProperty ShowValidationErrorsWithInstructionsProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( ShowValidationErrorsWithInstructions ),
            default( int )
         );

      public static readonly BindableProperty StringFormatProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( StringFormat ),
            default( string )
         );

      public static readonly FontAttributes VALID_FONT_ATTRIBUTES = FontAttributes.None;

      public static readonly BindableProperty ValidationFontSizeFactorProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( ValidationFontSizeFactor ),
            DEFAULT_VALIDATION_FONT_SIZE_FACTOR
         );

      public static readonly BindableProperty ValidatorProperty =
         CreateValidatableViewBindableProperty
         (
            nameof( Validator ),
            default( IValidationBehaviorBase_Forms ),
            callbackAction:

            // ReSharper disable once AsyncVoidLambda
            (
               view,
               oldVal,
               newVal
            ) =>
            {
               view.ResetValidator().FireAndFuhgetAboutIt();
            }
         );

      public static readonly BindableProperty ValidBorderViewStyleProperty =
         CreateValidatableViewBindableProperty
         (
            nameof( ValidBorderViewStyle ),
            default( Style )
         );

      public static readonly BindableProperty ValidFontAttributesProperty =
         CreateValidatableViewBindablePropertyAndRespondByRefreshingStyles
         (
            nameof( ValidFontAttributes ),
            VALID_FONT_ATTRIBUTES
         );

      public static readonly BindableProperty ValidInstructionsStyleProperty =
         CreateValidatableViewBindableProperty
         (
            nameof( ValidInstructionsStyle ),
            default( Style )
         );

      public static readonly BindableProperty ValidPlaceholderStyleProperty =
         CreateValidatableViewBindableProperty
         (
            nameof( ValidPlaceholderStyle ),
            default( Style )
         );

      public static readonly BindableProperty ValidTextColorProperty =
         CreateValidatableViewBindablePropertyAndRespondByRefreshingStyles
         (
            nameof( ValidTextColor ),
            DEFAULT_VALID_TEXT_COLOR
         );

      public static readonly BindableProperty ViewModelPropertyNameProperty =
         CreateValidatableViewBindablePropertyAndRespondByRecreatingViews
         (
            nameof( ViewModelPropertyName ),
            default( string )
         );

      private AbsoluteLayout                _canvas;
      private Rectangle                     _lastBorderViewBounds;
      private Rectangle                     _lastEditableViewContainerBounds;
      private IValidationBehaviorBase_Forms _lastValidator;
      private Style                         _lastValidBorderViewStyle;
      private Grid                          _placeholderGrid;
      private bool                          _placeholderLabelHasBeenShown;
      private bool                          _recreateAllViewsBindingsAndStylesEntered;

      protected ValidatableViewBase_Forms( BindableProperty              boundProp,
                                           IValidationBehaviorBase_Forms validator       = default,
                                           bool                          asleepInitially = false )
      {
         this.SetDefaults();

         BoundProperty   = boundProp;
         Validator       = validator;
         BackgroundColor = Color.Transparent;
         ColumnSpacing   = 0;
         RowSpacing      = 0;

         IsConstructing = asleepInitially;
      }

      protected abstract bool DerivedViewIsFocused       { get; }
      protected abstract View EditableViewContainer      { get; }
      protected          bool IsConstructing             { get; private set; }
      protected abstract bool UserHasEnteredValidContent { get; }

      private double InstructionsFactoredFontSize => GetFactoredFontSize( InstructionsLabelFontSizeFactor );

      private double PlaceholderFactoredFontSize => GetFactoredFontSize( PlaceholderLabelFontSizeFactor );

      private double SMALL_MARGIN               => GridSinglePadding / 2;
      private double ValidationFactoredFontSize => GetFactoredFontSize( ValidationFontSizeFactor );

      public string ViewModelPropertyName
      {
         get => (string)GetValue( ViewModelPropertyNameProperty );
         set => SetValue( ViewModelPropertyNameProperty, value );
      }

      public abstract View EditableView { get; }

      /// <summary>Gets the border view.</summary>
      /// <value>The border view.</value>
      public ShapeView_Forms BorderView { get; private set; }

      public Color BorderViewBorderColor
      {
         get => (Color)GetValue( BorderViewBorderColorProperty );
         set => SetValue( BorderViewBorderColorProperty, value );
      }

      public double BorderViewBorderWidth
      {
         get => (double)GetValue( BorderViewBorderWidthProperty );
         set => SetValue( BorderViewBorderWidthProperty, value );
      }

      public double BorderViewCornerRadiusFactor
      {
         get => (double)GetValue( BorderViewCornerRadiusFactorProperty );
         set => SetValue( BorderViewCornerRadiusFactorProperty, value );
      }

      public double BorderViewHeight
      {
         get => (double)GetValue( BorderViewHeightProperty );
         set => SetValue( BorderViewHeightProperty, value );
      }

      public BindingMode BoundMode
      {
         get => (BindingMode)GetValue( BoundModeProperty );
         set => SetValue( BoundModeProperty, value );
      }

      public BindableProperty BoundProperty
      {
         get => (BindableProperty)GetValue( BoundPropertyProperty );
         set => SetValue( BoundPropertyProperty, value );
      }

      public IValueConverter Converter
      {
         get => (IValueConverter)GetValue( ConverterProperty );
         set => SetValue( ConverterProperty, value );
      }

      public object ConverterParameter
      {
         get => GetValue( ConverterParameterProperty );
         set => SetValue( ConverterParameterProperty, value );
      }

      public string FontFamilyOverride
      {
         get => (string)GetValue( FontFamilyOverrideProperty );
         set => SetValue( FontFamilyOverrideProperty, value );
      }

      public double GridSinglePadding
      {
         get => (double)GetValue( GridSinglePaddingProperty );
         set => SetValue( GridSinglePaddingProperty, value );
      }

      public IValidationBehaviorBase_Forms Validator
      {
         get => (IValidationBehaviorBase_Forms)GetValue( ValidatorProperty );
         set => SetValue( ValidatorProperty, value );
      }

      public Style ValidBorderViewStyle
      {
         get => (Style)GetValue( ValidBorderViewStyleProperty );
         set => SetValue( ValidBorderViewStyleProperty, value );
      }

      public FontAttributes ValidFontAttributes
      {
         get => (FontAttributes)GetValue( ValidFontAttributesProperty );
         set => SetValue( ValidFontAttributesProperty, value );
      }

      public Style ValidInstructionsStyle
      {
         get => (Style)GetValue( ValidInstructionsStyleProperty );
         set => SetValue( ValidInstructionsStyleProperty, value );
      }

      public Style ValidPlaceholderStyle
      {
         get => (Style)GetValue( ValidPlaceholderStyleProperty );
         set => SetValue( ValidPlaceholderStyleProperty, value );
      }

      public Color ValidTextColor
      {
         get => (Color)GetValue( ValidTextColorProperty );
         set => SetValue( ValidTextColorProperty, value );
      }

      public async Task CallRevalidate()
      {
         if ( Validator.IsNotNullOrDefault() )
         {
            await Validator.RevalidateAllConditions().WithoutChangingContext();
         }
      }

      public virtual int SetTabIndexes( int incomingTabIndex )
      {
         BorderView.IsTabStop = false;

         // BorderView.TabIndex = incomingTabIndex++;

         if ( EditableViewContainer.IsNotAnEqualReferenceTo( EditableView ) )
         {
            EditableViewContainer.IsTabStop = false;

            // EditableViewContainer.TabIndex = incomingTabIndex++;
         }

         EditableView.IsTabStop = true;
         EditableView.TabIndex  = incomingTabIndex++;

         return incomingTabIndex;
      }

      public async Task StopConstructionAndRefresh()
      {
         IsConstructing = false;
         await RecreateAllViewsBindingsAndStyles().WithoutChangingContext();
      }

      public static BindableProperty CreateValidatableViewBindableProperty<PropertyTypeT>
      (
         string                                                          localPropName,
         PropertyTypeT                                                   defaultVal     = default,
         BindingMode                                                     bindingMode    = BindingMode.TwoWay,
         Action<ValidatableViewBase_Forms, PropertyTypeT, PropertyTypeT> callbackAction = null
      )
      {
         return BindableUtils_Forms.CreateBindableProperty( localPropName, defaultVal, bindingMode, callbackAction );
      }

      public static BindableProperty CreateValidatableViewBindablePropertyAndRespondByRecreatingViews<PropertyTypeT>
      (
         string        localPropName,
         PropertyTypeT defaultVal  = default,
         BindingMode   bindingMode = BindingMode.TwoWay
      )
      {
         return CreateValidatableViewBindableProperty( localPropName, defaultVal, bindingMode,
            (
               view,
               oldVal,
               newVal
            ) =>
            {
               view.RecreateAllViewsBindingsAndStyles().FireAndFuhgetAboutIt();
            } );
      }

      public static BindableProperty CreateValidatableViewBindablePropertyAndRespondByRefreshingStyles<PropertyTypeT>
      (
         string        localPropName,
         PropertyTypeT defaultVal  = default,
         BindingMode   bindingMode = BindingMode.TwoWay
      )
      {
         return CreateValidatableViewBindableProperty( localPropName, defaultVal, bindingMode,
            (
               view,
               oldVal,
               newVal
            ) =>
            {
               // Request style refresh
               view.ReapplyStyles();
            });
      }

      protected async void ConsiderLoweringPlaceholder(object sender, FocusEventArgs e)
      {
         await ResetPlaceholderPosition().WithoutChangingContext();
      }

      protected void CreateBindings()
      {
         // This *must* have the current binding context.
         if ( ViewModelPropertyName.IsEmpty() || BoundProperty.IsNullOrDefault() || EditableView.IsNullOrDefault() )
         {
            return;
         }

         EditableView.ConsiderBindings( BoundProperty, ViewModelPropertyName, BindingContext, StringFormat, BoundMode,
            Converter, ConverterParameter );
      }

      /// <summary>Creates the views.</summary>
      protected virtual async Task CreateViews()
      {
         this.ClearCompletely();

         BorderView               = UIUtils_Forms.GetShapeView();
         BorderView.HeightRequest = BorderViewHeight;
         BorderView.CornerRadius  = BorderViewHeight * BorderViewCornerRadiusFactor;

         await BorderView.SetContentSafelyAndAwaitAllBranchingTasks( EditableViewContainer ).WithoutChangingContext();

         BorderView.Margin = new Thickness( 0, GridSinglePadding, 0, 0 );

         BorderView.BorderColor = BorderViewBorderColor;
         BorderView.BorderWidth = BorderViewBorderWidth;

         // BorderView is newly created, so the += subscription is not redundant
         BorderView.PropertyChanged +=
            async
               ( sender, args ) =>
            {
               if ( BorderView.Bounds.AreValidAndHaveChanged( args.PropertyName, _lastBorderViewBounds ) )
               {
                  _lastBorderViewBounds = BorderView.Bounds;
                  await ResetPlaceholderPosition().WithoutChangingContext();
               }
            };

         this.AddAutoRow();
         this.AddAndSetRowsAndColumns( BorderView, 0 );

         // Row 1 (optional) holds the instructions InstructionsText might not show up until run-time Show instructions
         // is appropriate and if they don't conflict with validations
         if ( ShowInstructions.IsTrue() &&
              ( ShowValidationErrors.IsFalse() || ShowValidationErrorsWithInstructions.IsTrue() ) )
         {
            InstructionsLabel =
               UIUtils_Forms.GetSimpleLabel( fontFamilyOverride: FontFamilyOverride,
                  horizontalAlignment: TextAlignment.Start,
                  textColor: InstructionsTextColor,
                  fontAttributes: InstructionsFontAttributes,
                  fontSize: InstructionsFactoredFontSize,
                  bindingSourcePropName: nameof( InstructionsText ),
                  bindingSource: this );
            InstructionsLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
            InstructionsLabel.VerticalOptions   = LayoutOptions.StartAndExpand;

            InstructionsLabel.Margin = new Thickness( 0, GridSinglePadding, 0, 0 );

            this.AddAutoRow();
            this.AddAndSetRowsAndColumns( InstructionsLabel, 1 );

            HandleCurrentInstructionsChanged();
         }
         else
         {
            InstructionsLabel = default;
         }

         if ( ShowValidationErrors.IsTrue() )
         {
            ValidationsList                 = UIUtils_Forms.GetExpandingStackLayout();
            ValidationsList.VerticalOptions = LayoutOptions.StartAndExpand;
            ValidationsList.BindingContext  = Validator;

            ValidationsList.Margin = new Thickness( 0, GridSinglePadding, 0, 0 );

            this.AddAutoRow();
            this.AddAndSetRowsAndColumns( ValidationsList, 2 );

            ResetValidationConditions();
         }

         // Placeholder floats on the canvas
         if ( PlaceholderText.IsNotEmpty() )
         {
            PlaceholderLabel = UIUtils_Forms.GetSimpleLabel( PlaceholderText, fontFamilyOverride: FontFamilyOverride,
               textColor: PlaceholderTextColor,
               fontSize: PlaceholderFactoredFontSize );
            PlaceholderLabel.HeightRequest   = PlaceholderHeight;
            PlaceholderLabel.Text            = PlaceholderText;
            PlaceholderLabel.BackgroundColor = PlaceholderBackColor;

            _placeholderGrid                 = UIUtils_Forms.GetExpandingGrid();
            _placeholderGrid.BackgroundColor = PlaceholderBackColor;
            _placeholderGrid.AddAutoRow();
            _placeholderGrid.AddFixedColumn( PlaceholderLabelSideMargin );
            _placeholderGrid.AddAutoColumn();
            _placeholderGrid.AddAndSetRowsAndColumns( PlaceholderLabel, 0, 1 );
            _placeholderGrid.AddFixedColumn( PlaceholderLabelSideMargin );
            _placeholderGrid.HeightRequest = PlaceholderHeight;

            // Absolute layout for _canvas position -- this overlays the other rows
            _canvas = UIUtils_Forms.GetExpandingAbsoluteLayout();

            // IMPORTANT -- the canvas is on top of other things, so must let user input through
            _canvas.InputTransparent = true;

            // The exact position will be set once the border view gets its bounds (and whenever those bonds change)
            _canvas.Children.Add( _placeholderGrid );

            // The canvas has to be able to float higher than the top border
            this.AddAndSetRowsAndColumns( _canvas, 0 );
            RaiseChild( _canvas );

            EditableView.Focused   -= ReportGlobalFocusAndRaisePlaceholder;
            EditableView.Focused   += ReportGlobalFocusAndRaisePlaceholder;
            EditableView.Unfocused -= ConsiderLoweringPlaceholder;
            EditableView.Unfocused += ConsiderLoweringPlaceholder;
         }
         else
         {
            PlaceholderLabel = default;
         }

         // NOTE: Does not succeed most of the time; not a b-u-g
         await AttemptInitialValidation().WithoutChangingContext();

#if !SUPPRESS_PROP_CHANGED
         if ( EditableViewContainer.IsNotNullOrDefault() )
         {
            EditableViewContainer.PropertyChanged -= EditableViewContainerOnPropertyChanged();
            EditableViewContainer.PropertyChanged += EditableViewContainerOnPropertyChanged();
         }
#endif
      }

      protected override void OnBindingContextChanged()
      {
         base.OnBindingContextChanged();

         EditableView.BindingContext = BindingContext;

         ResetPlaceholderPosition().FireAndFuhgetAboutIt();
      }

      /// <summary>Resets the styles.</summary>
      protected virtual void ReapplyStyles()
      {
         if ( IsConstructing )
         {
            return;
         }

         ValidPlaceholderStyle    = CreatePlaceholderStyle();
         InvalidPlaceholderStyle  = CreatePlaceholderStyle();
         InvalidInstructionsStyle = CreateInstructionsStyle();
         ValidInstructionsStyle   = CreateInstructionsStyle();
         InvalidBorderViewStyle =
            UIUtils_Forms.CreateShapeViewStyle( borderColor: InvalidTextColor, borderThickness: BorderViewBorderWidth );
         ValidBorderViewStyle =
            UIUtils_Forms.CreateShapeViewStyle( borderColor: ValidTextColor, borderThickness: BorderViewBorderWidth );
      }

      protected async Task RecreateAllViewsBindingsAndStyles()
      {
         if ( _recreateAllViewsBindingsAndStylesEntered || IsConstructing )
         {
            return;
         }

         _recreateAllViewsBindingsAndStylesEntered = true;

         try
         {
            await CreateViews().WithoutChangingContext();
            CreateBindings();
            ReapplyStyles();

            if ( Validator.IsNotNullOrDefault() )
            {
               HandleValidatorIsValidChanged( Validator.IsValid.IsTrue() );
            }
         }
         finally
         {
            _recreateAllViewsBindingsAndStylesEntered = false;
         }
      }

      protected async void ReportGlobalFocusAndRaisePlaceholder(object sender, FocusEventArgs e)
      {
         await ResetPlaceholderPosition().WithoutChangingContext();
      }

      /// <summary>Resets the placeholder position.</summary>
      protected async Task ResetPlaceholderPosition()
      {
         // Make sure we have a valid placeholder
         if ( PlaceholderLabel.IsNullOrDefault() || PlaceholderLabel.Text.IsEmpty() || EditableView.IsNullOrDefault() ||
              BorderView.IsNullOrDefault()       || !BorderView.Height.IsGreaterThan( 0 ) ||
              _placeholderGrid.IsNullOrDefault() )
         {
            return;
         }

         var    targetX = PlaceholderInset;
         double targetY;

         // See if the entry has focus or not
         if ( EditableView.IsFocused || UserHasEnteredValidContent || DerivedViewIsFocused )
         {
            targetY = PlaceholderTopMarginAdjustment;
         }
         else
         {
            targetY = ( ( BorderViewHeight + ( GridSinglePadding * 2 ) ) - PlaceholderHeight ) / 2;
         }

         _canvas.RaiseChild( _placeholderGrid );

         await _placeholderGrid.TranslateTo( targetX, targetY ).WithoutChangingContext();

         if ( !_placeholderLabelHasBeenShown )
         {
            if ( PlaceholderLabel.Opacity.IsLessThan( UIUtils_Forms.VISIBLE_OPACITY ) )
            {
               PlaceholderLabel.FadeIn();
            }

            _placeholderLabelHasBeenShown = true;
         }
      }

      private async Task AttemptInitialValidation()
      {
         // Validate immediately if possible
         if ( Validator.IsNotNullOrDefault() && Validator is Behavior validatorAsBehavior )
         {
            if ( EditableView.IsNullOrDefault() )
            {
               return;
            }

            if ( EditableView is Entry )
            {
               // The entry is well-adapted to a hosting a behavior.
               if ( !EditableView.Behaviors.Contains( validatorAsBehavior ) )
               {
                  EditableView.Behaviors.Add( validatorAsBehavior );
               }
            }
            else
            {
               // The view is too generalized to host a behavior. The container has other information needed
               // for validation.
               if ( !Behaviors.Contains( validatorAsBehavior ) )
               {
                  Behaviors.Add( validatorAsBehavior );
               }
            }

            Validator.IsValidChangedTask.AddIfNotAlreadyThere( this, HandleValidatorIsValidChangedTask );

            await CallRevalidate().WithoutChangingContext();
         }
      }

      private Style CreateInstructionsStyle()
      {
         return UIUtils_Forms.CreateLabelStyle(
            FontFamilyOverride,
            InstructionsFactoredFontSize,
            Color.Transparent,
            InstructionsTextColor,
            FontAttributes.Italic
         );
      }

      private Style CreatePlaceholderStyle()
      {
         return UIUtils_Forms.CreateLabelStyle(
            FontFamilyOverride,
            PlaceholderFactoredFontSize,
            PlaceholderBackColor,
            PlaceholderTextColor
         );
      }

      private View CreateViewForCondition( IIsValidCondition_PI isValidConditionPi )
      {
         var grid = UIUtils_Forms.GetExpandingGrid();
         grid.AddAutoColumn();
         grid.AddStarColumn();

         var retImage =
            UIUtils_Forms.GetImage( "",
               CHECK_WIDTH_HEIGHT,
               CHECK_WIDTH_HEIGHT );
         retImage.Margin = new Thickness( 0, 0, VALIDATION_IMAGE_RIGHT_MARGIN, VALIDATION_IMAGE_BOTTOM_MARGIN );
         grid.AddAndSetRowsAndColumns( retImage, column: 0 );

         isValidConditionPi.IsValidChangedTask.AddIfNotAlreadyThere( this, HandleIsValidChangedForImage );

         var descLabel =
            UIUtils_Forms.GetSimpleLabel( horizontalAlignment: TextAlignment.Start,
               fontSize: ValidationFactoredFontSize,
               bindingSourcePropName: nameof( IIsValidCondition_PI.RuleDescription ),
               bindingSource: isValidConditionPi
            );
         descLabel.Margin = new Thickness( 0, 0, 0, SMALL_MARGIN );
         grid.AddAndSetRowsAndColumns( descLabel, column: 1 );

         // Call routinely to set up the invalid state
         SetImageSource( isValidConditionPi.IsValid );

         return grid;


         // PRIVATE MATHODS
         Task HandleIsValidChangedForImage( IResponsiveTaskParams paramDict )
         {
            var isValid = paramDict.GetTypeSafeValue<IThreadSafeAccessor>( 0 );

            SetImageSource( isValid );

            return Task.CompletedTask;
         }

         void SetImageSource( IThreadSafeAccessor isValid )
         {
            ThreadHelper.ConsiderBeginInvokeActionOnMainThread( SetSource,

#if SET_IMAGE_SOURCE_ON_MAIN_THREAD
               true
#else

               // ReSharper disable once RedundantArgumentDefaultValue
               false
#endif
            );


            // PRIVATE METHODS
            void SetSource()
            {
               retImage.Source =
                  isValid.IsTrue()
                     ? ImageSource.FromResource( SharedImageUtils.IMAGE_PRE_PATH + "checked.png",
                        typeof( SharedImageUtils ).Assembly )
                     : ImageSource.FromResource( SharedImageUtils.IMAGE_PRE_PATH + "unchecked.png",
                        typeof( SharedImageUtils ).Assembly );
            }
         }
      }

      private PropertyChangedEventHandler EditableViewContainerOnPropertyChanged()
      {
         return ( sender, args ) =>
                {
                   if ( EditableViewContainer.Bounds.AreValidAndHaveChanged( args.PropertyName,
                      _lastEditableViewContainerBounds ) )
                   {
                      _lastEditableViewContainerBounds = EditableViewContainer.Bounds;
                      ResetPlaceholderPosition().FireAndFuhgetAboutIt();
                   }
                };
      }

      private double GetFactoredFontSize( double factor )
      {
         return EditableView is Entry editableViewAsEntry
            ? editableViewAsEntry.FontSize * factor
            : Device.GetNamedSize( NamedSize.Small, typeof( Label ) );
      }

      private void HandleCurrentInstructionsChanged()
      {
         if ( InstructionsLabel.IsNullOrDefault() )
         {
            return;
         }

         // If no current instructions, hide the label
         InstructionsLabel.IsVisible =
            ShowInstructions.IsTrue()     &&
            InstructionsText.IsNotEmpty() &&
            (
               ValidationsList.IsNullOrDefault()        ||
               ValidationsList.Children.IsAnEmptyList() ||
               ShowValidationErrorsWithInstructions.IsTrue()
            );
      }

      private Task HandleValidationConditionsChanged( IResponsiveTaskParams paramDict )
      {
         ResetValidationConditions();

         return Task.CompletedTask;
      }

      private void HandleValidatorIsValidChanged( bool isValid )
      {
         ThreadHelper.ConsiderBeginInvokeActionOnMainThread( HandleIsValid,

#if VALIDATE_ON_MAIN_THREAD
            true
#else

            // ReSharper disable once RedundantArgumentDefaultValue
            false
#endif
         );


         // PRIVATE METHODS
         void HandleIsValid()
         {
            if ( isValid )
            {
               BorderView?.SetAndForceStyle( _lastValidBorderViewStyle.IsNotNullOrDefault()
                  ? _lastValidBorderViewStyle
                  : ValidBorderViewStyle );

               PlaceholderLabel?.SetAndForceStyle( ValidPlaceholderStyle );
               InstructionsLabel?.SetAndForceStyle( ValidInstructionsStyle );

               if ( HideValidationsWhenAllValid )
               {
                  // No validations to show.
                  if ( ValidationsList.IsNotNullOrDefault() )
                  {
                     ValidationsList.IsVisible = false;
                  }
               }

               // The corner radius is always uniform in or examples so randomly picking top left
               _lastValidBorderViewStyle = UIUtils_Forms.CreateShapeViewStyle( BorderView?.BackgroundColor,
                  BorderView?.CornerRadius,
                  BorderView?.BorderColor,
                  BorderView?.BorderWidth );
            }
            else
            {
               PlaceholderLabel?.SetAndForceStyle( InvalidPlaceholderStyle );
               BorderView?.SetAndForceStyle( InvalidBorderViewStyle );
               InstructionsLabel?.SetAndForceStyle( InvalidInstructionsStyle );

               // If the validator issues a validation error, show that in place of the instructions (below the border view).
               if ( ShowValidationErrors.IsTrue() && Validator.IsNotNullOrDefault() &&
                    Validator.ValidationConditions.IsNotAnEmptyList()
               )
               {
                  // We have validations to show.
                  if ( ValidationsList.IsNotNullOrDefault() )
                  {
                     ValidationsList.IsVisible = true;
                  }
               }
               else
               {
                  // No validations to show.
                  if ( ValidationsList.IsNotNullOrDefault() )
                  {
                     ValidationsList.IsVisible = false;
                  }
               }
            }

            // Consider how to handle instructions
            HandleCurrentInstructionsChanged();
         }
      }

      private Task HandleValidatorIsValidChangedTask( IResponsiveTaskParams paramDict )
      {
         var isValid = paramDict.GetTypeSafeValue<IThreadSafeAccessor>( 0 );

         HandleValidatorIsValidChanged( isValid.IsTrue() );

         return Task.CompletedTask;
      }

      private void ResetValidationConditions()
      {
         if ( ValidationsList.IsNullOrDefault() || ShowValidationErrors.IsFalse() )
         {
            return;
         }

         ValidationsList.Children.Clear();

         if ( Validator.IsNotNullOrDefault() && Validator.ValidationConditions.IsNotAnEmptyList() )
         {
            // The Validator will include all displayable conditions
            foreach ( var condition in Validator.ValidationConditions )
            {
               ValidationsList.Children.Add( CreateViewForCondition( condition ) );
            }
         }
      }

      private async Task ResetValidator()
      {
         if ( _lastValidator.IsNotNullOrDefault() )
         {
            foreach ( var condition in _lastValidator.ValidationConditions )
            {
               condition.IsValidChangedTask.UnsubscribeHost( this );
            }

            _lastValidator.ValidationConditionsChanged.RemoveIfThere( this, HandleValidationConditionsChanged );
            _lastValidator.IsValidChangedTask.RemoveIfThere( this, HandleValidatorIsValidChangedTask );
         }

         // Reset the UI for these new validation conditions
         ResetValidationConditions();

         // Listen for the conditions changed task.
         if ( Validator.IsNotNullOrDefault() )
         {
            Validator.ValidationConditionsChanged.AddIfNotAlreadyThere( this, HandleValidationConditionsChanged );
            Validator.IsValidChangedTask.AddIfNotAlreadyThere( this, HandleValidatorIsValidChangedTask );

            await AttemptInitialValidation().WithoutChangingContext();
         }

         _lastValidator = Validator;
      }

      public bool HideValidationsWhenAllValid { get; set; }
#if HIDE_VALIDATORS
         = true;
#endif

      public FontAttributes InstructionsFontAttributes
      {
         get => (FontAttributes)GetValue( InstructionsFontAttributesProperty );
         set => SetValue( InstructionsFontAttributesProperty, value );
      }

      public double InstructionsHeight
      {
         get => (double)GetValue( InstructionsHeightProperty );
         set => SetValue( InstructionsHeightProperty, value );
      }

      public Label InstructionsLabel { get; private set; }

      public double InstructionsLabelFontSizeFactor
      {
         get => (double)GetValue( InstructionsLabelFontSizeFactorProperty );
         set => SetValue( InstructionsLabelFontSizeFactorProperty, value );
      }

      public int CanUnmaskPassword { get; set; }

      public string InstructionsText
      {
         get => (string)GetValue( InstructionsTextProperty );
         set => SetValue( InstructionsTextProperty, value );
      }

      public Color InstructionsTextColor
      {
         get => (Color)GetValue( InstructionsTextColorProperty );
         set => SetValue( InstructionsTextColorProperty, value );
      }

      /// <summary>Gets or sets the invalid border view style.</summary>
      /// <value>The invalid border view style.</value>
      public Style InvalidBorderViewStyle
      {
         get => (Style)GetValue( InvalidBorderViewStyleProperty );
         set => SetValue( InvalidBorderViewStyleProperty, value );
      }

      public FontAttributes InvalidFontAttributes
      {
         get => (FontAttributes)GetValue( InvalidFontAttributesProperty );
         set => SetValue( InvalidFontAttributesProperty, value );
      }

      public Style InvalidInstructionsStyle
      {
         get => (Style)GetValue( InvalidInstructionsStyleProperty );
         set => SetValue( InvalidInstructionsStyleProperty, value );
      }

      public Style InvalidPlaceholderStyle
      {
         get => (Style)GetValue( InvalidPlaceholderStyleProperty );
         set => SetValue( InvalidPlaceholderStyleProperty, value );
      }

      public Color InvalidTextColor
      {
         get => (Color)GetValue( InvalidTextColorProperty );
         set => SetValue( InvalidTextColorProperty, value );
      }

      public Color PlaceholderBackColor
      {
         get => (Color)GetValue( PlaceholderBackColorProperty );
         set => SetValue( PlaceholderBackColorProperty, value );
      }

      public double PlaceholderHeight
      {
         get => (double)GetValue( PlaceholderHeightProperty );
         set => SetValue( PlaceholderHeightProperty, value );
      }

      public double PlaceholderInset
      {
         get => (double)GetValue( PlaceholderInsetProperty );
         set => SetValue( PlaceholderInsetProperty, value );
      }

      public Label PlaceholderLabel { get; private set; }

      public double PlaceholderLabelFontSizeFactor
      {
         get => (double)GetValue( PlaceholderLabelFontSizeFactorProperty );
         set => SetValue( PlaceholderLabelFontSizeFactorProperty, value );
      }

      public double PlaceholderLabelSideMargin
      {
         get => (double)GetValue( PlaceholderLabelSideMarginProperty );
         set => SetValue( PlaceholderLabelSideMarginProperty, value );
      }

      public string PlaceholderText
      {
         get => (string)GetValue( PlaceholderTextProperty );
         set => SetValue( PlaceholderTextProperty, value );
      }

      public Color PlaceholderTextColor
      {
         get => (Color)GetValue( PlaceholderTextColorProperty );
         set => SetValue( PlaceholderTextColorProperty, value );
      }

      public double PlaceholderTopMarginAdjustment
      {
         get => (double)GetValue( PlaceholderTopMarginAdjustmentProperty );
         set => SetValue( PlaceholderTopMarginAdjustmentProperty, value );
      }

      public int ShowInstructions
      {
         get => (int)GetValue( ShowInstructionsProperty );
         set => SetValue( ShowInstructionsProperty, value );
      }

      public int ShowValidationErrors
      {
         get => (int)GetValue( ShowValidationErrorsProperty );
         set => SetValue( ShowValidationErrorsProperty, value );
      }

      public int ShowValidationErrorsWithInstructions
      {
         get => (int)GetValue( ShowValidationErrorsWithInstructionsProperty );
         set => SetValue( ShowValidationErrorsWithInstructionsProperty, value );
      }

      public string StringFormat
      {
         get => (string)GetValue( StringFormatProperty );
         set => SetValue( StringFormatProperty, value );
      }

      public double ValidationFontSizeFactor
      {
         get => (double)GetValue( ValidationFontSizeFactorProperty );
         set => SetValue( ValidationFontSizeFactorProperty, value );
      }

      public StackLayout ValidationsList { get; private set; }
   }
}