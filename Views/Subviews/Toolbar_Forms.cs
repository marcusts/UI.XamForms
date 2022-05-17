// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=Toolbar_Forms.cs
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

#define HACK_BROKEN_BINDINGS

// #define SHOW_BACK_COLORS

namespace Com.MarcusTS.UI.XamForms.Views.Subviews
{
   using System;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Controls;
   using Xamarin.Essentials;
   using Xamarin.Forms;
#if !HACK_BROKEN_BINDINGS
   using System;
   using System.Globalization;
#endif

   public class Toolbar_Forms : ShapeView_Forms, IToolbar_PI
   {
      private static readonly double DEFAULT_IMAGE_WIDTH_HEIGHT = 30.0.AdjustForOsAndDevice();
      private static readonly double IMAGE_PUSH = ( DEFAULT_IMAGE_WIDTH_HEIGHT / 2.75 ).AdjustForOsAndDevice();
      private static readonly double LABEL_FONT_SIZE = NamedSize.Micro.ToFormsNamedSize().AdjustForOsAndDevice();
      private static readonly double LABEL_PUSH = ( DEFAULT_IMAGE_WIDTH_HEIGHT / 2.75 ).AdjustForOsAndDevice();
      private static readonly int    MASTER_TOOLBAR_SELECTION_GROUP = ++GlobalConstants.NEXT_SELECTION_GROUP;

      private static readonly double TOOLBAR_AND_ITEM_RADIUS = UIConst_Forms.MEDIUM_CORNER_RADIUS_FACTOR;

      private static readonly double TOOLBAR_HEIGHT = 70.0.AdjustForOsAndDevice();
      private readonly        Grid _grid = UIUtils_Forms.GetExpandingGrid();
      private readonly        IThreadSafeAccessor _rebuildToolbarEntered = new ThreadSafeAccessor( 0 );
      private                 IHaveAndReportCurrentState_Forms _bindingContextAsHavingCurrentState;
      private                 double _bottomToolbarHeight;
      private                 double? _fixedItemWidth = TOOLBAR_HEIGHT;
      private                 string _imageResourcePath;
      private                 Type _imageResourceType;
      private                 bool _isJustified;
      private                 object _lastBindingContext;
      private                 double _marginAndSpacing = UIConst_Forms.A_MARGIN_SPACING_SINGLE_FACTOR;
      private                 (string, string)[] _toolbarItemNamesAndStates;

      public interface IToolBarItem_Forms : ITriStateImageLabelButton_Forms
      { }

      public Toolbar_Forms()
      {
#if SHOW_BACK_COLORS
         BackgroundColor = Color.Yellow;
#else
         BackgroundColor = Color.Transparent;
#endif

         HorizontalOptions       = LayoutOptions.FillAndExpand;
         VerticalOptions         = LayoutOptions.End;
         _grid.HorizontalOptions = LayoutOptions.FillAndExpand;
         _grid.VerticalOptions   = LayoutOptions.FillAndExpand;
         _grid.ColumnSpacing     = MarginAndSpacing;
         SetGridMargin();
         HeightRequest = TOOLBAR_HEIGHT;
         CornerRadius  = TOOLBAR_AND_ITEM_RADIUS;

         // This will run after the base class, assuming the base class adds a similar handler.
         PostBindingContextTasks.AddIfNotAlreadyThere( this, HandlePostBindingContextTask );
      }

      public double BottomToolbarHeight
      {
         get => _bottomToolbarHeight;
         set => SetBottomToolbarHeight( value ).FireAndFuhgetAboutIt();
      }

      public IResponsiveTasks BottomToolbarHeightChangedTask { get; set; } = new ResponsiveTasks( 1 );

      public double? FixedItemWidth
      {
         get => _fixedItemWidth;
         set
         {
            if ( _fixedItemWidth.IsDifferentThan( value ) )
            {
               _fixedItemWidth = value;
               RebuildToolbar().FireAndFuhgetAboutIt();
            }
         }
      }

