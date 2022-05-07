// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=FlowableCollectionCanvas_Forms.cs
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

#define USE_FADES

#define SET_STATUS_ON_THREAD

// #define SHOW_BACK_COLORS
/*
 DOING NOTHING FOR ANDROID WIDTH ISSUES
#define USE_WIDTH_TIMER
#define ADD_CHILD_ON_MAIN_THREAD
*/

namespace Com.MarcusTS.UI.XamForms.Views.Controls
{
   using System;
   using System.Collections.Generic;
   using System.Collections.Specialized;
   using System.ComponentModel;
   using System.Linq;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Controls;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Subviews;
   using Xamarin.Essentials;
   using Xamarin.Forms;

#if USE_WIDTH_TIMER
   using System.Threading;
#endif

   public interface IFlowableCollectionCanvas_Forms :
      INotifyAfterAfterRequestingChildItemFlows,
      IVisibleShapeView_Forms,
      ICanAnimate_Forms,
      IProvideChildFlowGuidance_Forms
   {
      double ChildHeightWithoutScrollBottomMargin { get; }

      IResponsiveTasks ChildHeightWithoutScrollBottomMarginChangeTask { get; }

      // Not critically responsive, so allowing the public setter
      int MillisecondsBeforeFilterChanges { get; set; }

      IResponsiveTasks OnCanvasChildAddedTask { get; set; }

      IResponsiveTasks OnRowTappedTask { get; set; }

      ScrollView               ParentScroller { get; set; }
      IList<ICanBeSelected_PI> SelectedRows   { get; set; }

      Task SetAnimateRowTaps( bool animatRowTaps );

      Task SetChildSpacing( double spacing );

      Task SetDoNotListenForRowTaps( bool doNotListen );

      Task SetFilterCollection( ICanvasFilterCollection_Forms filters );

      Task SetFixedRowHeight( double? height );

      Task SetHeightBasedOnChildren( bool setFullHeightBasedOnChildren );

      Task SetIncludeHapticFeedbackOnRowTaps( bool includeHapticFeedbackOnRowTaps );

      Task SetRowHeightCalculationMethod( RowHeightCalculationMethods method );

      Task SetRowSelectionRule( SelectionRules rule );

      Task SetScrollBottomMargin( double scrollBottomMargin );

      Task SetSortComparer( ICanSortComparer comparer );
   }

   public class FlowableCollectionCanvas_Forms : VisibleShapeView_Forms, IFlowableCollectionCanvas_Forms
   {
      private const    int                    POST_SORT_DELAY_MILLISECONDS             = 250;
      private readonly IThreadSafeAccessor    _initialChildPositionStatusesHaveBeenSet = new ThreadSafeAccessor( 0 );
      private readonly IThreadSafeAccessor    _setChildPositionStatuses_Entered        = new ThreadSafeAccessor( 0 );
      private readonly IThreadSafeAccessor    _setSourceViews_Entered                  = new ThreadSafeAccessor( 0 );
      private readonly IThreadSafeAccessor    _sortingIsPossible                       = new ThreadSafeAccessor( 0 );
      private readonly IThreadSafeAccessor    _sourceViewsAreDirty                     = new ThreadSafeAccessor( 0 );
      private readonly IThreadSafeAccessor    _widthPropertyChangedEntered             = new ThreadSafeAccessor( 0 );
      private          bool                   _animateRowTaps                          = true;
      private          double                 _childHeightWithoutScrollBottomMargin;
      private          bool                   _doNotListenForRowTaps;
      private          bool                   _includeHapticFeedbackOnRowTaps = true;
      private          double                 _lastItemCanvasWidth;
      private          SelectionRules         _rowSelectionRule;
      private          double                 _scrollBottomMargin;
      private          bool                   _setFullHeightBasedOnChildren = true;
      private          ICanSortComparer       _sortComparer;
      private          IFlowableChild_Forms[] _sortedFlowableChildren;

#if USE_WIDTH_TIMER
      private const int                 WIDTH_TIMER_DELAY_MILLISECONDS = 250;
      private       Timer _lastItemCanvasWidthChangeTimer;
#endif

