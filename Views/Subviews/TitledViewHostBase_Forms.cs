// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=TitledViewHostBase_Forms.cs
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

#define HACK_LABEL_ALIGNMENT_ISSUES

// #define SHOW_BACK_COLORS

// DOES NOTHING FOR ANDROID VIEWS
// #define RECREATE_ON_MAIN_THREAD

namespace Com.MarcusTS.UI.XamForms.Views.Subviews
{
   using System;
   using System.Linq;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.ViewModels;
   using Com.MarcusTS.UI.XamForms.Views.Controls;
   using Xamarin.Forms;

   /// <summary>
   /// Uses animation to flow screen elements in:
   /// 1. Title (optional). Requires that the view model be of interface type ITitledViewModel.
   /// 2. Any sort of scrollable flex view
   /// 3. Any number of bottom UI items such as buttons. The deriver usually adds these.
   /// </summary>
   public interface ITitledViewHostBase_Forms : IShapeView_Forms, IOverrideProgressSpinner
   {
      bool PreserveSpinnerAfterRecreateUI { get; set; }

      Task SetForceFullScreen( bool forceFullScreen );
   }

   public abstract class TitledViewHostBase_Forms : ShapeView_Forms, ITitledViewHostBase_Forms
   {
      public static readonly  string         CANCEL_TEXT                         = "CANCEL".Expanded();
      public static readonly  string         CREATE_ACCOUNT_TEXT                 = "Create Account";
      private static readonly double         DEFAULT_TITLE_LABEL_BOTTOM_MARGIN   = 15.0.AdjustForOsAndDevice();
      private static readonly FontAttributes DEFAULT_TITLE_LABEL_FONT_ATTRIBUTES = FontAttributes.Bold;

      private static readonly double DEFAULT_TITLE_LABEL_FONT_SIZE =
         Device.GetNamedSize( NamedSize.Title, typeof( Label ) ).AdjustForOsAndDevice();

      private static readonly LayoutOptions DEFAULT_TITLE_LABEL_HORIZONTAL_OPTIONS = LayoutOptions.Center;
      private static readonly TextAlignment DEFAULT_TITLE_LABEL_HORIZONTAL_TEXT_ALIGNMENT = TextAlignment.Center;
      private static readonly Color DEFAULT_TITLE_LABEL_TEXT_COLOR = Color.White;
      private static readonly double SCROLLER_SLOP = 20.0.AdjustForOsAndDevice();
      private readonly IThreadSafeAccessor _considerConstrainingFlexViewHeight_Entered = new ThreadSafeAccessor( 0 );
      private readonly IThreadSafeAccessor _forceFullScreen = new ThreadSafeAccessor( 0 );
      private readonly IThreadSafeAccessor _mustReenterConsiderConstrainingFlexViewHeight = new ThreadSafeAccessor( 0 );
      private readonly IThreadSafeAccessor _mustRetryRecreateTitledFlexHostViewUI = new ThreadSafeAccessor( 0 );
      private readonly IThreadSafeAccessor _recreateTitledFlexHostViewUI_Entered = new ThreadSafeAccessor( 0 );
      private IFlowableCollectionCanvas_Forms _animatedBaseLayout;
      private View _derivedView;

      protected TitledViewHostBase_Forms( ICanShowProgressSpinner_Forms spinnerHost )
      {
         SpinnerHost = spinnerHost;

         // HACK to maintain views within the MasterSinglePage's bounds
         if ( SpinnerHost.IsNotNullOrDefault() )
         {
            SpinnerHost.SpinnerOverlayHeightChanged.AddIfNotAlreadyThere( this, HandleSpinnerOverlayHeightChanged );
         }

         // This will run after the base class, assuming the base class adds a similar handler.
         PostBindingContextTasks.AddIfNotAlreadyThere( this, HandlePostBindingContextTask );

         // No corner radius
         CornerRadius = 0;
         SetCornerRadiusFactor( 0 ).FireAndFuhgetAboutIt();
      }