      public string ImageResourcePath
      {
         get => _imageResourcePath;
         set
         {
            if ( _imageResourcePath.IsDifferentThan( value ) )
            {
               _imageResourcePath = value;
               OnPropertyChanged();
            }
         }
      }

      public Type ImageResourceType
      {
         get => _imageResourceType;
         set
         {
            _imageResourceType = value;
            OnPropertyChanged();
         }
      }

      public bool IsJustified
      {
         get => _isJustified;
         set
         {
            if ( _isJustified != value )
            {
               _isJustified = value;
               RebuildToolbar().FireAndFuhgetAboutIt();
            }
         }
      }

      public double MarginAndSpacing
      {
         get => _marginAndSpacing;
         set
         {
            if ( _marginAndSpacing.IsDifferentThan( value ) )
            {
               _marginAndSpacing = value;
               OnPropertyChanged();
               SetGridMargin();
            }
         }
      }

      public override Task<View> GetDefaultContent()
      {
         return Task.FromResult( _grid as View );
      }

      public async Task SetBottomToolbarHeight( double newHeight )
      {
         if ( _bottomToolbarHeight.IsDifferentThan( newHeight ) )
         {
            _bottomToolbarHeight = newHeight;

            await RaiseBottomToolbarHeightChangedTask().WithoutChangingContext();
         }
      }

      private Task HandleIsToolbarVisibleChangedTask( IResponsiveTaskParams paramDict )
      {
         var isVisible = paramDict.GetTypeSafeValue<bool>( 0 );

         if ( isVisible != IsVisible )
         {
            // HACK Added 2021-09-01 due to native crashes
            MainThread.BeginInvokeOnMainThread(
               async
               () =>
               {
                  IsVisible = isVisible;
                  await RaiseBottomToolbarHeightChangedTask().WithoutChangingContext();
               } );
         }

         return Task.CompletedTask;
      }

      private async Task HandlePostBindingContextTask( IResponsiveTaskParams paramDict )
      {
         if ( _lastBindingContext.IsNotNullOrDefault() )
         {
            if ( _lastBindingContext is IProvideToolbarItemNamesAndSelectionStates_PI
               _lastBindingContextAsProvidingNames )
            {
               _lastBindingContextAsProvidingNames.IsToolbarVisibleChangedTask
                                                  .RemoveIfThere( this, HandleIsToolbarVisibleChangedTask );
            }
         }

         // Must clear all handlers. These are dedicated to the toolbar, so must not be duplicated.
         _bindingContextAsHavingCurrentState?.CurrentStateChangedTask.UnsubscribeHost( this );

         // Might succeed or fail
         _bindingContextAsHavingCurrentState = BindingContext as IHaveAndReportCurrentState_Forms;

         if ( BindingContext is IProvideToolbarItemNamesAndSelectionStates_PI bindingContextAsProvidingNames )
         {
            _toolbarItemNamesAndStates = bindingContextAsProvidingNames.ToolbarItemNamesAndStates;
            bindingContextAsProvidingNames.IsToolbarVisibleChangedTask
                                          .AddIfNotAlreadyThere( this, HandleIsToolbarVisibleChangedTask );
            IsVisible = bindingContextAsProvidingNames.IsToolbarVisible;

            // Calls grid.ClearCompletely
            await RebuildToolbar().WithoutChangingContext();
         }
         else
         {
            // Safety measure
            _grid.ClearCompletely();
         }

         _grid.BindingContext = BindingContext;

         _lastBindingContext = BindingContext;
      }

      private async Task RaiseBottomToolbarHeightChangedTask()
      {
         // This originates in PropertyChanged, a void handler, hence no way to leverage proper a TPL await.
         await BottomToolbarHeightChangedTask.RunAllTasksUsingDefaults( new[] { BottomToolbarHeight, } )
                                             .WithoutChangingContext();
      }