      public FlowableCollectionCanvas_Forms()
      {
         HorizontalOptions = LayoutOptions.FillAndExpand;

         // WARNING DO NOT CHANGE
          VerticalOptions = LayoutOptions.FillAndExpand;

         IsClippedToBounds = true;
         Margin            = 0;
         Padding           = 0;

#if SHOW_BACK_COLORS
         ItemsCanvas.BackgroundColor = Color.Orchid;
         FillColor                   = Color.Yellow;
         // BackgroundColor             = Color.Yellow;
#endif

         SourceViews = new BetterObservableCollection<View>();

         // Will not fire during the collection change that just occurred above.
         ( (BetterObservableCollection<View>)SourceViews ).CollectionChanged +=
            HandleSourceViewsCollectionChanged;

         ItemsCanvas.PropertyChanged += HandleWidthChanged;


         // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // PRIVATE METHODS
         // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         void HandleWidthChanged( object sender, PropertyChangedEventArgs args )
         {
            if ( ItemsCanvas.Width.IsValidAndHasChanged( nameof( Width ), _lastItemCanvasWidth ) )
            {
               //if ( _widthPropertyChangedEntered.IsTrue() )
               //{
               //   _mustReenterWidthPropertyChanged.SetTrue();
               //   return;
               //}

               _widthPropertyChangedEntered.SetTrue();

#if USE_WIDTH_TIMER
               _lastItemCanvasWidthChangeTimer =
                  new Timer(
                     state =>
                     {
#endif

               // Corner case; resets all children.  This occurs when we start from an invalid width such as -1, so never get set up properly.
               // This sometimes occurs while inside of set child position statuses.
               if ( IsVisibleToUser && _initialChildPositionStatusesHaveBeenSet.IsFalse() && _setChildPositionStatuses_Entered.IsFalse() )
               {
                  // await SetChildPositionStatuses( true ).ConfigureAwait(true);
                  SetChildPositionStatuses().FireAndFuhgetAboutIt();
               }
               else
               {
                  RequestMassChildChangesInParallel( FlowableChildChanges.CanvasWidthChange );
               }

               _lastItemCanvasWidth = ItemsCanvas.Width;
               _widthPropertyChangedEntered.SetFalse();

               //if ( _mustReenterWidthPropertyChanged.IsTrue() )
               //{
               //   _mustReenterWidthPropertyChanged.SetFalse();
               //   HandleWidthChanged( sender, args );
               //}

#if USE_WIDTH_TIMER
                        _lastItemCanvasWidthChangeTimer.Dispose();
                     },
                     null,
                     TimeSpan.FromMilliseconds( WIDTH_TIMER_DELAY_MILLISECONDS ),
                     Timeout.InfiniteTimeSpan );
#endif
            }
         }
         // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

         this.SetContentSafelyAndAwaitAllBranchingTasks( ItemsCanvas ).FireAndFuhgetAboutIt();
      }

      public IResponsiveTasks AfterFlowingToOnScreenPositionsTask { get; set; } = new ResponsiveTasks();

      public int AnimateInDelayMilliseconds { get; set; } =
         FlowableConst_PI.MILLISECOND_DELAY_BETWEEN_ANIMATION_TRANSITIONS;

      public bool AutoReloadOnAnySourceViewChange { get; set; }

      public bool CascadeChildAnimations { get; set; }

      public double ChildHeightWithoutScrollBottomMargin
      {
         get => _childHeightWithoutScrollBottomMargin;
         private set
         {
            if ( _childHeightWithoutScrollBottomMargin.IsDifferentThan( value ) )
            {
               _childHeightWithoutScrollBottomMargin = value;
               ChildHeightWithoutScrollBottomMarginChangeTask.RunAllTasksUsingDefaults(
                  _childHeightWithoutScrollBottomMargin );
            }
         }
      }

      public IResponsiveTasks ChildHeightWithoutScrollBottomMarginChangeTask { get; } = new ResponsiveTasks( 1 );

      public double ChildSpacing { get; private set; }

      public ICanvasFilterCollection_Forms FilterCollection { get; private set; }

      public double? FixedRowHeight { get; private set; }

      // Instantiates to defaults
      public IFlowableSettings_Forms FlowSettings { get; set; } = new FlowableSettings_Forms();

      public AbsoluteLayout ItemsCanvas { get; } = UIUtils_Forms.GetExpandingAbsoluteLayout();

      public int MillisecondsBeforeFilterChanges { get; set; } = FlowableConst_PI.MILLISECONDS_BEFORE_FILTER_CHANGES;

