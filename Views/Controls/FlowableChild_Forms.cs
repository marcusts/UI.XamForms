// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=FlowableChild_Forms.cs
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

/*
DOING  NOTHING FOR ANDROID
// #define LOG_WIDTH_CHANGES
// #define LOG_WIDTH_CHANGES
// #define SET_CANVAS_WIDTH_ON_MAIN_THREAD
*/

namespace Com.MarcusTS.UI.XamForms.Views.Controls
{
   using System;
   using System.ComponentModel;
   using System.Diagnostics;
   using System.Threading;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Subviews;
   using Xamarin.Forms;

   public interface IFlowableChild_Forms :
      IShapeView_Forms,
      IBroadcastSuccessorMustReconsiderBounds_Forms,
      ICanBeSelected_PI,
      IHaveFlowableSettings
   {
      IBroadcastSuccessorMustReconsiderBounds_Forms FlowPredecessor { get; }

      int DisplayOrder { get; set; }

      Task HandleRequestChildChangeTask( IResponsiveTaskParams paramDict );

      Task MakeAllPendingChanges();

      Task OnItemTapped();

      Task RefreshNextBoundsAndAlternationFromPredecessor( bool allowChangeNotification,
                                                           bool forceChangeNotification );

      void SetChildWidthBasedOnParentCanvas();

      Task SetFlowPredecessor( IBroadcastSuccessorMustReconsiderBounds_Forms predecessor );

      Task SetIsAnAlternate( bool isAnAlternate );

      Task SetNextBoundsAndMakeAllPendingChanges( Rect rect );

      Task SetNextFilteredOut( bool isFilteredOut );

      Task SetRowHeightOverride( double? heightOverride );
   }