      protected IFlowableCollectionCanvas_Forms AnimatedBaseLayout
      {
         get
         {
            if ( _animatedBaseLayout.IsNullOrDefault() )
            {
               _animatedBaseLayout = new FlowableCollectionCanvas_Forms();

               _animatedBaseLayout.SetDoNotListenForRowTaps( true ).FireAndFuhgetAboutIt();
            }

            return _animatedBaseLayout;
         }
      }

      protected View DerivedView
      {
         get
         {
            if ( _derivedView.IsNullOrDefault() )
            {
               _derivedView = GetDerivedView;
            }

            return _derivedView;
         }
      }

      protected virtual  double FlexViewWidth { get; set; } = UIConst_PI.NEUTRAL_WIDTH_HEIGHT;
      protected abstract View GetDerivedView { get; }
      protected          ICanShowProgressSpinner_Forms SpinnerHost { get; }
      protected virtual  FontAttributes TitleLabelFontAttributes { get; set; } = DEFAULT_TITLE_LABEL_FONT_ATTRIBUTES;
      protected virtual  double TitleLabelFontSize { get; set; } = DEFAULT_TITLE_LABEL_FONT_SIZE;

      protected virtual LayoutOptions TitleLabelHorizontalOptions { get; set; } =
         DEFAULT_TITLE_LABEL_HORIZONTAL_OPTIONS;

      protected virtual TextAlignment TitleLabelHorizontalTextAlignment { get; set; } =
         DEFAULT_TITLE_LABEL_HORIZONTAL_TEXT_ALIGNMENT;

      protected virtual Color TitleLabelTextColor      { get; set; } = DEFAULT_TITLE_LABEL_TEXT_COLOR;
      private           View  AnimatedBaseLayoutAsView => AnimatedBaseLayout as View;

      public bool PreserveSpinnerAfterRecreateUI { get; set; }

      public Task SetForceFullScreen( bool forceFullScreen )
      {
         if ( _forceFullScreen.IsTrue() != forceFullScreen )
         {
            if ( forceFullScreen )
            {
               _forceFullScreen.SetTrue();
            }
            else
            {
               _forceFullScreen.SetFalse();
            }

            ConsiderConstrainingFlexViewHeight();
         }

         return Task.CompletedTask;
      }

      protected virtual Task AfterHandlingPostBindingContextTask()
      {
         return Task.CompletedTask;
      }

      protected Task CreateStandardNextAndCancelButtons( string nextText = "", string cancelText = "" )
      {
         // Add the save and cancel buttons
         if ( DerivedView.BindingContext.IsNotNullOrDefault()                             &&
              DerivedView.BindingContext is IWizardViewModel_Forms bindingContextAsWizard &&
              AnimatedBaseLayout.IsNotNullOrDefault() )
         {
            var nextTabIndex = AnimatedBaseLayout.SourceViews.Count;

            var saveButton =
               FlowableUtils_Forms.CreateFlowableControlButton(
                  nextText.IsEmpty() ? CREATE_ACCOUNT_TEXT : nextText,
                  bindingContextAsWizard.NextCommand,
                  bindingContextAsWizard,
                  nextTabIndex++,
                  true,
                  extraTopSpace: FlowableConst_PI.DEFAULT_EXTRA_TOP_MARGIN );

            AnimatedBaseLayout.SourceViews.Add( saveButton as View );

            var cancelButton =
               FlowableUtils_Forms.CreateFlowableControlButton(
                  cancelText.IsEmpty() ? CANCEL_TEXT : cancelText,
                  bindingContextAsWizard.CancelCommand,
                  bindingContextAsWizard,

                  // ReSharper disable once RedundantAssignment
                  nextTabIndex++,
                  false,
                  Color.Red,
                  extraTopSpace: FlowableConst_PI.SMALL_EXTRA_TOP_MARGIN,
                  backColor: Color.Transparent );

            ( (View)cancelButton ).Margin = new Thickness( ( (View)cancelButton ).Margin.Left,
               ( (View)cancelButton ).Margin.Top, ( (View)cancelButton ).Margin.Right,
               ( (View)cancelButton ).Margin.Bottom + SCROLLER_SLOP );

            AnimatedBaseLayout.SourceViews.Add( cancelButton as View );
         }

         return Task.CompletedTask;
      }