      public IResponsiveTasks OnCanvasChildAddedTask { get; set; } = new ResponsiveTasks( 1 );

      public IResponsiveTasks OnRowTappedTask { get; set; } = new ResponsiveTasks( 1 );

      public ScrollView ParentScroller { get; set; }

      // 1. Broadcast type
      // 2. Any sort of data
      public IResponsiveTasks RequestChildChangeTask { get; set; } = new ResponsiveTasks( 1 );

      public RowHeightCalculationMethods RowHeightCalculationMethod { get; private set; } =
         RowHeightCalculationMethods.AutoCalc;

      public IList<ICanBeSelected_PI> SelectedRows { get; set; } = new List<ICanBeSelected_PI>();

      public IBetterObservableCollection<View> SourceViews { get; }

      public async Task AnimateIn()
      {
         await SetIsVisibleToUser( true, true ).ConfigureAwait(true);
      }

      public void CompareParentHeightToChildHeight( double childHeight )
      {
         childHeight += Padding.VerticalThickness;

         if ( childHeight.IsDifferentThan( ChildHeightWithoutScrollBottomMargin ) )
         {
            ThreadHelper.ConsiderBeginInvokeActionOnMainThread(
               () => { ChildHeightWithoutScrollBottomMargin = childHeight; } );
         }

         if ( _setFullHeightBasedOnChildren && ( childHeight + _scrollBottomMargin ).IsDifferentThan( Height ) )
         {
            ThreadHelper.ConsiderBeginInvokeActionOnMainThread(
               () => { HeightRequest = childHeight + _scrollBottomMargin; } );
         }
      }

      public Task SetAnimateRowTaps( bool animateRowTaps )
      {
         _animateRowTaps = animateRowTaps;

         return Task.CompletedTask;
      }

      public async Task SetChildSpacing( double spacing )
      {
         if ( ChildSpacing.IsDifferentThan( spacing ) )
         {
            ChildSpacing = spacing;

            await ConsiderResetSourceViewsAndPositionStatuses().ConfigureAwait(true);
         }
      }

      public Task SetDoNotListenForRowTaps( bool doNotListen )
      {
         _doNotListenForRowTaps = doNotListen;

         return Task.CompletedTask;
      }

      public async Task SetFilterCollection( ICanvasFilterCollection_Forms filters )
      {
         if ( FilterCollection.IsNotNullOrDefault() && FilterCollection.Filters.IsNotAnEmptyList() )
         {
            FilterCollection.FilterCollectionChangedTask.RemoveIfThere( this,
               HandleFilterCollectionChanged );
         }

         FilterCollection = filters;

         if ( FilterCollection.Filters.IsNotAnEmptyList() )
         {
            FilterCollection.FilterCollectionChangedTask.AddIfNotAlreadyThere( this,
               HandleFilterCollectionChanged );

            await AskAllChildrenToRefilter().ConfigureAwait(true);
         }
      }

      public async Task SetFixedRowHeight( double? height )
      {
         if ( FixedRowHeight.GetValueOrDefault().IsDifferentThan( height.GetValueOrDefault() ) )
         {
            FixedRowHeight = height;

            await ConsiderResetSourceViewsAndPositionStatuses().ConfigureAwait(true);
         }
      }

      public async Task SetHeightBasedOnChildren( bool setFullHeightBasedOnChildren )
      {
         _setFullHeightBasedOnChildren = setFullHeightBasedOnChildren;

         await ForceResetChildPositionStatuses().ConfigureAwait(true);
      }

      public Task SetIncludeHapticFeedbackOnRowTaps( bool includeHapticFeedbackOnRowTaps )
      {
         _includeHapticFeedbackOnRowTaps = includeHapticFeedbackOnRowTaps;

         return Task.CompletedTask;
      }

      public async Task SetRowHeightCalculationMethod( RowHeightCalculationMethods method )
      {
         if ( RowHeightCalculationMethod != method )
         {
            RowHeightCalculationMethod = method;

            await ConsiderResetSourceViewsAndPositionStatuses().ConfigureAwait(true);
         }
      }

      public async Task SetRowSelectionRule( SelectionRules rule )
      {
         if ( _rowSelectionRule != rule )
         {
            _rowSelectionRule = rule;

            await ConfirmLegalSelections().ConfigureAwait(true);
         }
      }

