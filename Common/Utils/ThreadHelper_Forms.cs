// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ThreadHelper_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Common.Utils
{
   using System;
   using System.Threading.Tasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Essentials;

   public static class ThreadHelper_Forms
   {
      public static void ConsiderBeginInvokeActionOnMainThread
      (
         Action action,
         bool forceBeginInvoke =
#if FORCE_ALL_BEGIN_INVOKE
            true
#else
            false
#endif
      )
      {
         if ( action.IsNullOrDefault() )
         {
            return;
         }

         // ELSE a valid action
         if ( !forceBeginInvoke && MainThread.IsMainThread )
         {
            action.Invoke();
         }
         else
         {
            MainThread.BeginInvokeOnMainThread( action.Invoke );
         }
      }

      public static async Task ConsiderBeginInvokeTaskOnMainThread
      (
         this Task task,
         bool forceBeginInvoke =
#if FORCE_ALL_BEGIN_INVOKE
         true
#else
            false
#endif
      )
      {
         if ( !forceBeginInvoke && MainThread.IsMainThread )
         {
            await task.WithoutChangingContext();
         }
         else
         {
            MainThread.BeginInvokeOnMainThread( () => { task.FireAndFuhgetAboutIt(); } );
         }
      }
   }
}