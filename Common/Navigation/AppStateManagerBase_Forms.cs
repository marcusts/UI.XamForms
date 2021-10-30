// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=AppStateManagerBase_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Common.Navigation
{
   using System.Linq;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Navigation;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.SmartDI;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.ViewModels;
   using Xamarin.Essentials;

   public interface IAppStateManagerBase_Forms :
      ISetCurrentModule_PI,
      IProvideToolbarItemNamesAndSelectionStates_PI,
      IHaveAndReportCurrentState_PI
   { }

   public abstract class AppStateManagerBase_Forms : SimpleStateMachineBase_PI, IAppStateManagerBase_Forms
   {
      private readonly ICanShowProgressSpinner_Forms _spinnerHost;

      protected AppStateManagerBase_Forms( ICanShowProgressSpinner_Forms spinnerHost )
      {
         _spinnerHost = spinnerHost;

         StartUpAppStateManagerBase_Forms();
      }

      public string CurrentState { get; private set; }

      public IResponsiveTasks CurrentStateChangedTask { get; set; } = new ResponsiveTasks( 1 );

      public virtual bool IsToolbarVisible { get; private set; }

      public IResponsiveTasks IsToolbarVisibleChangedTask { get; } = new ResponsiveTasks( 1 );

      public IResponsiveTasks SetCurrentViewModelTask { get; set; } = new ResponsiveTasks( 1 );

      public abstract (string, string)[] ToolbarItemNamesAndStates { get; }

      protected Task ChangeLoginViewModelState<InterfaceT, ClassT>(
         string                                  nextState, string cancelState,
         Delegates_Forms.FinalValidationDelegate finalValidation = default )
         where ClassT : class, InterfaceT
         where InterfaceT : class, IWizardViewModel_Forms
      {
         // HACK Added 2021-09-01 due to native crashes
         MainThread.BeginInvokeOnMainThread(
            // ReSharper disable once AsyncVoidLambda
            async
            () =>
            {
               var viewModel = await PrepareForViewModelStateChange<InterfaceT, ClassT>( nextState, false )
                 .WithoutChangingContext();

               // Redundant assignments because the view model is stored in the DI Container. ... but harmless
               viewModel.NextState        = nextState;
               viewModel.CancelState      = cancelState;
               viewModel.FinalValidation  = finalValidation;
               viewModel.OnOutcomeChanged = HandleOnOutcomeChanged;

               // Fires the binding context changed for the presenter
               await SetCurrentViewModelTask.RunAllTasksUsingDefaults( viewModel ).WithoutChangingContext();
            } );

         return Task.CompletedTask;
      }

      protected Task ChangeToolbarViewModelState<InterfaceT, ClassT>( string newState )
         where ClassT : class, InterfaceT
         where InterfaceT : class
      {
         // HACK Added 2021-09-01 due to native crashes
         MainThread.BeginInvokeOnMainThread(
            // ReSharper disable once AsyncVoidLambda
            async
            () =>
            {
               var viewModel = await PrepareForViewModelStateChange<InterfaceT, ClassT>( newState, true )
                 .WithoutChangingContext();

               // Don't need to worry about the bool return condition in this case. The toolbar will ignore this change except
               // to set the physical toolbar item as selected.
               CurrentState = newState;


               // WARNING Do not use main thread -- crashes IOS
               // Forcing app state; can't get the spinner turned off without this
               await GoToAppState( CurrentState ).WithoutChangingContext();

               if ( CurrentStateChangedTask.IsNotNullOrDefault() )
               {
                  await CurrentStateChangedTask.RunAllTasksUsingDefaults( CurrentState ).WithoutChangingContext();
               }

               // Fires the binding context changed for the presenter
               await SetCurrentViewModelTask.RunAllTasksUsingDefaults( viewModel ).WithoutChangingContext();
            } );

         return Task.CompletedTask;
      }

      protected virtual Task ProcessViewModelBeforeSettingAsCurrent<InterfaceT>( InterfaceT viewModel )
      {
         return Task.CompletedTask;
      }

      protected virtual Task SetUpViewModel<InterfaceT>( InterfaceT viewModel, string newState )
      {
         return Task.CompletedTask;
      }

      protected string TitleFromPairs( string newState )
      {
         var foundPair = ToolbarItemNamesAndStates.FirstOrDefault( pair => pair.Item2.IsSameAs( newState ) );

         if ( foundPair != default )
         {
            return foundPair.Item1;
         }

         return default;
      }

      private Task HandleOnOutcomeChanged( IWizardViewModel_Forms viewModel, string nextState, string cancelState )
      {
         if ( viewModel.Outcome == Outcomes.TryAgain )
         {
            return Task.CompletedTask;
         }

         // ELSE go to the valid app state

         // Cross threaded after button taps in the UI
         MainThread.BeginInvokeOnMainThread(

            // ReSharper disable once AsyncVoidLambda
            async
            () =>
            {
               // Force the state; the spinner can gets stuck if the app state does not change.
               await GoToAppState( viewModel.Outcome == Outcomes.Next
                  ? nextState
                  : cancelState ).WithoutChangingContext();
            } );

         return Task.CompletedTask;
      }

      private async Task<InterfaceT> PrepareForViewModelStateChange<InterfaceT, ClassT>(
         string newState, bool isToolbarVisible )
         where ClassT : class, InterfaceT where InterfaceT : class
      {
         if ( IsToolbarVisible != isToolbarVisible )
         {
            IsToolbarVisible = isToolbarVisible;
            await IsToolbarVisibleChangedTask.RunAllTasksUsingDefaults( IsToolbarVisible ).WithoutChangingContext();
         }

         // Turn on the progress spinner
         _spinnerHost.IsBusyShowing = true;

         var viewModel = DIContainer.RegisterAndResolveAsInterface<ClassT, InterfaceT>();
         await SetUpViewModel( viewModel, newState ).WithoutChangingContext();
         await ProcessViewModelBeforeSettingAsCurrent( viewModel ).WithoutChangingContext();

         return viewModel;
      }

      private void StartUpAppStateManagerBase_Forms()
      {
         // Add the spinner to the DI container in case it is ever needed for injection.
         // Quite likely for various view models
         DIContainer.RegisterTypeAsInterface( _spinnerHost.GetType(), typeof( ICanShowProgressSpinner_Forms ),
            creator: () => _spinnerHost );
      }
   }
}