      public async Task SetScrollBottomMargin( double scrollBottomMargin )
      {
         _scrollBottomMargin = scrollBottomMargin;

         await ForceResetChildPositionStatuses().ConfigureAwait(true);
      }

      public async Task SetSortComparer( ICanSortComparer comparer )
      {
         _sortComparer = comparer;

         await SortWithComparer().ConfigureAwait(true);
      }

      public async Task SetSourceViews( View[] views )
      {
         if ( _setSourceViews_Entered.IsTrue() )
         {
            return;
         }

         _setSourceViews_Entered.SetTrue();

         // Handle this early; can't sort if we can't build a list
         _sortingIsPossible.SetFalse();

         SourceViews.ClearWithoutNotification();

         ItemsCanvas.Children.Clear();

         if ( BindingContext.IsNullOrDefault() )
         {
            // REQUIRED
            _setSourceViews_Entered.SetFalse();
            return;
         }

         // Can set early
         _sourceViewsAreDirty.SetFalse();

         if ( views.IsAnEmptyList() )
         {
            // REQUIRED
            _setSourceViews_Entered.SetFalse();
            return;
         }

         // ELSE
         SourceViews.AddRangeSortedAndWithoutNotification( views );

         var lastBindingContextType = default( Type );
         if ( SourceViews.Count > 1 )
         {
            // Initially only -- needs to be verified
            _sortingIsPossible.SetTrue();
         }

         foreach ( var newView in SourceViews )
         {
            await AddFlowableChild( newView ).ConsiderBeginInvokeTaskOnMainThread(

#if ADD_CHILD_ON_MAIN_THREAD
               true
#else

               // ReSharper disable once RedundantArgumentDefaultValue
               false
#endif
            );
         }

         _sortedFlowableChildren = ItemsCanvas.Children.OfType<IFlowableChild_Forms>().ToArray();

         // Sort by the last key(s)
         await SortWithComparer().ConfigureAwait(true);

         // REQUIRES sort to occur first
         // If it is the first row, it might need to be auto-selected.
         await ConfirmLegalSelections().ConfigureAwait(true);

         _setSourceViews_Entered.SetFalse();


         // ----------------------------------------------------------------------------------------------------------------------------------------------------------
         // P R I V A T E   M E T H O D S
         // ----------------------------------------------------------------------------------------------------------------------------------------------------------
         async Task AddFlowableChild( View newView )
         {
            // Cannot sort if no type found, or the types change, etc.
            if ( newView.BindingContext.IsNullOrDefault() || newView.BindingContext.GetType().IsNullOrDefault() )
            {
               _sortingIsPossible.SetFalse();
            }
            else if ( lastBindingContextType.IsNotNullOrDefault() &&
                      ( newView.BindingContext.GetType() != lastBindingContextType ) )
            {
               _sortingIsPossible.SetFalse();
            }

            // Corner case -- the view should always have a binding context -- even if it is just this class's.
            if ( newView.BindingContext.IsNullOrDefault() )
            {
               // This class's BindingContext was confirmed on entry to this method.
               await newView.SetBindingContextSafelyAndAwaitAllBranchingTasks_Forms( BindingContext )
                            .ConfigureAwait(true);
            }

            // Unconditional
            lastBindingContextType = newView.BindingContext.GetType();

            // Need to create a new flowable view host as the view's parent container
            // The position status starts out as UNSET
            IFlowableChild_Forms flowableChild = new FlowableChild_Forms( this )
                                                 {
#if USE_FADES
                                                    Opacity = UIConst_PI.NOT_VISIBLE_OPACITY
#else
                                                    Opacity = UIConst_PI.VISIBLE_OPACITY,
#endif
                                                 };

            RequestChildChangeTask.AddIfNotAlreadyThere( flowableChild, flowableChild.HandleRequestChildChangeTask );

            var flowableChildView = (View)flowableChild;

            // IMPORTANT This might accidentally over-write the binding context
            ItemsCanvas.Children.Add( flowableChildView );

            // Important The flowable child needs the newView's binding context, which is backwards -- turn this feature off.
            flowableChild.SetContentBindingContextAfterAssignment = false;

            // NEW 2021-10-21  0137 PST
            await flowableChild.SetContentSafelyAndAwaitAllBranchingTasks( newView )
                               .ConsiderBeginInvokeTaskOnMainThread().ConfigureAwait(true);

            if ( newView.BindingContext.IsNotNullOrDefault() )
            {
               await flowableChild.SetBindingContextSafelyAndAwaitAllBranchingTasks( newView.BindingContext )
                                  .ConfigureAwait(true);
            }

            await OnCanvasChildAddedTask.RunAllTasksUsingDefaults( flowableChild );

            // Tap listeners -- leave in place regardless of row selection so if the user changes CanSelectRows,
            // this always works.
            var rowTapListener = CreateRowTapListener( flowableChild );
            flowableChildView.GestureRecognizers.Add( rowTapListener );

            // IMPORTANT This is the only place where we can initiate the child width timer.
            //                      The parent canvas width only changes once, which calls this method (once).
            flowableChild.SetChildWidthBasedOnParentCanvas();
         }
         // ----------------------------------------------------------------------------------------------------------------------------------------------------------
      }

