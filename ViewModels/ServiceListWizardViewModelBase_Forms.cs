// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=ServiceListWizardViewModelBase_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.ViewModels
{
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.Linq;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Essentials;

   public interface IServiceListWizardViewModelBase_Forms<InterfaceT> : IWizardViewModel_Forms,
                                                                        IProvideRawData
   {
      IList<InterfaceT> ServiceData            { get; set; }
      void LoadServiceDataAndNotify();
   }

   public abstract class ServiceListWizardViewModelBase_Forms<InterfaceT> :
      WizardViewModel_Forms, IServiceListWizardViewModelBase_Forms<InterfaceT>
   {
      protected ServiceListWizardViewModelBase_Forms()
      {
         StartUp();
      }

      public Func<IList>      GetRawData         => () => ServiceData.ToArray();
      public IResponsiveTasks RawDataChangedTask { get; set; } = new ResponsiveTasks( 1 );

      public IList<InterfaceT> ServiceData            { get; set; } = new List<InterfaceT>();

      public void LoadServiceDataAndNotify()
      {
         try
         {
            // NOTE Async void inside try/catch is OK in this case
            _ = Task.Run( async () =>
                          {
                             // WARNING Result blocks this thread; should not affect performance
                             ServiceData = await LoadServiceData();

                             MainThread.BeginInvokeOnMainThread(
                                // NOTE Just running a notification, so not worried about the async void
                                // ReSharper disable once AsyncVoidLambda
                                async () =>
                                {
                                   // Regardless of the result, run the notification(s)
                                   await RawDataChangedTask.RunAllTasksUsingDefaults( ServiceData ).WithoutChangingContext();
                                } );
                          } );
         }
         catch ( Exception ex )
         {
            Debug.WriteLine( nameof( ServiceListWizardViewModelBase_Forms<InterfaceT> ) + ": " +
                             nameof( LoadServiceDataAndNotify ) + ": ERROR ->" + ex.Message + "<-" );
         }
      }

      protected abstract Task<IList<InterfaceT>> LoadServiceData();

      private void StartUp()
      {
         LoadServiceDataAndNotify();
      }
   }
}