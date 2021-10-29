// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=ScrollableTable_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Views.Subviews
{
   using System.Collections.Concurrent;
   using System.Collections.Generic;
   using System.Linq;
   using System.Reflection;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Controls;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Controls;
   using Xamarin.Forms;

   public interface IScrollableTable_Forms : IScrollableViewBase_Forms
   {
      bool      HasHorizontalGridlines      { get; set; }
      bool      HasVerticalGridlines        { get; set; }
      Color     HorizontalGridlineColor     { get; set; }
      double    HorizontalGridlineGirth     { get; set; }
      Thickness HorizontalGridlineMargin    { get; set; }
      bool      IncludeGridlineBelowHeader  { get; set; }
      bool      IncludeGridlineInsideHeader { get; set; }
      Color     VerticalGridlineColor       { get; set; }
      double    VerticalGridlineGirth       { get; set; }
      Thickness VerticalGridlineMargin      { get; set; }
   }

   /// <summary>
   /// This produces an animated list of "table" style grid rows.
   /// The rows are made of cells; the cell views are auto-generated based on custom view model attributes.
   /// This requires a view model that provides properties adorned by
   /// <see cref="IViewModelTableColumnAttribute_PI" />
   /// Also requires a BindingContext that is of type IProvideRawData
   /// A master scroller is optional (_useScrollView), and included by default.
   /// </summary>
   public class ScrollableTable_Forms<RowViewModelT> : ScrollableViewBase_Forms, IScrollableTable_Forms
   {
      private static readonly IDictionary<PropertyInfo, ViewModelTableColumnAttribute_PI> _propInfoDict;

      private readonly IDictionary<PropertyInfo, ICanSortWithTapped> _defaultSortDict =
         new ConcurrentDictionary<PropertyInfo, ICanSortWithTapped>();

      private readonly IThreadSafeAccessor _isInitializing = new ThreadSafeAccessor( 0 );
      private          bool                _hasHorizontalGridlines;
      private          bool                _hasVerticalGridlines;
      private          View                _header;
      private          Color               _horizontalGridlineColor;
      private          double              _horizontalGridlineGirth;
      private          Thickness           _horizontalGridlineMargin;
      private          bool                _includeGridlineBelowHeader;
      private          bool                _includeGridlineInsideHeader;
      private          object              _lastBindingContext;
      private          Color               _verticalGridlineColor;
      private          double              _verticalGridlineGirth;
      private          Thickness           _verticalGridlineMargin;

      static ScrollableTable_Forms()
      {
         _propInfoDict = typeof( RowViewModelT ).CreateViewModelCustomAttributeDict<ViewModelTableColumnAttribute_PI>();
      }

      protected ScrollableTable_Forms( ICanShowProgressSpinner_Forms spinnerHost ) : base( spinnerHost )
      {
         _isInitializing.SetTrue();

         CreateHeaderFromPropInfo().FireAndFuhgetAboutIt();

         // Speed this one up compared to the norm
         AnimatableLayout.RescaleDelays( 0.25 );

         // Set the default comparer (until we tap on something)
         SetAnimatableLayoutSortComparer().FireAndFuhgetAboutIt();

         // Creates the header info
         EndOfConstruction();
      }

      protected virtual ITableAttributeViewManager_Forms CurrentCustomizations { get; } =
         new TableAttributeViewManager_Forms();

      private bool DataIsReady =>
         BindingContext is IProvideRawData bindingContextAsProvidingRawData &&
         bindingContextAsProvidingRawData.GetRawData.IsNotNullOrDefault()   &&
         bindingContextAsProvidingRawData.GetRawData().IsNotAnEmptyList();

      public bool HasHorizontalGridlines
      {
         get => _hasHorizontalGridlines;
         set
         {
            if ( _hasHorizontalGridlines != value )
            {
               _hasHorizontalGridlines = value;
               RequestCreateAnimatableLayoutSourceViewsIfDataIsReady().FireAndFuhgetAboutIt();
            }
         }
      }

      public bool HasVerticalGridlines
      {
         get => _hasVerticalGridlines;
         set
         {
            if ( _hasVerticalGridlines != value )
            {
               _hasVerticalGridlines = value;
               RequestCreateAnimatableLayoutSourceViewsIfDataIsReady().FireAndFuhgetAboutIt();
            }
         }
      }

      public Color HorizontalGridlineColor
      {
         get => _horizontalGridlineColor;
         set
         {
            if ( _horizontalGridlineColor != value )
            {
               _horizontalGridlineColor = value;
               RequestCreateAnimatableLayoutSourceViewsIfDataIsReady().FireAndFuhgetAboutIt();
            }
         }
      }

      public double HorizontalGridlineGirth
      {
         get => _horizontalGridlineGirth;
         set
         {
            if ( _horizontalGridlineGirth.IsDifferentThan( value ) )
            {
               _horizontalGridlineGirth = value;
               RequestCreateAnimatableLayoutSourceViewsIfDataIsReady().FireAndFuhgetAboutIt();
            }
         }
      }

      public Thickness HorizontalGridlineMargin
      {
         get => _horizontalGridlineMargin;
         set
         {
            if ( _horizontalGridlineMargin.IsDifferentThan( value ) )
            {
               _horizontalGridlineMargin = value;
               RequestCreateAnimatableLayoutSourceViewsIfDataIsReady().FireAndFuhgetAboutIt();
            }
         }
      }

      public bool IncludeGridlineBelowHeader
      {
         get => _includeGridlineBelowHeader;
         set
         {
            if ( _includeGridlineBelowHeader != value )
            {
               _includeGridlineBelowHeader = value;
               CreateHeaderFromPropInfo().FireAndFuhgetAboutIt();
            }
         }
      }

      public bool IncludeGridlineInsideHeader
      {
         get => _includeGridlineInsideHeader;
         set
         {
            if ( _includeGridlineInsideHeader != value )
            {
               _includeGridlineInsideHeader = value;
               CreateHeaderFromPropInfo().FireAndFuhgetAboutIt();
            }
         }
      }

      public Color VerticalGridlineColor
      {
         get => _verticalGridlineColor;
         set
         {
            if ( _verticalGridlineColor != value )
            {
               _verticalGridlineColor = value;
               RequestCreateAnimatableLayoutSourceViewsIfDataIsReady().FireAndFuhgetAboutIt();
            }
         }
      }

      public double VerticalGridlineGirth
      {
         get => _verticalGridlineGirth;
         set
         {
            if ( _verticalGridlineGirth.IsDifferentThan( value ) )
            {
               _verticalGridlineGirth = value;
               RequestCreateAnimatableLayoutSourceViewsIfDataIsReady().FireAndFuhgetAboutIt();
            }
         }
      }

      public Thickness VerticalGridlineMargin
      {
         get => _verticalGridlineMargin;
         set
         {
            if ( _verticalGridlineMargin.IsDifferentThan( value ) )
            {
               _verticalGridlineMargin = value;
               RequestCreateAnimatableLayoutSourceViewsIfDataIsReady().FireAndFuhgetAboutIt();
            }
         }
      }

      protected virtual Task AfterRowFrameCreated( ShapeView_Forms rowFrame, object child, List<View> rowCellViews,
                                                   List<IViewModelTableColumnAttribute_PI>
                                                      viewModelTableColumnAttributePis )
      {
         return Task.CompletedTask;
      }

      /// <remarks>
      /// This occurs after a binding context change, among other things.
      /// </remarks>
      /// <returns></returns>
      protected override async Task<(bool, BetterObservableCollection<View>)> CreateScrollableViews()
      {
         if ( !DataIsReady || !( BindingContext is IProvideRawData bindingContextAsProvidingRawData ) )
         {
            return ( false, default );
         }


         // ELSE
         var retViews = new BetterObservableCollection<View>();

         foreach ( var child in bindingContextAsProvidingRawData.GetRawData() )
         {
            // Reset per row
            var currentColumn = 0;
            var rowGrid       = UIUtils_Forms.GetExpandingGrid();

            // Create two rows: the first for the cells and the second for a (potential) line below each row
            rowGrid.AddAutoRow();
            rowGrid.AddAutoRow();

            var rowCellViews      = new List<View>();
            var rowCellAttributes = new List<IViewModelTableColumnAttribute_PI>();

            foreach ( var propInfo in _propInfoDict.OrderBy( pi => pi.Value.DisplayOrder ) )
            {
               var rowCell =
                  await CurrentCustomizations.CreateCellViewForAttribute( propInfo.Value, child )
                                             .WithoutChangingContext();

               AddViewToGridColumn( propInfo.Value, rowGrid, rowCell, currentColumn++ );

               rowCellViews.Add( rowCell );
               rowCellAttributes.Add( propInfo.Value );

               if ( CanAddLines( HasVerticalGridlines, VerticalGridlineColor, VerticalGridlineGirth ) )
               {
                  AddVerticalGridline( rowGrid, currentColumn++ );
               }
            }

            // Add to a configurable shape view
            var rowFrame = new ShapeView_Forms();
            rowFrame.SetDefaults();
            await rowFrame.SetBindingContextSafely( child ).WithoutChangingContext();

            // Consider adding a line beneath using a column span
            if ( CanAddLines( HasHorizontalGridlines, HorizontalGridlineColor, HorizontalGridlineGirth ) )
            {
               var horizontalLine = CreateHorizontalLine();

               // horizontalLine.SetUpBinding( WidthRequestProperty, nameof( Width ), source: rowGrid );
               // retViews.AddRangeSortedAndWithoutNotification( new View[] { horizontalLine, } );
               rowGrid.AddAndSetRowsAndColumns( horizontalLine, 1, 0, 1, currentColumn );
            }

            await rowFrame.SetContentSafelyAndAwaitAllBranchingTasks( rowGrid ).WithoutChangingContext();

            await AfterRowFrameCreated( rowFrame, child, rowCellViews, rowCellAttributes ).WithoutChangingContext();

            retViews.AddRangeSortedAndWithoutNotification( new View[] { rowFrame, } );
         }

         return ( true, retViews );
      }

      protected override async Task<View> CreateViewToSet()
      {
         var existingView = await base.CreateViewToSet().WithoutChangingContext();

         if ( _header.IsNullOrDefault() )
         {
            return existingView;
         }

         // ELSE

         // Create a grid
         var retGrid = UIUtils_Forms.GetExpandingGrid();
         retGrid.AddAutoRow();
         retGrid.AddStarRow();

         // Add the header to the first row
         retGrid.AddAndSetRowsAndColumns( _header,      0 );
         retGrid.AddAndSetRowsAndColumns( existingView, 1 );

         return retGrid;
      }

      protected override async Task EndInitialization()
      {
         if ( _isInitializing.IsTrue() )
         {
            return;
         }

         await base.EndInitialization().WithoutChangingContext();
      }

      protected override async Task HandlePostBindingContextTask( IResponsiveTaskParams paramDict )
      {
         if ( _lastBindingContext.IsNotNullOrDefault() &&
              _lastBindingContext is IProvideRawData lastBindingContextAsProvidingRawData )
         {
            lastBindingContextAsProvidingRawData.RawDataChangedTask.RemoveIfThere( this, HandleRawDataChangedTask );
         }

         // Unconditional
         _lastBindingContext = BindingContext;

         if ( BindingContext.IsNotNullOrDefault() &&
              BindingContext is IProvideRawData bindingContextAsProvidingRawData )
         {
            // Set  the task handler for new data
            bindingContextAsProvidingRawData.RawDataChangedTask.AddIfNotAlreadyThere( this, HandleRawDataChangedTask );
         }

         // Then the base
         await base.HandlePostBindingContextTask( paramDict );
      }

      private static void AddViewToGridColumn
      (
         IViewModelTableColumnAttribute_PI attribute,
         Grid                              grid,
         View                              cell,
         int                               column
      )
      {
         var colWidth = attribute.ColumnWidth_OS.ToOsEquivalentPositiveNumber( attribute );

         if ( colWidth.IsANumberGreaterThanZero() )
         {
            if ( attribute.IsFlexWidth.IsTrue() )
            {
               grid.AddStarColumn( colWidth );
            }
            else
            {
               grid.AddFixedColumn( colWidth );
            }
         }
         else
         {
            // No specified factor
            grid.AddStarColumn();
         }

         grid.AddAndSetRowsAndColumns( cell, 0, column );
      }

      private static bool CanAddLines( bool hasGridlines, Color gridlineColor, double gridlineGirth )
      {
         return
            hasGridlines                           &&
            ( gridlineColor != Color.Transparent ) &&
            gridlineGirth.IsANumberGreaterThanZero();
      }

      private void AddVerticalGridline( Grid headerGrid, int currentColumn )
      {
         headerGrid.AddFixedColumn( VerticalGridlineGirth );

         var verticalLine = CreateVerticalLine();

         headerGrid.AddAndSetRowsAndColumns( verticalLine, 0, currentColumn );
      }

      private async Task CreateHeaderFromPropInfo()
      {
         _header = default;
         _defaultSortDict.Clear();

         // Create the header (from a grid)
         var headerGrid = UIUtils_Forms.GetExpandingGrid();

#if SHOW_BACK_COLORS
         headerGrid.BackgroundColor = Color.Red;
#endif

         var currentColumn = 0;

         // Might need to add vertical gridlines
         foreach ( var keyValuePair in _propInfoDict.OrderBy( pi => pi.Value.DisplayOrder ) )
         {
            var headerCell =
               await CurrentCustomizations.CreateHeaderViewForAttribute( keyValuePair.Value )
                                          .WithoutChangingContext();

            AddViewToGridColumn( keyValuePair.Value, headerGrid, headerCell, currentColumn++ );

            if ( CanAddLines( HasVerticalGridlines, VerticalGridlineColor, VerticalGridlineGirth ) &&
                 IncludeGridlineInsideHeader )
            {
               AddVerticalGridline( headerGrid, currentColumn++ );
            }

            if ( keyValuePair.Value.CanSort.IsTrue() &&
                 headerCell is IImageLabelButton_Forms headerCellAsImageLabelButton )
            {
               headerCellAsImageLabelButton.ButtonCommand =
                  new Command(
                     () =>
                     {
                        // IMPORTANT Creating a new root because the tap breaks any existing TPL chain.
                        SortAnimatableLayout().ConsiderBeginInvokeTaskOnMainThread( true ).FireAndFuhgetAboutIt();
                     } );

               var canSort = new CanSort();
               canSort.CopySettablePropertyValuesFrom<ICanSort>( keyValuePair.Value );

               // The default sort order is "tapped" because that us how the list arrives initially
               _defaultSortDict.Add( keyValuePair.Key,
                  new CanSortWithTapped { CanSortRec = canSort, HasBeenTapped = canSort.DefaultSortOrder == 0, } );
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------------------
            // PRIVATE METHODS
            // ---------------------------------------------------------------------------------------------------------------------------------------------------
            async Task SortAnimatableLayout()
            {
               SpinnerHost.IsBusyShowing = true;

               try
               {
                  // NOTE Cannot get there without this property/column being in the _defaultSortDict
                  //            But check anyway.
                  if ( !_defaultSortDict.ContainsKey( keyValuePair.Key ) )
                  {
                     // Will never occur
                     return;
                  }

                  // ELSE
                  // Copy the dictionary's ICanSort Value into a variable;
                  //    this will update the dictionary whenever it is changed, since it is a reference type
                  var activeCanSortTapped = _defaultSortDict[ keyValuePair.Key ];

                  // Corner case: failing to set descending actually means that it is false -- it's never unset
                  if ( activeCanSortTapped.CanSortRec.DefaultSortDescending.IsUnset() )
                  {
                     activeCanSortTapped.CanSortRec.DefaultSortDescending =
                        ViewModelCustomAttribute_Static_PI.FALSE_BOOL;
                  }

                  // The user hasn't tapped this header yet, so maintain the current sort descending order
                  if ( !activeCanSortTapped.HasBeenTapped )
                  {
                     activeCanSortTapped.HasBeenTapped = true;
                  }
                  else

                     // The user has tapped the same header again
                     // Reverse the existing sort, whatever that might be
                  {
                     activeCanSortTapped.CanSortRec.DefaultSortDescending =
                        activeCanSortTapped.CanSortRec.DefaultSortDescending.Reverse();
                  }

                  // ELSE the user has tapped a new header; re-use the existing descending setting

                  await SetAnimatableLayoutSortComparer( keyValuePair.Key, activeCanSortTapped.CanSortRec )
                    .WithoutChangingContext();
               }
               finally
               {
                  SpinnerHost.IsBusyShowing = false;
               }
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------------------
         }

         if ( CanAddLines( HasHorizontalGridlines, HorizontalGridlineColor, HorizontalGridlineGirth ) &&
              IncludeGridlineBelowHeader )
         {
            var comboGrid = UIUtils_Forms.GetExpandingGrid();

            comboGrid.AddAutoRow();
            comboGrid.AddAutoRow();

            comboGrid.AddAndSetRowsAndColumns( headerGrid, 0 );

            var horizontalLine = CreateHorizontalLine();
            comboGrid.AddAndSetRowsAndColumns( horizontalLine, 1, 0, 1, comboGrid.ColumnDefinitions.Count );

            _header = comboGrid;
         }
         else
         {
            _header = headerGrid;
         }
      }

      private BoxView CreateHorizontalLine()
      {
         var horizontalLine =
            new BoxView
            {
               BackgroundColor   = Color.Transparent,
               Color             = HorizontalGridlineColor,
               HeightRequest     = HorizontalGridlineGirth,
               HorizontalOptions = LayoutOptions.FillAndExpand,
               VerticalOptions   = LayoutOptions.Center,
               Margin            = HorizontalGridlineMargin,
            };
         return horizontalLine;
      }

      private BoxView CreateVerticalLine()
      {
         var verticalLine =
            new BoxView
            {
               BackgroundColor   = Color.Transparent,
               Color             = VerticalGridlineColor,
               HeightRequest     = VerticalGridlineGirth,
               VerticalOptions   = LayoutOptions.FillAndExpand,
               HorizontalOptions = LayoutOptions.Center,
               Margin            = VerticalGridlineMargin,
            };
         return verticalLine;
      }

      private void EndOfConstruction()
      {
         _isInitializing.SetFalse();
         EndInitialization().FireAndFuhgetAboutIt();
      }

      private async Task HandleRawDataChangedTask( IResponsiveTaskParams paramdict )
      {
         await RequestCreateScrollableViewsAndSetSourceViews().WithoutChangingContext();
      }

      private async Task RequestCreateAnimatableLayoutSourceViewsIfDataIsReady()
      {
         if ( DataIsReady )
         {
            await RequestCreateScrollableViewsAndSetSourceViews().WithoutChangingContext();
         }
      }

      private async Task SetAnimatableLayoutSortComparer( PropertyInfo seedPropInfo = default,
                                                          ICanSort     seedCanSort  = default )
      {
         // Create a comparer to sort the animatable layout (flowable canvas) children.
         // Inject the tapped item as the first member.
         var sortedKeyValuePairList =
            new List<KeyValuePair<PropertyInfo, ICanSort>>();

         if ( seedPropInfo.IsNotNullOrDefault() && seedCanSort.IsNotNullOrDefault() )
         {
            sortedKeyValuePairList.Add(
               new KeyValuePair<PropertyInfo, ICanSort>
               (
                  seedPropInfo, seedCanSort
               )
            );
         }

         // Add all sortable keys (*excluding* the one we already tapped) --- in the 'normal' default order.
         foreach ( var additionalKVP in _defaultSortDict.Where(
            p => ( seedPropInfo.IsNullOrDefault() ||
                   p.Key.Name.IsDifferentThan( seedPropInfo?.Name ) ) &&
                 ( p.Value.CanSortRec.DefaultSortOrder >= 0 ) ).OrderBy( p => p.Value.CanSortRec.DefaultSortOrder ) )
         {
            // Corner case: failing to set descending actually means that it is false -- it's never unset
            if ( additionalKVP.Value.CanSortRec.DefaultSortDescending.IsUnset() )
            {
               additionalKVP.Value.CanSortRec.DefaultSortDescending = ViewModelCustomAttribute_Static_PI.FALSE_BOOL;
            }

            sortedKeyValuePairList.Add(
               new KeyValuePair<PropertyInfo, ICanSort>( additionalKVP.Key, additionalKVP.Value.CanSortRec ) );
         }

         var comparer = new CanSortComparer( sortedKeyValuePairList.ToArray() );

         await AnimatableLayout.SetSortComparer( comparer ).WithoutChangingContext();
      }

      private interface ICanSortWithTapped
      {
         ICanSort CanSortRec    { get; set; }
         bool     HasBeenTapped { get; set; }
      }

      private class CanSortWithTapped : ICanSortWithTapped
      {
         public ICanSort CanSortRec    { get; set; }
         public bool     HasBeenTapped { get; set; }
      }
   }
}