      protected async Task AddToRowSelections( ICanBeSelected_PI selectable )
      {
         await selectable.SetIsSelected( true ).ConfigureAwait(true);

         if ( !SelectedRows.Contains( selectable ) )
         {
            SelectedRows.Add( selectable );
         }
      }

      protected virtual async Task ConfirmLegalSelections()
      {
         // This will check for the conditions required to select the initial row.
         await MakeInitialRowSelection().ConfigureAwait(true);
      }

      protected async Task DeselectAndClearRowSelections( ICanBeSelected_PI exceptedItem )
      {
         if ( SelectedRows.IsNotEmpty() )
         {
            foreach ( var row in SelectedRows.Except( new[] { exceptedItem, } ).ToArray() )
            {
               await row.SetIsSelected( false ).ConfigureAwait(true);
            }

            SelectedRows.Clear();
         }
      }

      protected async Task MakeInitialRowSelection()
      {
         if ( _rowSelectionRule.MustHaveAtLeastOneSelection() &&
              _sortedFlowableChildren.IsNotAnEmptyList()      &&
              SelectedRows.IsEmpty()
         )
         {
            await AddToRowSelections( _sortedFlowableChildren[ 0 ] ).ConfigureAwait(true);
         }
      }

      protected override async Task OnSetIsVisibleToUser( bool isVisible )
      {
         await base.OnSetIsVisibleToUser( isVisible ).ConfigureAwait(true);

         if ( isVisible )
         {
            await ConsiderResetSourceViewsAndPositionStatuses().ConfigureAwait(true);
         }
      }

      protected async Task RemoveFromRowSelections( ICanBeSelected_PI selectable )
      {
         await selectable.SetIsSelected( false ).ConfigureAwait(true);

         if ( SelectedRows.Contains( selectable ) )
         {
            SelectedRows.Remove( selectable );
         }
      }

