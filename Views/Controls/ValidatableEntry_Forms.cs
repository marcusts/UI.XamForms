// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=ValidatableEntry_Forms.cs
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
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Converters;
   using Com.MarcusTS.UI.XamForms.Common.Images;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Xamarin.Essentials;
   using Xamarin.Forms;

   /// <summary>
   /// Interface IValidatableEntry_Forms Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
   /// </summary>
   /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
   public interface IValidatableEntry_Forms : IValidatableViewBase_Forms
   {
      Entry     EditableEntry       { get; }
      Thickness EditableEntryMargin { get; set; }
   }

   /// <summary>
   /// A UI element that includes an Entry surrounded by a border. Implements the <see cref="Xamarin.Forms.Grid" />
   /// Implements the <see cref="IValidatableEntry_Forms" />
   /// </summary>
   /// <seealso cref="Xamarin.Forms.Grid" />
   /// <seealso cref="IValidatableEntry_Forms" />
   public class ValidatableEntry_Forms : ValidatableViewBase_Forms, IValidatableEntry_Forms
   {
      private const double IMAGE_WIDTH_HEIGHT_AS_PERCENT_OF_BORDER_VIEW_HEIGHT = 0.85;

      /// <summary>The hide password image</summary>
      private const string HIDE_PASSWORD_IMAGE = SharedImageUtils.IMAGE_PRE_PATH + "hide_password.png";

      /// <summary>The show password image</summary>
      private const string SHOW_PASSWORD_IMAGE = SharedImageUtils.IMAGE_PRE_PATH + "show_password.png";

      private static readonly double DEFAULT_EDITABLE_ENTRY_MARGIN_A_WIDTH  = 7.5.AdjustForOsAndDevice();
      private static readonly double DEFAULT_EDITABLE_ENTRY_MARGIN_B_HEIGHT = DEFAULT_EDITABLE_ENTRY_MARGIN_A_WIDTH / 2;

      protected static readonly Thickness DEFAULT_EDITABLE_ENTRY_MARGIN =
         new Thickness( DEFAULT_EDITABLE_ENTRY_MARGIN_A_WIDTH, 0, DEFAULT_EDITABLE_ENTRY_MARGIN_B_HEIGHT, 0 );

      public static readonly BindableProperty EditableEntryMarginProperty =
         CreateValidatableEntryBindableProperty
         (
            nameof( EditableEntryMargin ),
            DEFAULT_EDITABLE_ENTRY_MARGIN,
            BindingMode.OneWay,
            (
               view,
               oldVal,
               newVal
            ) =>
            {
               // Fore reconstruction
               view._editableEntry = default;
            }
         );

      private readonly double   _entryFontSize;
      private readonly bool     _isPassword;
      private readonly Keyboard _keyboard;
      private          Entry    _editableEntry;
      private          View     _editableViewContainer;
      private          bool     _isPasswordShowing;
      private          Image    _showHideImage;

      public ValidatableEntry_Forms
      (
         double?  entryFontSize   = null,
         bool     isPassword      = false,
         Keyboard keyboard        = null,
         bool     asleepInitially = false
      )
         : base( Entry.TextProperty, asleepInitially: asleepInitially )
      {
         _keyboard      = keyboard;
         _isPassword    = isPassword;
         _entryFontSize = entryFontSize ?? UIConst_Forms.EDITABLE_VIEW_FONT_SIZE;

         if ( !IsConstructing )
         {
            RecreateAllViewsBindingsAndStyles().FireAndFuhgetAboutIt();
         }
      }

      protected override bool DerivedViewIsFocused => _showHideImage.IsNotNullOrDefault() && _showHideImage.IsFocused;

      protected override View EditableViewContainer
      {
         get
         {
            if ( _editableViewContainer.IsNullOrDefault() && !IsConstructing && EditableEntry.IsNotNullOrDefault() )
            {
               if ( _isPassword && CanUnmaskPassword.IsTrue() )
               {
                  // Put the editable entry in a grid with the password unmask button
                  var editGrid = UIUtils_Forms.GetExpandingGrid();

                  editGrid.AddStarColumn();
                  editGrid.AddAutoColumn();
                  editGrid.AddAndSetRowsAndColumns( EditableEntry, column: 0 );

                  _showHideImage = UIUtils_Forms.GetImage( "", 0, 0, Aspect.AspectFit, getFromResources: true,
                     resourceClass: GetType() );

                  // The image size is sensitive to the border view, but that is not alwys set when expected.
                  _showHideImage.SetUpBinding( HeightRequestProperty, nameof( BorderViewHeight ), source: this,
                     converter: BorderViewHeightToImageWidthHeightConverter.INSTANCE );
                  _showHideImage.SetUpBinding( WidthRequestProperty, nameof( BorderViewHeight ), source: this,
                     converter: BorderViewHeightToImageWidthHeightConverter.INSTANCE );

                  _showHideImage.Margin           = new Thickness( 0, 0, 5, 0 );
                  _showHideImage.InputTransparent = false;
                  var showHidePasswordGesture = new TapGestureRecognizer();

                  showHidePasswordGesture.Tapped += ( sender, args ) =>
                                                    {
                                                       var cursorIdx = EditableEntry.CursorPosition;
                                                       IsPasswordShowing = !IsPasswordShowing;

                                                       MainThread.BeginInvokeOnMainThread(
                                                          () =>
                                                          {
                                                             Task.Delay( 100 ).FireAndFuhgetAboutIt();

                                                             MainThread.BeginInvokeOnMainThread( () =>
                                                             {
                                                                // Go back to editing
                                                                EditableEntry.Focus();
                                                                EditableEntry.CursorPosition = cursorIdx;
                                                             } );
                                                          } );
                                                    };
                  _showHideImage.GestureRecognizers.Add( showHidePasswordGesture );
                  SetShowHideImageSource();

                  _showHideImage.Focused   -= ReportGlobalFocusAndRaisePlaceholder;
                  _showHideImage.Focused   += ReportGlobalFocusAndRaisePlaceholder;
                  _showHideImage.Unfocused -= ConsiderLoweringPlaceholder;
                  _showHideImage.Unfocused += ConsiderLoweringPlaceholder;

                  editGrid.AddAndSetRowsAndColumns( _showHideImage, column: 1 );
                  editGrid.RaiseChild( _showHideImage );

                  _editableViewContainer = editGrid;
               }
               else
               {
                  _editableViewContainer = EditableEntry;
               }
            }

            return _editableViewContainer;
         }
      }

      protected override bool UserHasEnteredValidContent => EditableEntry.Text.IsNotEmpty();

      /// <summary>Gets or sets a value indicating whether this instance is password showing.</summary>
      /// <value><c>true</c> if this instance is password showing; otherwise, <c>false</c>.</value>
      private bool IsPasswordShowing
      {
         get => _isPasswordShowing;
         set
         {
            if ( _isPasswordShowing == value )
            {
               return;
            }

            _isPasswordShowing = value;

            EditableEntry.IsPassword = !_isPasswordShowing;

            // BUG - HACK - Sometimes fails to refresh
            EditableEntry.IsPassword = _isPasswordShowing;
            EditableEntry.IsPassword = !_isPasswordShowing;

            SetShowHideImageSource();
         }
      }

      public Thickness EditableEntryMargin
      {
         get => (Thickness)GetValue( EditableEntryMarginProperty );
         set => SetValue( EditableEntryMarginProperty, value );
      }

      public override View EditableView => EditableEntry;

      public Entry EditableEntry
      {
         get
         {
            if ( _editableEntry.IsNullOrDefault() && !IsConstructing )
            {
               _editableEntry = new CustomEntry
                                {
                                   // Cannot use non-standard keyboards when there is a mask
                                   Keyboard          = _keyboard,
                                   BackgroundColor   = Color.Transparent,
                                   FontSize          = _entryFontSize,
                                   IsEnabled         = true,
                                   IsReadOnly        = false,
                                   MaxLength         = int.MaxValue,
                                   IsPassword        = _isPassword,
                                   TextColor         = Color.Black,
                                   HorizontalOptions = LayoutOptions.FillAndExpand,
                                   VerticalOptions   = LayoutOptions.FillAndExpand,
                                   Margin            = DEFAULT_EDITABLE_ENTRY_MARGIN,
                                };
            }

            return _editableEntry;
         }
      }

      public static BindableProperty CreateValidatableEntryBindableProperty<PropertyTypeT>
      (
         string                                                       localPropName,
         PropertyTypeT                                                defaultVal     = default,
         BindingMode                                                  bindingMode    = BindingMode.OneWay,
         Action<ValidatableEntry_Forms, PropertyTypeT, PropertyTypeT> callbackAction = null
      )
      {
         return BindableUtils_Forms.CreateBindableProperty( localPropName, defaultVal, bindingMode, callbackAction );
      }

      private void SetShowHideImageSource()
      {
         if ( _showHideImage.IsNotNullOrDefault() )
         {
            // ReSharper disable once RedundantAssignment
            var imageResourceStr = string.Empty;

            imageResourceStr = _isPasswordShowing ? SHOW_PASSWORD_IMAGE : HIDE_PASSWORD_IMAGE;

            _showHideImage.Source = ImageSource.FromResource( imageResourceStr, typeof( SharedImageUtils ).Assembly );
         }
      }

      private class BorderViewHeightToImageWidthHeightConverter : OneWayConverter<double, double>
      {
         public static readonly BorderViewHeightToImageWidthHeightConverter INSTANCE =
            new BorderViewHeightToImageWidthHeightConverter();

         protected override double Convert( double value, object parameter )
         {
            return value * IMAGE_WIDTH_HEIGHT_AS_PERCENT_OF_BORDER_VIEW_HEIGHT;
         }
      }
   }
}