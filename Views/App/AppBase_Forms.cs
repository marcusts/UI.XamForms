// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=AppBase_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Views.App
{
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Navigation;
   using Com.MarcusTS.UI.XamForms.Views.Pages;
   using Com.MarcusTS.UI.XamForms.Views.Presenters;
   using Xamarin.Forms;

   public interface IAppBase_Forms
   { }

   public abstract class AppBaseForms : Application, IAppBase_Forms
   {
      protected AppBaseForms()
      {
         StartUp();
      }

      private void StartUp()
      {
         // This is the only page; all stages are views within this page.
         var masterPage =
            new MasterSinglePage_Forms
            {
               IsBusyShowing = true,
            };

         var _masterPresenter = GetMasterPresenter( masterPage );

         masterPage.AddGridView( _masterPresenter as View, AvailableGrids.Stage );

         var viewModel     = GetAppStateManager( masterPage );
         var masterToolbar = GetMasterToolbar();

         if ( masterToolbar is View masterToolbarAsView )
         {
            masterPage.AddGridView( masterToolbarAsView, AvailableGrids.Keyboard );
            masterPage.BottomToolbarHeightReporter                = masterToolbar;
            masterPage.ToolbarItemNamesAndSelectionStatesReporter = viewModel;
         }

         MainPage = masterPage;

         masterPage.SetContentSafelyAndAwaitAllBranchingTasks().FireAndFuhgetAboutIt();
         masterPage.SetBindingContextSafelyAndAwaitAllBranchingTasks( viewModel ).FireAndFuhgetAboutIt();

         _masterPresenter.SetContentSafelyAndAwaitAllBranchingTasks().FireAndFuhgetAboutIt();
         _masterPresenter.SetBindingContextSafelyAndAwaitAllBranchingTasks( viewModel ).FireAndFuhgetAboutIt();

         if ( masterToolbar is ICanSetContentSafely_PI<View> masterToolbarAsContentSetter )
         {
            masterToolbarAsContentSetter.SetContentSafelyAndAwaitAllBranchingTasks().FireAndFuhgetAboutIt();
         }

         if ( masterToolbar is ICanSetBindingContextContextSafely_PI masterToolbarAsResponsiveBindingSetter )
         {
            masterToolbarAsResponsiveBindingSetter.SetBindingContextSafelyAndAwaitAllBranchingTasks( viewModel )
                                                  .FireAndFuhgetAboutIt();
         }

         viewModel.GoToStartUpState().WithoutChangingContext();
      }

      protected abstract IMasterViewPresenterBase_Forms GetMasterPresenter( ICanShowProgressSpinner_Forms spinnerHost );

      protected abstract IAppStateManagerBase_Forms GetAppStateManager( ICanShowProgressSpinner_Forms spinnerHost );

      protected abstract IToolbar_PI GetMasterToolbar();
   }
}