      // Occurs upon binding context changed, hence _bindingContextAsHavingCurrentState is set before entry.
      private async Task RebuildToolbar()
      {
         if ( _rebuildToolbarEntered.IsTrue() )
         {
            return;
         }

         _rebuildToolbarEntered.SetTrue();

         try
         {
            _grid.ClearCompletely();

            if ( _toolbarItemNamesAndStates.IsAnEmptyList() )
            {
               return;
            }

            var currentColumn = 0;

            //var executingAssembly = Assembly.GetExecutingAssembly();
            //var resources         = executingAssembly.GetManifestResourceNames().ToArray();

            foreach ( var nameAndState in _toolbarItemNamesAndStates )
            {
               var nameToLower         = nameAndState.Item1.WithoutSpaces().ToLower();
               var imagePrefix         = ImageResourcePath + nameToLower;
               var imageSelectedPath   = imagePrefix       + "_selected"   + UIConst_PI.DEFAULT_IMAGE_SUFFIX;
               var imageDeselectedPath = imagePrefix       + "_deselected" + UIConst_PI.DEFAULT_IMAGE_SUFFIX;

               var toolbarItem =
                  new ToolBarItem_Forms
                  {
                     AnimateButton = true,
                     ButtonDeselectedStyle =
                        ImageLabelButtonBase_Forms.CreateButtonStyle( Color.Transparent, 0f, Color.Transparent ),
                     ButtonSelectedStyle = ImageLabelButtonBase_Forms.CreateButtonStyle( Color.White,
                        0, Color.Transparent ),
                     ButtonLabel             = UIUtils_Forms.GetSimpleLabel( nameAndState.Item1 ),
                     ButtonToggleSelection   = true,
                     CanSelect               = true,
                     CornerRadius            = TOOLBAR_AND_ITEM_RADIUS,
                     GetImageFromResource    = true,
                     HorizontalOptions       = LayoutOptions.FillAndExpand,
                     ImageDeselectedFilePath = imageDeselectedPath,
                     ImageResourceClassType  = ImageResourceType,
                     ImageSelectedFilePath   = imageSelectedPath,
                     ImageHeight             = DEFAULT_IMAGE_WIDTH_HEIGHT,
                     ImageHorizontalAlign    = LayoutOptions.Center,
                     ImageMargin             = new Thickness( 0, -IMAGE_PUSH, 0, IMAGE_PUSH ),
                     ImageVerticalAlign      = LayoutOptions.Fill,
                     ImageWidth              = DEFAULT_IMAGE_WIDTH_HEIGHT,
                     IncludeHapticFeedback   = true,
                     LabelDeselectedStyle =
                        ImageLabelButtonBase_Forms.CreateLabelStyle( Color.White, LABEL_FONT_SIZE,
                           FontAttributes.None ),
                     LabelSelectedStyle =
                        ImageLabelButtonBase_Forms.CreateLabelStyle( ThemeUtils_Forms.MAIN_STAGE_THEME_COLOR,
                           LABEL_FONT_SIZE,
                           FontAttributes.Bold ),
                     SelectionGroup  = MASTER_TOOLBAR_SELECTION_GROUP,
                     SelectionKey    = nameAndState.Item2,
                     SelectionStyle  = ImageLabelButtonSelectionStyles.ToggleSelectionAsFirstTwoStyles,
                     VerticalOptions = LayoutOptions.FillAndExpand,
                  };

               if ( BindingContext.IsNotNullOrDefault() )
               {
                  await toolbarItem.SetBindingContextSafelyAndAwaitAllBranchingTasks( BindingContext )
                                   .WithoutChangingContext();
               }

               // The button label exists because we create in above, so we can easily set its properties.
               toolbarItem.ButtonLabel.VerticalTextAlignment = TextAlignment.Center;
               toolbarItem.ButtonLabel.VerticalOptions       = LayoutOptions.Center;

               // Push the label down a bit
               toolbarItem.ButtonLabel.Margin = new Thickness( 0, LABEL_PUSH, 0, -LABEL_PUSH );

               if ( !IsJustified && FixedItemWidth.HasValue )
               {
                  toolbarItem.WidthRequest = FixedItemWidth.GetValueOrDefault();
               }

               if ( _bindingContextAsHavingCurrentState.IsNotNullOrDefault() )
               {
#if HACK_BROKEN_BINDINGS
                  toolbarItem.ButtonStateChangedTask.AddIfNotAlreadyThere
                  (
                     this,
                     async dict =>
                     {
                        if ( toolbarItem.IsSelected )
                        {
                           // This all raises the CurrentStateChangedTask task; see below.
                           await _bindingContextAsHavingCurrentState.GoToAppState( toolbarItem.SelectionKey ).WithoutChangingContext();
                        }
                     }
                  );

                  _bindingContextAsHavingCurrentState?.CurrentStateChangedTask.AddIfNotAlreadyThere(
                     this,
                     dict =>
                     {
                        var selection = dict.GetTypeSafeValue<string>( 0 );
                        if ( toolbarItem.SelectionKey.IsSameAs( selection ) )
                        {
                           toolbarItem.ButtonState = TriStateImageLabelButton_Forms.SELECTED_BUTTON_STATE;
                        }

                        return Task.CompletedTask;
                     } );
#else
                  toolbarItem.SetUpBinding(ImageLabelButtonBase.ButtonStateProperty,
                     nameof(IHaveCurrentState.CurrentState),
                     converter: new SelectionToButtonStateConverter { ToolbarItem = toolbarItem },
                     boundMode: BindingMode.TwoWay);
#endif
               }

               if ( IsJustified )
               {
                  _grid.AddStarColumn();
               }
               else
               {
                  _grid.AddAutoColumn();
               }

               _grid.AddAndSetRowsAndColumns( toolbarItem, column: currentColumn++ );
            }

            await SetBottomToolbarHeight( TOOLBAR_HEIGHT ).WithoutChangingContext();
         }
         finally
         {
            _rebuildToolbarEntered.SetFalse();
         }
      }