      private void ConsiderConstrainingFlexViewHeight()
      {
         if ( !SpinnerHost.SpinnerOverlayHeight.IsANumberGreaterThanZero() ||
              !AnimatedBaseLayout.ChildHeightWithoutScrollBottomMargin.IsANumberGreaterThanZero() )
         {
            return;
         }

         if ( _considerConstrainingFlexViewHeight_Entered.IsTrue() )
         {
            _mustReenterConsiderConstrainingFlexViewHeight.SetTrue();
            return;
         }

         // ELSE

         _considerConstrainingFlexViewHeight_Entered.SetTrue();

         var nonScrollingChildren = AnimatedBaseLayout.SourceViews
                                                      .Where( sv => sv.IsNotAnEqualReferenceTo( DerivedView ) )
                                                      .ToArray();
         var nonScrollingChildrenHeight = nonScrollingChildren.Sum( sv => sv.Height + sv.Margin.VerticalThickness );

         var maxHeightAllowed =
            SpinnerHost.SpinnerOverlayHeight - Margin.VerticalThickness;

         var newHeightRequest =
            _forceFullScreen.IsTrue()
               ? maxHeightAllowed
               : Math.Min( AnimatedBaseLayout.ChildHeightWithoutScrollBottomMargin, maxHeightAllowed );

         if ( newHeightRequest.IsANumberGreaterThanZero() && newHeightRequest.IsDifferentThan( Height ) )
         {
            HeightRequest = newHeightRequest;
         }

         if ( newHeightRequest.IsGreaterThanOrEqualTo( maxHeightAllowed ) )
         {
            var maxScrollableHeightAllowed =
               SpinnerHost.SpinnerOverlayHeight     -
               nonScrollingChildrenHeight           -
               DerivedView.Margin.VerticalThickness -
               AnimatedBaseLayoutAsView.Margin.VerticalThickness;

            var newScrollerHeightRequest =
               _forceFullScreen.IsTrue()
                  ? maxScrollableHeightAllowed
                  : Math.Min( AnimatedBaseLayout.ChildHeightWithoutScrollBottomMargin, maxScrollableHeightAllowed );

            if ( newScrollerHeightRequest.IsANumberGreaterThanZero() &&
                 newScrollerHeightRequest.IsDifferentThan( DerivedView.Height ) )
            {
               DerivedView.HeightRequest = newScrollerHeightRequest;
            }
         }

         _considerConstrainingFlexViewHeight_Entered.SetFalse();

         if ( _mustReenterConsiderConstrainingFlexViewHeight.IsTrue() )
         {
            _mustReenterConsiderConstrainingFlexViewHeight.SetFalse();
            ConsiderConstrainingFlexViewHeight();
         }
      }

      private Task HandleChildHeightWithoutScrollBottomMarginChangeTask( IResponsiveTaskParams paramdict )
      {
         ConsiderConstrainingFlexViewHeight();
         return Task.CompletedTask;
      }

      /// <summary>
      /// Since the flex view is driven by this class's binding context (it literally populates from the view model),
      /// this is the best place to create the UI.
      /// </summary>
      private async Task HandlePostBindingContextTask( IResponsiveTaskParams paramDict )
      {
         if ( BindingContext.IsNullOrDefault() )
         {
            return;
         }

         // ELSE
         // Force-fire the creation of the views; they'll set their own binding contexts as needed
         await AnimatedBaseLayout.SetBindingContextSafelyAndAwaitAllBranchingTasks( BindingContext )
                                 .WithoutChangingContext();
         await DerivedView.SetBindingContextSafelyAndAwaitAllBranchingTasks_Forms( BindingContext )
                          .WithoutChangingContext();

         await RecreateTitledFlexHostViewUI().WithoutChangingContext();
      }

