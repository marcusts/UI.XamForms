// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ScrollableViewBase_Forms.cs
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
   using System.Linq;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Controls;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Controls;
   using Xamarin.Forms;

   public interface IScrollableViewBase_Forms : IShapeView_Forms
   {
      IFlowableCollectionCanvas_Forms AnimatableLayout { get; }

      Task SetFlowableChildSpacing( double childSpacing );

      Task SetUseScrollView( bool useScrollView );
   }

   /// <summary>
   /// Includes a collection of views to be assigned to the animatable layout.
   /// Manages certain common properties of views, but does not determine how to create the actual views.  That is for the
   /// deriver(s).
   /// The scroller is optional (_useScrollView), and included by default.
   /// </summary>
   public abstract class ScrollableViewBase_Forms : ShapeView_Forms, IScrollableViewBase_Forms
   {
      private readonly IThreadSafeAccessor _requestCreateAnimatableLayoutSourceViewsEntered =
         new ThreadSafeAccessor( 0 );

      private readonly IThreadSafeAccessor _requestCreateAnimatableLayoutSourceViewsShouldBeReEntered =
         new ThreadSafeAccessor( 0 );

      private readonly IThreadSafeAccessor             IsInitializing = new ThreadSafeAccessor( 0 );
      private          IFlowableCollectionCanvas_Forms _animatableLayout;
      private          double                          _childSpacing = UIConst_PI.MARGIN_SPACING_HALF_FACTOR;
      private          ScrollView                      _scroller;
      private          bool                            _useScrollView = true;
      private          View                            _viewToSet;

      protected ScrollableViewBase_Forms( ICanShowProgressSpinner_Forms spinnerHost )
      {
         SpinnerHost = spinnerHost;

         IsInitializing.SetTrue();

         this.SetDefaults();

         FillColor = Color.White;

         Margin =
            new Thickness(
               UIConst_PI.MARGIN_SPACING_MEDIUM_FACTOR,
               0,
               UIConst_PI.MARGIN_SPACING_MEDIUM_FACTOR,
               UIConst_PI.MARGIN_SPACING_MEDIUM_FACTOR
            );

         InputTransparent                        = false;
         AnimatableLayoutAsView.InputTransparent = false;
         RunContentTasksAfterAssignment          = true;

         // IMPORTANT -- can't allow content to set binding context, as we start from binding context and go to content.
         SetContentBindingContextAfterAssignment = false;

         // This will run *after* the base, assuming the base has the same sort of handler
         PostBindingContextTasks.AddIfNotAlreadyThere( this, HandlePostBindingContextTask );

         EndOfConstruction();
      }

      // Convenience only
      protected View AnimatableLayoutAsView => AnimatableLayout as View;

      protected ICanShowProgressSpinner_Forms SpinnerHost { get; }

      public IFlowableCollectionCanvas_Forms AnimatableLayout
      {
         get
         {
            if ( _animatableLayout.IsNullOrDefault() )
            {
               _animatableLayout = new FlowableCollectionCanvas_Forms();
               _animatableLayout.SetChildSpacing( _childSpacing ).FireAndFuhgetAboutIt();
               _animatableLayout.SetDoNotListenForRowTaps( true ).FireAndFuhgetAboutIt();

               // NOTE True by default
               // _animatableLayout.SetHeightBasedOnChildren(true);
            }

            return _animatableLayout;
         }
      }

      public async Task SetFlowableChildSpacing( double childSpacing )
      {
         _childSpacing = childSpacing;
         await AnimatableLayout.SetChildSpacing( _childSpacing ).WithoutChangingContext();
      }

      public async Task SetUseScrollView( bool useScrollView )
      {
         if ( _useScrollView != useScrollView )
         {
            _useScrollView = useScrollView;
            await RecreateScrollableViewUI().WithoutChangingContext();
         }
      }

      protected virtual async Task AfterSourceViewsAssigned()
      {
         // This sets "is visible to user" to true
         await AnimatableLayout.AnimateIn().WithoutChangingContext();
      }

      protected virtual Task BeforeSourceViewsAssigned( BetterObservableCollection<View> retViews )
      {
         return Task.CompletedTask;
      }

      protected abstract Task<(bool, BetterObservableCollection<View>)> CreateScrollableViews();

      protected virtual Task<View> CreateViewToSet()
      {
         if ( _useScrollView )
         {
            _scroller = UIUtils_Forms.GetExpandingScrollView();

            // Not responsive, so direct assignment is OK
            _scroller.BindingContext = BindingContext;

            // Not responsive, so direct assignment is OK
            _scroller.Content = AnimatableLayoutAsView;

            _scroller.InputTransparent = false;

            // the animatable layout now needs the scroller for calcuating child screen positions
            AnimatableLayout.ParentScroller = _scroller;

            return Task.FromResult( (View)_scroller );
         }

         // ELSE
         AnimatableLayout.ParentScroller = default;

         return Task.FromResult( AnimatableLayoutAsView );
      }

      protected virtual Task EndInitialization()
      {
         IsInitializing.SetFalse();

         return Task.CompletedTask;
      }

      /// <remarks>
      /// The flex view auto-populates based on the view model, so it is constructed here,
      /// at the binding context changed (where the view model is assigned)
      /// ***NOTE*** The paramDict is ignored.
      /// </remarks>
      protected virtual async Task HandlePostBindingContextTask( IResponsiveTaskParams paramDict )
      {
         if ( BindingContext.IsNullOrDefault() )
         {
            return;
         }

         if ( BindingContext.IsNotNullOrDefault() &&
              BindingContext.IsNotAnEqualReferenceTo( AnimatableLayoutAsView.BindingContext ) )
         {
            await AnimatableLayout.SetBindingContextSafelyAndAwaitAllBranchingTasks( BindingContext )
                                  .WithoutChangingContext();
         }

         await RecreateScrollableViewUI().WithoutChangingContext();

         await RequestCreateScrollableViewsAndSetSourceViews().WithoutChangingContext();
      }

      protected async Task RecreateScrollableViewUI()
      {
         if ( IsInitializing.IsTrue() || BindingContext.IsNullOrDefault() || AnimatableLayoutAsView.IsNullOrDefault() )
         {
            return;
         }

         _viewToSet = await CreateViewToSet().WithoutChangingContext();

#if SHOW_BACK_COLORS
         BackgroundColor = Color.Maroon;
         _viewToSet.BackgroundColor = Color.LightSeaGreen;
#endif

         // TODO Consider moving this
         _viewToSet.Margin = new Thickness( UIConst_PI.MARGIN_SPACING_MEDIUM_FACTOR );

         await this.SetContentSafelyAndAwaitAllBranchingTasks( _viewToSet ).WithoutChangingContext();
      }

      protected async Task RequestCreateScrollableViewsAndSetSourceViews()
      {
         if ( _requestCreateAnimatableLayoutSourceViewsEntered.IsTrue() )
         {
            _requestCreateAnimatableLayoutSourceViewsShouldBeReEntered.SetTrue();
            return;
         }

         _requestCreateAnimatableLayoutSourceViewsEntered.SetTrue();

         SpinnerHost.IsBusyShowing = true;

         ( var fetchedOK, var retViews ) = await CreateScrollableViews().WithoutChangingContext();

         if ( !fetchedOK || retViews.IsNullOrDefault() )
         {
            _requestCreateAnimatableLayoutSourceViewsEntered.SetFalse();
            return;
         }


         // ELSE
         // Allow changes to the views
         await BeforeSourceViewsAssigned( retViews ).WithoutChangingContext();

         await AnimatableLayout.SetSourceViews( retViews.ToArray() ).WithoutChangingContext();

         await AfterSourceViewsAssigned().WithoutChangingContext();

         _requestCreateAnimatableLayoutSourceViewsEntered.SetFalse();

         if ( _requestCreateAnimatableLayoutSourceViewsShouldBeReEntered.IsTrue() )
         {
            _requestCreateAnimatableLayoutSourceViewsShouldBeReEntered.SetFalse();
            await RequestCreateScrollableViewsAndSetSourceViews().WithoutChangingContext();
         }

         SpinnerHost.IsBusyShowing = false;
      }

      private void EndOfConstruction()
      {
         EndInitialization().FireAndFuhgetAboutIt();
      }
   }
}