      private void SetGridMargin()
      {
         _grid.Margin = new Thickness( MarginAndSpacing, 0, MarginAndSpacing, MarginAndSpacing );
      }

      private class ToolBarItem_Forms : TriStateImageLabelButton_Forms, IToolBarItem_Forms
      {
         public string SelectionKey { get; set; }
      }

#if !HACK_BROKEN_BINDINGS
      public interface ISelectionToButtonStateConverter : IValueConverter
      {
         IToolBarItem ToolbarItem { get; set; }
      }

      private class SelectionToButtonStateConverter : ISelectionToButtonStateConverter
      {
         public IToolBarItem ToolbarItem { get; set; }

         // From the view model's CurrentState to the view's tri state image label button's button state
         public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
         {
            // Required
            if (ToolbarItem.IsNullOrDefault())
            {
               return default;
            }

            // Both value and param must be valid
            if (!ValueIsValid(value, out var valueStr))
            {
               return default;
            }

            var retState =
               ToolbarItem.CurrentState.IsSameAs(valueStr)
                  ? TriStateImageLabelButton.SELECTED_BUTTON_STATE
                  : TriStateImageLabelButton.DESELECTED_BUTTON_STATE;

            return retState;
         }

         // From the view's tri state image label button's button state to the view model's CurrentState
         public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
         {
            // Required
            if (ToolbarItem.IsNullOrDefault())
            {
               return default;
            }

            var bindingContextAsHavingSelectionKey = ((BindableObject)ToolbarItem).BindingContext as IHaveCurrentState;

            if (bindingContextAsHavingSelectionKey.IsNullOrDefault())
            {
               return default;
            }

            // If selected, send back the upper-cased button text, else just send back the original value
            var isSelected = ToolbarItem.ButtonState.IsSameAs(TriStateImageLabelButton.SELECTED_BUTTON_STATE);
            var retSelectedState =
               isSelected
                  ? ToolbarItem.CurrentState
                  // ReSharper disable once PossibleNullReferenceException
                  : bindingContextAsHavingSelectionKey.CurrentState;

            return retSelectedState;
         }

         private bool ValueIsValid(object value, out string valueStrOut)
         {
            if (value.IsNotNullOrDefault() && value is string valueStr)
            {
               valueStrOut = valueStr;

               return true;
            }

            valueStrOut = "";
            return false;
         }
      }
#endif
   }
}