      private Task HandleSpinnerOverlayHeightChanged( IResponsiveTaskParams paramDict )
      {
         ConsiderConstrainingFlexViewHeight();
         return Task.CompletedTask;
      }

      private async Task RecreateTitledFlexHostViewUI()
      {
         if ( _recreateTitledFlexHostViewUI_Entered.IsTrue() )
         {
            _mustRetryRecreateTitledFlexHostViewUI.SetTrue();
            return;
         }


         // ELSE
         _recreateTitledFlexHostViewUI_Entered.SetTrue();

         await RecreateUI().ConsiderBeginInvokeTaskOnMainThread(

#if RECREATE_ON_MAIN_THREAD
            true
#else

            // ReSharper disable once RedundantArgumentDefaultValue
            false
#endif
         ).WithoutChangingContext();


         // PRIVATE METHODS
         async Task RecreateUI()
         {
            SpinnerHost.IsBusyShowing = true;

            // IMPORTANT Wipes out the existing collection, if any
            _animatedBaseLayout = default;

            AnimatedBaseLayout.ChildHeightWithoutScrollBottomMarginChangeTask.AddIfNotAlreadyThere( this,
               HandleChildHeightWithoutScrollBottomMarginChangeTask );

            if ( BindingContext is ITitledViewModel bindingContextAsTitledView )
            {
               var titleLabel =
                  UIUtils_Forms.GetSimpleLabel(
                     textColor: TitleLabelTextColor,
                     horizontalAlignment: TitleLabelHorizontalTextAlignment,
                     fontSize: TitleLabelFontSize,
                     fontAttributes: TitleLabelFontAttributes,
                     bindingSourcePropName: nameof( ITitledViewModel.Title ),
                     bindingSource: bindingContextAsTitledView
                  );
               titleLabel.Margin            = new Thickness( 0, DEFAULT_TITLE_LABEL_BOTTOM_MARGIN );
               titleLabel.HorizontalOptions = TitleLabelHorizontalOptions;

#if HACK_LABEL_ALIGNMENT_ISSUES
               AnimatedBaseLayout.SourceViews.Add( titleLabel.ConvertToHorizontallyAlignGrid() );
#else
               AnimatedBaseLayout.SourceViews.Add( titleLabel );
#endif
            }

            // ReSharper disable once RedundantAssignment
            if ( DerivedView.IsNotNullOrDefault() )
            {
               if ( FlexViewWidth.IsANumberGreaterThanZero() )
               {
                  DerivedView.WidthRequest = FlexViewWidth;
               }

               AnimatedBaseLayout.SourceViews.Add( DerivedView );
            }

#if SHOW_BACK_COLORS
            BackgroundColor = Color.Turquoise;
            Margin = new Thickness(15.0.AdjustForOsAndDevice());

            AnimatedBaseLayoutAsView.Margin = new Thickness(15.0.AdjustForOsAndDevice());
            AnimatedBaseLayoutAsView.BackgroundColor = Color.DarkViolet;

            DerivedView.Margin = new Thickness(15.0.AdjustForOsAndDevice());
            DerivedView.BackgroundColor = Color.HotPink;

#endif

            await AfterHandlingPostBindingContextTask().WithoutChangingContext();

            // Set our own content
            await this.SetContentSafelyAndAwaitAllBranchingTasks( AnimatedBaseLayoutAsView ).WithoutChangingContext();

            await AnimatedBaseLayout.AnimateIn().WithoutChangingContext();

            ConsiderConstrainingFlexViewHeight();

            _recreateTitledFlexHostViewUI_Entered.SetFalse();

            if ( _mustRetryRecreateTitledFlexHostViewUI.IsTrue() )
            {
               // Recur
               _mustRetryRecreateTitledFlexHostViewUI.SetFalse();
               await RecreateTitledFlexHostViewUI().WithoutChangingContext();
            }

            if ( !PreserveSpinnerAfterRecreateUI )
            {
               SpinnerHost.IsBusyShowing = false;
            }
         }
      }
   }
}