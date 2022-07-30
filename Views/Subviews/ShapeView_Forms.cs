// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ShapeView_Forms.cs
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

// #define PREVENT_BINDING_CONTEXT_REDUNDANCY
// #define PREVENT_CONTENT_REDUNDANCY

// #define SET_BINDING_CONTEXT_ON_MAIN_THREAD
// #define SET_CONTENT_ON_MAIN_THREAD

namespace Com.MarcusTS.UI.XamForms.Views.Subviews
{
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Controls;
   using Com.MarcusTS.UI.XamForms.Views.Pages;
   using System;
   using System.Threading.Tasks;
   using Xamarin.Forms;

   public interface IShapeView_Forms :
      ICanSetContentSafely_PI<View>,
      ICanSetBindingContextContextSafely_PI
   {
      Task SetCornerRadiusFactor( double factor );
   }

   /// <summary>Much of the code is the same as that at the <see cref="MasterSinglePage_Forms" />.</summary>
   public class ShapeView_Forms : RoundedContentView, IShapeView_Forms
   {
      private readonly IThreadSafeAccessor _bindingContextSetOnce = new ThreadSafeAccessor( 0 );
      private readonly IThreadSafeAccessor _contentSetOnce        = new ThreadSafeAccessor( 0 );
      private          double?             _cornerRadiusFactor    = UIConst_Forms.DEFAULT_CORNER_RADIUS_FACTOR;
      private          double              _lastHeight;
      private          double?             _lastMinOfWidthAndHeight;
      private          double              _lastWidth;

      public ShapeView_Forms()
      {
         CallPrepare();
      }

      // Hide the misbehaved void setter
      public new object BindingContext => base.BindingContext;

      // Hide the misbehaved void setter
      public new View Content => base.Content;

      public IThreadSafeAccessor IsPostBindingContextCompleted { get; set; } = new ThreadSafeAccessor( 0 );

      public IThreadSafeAccessor IsPostContentCompleted { get; set; } = new ThreadSafeAccessor( 0 );

      public IResponsiveTasks PostBindingContextTasks { get; set; }

      public int PostBindingContextTasksParamCount { get; set; } = 1;

      public IResponsiveTasks PostContentTasks { get; set; }

      public int PostContentTasksParamCount { get; set; } = 1;

      public bool RunContentTasksAfterAssignment { get; set; } = true;

      // Could run multiple times if the same view model is shared between views
      public bool RunSubBindingContextTasksAfterAssignment { get; set; } = true;

      public bool SetContentBindingContextAfterAssignment { get; set; } = true;

      public virtual Task<View> GetDefaultContent()
      {
         return Task.FromResult<View>( default );
      }

      // The taskParams length must be the PostBindingContextTasksParamCount.
      public async Task RunPostBindingContextTasks( params object[] taskParams )
      {
         IsPostBindingContextCompleted.SetFalse();
         await PostBindingContextTasks.RunAllTasksUsingDefaults( taskParams ).AndReturnToCallingContext();
         IsPostBindingContextCompleted.SetTrue();
      }

      // The taskParams length must be the PostContentTasksParamCount.
      public async Task RunPostContentTasks( params object[] taskParams )
      {
         IsPostContentCompleted.SetFalse();
         await PostContentTasks.RunAllTasksUsingDefaults( taskParams ).AndReturnToCallingContext();
         IsPostContentCompleted.SetTrue();
      }

      public async Task SetBindingContextSafely( object context )
      {
         await SetBindingContext().ConsiderBeginInvokeTaskOnMainThread(
#if SET_BINDING_CONTEXT_ON_MAIN_THREAD
            true
#else

            // ReSharper disable once RedundantArgumentDefaultValue
            false
#endif
         ).AndReturnToCallingContext();


         // PRIVATE METHODS
         async Task SetBindingContext()
         {
#if PREVENT_BINDING_CONTEXT_REDUNDANCY
            if (base.BindingContext.IsNotAnEqualReferenceTo(context))
#endif
            {
               // ELSE
               base.BindingContext = context;

               // Raise the current class's post binding context tasks
               await RunPostBindingContextTasks( this ).AndReturnToCallingContext();

               if (
                  RunSubBindingContextTasksAfterAssignment &&
                  BindingContext.IsNotNullOrDefault() &&
                  BindingContext is IProvidePostBindingContextTasks_PI bindingContextAsRunningPostBindingContextTasks
               )
               {
                  // Raise the *view model's* post binding context tasks
                  await bindingContextAsRunningPostBindingContextTasks.RunPostBindingContextTasks( this ).AndReturnToCallingContext();
               }

               await ConsiderSettingContentBindingContext().AndReturnToCallingContext();
            }
         }
      }

