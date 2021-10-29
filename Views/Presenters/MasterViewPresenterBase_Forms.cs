// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=MasterViewPresenterBase_Forms.cs
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

// DOES NOTHING FOR ANDROID VIEWS
#define CHANGE_CONTENT_VIEW_ON_MAIN_THREAD

namespace Com.MarcusTS.UI.XamForms.Views.Presenters
{
   using System.Linq;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.SmartDI;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Subviews;
   using Xamarin.Forms;

   public interface IMasterViewPresenterBase_Forms : IShapeView_Forms
   { }

   public abstract class MasterViewPresenterBase_Forms : ShapeView_Forms, IMasterViewPresenterBase_Forms
   {
      private const           double                        OPACITY_EDGE          = UIConst_PI.MODERATE_OPACITY;
      private static readonly Easing                        OPACITY_EASING        = Easing.Linear;
      private static readonly uint                          VIEW_FADE_IN_OUT_TIME = 1000;
      private readonly        ISmartDIContainer             _diContainer          = new SmartDIContainer();
      private readonly        Grid                          _masterGrid           = UIUtils_Forms.GetExpandingGrid();
      private readonly        ICanShowProgressSpinner_Forms _spinnerHost;
      private                 ISetCurrentModule_PI          _bindingContextAsProvidingCurrentModule;
      private                 object                        _lastModule;

      protected MasterViewPresenterBase_Forms( ICanShowProgressSpinner_Forms spinnerHost )
      {
         _spinnerHost = spinnerHost;

         // The host views all need a spinner host
         _diContainer.RegisterTypeAsInterface( _spinnerHost.GetType(), typeof( ICanShowProgressSpinner_Forms ),
            creator: () => _spinnerHost );

         BackgroundColor               = ThemeUtils_Forms.MAIN_STAGE_THEME_COLOR;
         _masterGrid.BackgroundColor   = ThemeUtils_Forms.MAIN_STAGE_THEME_COLOR;
         _masterGrid.VerticalOptions   = LayoutOptions.FillAndExpand;
         _masterGrid.HorizontalOptions = LayoutOptions.FillAndExpand;
         _masterGrid.AddAutoRow();

         IsClippedToBounds = true;
         CornerRadius      = 0;
         SetCornerRadiusFactor( 0 );

         // This is a presenter, so does not propagate binding contexts, etc.
         RunSubBindingContextTasksAfterAssignment = false;
         RunContentTasksAfterAssignment           = false;

         PostBindingContextTasks.AddIfNotAlreadyThere( this, HandlePostBindingContextChangedTask );
      }

      public override Task<View> GetDefaultContent()
      {
         return Task.FromResult<View>( _masterGrid );
      }

      protected async Task ChangeContentView<InterfaceT, ClassT>( object viewModel )
         where ClassT : View, InterfaceT
         where InterfaceT : class
      {
         if ( _masterGrid.IsNullOrDefault() )
         {
            return;
         }

         await ChangeTheContent().ConsiderBeginInvokeTaskOnMainThread(

#if CHANGE_CONTENT_VIEW_ON_MAIN_THREAD
            true
#else

            // ReSharper disable once RedundantArgumentDefaultValue
            false
#endif
         ).WithoutChangingContext();


         // PRIVATE METHODS
         async Task ChangeTheContent()
         {
            var newView       = _diContainer.RegisterAndResolveAsInterface<ClassT, InterfaceT>();
            var newViewAsView = newView as View;

            // Will never occur
            if ( newViewAsView == default )
            {
               // turn the spinner off
               _spinnerHost.IsBusyShowing = false;
               return;
            }

            // Ensure that content has been set up (if not yet done)
            if ( newView is ICanSetContentSafely_PI<View> newViewAsContentOwner &&
                 newViewAsContentOwner.Content.IsNullOrDefault() )
            {
               // Sets the default content
               await newViewAsContentOwner.SetContentSafelyAndAwaitAllBranchingTasks().WithoutChangingContext();
            }

            // Transition from one to the other
            View oldView = null;
            if ( _masterGrid.Children.Any() )
            {
               oldView = _masterGrid.Children[ 0 ];
            }

            // 1. Add the new view to the bottom, with opacity at 0.
            newViewAsView.Opacity = OPACITY_EDGE;
            _masterGrid.AddAndSetRowsAndColumns( newViewAsView, 0 );
            _masterGrid.RaiseChild( newViewAsView );

            var fadeInAnimation = new Animation( f => newViewAsView.Opacity = f,
               newViewAsView.Opacity,
               UIConst_PI.VISIBLE_OPACITY, OPACITY_EASING );

            var joinedAnimation = new Animation { { 0, 1, fadeInAnimation }, };

            // If the top(old) view exists, arrange to fade it out, etc.
            if ( oldView != null )
            {
               var fadeOutAnimation = new Animation( f => oldView.Opacity = f,
                  oldView.Opacity, 0, OPACITY_EASING );
               joinedAnimation.Add( 0, 0.5, fadeOutAnimation );
            }

            joinedAnimation.Commit( _masterGrid, "fadeInOutAnimation", length: VIEW_FADE_IN_OUT_TIME );

            if ( oldView != null )
            {
               _masterGrid.LowerChild( oldView );
               _masterGrid.Children.Remove( oldView );
            }

            await newViewAsView.SetBindingContextSafelyAndAwaitAllBranchingTasks_Forms( viewModel )
                               .WithoutChangingContext();

            // turn the spinner off (Unless the view requests control)
            if ( !( newViewAsView is IOverrideProgressSpinner ) )
            {
               _spinnerHost.IsBusyShowing = false;
            }
         }
      }

      protected abstract Task RespondToViewModelChange( object newModule );

      private Task HandlePostBindingContextChangedTask( IResponsiveTaskParams paramDict )
      {
         if ( _bindingContextAsProvidingCurrentModule.IsNotNullOrDefault() )
         {
            // Remove any handler
            _bindingContextAsProvidingCurrentModule.SetCurrentViewModelTask.RemoveIfThere( this,
               HandleSetCurrentModuleTask );
         }

         _bindingContextAsProvidingCurrentModule = BindingContext as ISetCurrentModule_PI;

         if ( _bindingContextAsProvidingCurrentModule.IsNotNullOrDefault() )
         {
            // Set new handler
            _bindingContextAsProvidingCurrentModule?.SetCurrentViewModelTask.AddIfNotAlreadyThere( this,
               HandleSetCurrentModuleTask );
         }

         return Task.CompletedTask;
      }

      private async Task HandleSetCurrentModuleTask( IResponsiveTaskParams paramDict )
      {
         if ( paramDict.IsAnEmptyList() )
         {
            _spinnerHost.IsBusyShowing = false;
            ErrorUtils.ThrowArgumentError( nameof( HandleSetCurrentModuleTask ) + " requires a parameter" );
         }

         // This is the view model
         var newModule = paramDict[ 0 ];

         if ( newModule.IsNullOrDefault() )
         {
            _spinnerHost.IsBusyShowing = false;
            ErrorUtils.ThrowArgumentError( nameof( HandleSetCurrentModuleTask ) + " requires a module" );
         }

         if ( newModule.IsAnEqualReferenceTo( _lastModule ) )
         {
            // Nothing to do
            _spinnerHost.IsBusyShowing = false;
         }

         await RespondToViewModelChange( newModule ).WithoutChangingContext();

         _lastModule = newModule;
      }
   }
}