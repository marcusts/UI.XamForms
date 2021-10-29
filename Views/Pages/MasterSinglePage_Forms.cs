// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=MasterSinglePage.cs
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
// #define DEFEAT_SPINNER

namespace Com.MarcusTS.UI.XamForms.Views.Pages
{
   using System;
   using System.Diagnostics;
   using System.Linq;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Devices;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Subviews;
   using Xamarin.Essentials;
   using Xamarin.Forms;

   public interface IMasterSinglePage_Forms :
      IContentPageBase_Forms,
      ICanShowProgressSpinner_Forms
   {
      IReportBottomToolbarHeight_PI BottomToolbarHeightReporter { get; set; }

      IProvideToolbarItemNamesAndSelectionStates_PI ToolbarItemNamesAndSelectionStatesReporter { get; set; }

      void AddGridView( View view, AvailableGrids grid, bool removeOtherChildren = false );

      void RemoveAllForeignViews( AvailableGrids grid );

      void RemoveGridView( View view, AvailableGrids grid );
   }

   /// <summary>Much of the code is the same as that at the <see cref="ShapeView_Forms" />.</summary>
   public class MasterSinglePage_Forms : ContentPage_Forms, IMasterSinglePage_Forms
   {
      private const double ANDROID_INDICATOR_SCALE = 1;
      private const double INDICATOR_WIDTH_HEIGHT  = 50;
      private const double IOS_INDICATOR_SCALE     = 3.0;
      private static readonly Color DEFAULT_SPINNER_BACK_COLOR =  Color.FromRgba( 1, 1, 1, 50 );

      private readonly Grid                          _grossPageGrid;
      private readonly Grid                          _keyboardAreaGrid;
      private readonly Grid                          _mainStageGrid;
      private          IReportBottomToolbarHeight_PI _bottomToolbarHeightReporter;
      private          RelativeLayout                _isBusyBackground;
      private          ActivityIndicator             _isBusyIndicator; // = new ActivityIndicator();
      private          bool                          _isBusyShowing;

      private double                                        _lastMainStageHeight;
      private double                                        _softKeybardHeight;
      private bool                                          _softKeyboardIsVisible;
      private Color                                         _spinnerColor = Color.PaleGreen;
      private IProvideToolbarItemNamesAndSelectionStates_PI _toolbarItemNamesAndSelectionStatesReporter;
      private Color                                         _spinnerBackColor = DEFAULT_SPINNER_BACK_COLOR;

      public MasterSinglePage_Forms()
      {
         Padding = new Thickness( 0, DeviceUtils_PI.IsIos() ? 30.0.AdjustForOsAndDevice() : 0, 0,
            DeviceUtils_PI.IsIos() ? 10.0.AdjustForOsAndDevice() : 0 );

#if SHOW_BACK_COLORS
         BackgroundColor = Color.Yellow;
#else
         BackgroundColor = ThemeUtils_Forms.MAIN_STAGE_THEME_COLOR;
#endif

         CreateBackgroundLayout();
         _grossPageGrid = UIUtils_Forms.GetExpandingGrid();

#if SHOW_BACK_COLORS
         _grossPageGrid.BackgroundColor = Color.Orange;
#else
         _grossPageGrid.BackgroundColor = Color.Transparent;
#endif

         _grossPageGrid.AddStarRow();
         _grossPageGrid.AddAutoRow();
         _mainStageGrid = UIUtils_Forms.GetExpandingGrid();

         _mainStageGrid.PropertyChanged +=
            async
               ( sender, args ) =>
            {
               if ( args.PropertyName.IsSameAs( nameof( Height ) ) &&
                    _mainStageGrid.Height.IsDifferentThan( _lastMainStageHeight ) )
               {
                  Debug.WriteLine( "Master Single page main stage grid height has changed ->" +
                                   _mainStageGrid.Height.ToRoundedInt()                       + "<-" );
                  await SetSpinnerOverlayHeight( _mainStageGrid.Height ).WithoutChangingContext();
                  _lastMainStageHeight = _mainStageGrid.Height;
               }
            };

#if SHOW_BACK_COLORS
         _mainStageGrid.BackgroundColor = Color.MediumPurple;
#else
         _mainStageGrid.BackgroundColor = Color.Transparent;
#endif

         _grossPageGrid.AddAndSetRowsAndColumns( _mainStageGrid, 0 );

         _keyboardAreaGrid               = UIUtils_Forms.GetExpandingGrid();
         _keyboardAreaGrid.HeightRequest = 0;

#if SHOW_BACK_COLORS
         _keyboardAreaGrid.BackgroundColor = Color.LimeGreen;
#else
         _keyboardAreaGrid.BackgroundColor = Color.Transparent;
#endif

         _grossPageGrid.AddAndSetRowsAndColumns( _keyboardAreaGrid, 1 );

         SoftKeyboard_Forms.Current.VisibilityChangedTask.AddIfNotAlreadyThere( this,
            HandleSoftKeyboardVisibilityChangedTask );


         // -----------------------------------------------------------------------------------------------------------
         // PRIVATE METHODS
         // -----------------------------------------------------------------------------------------------------------
         Task HandleSoftKeyboardVisibilityChangedTask( IResponsiveTaskParams paramDict )
         {
            _softKeyboardIsVisible = paramDict.GetTypeSafeValue<bool>( 0 );
            _softKeybardHeight     = paramDict.GetTypeSafeValue<double>( 1 );
            SetKeyboardGridHeight();

            return Task.CompletedTask;
         }

         // -----------------------------------------------------------------------------------------------------------
      }