      public async Task SetContentSafely( View view )
      {
         await SetContent().ConsiderBeginInvokeTaskOnMainThread(
#if SET_CONTENT_ON_MAIN_THREAD
            true
#else
            // ReSharper disable once RedundantArgumentDefaultValue
            false
#endif
         ).AndReturnToCallingContext();


         // PRIVATE METHODS
         async Task SetContent()
         {
#if PREVENT_CONTENT_REDUNDANCY
            if ( base.Content.IsNotAnEqualReferenceTo( view ) )
#endif

            {
               var newView = view ?? await GetDefaultContent().AndReturnToCallingContext();

               //CRITICAL ANDROID CRASH - Cannot access a disposed PancakeView renderer
               base.Content = newView;

               // Raise the current class's post content tasks
               await RunPostContentTasks( this ).AndReturnToCallingContext();

               if (
                  RunContentTasksAfterAssignment &&
                  Content.IsNotNullOrDefault() &&
                  Content is IProvidePostContentTasks_PI contentAsRunningPostContentTasks
               )
               {
                  // Raise the *content's* post content tasks
                  await contentAsRunningPostContentTasks.RunPostContentTasks( this ).AndReturnToCallingContext();
               }

               await ConsiderSettingContentBindingContext().AndReturnToCallingContext();
            }
         }
      }

      public Task SetCornerRadiusFactor( double factor )
      {
         if ( _cornerRadiusFactor.IsDifferentThan( factor ) )
         {
            _cornerRadiusFactor = factor;
            SetCornerRadiusFromFactor( Width, Height );
         }

         return Task.CompletedTask;
      }

      protected override void OnSizeAllocated( double width, double height )
      {
         base.OnSizeAllocated( width, height );

         if ( width.IsDifferentThan( _lastWidth ) || height.IsDifferentThan( _lastHeight ) )
         {
            SetCornerRadiusFromFactor( width, height );

            _lastWidth = width;
            _lastHeight = height;
         }
      }

      protected virtual void PrepareVirtually()
      {
         this.SetDefaults();

         PostContentTasks = new ResponsiveTasks( PostContentTasksParamCount );
         PostBindingContextTasks = new ResponsiveTasks( PostBindingContextTasksParamCount );
      }

      private void CallPrepare()
      {
         PrepareVirtually();
      }

      private async Task ConsiderSettingContentBindingContext()
      {
         var finalNestedContent = Content is ScrollView contentAsScrollView ? contentAsScrollView.Content : Content;

         if ( SetContentBindingContextAfterAssignment )
         {
            await finalNestedContent.SetBindingContextSafelyAndAwaitAllBranchingTasks_Forms( BindingContext ).AndReturnToCallingContext();
         }
      }

      private void SetCornerRadiusFromFactor( double width, double height )
      {
         if ( !_cornerRadiusFactor.GetValueOrDefault().IsANumberGreaterThanZero() ||
              !width.IsANumberGreaterThanZero() ||
              !height.IsANumberGreaterThanZero() )
         {
            return;
         }

         var minOfWidthAndHeight = Math.Min( width, height );

         if ( ( _lastMinOfWidthAndHeight != null ) &&
              minOfWidthAndHeight.IsSameAs( _lastMinOfWidthAndHeight.GetValueOrDefault() ) )
         {
            return;
         }

         CornerRadius = (float)( minOfWidthAndHeight * _cornerRadiusFactor.GetValueOrDefault() );

         _lastMinOfWidthAndHeight = minOfWidthAndHeight;
      }
   }
}