   public class FlowableChild_Forms :
      ShapeView_Forms,
      IFlowableChild_Forms
   {
      private static readonly int MAX_TICKS = 15;
      private static readonly int FORCE_WIDTH_CHANGE_TIMER_DELAY_MILLISECONDS = 75;
      private readonly        IProvideChildFlowGuidance_Forms _flowParent;
      private readonly        IThreadSafeAccessor _heightPropertyChangedEntered = new ThreadSafeAccessor();
      private readonly        IThreadSafeAccessor _mustReenterHeightPropertyChanged = new ThreadSafeAccessor();
      private                 Timer _forceLastWidthChangeTimer;

      private          double? _rowHeightOverride;
      private volatile int     _tickCount;

      public FlowableChild_Forms( IProvideChildFlowGuidance_Forms flowParent )
      {
         _flowParent  = flowParent;
         CornerRadius = 0;
         SetCornerRadiusFactor( 0 ).FireAndFuhgetAboutIt();

#if SHOW_BACK_COLORS
         FillColor       = Color.Lime;
#endif

         PropertyChanged += HandleHeightChanged;

         // -----------------------------------------------------------------------------------------------------------------------------------
         // PRIVATE METHODS
         // -----------------------------------------------------------------------------------------------------------------------------------
         void HandleHeightChanged( object sender, PropertyChangedEventArgs args )
         {
            // On any bounds change, ask to re-assess the child's size
            if ( IsFilteredOut || ( DisplayOrder < 0 ) )
            {
               return;
            }

            // ELSE
            var resetHeight =
               ( _flowParent.RowHeightCalculationMethod == RowHeightCalculationMethods.AutoCalc ) &&
               !_rowHeightOverride.IsANumberGreaterThanZero()                                     &&
               args.PropertyName.IsSameAs( nameof( Height ) )                                     &&
               Height.IsDifferentThan( NextBounds.Height );

            if ( !resetHeight )
            {
               return;
            }

            // ELSE
            if ( _heightPropertyChangedEntered.IsTrue() )
            {
               _mustReenterHeightPropertyChanged.SetTrue();
               return;
            }

            _heightPropertyChangedEntered.SetTrue();

            var nextBounds = NextBounds;

            // HACK
            nextBounds.Height = Bounds.Height;

            // Set next bounds and flow
            SetNextBoundsAndMakeAllPendingChanges( nextBounds ).FireAndFuhgetAboutIt();

            // Notify always
            ConsiderNotifyingAboutChanges( true, true, true ).FireAndFuhgetAboutIt();

            // See if this height change might affect the parent canvas
            _flowParent.CompareParentHeightToChildHeight( NextBounds.Y + NextBounds.Height );

            _heightPropertyChangedEntered.SetFalse();

            if ( _mustReenterHeightPropertyChanged.IsTrue() )
            {
               _mustReenterHeightPropertyChanged.SetFalse();
               HandleHeightChanged( sender, args );
            }
         }
         // -----------------------------------------------------------------------------------------------------------------------------------
      }

      public IBroadcastSuccessorMustReconsiderBounds_Forms FlowPredecessor { get; private set; }

      public IFlowableSettings_Forms FlowSettings { get; set; }

      public bool IsAnAlternate { get; private set; }

      public bool IsFilteredOut { get; private set; }

      public bool IsSelected { get; private set; }

      // 1. this class
      public IResponsiveTasks IsSelectedChangedTask { get; set; } = new ResponsiveTasks( 1 );

      public bool IsTryingToBeSelected { get; set; }

      public Rectangle NextBounds { get; private set; }

      public double NextOpacity  { get; private set; }
      public int    DisplayOrder { get; set; } = -1;

      public IResponsiveTasks SuccessorMustReconsiderBoundsTask { get; set; } = new ResponsiveTasks( 1 );

      public bool CanSelectionBeMade( bool isSelected )
      {
         return true;
      }

      public Task HandleRequestChildChangeTask( IResponsiveTaskParams paramDict )
      {
         var changeType = paramDict.GetTypeSafeValue<FlowableChildChanges>( 0 );

         switch ( changeType )
         {
            case FlowableChildChanges.CanvasWidthChange:
               SetChildWidthBasedOnParentCanvas();

               break;

            default:
               Debug.WriteLine( nameof( HandleRequestChildChangeTask ) + ": ERROR: invalid enum." );
               break;
         }

         return Task.CompletedTask;
      }

      public async Task MakeAllPendingChanges()
      {
         // IMPORTANT  No longer optional
         await FlowToNext().ConsiderBeginInvokeTaskOnMainThread( true ).ConfigureAwait(true);

         // PRIVATE METHODS
         async Task FlowToNext()
         {
            // Moves slowly; opacity is set to visible
            await this.FadeInOrOutAndTranslateXYLoc
                       (
                          NextBounds,
                          FlowSettings.IsNotNullOrDefault()
                             ? FlowSettings.TranslateBoundsMilliseconds
                             : _flowParent.FlowSettings.TranslateBoundsMilliseconds,
                          FlowSettings.IsNotNullOrDefault()
                             ? FlowSettings.TranslateBoundsEasing
                             : _flowParent.FlowSettings.TranslateBoundsEasing,
                          FlowSettings.IsNotNullOrDefault()
                             ? FlowSettings.FadeInMilliseconds
                             : _flowParent.FlowSettings.FadeInMilliseconds,
                          FlowSettings.IsNotNullOrDefault()
                             ? FlowSettings.FadeInEasing
                             : _flowParent.FlowSettings.FadeInEasing,
                          NextOpacity
                       )
                      .ConfigureAwait(true);
         }
      }

      public Task OnItemTapped()
      {
         return Task.CompletedTask;
      }

      public virtual async Task RefreshNextBoundsAndAlternationFromPredecessor(
         bool allowChangeNotification, bool forceChangeNotification )
      {
         // Find the first predecessor that is not filtered out
         var realPredecessor = VerifyThatPredecessorIsNotFilteredOut( FlowPredecessor );

         var predecessorIsValid = realPredecessor.IsNotNullOrDefault();

         await SetIsAnAlternate( predecessorIsValid && !realPredecessor.IsAnAlternate ).ConfigureAwait(true);

         var newBounds =
            new Rectangle
            {
               Height = GetRowHeight(),
               Width  = _flowParent.ItemsCanvas.Width,
               X      = predecessorIsValid ? realPredecessor.NextBounds.X : 0,
               Y      = predecessorIsValid ? realPredecessor.NextBounds.Y + realPredecessor.NextBounds.Height : 0,
            };

         // No spacing "above" the first item
         if ( predecessorIsValid && _flowParent.ChildSpacing.IsNotEmpty() )
         {
            newBounds.Y += _flowParent.ChildSpacing;
         }

         var isChanged = false;

         // For assignment, requiring either a width or a height
         if ( NextBounds.IsDifferentThan( newBounds ) || Bounds.IsDifferentThan( newBounds ) )
         {
            await SetNextBoundsAndMakeAllPendingChanges( newBounds ).ConfigureAwait(true);

            isChanged = true;
         }

         await ConsiderNotifyingAboutChanges( allowChangeNotification, forceChangeNotification, isChanged );
      }

      public void SetChildWidthBasedOnParentCanvas()
      {
         // Unconditional
         StartForceLastWidthChangeTimer();
      }

      public Task SetFlowPredecessor( IBroadcastSuccessorMustReconsiderBounds_Forms predecessor )
      {
         if ( predecessor.IsAnEqualReferenceTo( FlowPredecessor ) )
         {
            return Task.CompletedTask;
         }

         // ELSE a change
         if ( FlowPredecessor.IsNotNullOrDefault() )
         {
            // Get rid of everything for safety
            FlowPredecessor.SuccessorMustReconsiderBoundsTask.RemoveAllSubscribers();
         }

         FlowPredecessor = predecessor;

         if ( FlowPredecessor.IsNotNullOrDefault() )
         {
            FlowPredecessor.SuccessorMustReconsiderBoundsTask.AddIfNotAlreadyThere( this,
               HandleSuccessorMustReconsiderBoundsTask );
         }

         return Task.CompletedTask;
      }

      public Task SetIsAnAlternate( bool isAnAlternate )
      {
         IsAnAlternate = isAnAlternate;

         return Task.CompletedTask;
      }

      public async Task SetIsSelected( bool isSelected )
      {
         if ( IsSelected != isSelected )
         {
            // Deriver can override
            if ( CanSelectionBeMade( IsSelected ) )
            {
               IsSelected = isSelected;

               // The styling helper applies its styles on this view externally

               await IsSelectedChangedTask.RunAllTasksUsingDefaults( this ).ConfigureAwait(true);
            }
         }
      }

      public async Task SetNextBoundsAndMakeAllPendingChanges( Rect rect )
      {
         NextBounds = rect;

         await MakeAllPendingChanges().ConfigureAwait(true);
      }

      public Task SetNextFilteredOut( bool isFilteredOut )
      {
         IsFilteredOut = isFilteredOut;

         NextOpacity = IsFilteredOut ? UIConst_PI.NOT_VISIBLE_OPACITY : UIConst_PI.VISIBLE_OPACITY;

         return Task.CompletedTask;
      }

      public async Task SetRowHeightOverride( double? heightOverride )
      {
         if ( _rowHeightOverride.IsDifferentThan( heightOverride ) )
         {
            _rowHeightOverride = heightOverride;

            if ( DisplayOrder >= 0 )
            {
               // Force the position notification
               await RefreshNextBoundsAndAlternationFromPredecessor( true, true ).ConfigureAwait(true);
            }
         }
      }

      private async Task ConsiderNotifyingAboutChanges( bool allowChangeNotification, bool forceChangeNotification,
                                                        bool isChanged )
      {
         // If filtered out, must notify because this child's predecessor must make the determination about bounds (ours is 0 height).
         if ( forceChangeNotification || ( allowChangeNotification && ( isChanged || IsFilteredOut ) ) )
         {
            // Successor only reacts if they have a next bounds change.
            await SuccessorMustReconsiderBoundsTask.RunAllTasksUsingDefaults( forceChangeNotification )
                                                   .ConfigureAwait(true);
         }
      }

      protected virtual double GetRowHeight()
      {
         // This is the tile "height" for top-to-bottom flows
         if ( ( _flowParent.FixedRowHeight             != null ) &&
              _flowParent.FixedRowHeight.IsANumberGreaterThanZero() &&
              ( _flowParent.RowHeightCalculationMethod == RowHeightCalculationMethods.ParentDecides ) )
         {
            return _flowParent.FixedRowHeight.Value;
         }

         // ELSE
         if ( ( _rowHeightOverride                     != null ) &&
              ( _flowParent.RowHeightCalculationMethod == RowHeightCalculationMethods.OverrideLocally ) )
         {
            return _rowHeightOverride.Value;
         }

         // ELSE
         if ( NextBounds.Height.IsANumberGreaterThanZero() )
         {
            return NextBounds.Height;
         }

         // ELSE
         if ( this is View childAsView )
         {
            return Math.Max( 0, childAsView.Height );
         }

         // ELSE
         return default;
      }

      private async Task HandleSuccessorMustReconsiderBoundsTask( IResponsiveTaskParams paramDict )
      {
         var forceChangeNotification = paramDict.GetTypeSafeValue<bool>( 0 );

         await RefreshNextBoundsAndAlternationFromPredecessor( true, forceChangeNotification ).ConfigureAwait(true);
      }

      /// <remarks>
      /// The Width is not set through <see cref="MakeAllPendingChanges" />, but rather through a broadcast
      /// task:
      /// <see cref="HandleRequestChildChangeTask" />.
      /// This method ensures that the requesteed width actually gets set.
      /// </remarks>
      private void StartForceLastWidthChangeTimer()
      {
         _forceLastWidthChangeTimer?.Dispose();
         _tickCount = 0;

#if LOG_WIDTH_CHANGES
         Debug.WriteLine( nameof( FlowableChild_Forms ) + ": " + nameof( StartForceLastWidthChangeTimer ) +
                          ": A timer has been created." );
#endif

         _forceLastWidthChangeTimer =
            new Timer(

               // ReSharper disable once AsyncVoidLambda
               state =>
               {
                  Interlocked.Increment( ref _tickCount );

#if LOG_WIDTH_CHANGES
                  Debug.WriteLine( nameof( FlowableChild_Forms ) + ": " + nameof( StartForceLastWidthChangeTimer ) +
                                   ": Now on ->"                 + _tickCount + "<- ticks." );
#endif

                  if ( _tickCount > MAX_TICKS )
                  {
#if LOG_WIDTH_CHANGES
                     Debug.WriteLine( nameof( FlowableChild_Forms ) + ": " + nameof( StartForceLastWidthChangeTimer ) +
                                      ": A timer has been DISPOSED." );
#endif

                     // Destroy the timer
                     _forceLastWidthChangeTimer.Dispose();
                  }
                  else if ( Width.IsDifferentThan( _flowParent.ItemsCanvas.Width ) )
                  {
#if LOG_WIDTH_CHANGES
                     Debug.WriteLine( nameof( FlowableChild_Forms ) + ": " + nameof( StartForceLastWidthChangeTimer ) +
                                      ":           existing width ->" + Bounds.Width.ToRoundedInt() + "<-" );
#endif

#if LOG_WIDTH_CHANGES
                     Debug.WriteLine( nameof( FlowableChild_Forms ) + ": " + nameof( StartForceLastWidthChangeTimer ) +
                                      ": flow parent canvas width ->" +
                                      _flowParent.ItemsCanvas.Width.ToRoundedInt() + "<-" );
#endif

                     ThreadHelper.ConsiderBeginInvokeActionOnMainThread( SetWidthBasedOnCanvas,

#if SET_CANVAS_WIDTH_ON_MAIN_THREAD
                        true
#else

                        // ReSharper disable once RedundantArgumentDefaultValue
                        false
#endif
                     );

                     // PRIVATE METHODS
                     void SetWidthBasedOnCanvas()
                     {
                        var nextBounds = NextBounds;
                        nextBounds.Width = _flowParent.ItemsCanvas.Width;
                        NextBounds   = nextBounds;
                        WidthRequest = NextBounds.Width;
                     }
                  }
               },
               null,
               TimeSpan.FromMilliseconds( FORCE_WIDTH_CHANGE_TIMER_DELAY_MILLISECONDS ),
               Timeout.InfiniteTimeSpan
            );
      }

      private IBroadcastSuccessorMustReconsiderBounds_Forms VerifyThatPredecessorIsNotFilteredOut(
         IBroadcastSuccessorMustReconsiderBounds_Forms childFlowPredecessor )
      {
         if ( childFlowPredecessor is IFlowableChild_Forms childFlowPredecessorAsFlowable &&
              childFlowPredecessorAsFlowable.IsFilteredOut )
         {
            // Send back the predecessor's predecessor
            return VerifyThatPredecessorIsNotFilteredOut( childFlowPredecessorAsFlowable.FlowPredecessor );
         }

         // ELSE send back the current predecessor
         return childFlowPredecessor;
      }
   }
}