      public bool AllowInputWhileIsBusy { get; set; }

      public IReportBottomToolbarHeight_PI BottomToolbarHeightReporter
      {
         get => _bottomToolbarHeightReporter;
         set
         {
            if ( _bottomToolbarHeightReporter.IsNotAnEqualReferenceTo( value ) )
            {
               if ( _bottomToolbarHeightReporter.IsNotNullOrDefault() )
               {
                  _bottomToolbarHeightReporter.BottomToolbarHeightChangedTask.RemoveIfThere( this,
                     HandleBottomToolbarHeightChanged );
               }

               _bottomToolbarHeightReporter = value;

               if ( _bottomToolbarHeightReporter.IsNotNullOrDefault() )
               {
                  _bottomToolbarHeightReporter.BottomToolbarHeightChangedTask.AddIfNotAlreadyThere( this,
                     HandleBottomToolbarHeightChanged );
                  SetKeyboardGridHeight();
               }
            }
         }
      }

      public bool IsBusyShowing
      {
         get => _isBusyShowing;
         set
         {
            if ( _isBusyShowing != value )
            {
               _isBusyShowing = value;

               if ( _isBusyShowing )
               {
                  AddIsBusyIndicator();
               }
               else
               {
                  RemoveIsBusyIndicator();
               }
            }
         }
      }

      public Color SpinnerForeColor
      {
         get => _spinnerColor;
         set
         {
            _spinnerColor = value;

            if ( _isBusyIndicator.IsNotNullOrDefault() )
            {
               _isBusyIndicator.Color = value;
            }
         }
      }

      public Color SpinnerBackColor
      {
         get => _spinnerBackColor;
         set
         {
            _spinnerBackColor = value;
            SetSpinnerBackColor();
         }
      }

      public double SpinnerOverlayHeight { get; private set; }

      public IResponsiveTasks SpinnerOverlayHeightChanged { get; } = new ResponsiveTasks( 1 );

      public IProvideToolbarItemNamesAndSelectionStates_PI ToolbarItemNamesAndSelectionStatesReporter
      {
         get => _toolbarItemNamesAndSelectionStatesReporter;
         set
         {
            if ( _toolbarItemNamesAndSelectionStatesReporter.IsNotAnEqualReferenceTo( value ) )
            {
               _toolbarItemNamesAndSelectionStatesReporter = value;
               SetKeyboardGridHeight();
            }
         }
      }

      public void AddGridView( View view, AvailableGrids grid, bool removeOtherChildren = false )
      {
         if ( removeOtherChildren )
         {
            RemoveAllForeignViews( grid );
         }

         // ReSharper disable once PossibleNullReferenceException
         if ( view.Opacity.IsDifferentThan( UIConst_PI.VISIBLE_OPACITY ) )
         {
            view.FadeIn();
         }

         var gridToSet = GetGridToSet( grid );

         if ( gridToSet.IsNotNullOrDefault() )
         {
            if ( !gridToSet.RowDefinitions.Any() )
            {
               gridToSet.AddStarRow();
            }

            gridToSet.AddAndSetRowsAndColumns( view, 0, rowSpan: grid == AvailableGrids.Gross ? 2 : 1 );

            // ReSharper disable once PossibleNullReferenceException
            gridToSet.RaiseChild( view );

            SetKeyboardGridHeight();
         }
      }

      public override Task<View> GetDefaultContent()
      {
#if SHOW_BACK_COLORS
         var retLayout = UIUtils_Forms.GetExpandingRelativeLayout();
         retLayout.Margin = 15.0.AdjustForOsAndDevice();
         retLayout.CreateRelativeOverlay(_grossPageGrid);
         return Task.FromResult<View>(retLayout);
#else
         return Task.FromResult<View>( _grossPageGrid );
#endif
      }