      /// <summary>
      /// Called once, for utility and simplicity only
      /// </summary>
      protected
#if !SET_STATUS_ON_THREAD
         async
#endif
         Task<bool> SetChildPositionStatuses()
      {
         // Wait for visibility to do this
         if (
            _setChildPositionStatuses_Entered.IsTrue()
          ||
            !IsVisibleToUser
          ||
            _sortedFlowableChildren.IsAnEmptyList()
          ||
            !ItemsCanvas.Width.IsANumberGreaterThanZero()
         )
         {
#if SET_STATUS_ON_THREAD
            return Task.FromResult(false);
#else
            return false;
#endif
         }


#if SET_STATUS_ON_THREAD
         MainThread.BeginInvokeOnMainThread(
            async
               () =>
            {
#endif

         // ELSE
         _setChildPositionStatuses_Entered.SetTrue();

         // --------------------------------------------------------------------------------------------------------------------------------------------------
         // Reset the flow predecessors initially
         // --------------------------------------------------------------------------------------------------------------------------------------------------
         // This is a fake record that we use to fire a new bounds rearrangement from the start to the end.
         IBroadcastSuccessorMustReconsiderBounds_Forms previousChild = default;

         double maxChildHeight   = 0;
         var nextDisplayOrder = 0;

         foreach ( var flowableChild in _sortedFlowableChildren )
         {
            flowableChild.DisplayOrder = nextDisplayOrder++;

            if ( _initialChildPositionStatusesHaveBeenSet.IsFalse() )
            {
               // Set the predecessor
               await flowableChild.SetFlowPredecessor( previousChild ).ConfigureAwait(true);
            }

            // Even if filtered out, we need to know our next Y, etc.
            // Broadcast here if the list exists;
            //    Force broadcast if we have been *not* been asked to change predecessors.  This is because if managePredecessors is true, we have no reliable successors (yet).
            await flowableChild
                 .RefreshNextBoundsAndAlternationFromPredecessor( _initialChildPositionStatusesHaveBeenSet.IsTrue(),
                     _initialChildPositionStatusesHaveBeenSet.IsTrue() ).ConfigureAwait(true);

            var isFilteredOut =
               FilterCollection.IsNotNullOrDefault()       &&
               FilterCollection.Filters.IsNotAnEmptyList() &&

               // FilterCollection.Filters.Any( f => !f.IsSelected && f.AppliesToFlowableItem( BindingContext ) );
               // WARNING BindingContext being is generally for a specific filter.
               //                 Pass the flowable child's binding context instead?
               FilterCollection.Filters.Any(
                  f => f.AppliesToFlowableItem( ( (View)flowableChild ).BindingContext ?? BindingContext ) );

            // The child will check to see if this is an actual change or not
            // This *overrides* the next bounds set in the step above
            await flowableChild.SetNextFilteredOut( isFilteredOut ).ConfigureAwait(true);

            var childHeight =
               flowableChild.NextBounds.Y +
               flowableChild.NextBounds.Height;

            if ( childHeight.IsGreaterThan( maxChildHeight ) )
            {
               maxChildHeight = childHeight;
            }

            previousChild = flowableChild;
         }

         if ( maxChildHeight.IsANumberGreaterThanZero() )
         {
            CompareParentHeightToChildHeight( maxChildHeight );
         }

         // If true, the items are already flowng in as expected
         if ( _initialChildPositionStatusesHaveBeenSet.IsFalse() )
         {
            // Animate on-screen items based on their sort order
            var orderedChildren = _sortedFlowableChildren.OrderBy( i => i.DisplayOrder ).ToArray();

            foreach ( var orderedChild in orderedChildren )
            {
               if ( AnimateInDelayMilliseconds > 0 )
               {
                  await Task.Delay( AnimateInDelayMilliseconds ).ConfigureAwait(true);
               }

               await orderedChild.MakeAllPendingChanges().ConfigureAwait(true);

               // NOTE Would require an animatable child inside of this canvas (not likely)
               if ( CascadeChildAnimations && orderedChild is ICanAnimate_Forms childAsAnimatable )
               {
                  await childAsAnimatable.AnimateIn().ConfigureAwait(true);
               }
            }

            _initialChildPositionStatusesHaveBeenSet.SetTrue();
         }

         // NOTE: This task occures *before* the children complete their flows, since those are threaded in parallel
         await AfterFlowingToOnScreenPositionsTask.RunAllTasksUsingDefaults().ConfigureAwait(true);

         _setChildPositionStatuses_Entered.SetFalse();

         // HACK For Android latency
         if ( !DeviceUtils_PI.IsIos() )
         {
            RequestMassChildChangesInParallel( FlowableChildChanges.CanvasWidthChange );
         }

#if SET_STATUS_ON_THREAD
            });
#endif

#if SET_STATUS_ON_THREAD
         return Task.FromResult(true);
#else
         return true;
#endif
      }

      private async Task AskAllChildrenToRefilter()
      {
         // This method now handles filtering, but the predecessors are already set.
         await SetChildPositionStatuses();
      }

      private async Task ConsiderResetSourceViewsAndPositionStatuses()
      {
         if ( _sourceViewsAreDirty.IsTrue() )
         {
            await ResetExistingSourceViews().ConfigureAwait(true);
         }
         else
         {
            // Leave the predecessors alone
            await SetChildPositionStatuses().ConfigureAwait(true);
         }
      }

      private TapGestureRecognizer CreateRowTapListener( IFlowableChild_Forms child )
      {
         var rowTapListener = new TapGestureRecognizer();

         rowTapListener.Tapped +=
            ( sender, args ) =>
            {
               if ( _doNotListenForRowTaps )
               {
                  return;
               }

               // ELSE
               MainThread.BeginInvokeOnMainThread(

                  // ReSharper disable once AsyncVoidLambda
                  async
                     () =>
                  {
                     await ( (View)child )
                          .AddAnimationAndHapticFeedback( _animateRowTaps, _includeHapticFeedbackOnRowTaps )
                          .ConfigureAwait(true);

                     // Can't deselect the only selected item (if these rules are set)
                     // Unless the item cannot be selected at all.
                     if ( _rowSelectionRule != SelectionRules.NoSelection )
                     {
                        if ( ( child.IsSelected  && RowCannotBeDeselected( child ) ) ||
                             ( !child.IsSelected && RowCannotBeSelected() ) )
                        {
                           await RemoveFromRowSelections( child ).ConfigureAwait(true);
                           return;
                        }

                        await child.SetIsSelected( !child.IsSelected ).ConfigureAwait(true);

                        // Corner case: if only one selection is allowed, we must deselect all other selections.
                        if ( ( _rowSelectionRule ==
                               SelectionRules.SingleSelectionAtLeastOneRequired ) ||
                             ( _rowSelectionRule == SelectionRules.SingleSelectionCanBeNull ) )
                        {
                           await DeselectAndClearRowSelections( child ).ConfigureAwait(true);
                        }

                        await AddToRowSelections( child ).ConfigureAwait(true);
                     }
                     else
                     {
                        await RemoveFromRowSelections( child ).ConfigureAwait(true);

                        // If we cannot select, we allow the item to handle its own on tapped event.
                        if ( _rowSelectionRule == SelectionRules.NoSelection )

                           // Send this through to any deriver
                        {
                           await child.OnItemTapped().ConfigureAwait(true);
                        }
                     }

                     await OnRowTappedTask.RunAllTasksUsingDefaults( child ).ConfigureAwait(true);
                  }
               );
            };

         return rowTapListener;
      }

      private async Task ForceResetChildPositionStatuses()
      {
         _initialChildPositionStatusesHaveBeenSet.SetFalse();

         // Require predecessors to be reset
         await SetChildPositionStatuses().ConfigureAwait(true);
      }

      private async Task HandleFilterCollectionChanged( IResponsiveTaskParams paramDict )
      {
         // Params are ignored
         if ( MillisecondsBeforeFilterChanges > 0 )
         {
            await Task.Delay( MillisecondsBeforeFilterChanges ).ConfigureAwait(true);
         }

         await AskAllChildrenToRefilter().ConfigureAwait(true);
      }

      private void HandleSourceViewsCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
      {
         if ( AutoReloadOnAnySourceViewChange )
         {
            ResetExistingSourceViews().FireAndFuhgetAboutIt();
         }
         else
         {
            _sourceViewsAreDirty.SetTrue();
         }
      }

      private void RequestMassChildChangesInParallel( FlowableChildChanges change )
      {
         RequestChildChangeTask.RunAllTasksInParallelFromVoid( change );
      }

      private async Task ResetExistingSourceViews()
      {
         await SetSourceViews( SourceViews.ToArray() ).ConfigureAwait(true);
      }

      private bool RowCannotBeDeselected( ICanBeSelected_PI item )
      {
         return
            _rowSelectionRule.MustHaveAtLeastOneSelection()
          &&
            ( SelectedRows?.Count == 1 )
          &&
            item.IsSelected;
      }

      private bool RowCannotBeSelected()
      {
         return _rowSelectionRule == SelectionRules.NoSelection;
      }

      /// <summary>
      /// Sorts using the curent comparer.
      /// *Always* sorts, even if the comparer has not changed -- we can't determine its state except that it is not default.
      /// </summary>
      /// <returns></returns>
      private async Task SortWithComparer()
      {
         if ( _sortedFlowableChildren.IsAnEmptyList() )
         {
            return;
         }

         if ( _sortingIsPossible.IsFalse() || _sortComparer.IsNullOrDefault() || !_sortComparer.IsReady )
         {
            return;
         }

         // ELSE
         _sortedFlowableChildren =
            _sortedFlowableChildren.OrderBy(
               o =>

                  // CRITICAL to pass the content's binding context below, as this is the variable that is described by our custom attributes.
                  o?.Content?.BindingContext,
               _sortComparer
            ).ToArray();

         await Task.Delay( POST_SORT_DELAY_MILLISECONDS );

         // Unconditionally rearrange the children *fresh* -- ignore their current positions
         await ForceResetChildPositionStatuses().ConfigureAwait(true);
      }
   }
}