      public void RemoveAllForeignViews( AvailableGrids grid )
      {
         var gridToSet = GetGridToSet( grid );

         if ( gridToSet.IsNotNullOrDefault() )
         {
            // Can't remove structural grids Can remove the is busy indicator

            foreach ( var child in gridToSet.Children.Where( child =>
                                                                child.IsNotAnEqualReferenceTo( _keyboardAreaGrid ) &&
                                                                child.IsNotAnEqualReferenceTo( _mainStageGrid ) )
                                            .ToArray() )
            {
               RemoveGridView( child, grid );
            }
         }
      }

      public void RemoveGridView( View view, AvailableGrids grid )
      {
         var gridToSet = GetGridToSet( grid );

         if ( gridToSet.IsNotNullOrDefault() && gridToSet.Children.Contains( view ) )
         {
            gridToSet.Children.Remove( view );

            SetKeyboardGridHeight();
         }
      }

      public async Task SetSpinnerOverlayHeight( double newHeight )
      {
         if ( SpinnerOverlayHeight.IsDifferentThan( newHeight ) )
         {
            SpinnerOverlayHeight = newHeight;
            await SpinnerOverlayHeightChanged.RunAllTasksUsingDefaults( SpinnerOverlayHeight ).WithoutChangingContext();
         }
      }

      private void AddIsBusyIndicator()
      {
#if !DEFEAT_SPINNER
         MainThread.BeginInvokeOnMainThread( () =>
                                             {
                                                AddGridView( _isBusyBackground, AvailableGrids.Gross );
                                                _isBusyIndicator.IsRunning = true;
                                             } );
#endif
      }

      private void SetSpinnerBackColor()
      {
         _isBusyBackground.BackgroundColor = SpinnerBackColor;
      }

      private void CreateBackgroundLayout()
      {
         _isBusyBackground                  = UIUtils_Forms.GetExpandingRelativeLayout();
         _isBusyBackground.InputTransparent = AllowInputWhileIsBusy;
         SetSpinnerBackColor();

         _isBusyIndicator =
            new ActivityIndicator
            {
               Color = SpinnerForeColor,
               Scale = Device.RuntimePlatform.IsSameAs( Device.iOS ) ? IOS_INDICATOR_SCALE : ANDROID_INDICATOR_SCALE,
               HeightRequest = INDICATOR_WIDTH_HEIGHT,
               WidthRequest = INDICATOR_WIDTH_HEIGHT,
               HorizontalOptions = LayoutOptions.Center,
               VerticalOptions = LayoutOptions.Center,
            };

         _isBusyBackground.CreateRelativeOverlay( _isBusyIndicator );
      }

      private Grid GetGridToSet( AvailableGrids grid )
      {
         Grid gridToSet = default;

         switch ( grid )
         {
            case AvailableGrids.Stage:
               gridToSet = _mainStageGrid;
               break;

            case AvailableGrids.Keyboard:
               gridToSet = _keyboardAreaGrid;
               break;

            case AvailableGrids.Gross:
               gridToSet = _grossPageGrid;
               break;
         }

         return gridToSet;
      }

      private Task HandleBottomToolbarHeightChanged( IResponsiveTaskParams paramDict )
      {
         SetKeyboardGridHeight();

         return Task.CompletedTask;
      }

      private void RemoveIsBusyIndicator()
      {
#if !DEFEAT_SPINNER
         MainThread.BeginInvokeOnMainThread( () =>
                                             {
                                                _isBusyIndicator.IsRunning = false;
                                                RemoveGridView( _isBusyBackground, AvailableGrids.Gross );
                                             } );
#endif
      }

      private void SetKeyboardGridHeight()
      {
         MainThread.BeginInvokeOnMainThread(
            () =>
            {
               var newHeightRequest = 0.0;

               try
               {
                  // Focus is sometimes conflated into showing the keyboard, though that is not always true When the keyboard
                  // is not showing, its height is always 0.
                  newHeightRequest =
                     _softKeyboardIsVisible && _softKeybardHeight.IsGreaterThan( 0 )
                        ? _softKeybardHeight
                        : BottomToolbarHeightReporter.IsNotNullOrDefault()                &&
                          ToolbarItemNamesAndSelectionStatesReporter.IsNotNullOrDefault() &&
                          ToolbarItemNamesAndSelectionStatesReporter.IsToolbarVisible
                           ? BottomToolbarHeightReporter.BottomToolbarHeight
                           : 0;
               }
               catch ( Exception ex )
               {
                  Debug.WriteLine( ex.Message );
               }

               if ( newHeightRequest.IsDifferentThan( Math.Max( 0, _keyboardAreaGrid.Height ) ) )
               {
                  _keyboardAreaGrid.HeightRequest = newHeightRequest;
               }
            } );
      }
   }
}