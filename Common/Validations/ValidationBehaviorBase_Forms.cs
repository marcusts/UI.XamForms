// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ValidationBehaviorBase_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Common.Validations
{
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Behaviors;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Essentials;
   using Xamarin.Forms;

   public interface IValidationBehaviorBase_Forms : IValidationBehaviorBase_PI
   {
      VisualElement BoundHost { get; }
   }

   public abstract class ValidationBehaviorBase_Forms : Behavior, IValidationBehaviorBase_Forms
   {
      private bool                   _validationConditionsSetOnce;
      public  bool                   IsFocused            { get; private set; }
      public  VisualElement          BoundHost            { get; private set; }
      public  IThreadSafeAccessor    IsValid              { get; } = new ThreadSafeAccessor( 0 );
      public  IResponsiveTasks       IsValidChangedTask   { get; } = new ResponsiveTasks( 1 );
      public  IIsValidCondition_PI[] ValidationConditions { get; private set; }

      // WARNING Cannot send in ValidationConditions or any list,
      //         as these get converted to separate param value, so do not arrive in the same form as they were published.
      public IResponsiveTasks ValidationConditionsChanged { get; set; } = new ResponsiveTasks();

      public Task RevalidateAllConditions()
      {
         MainThread.BeginInvokeOnMainThread(
            // ReSharper disable once AsyncVoidLambda
            async
               () =>
            {
               if ( BoundHost.IsNullOrDefault() )
               {
                  IsValid.SetFalse();
               }
               else if ( ValidationConditions.IsAnEmptyList() && !_validationConditionsSetOnce )
               {
                  await ResetAndRevalidateAllConditions().AndReturnToCallingContext();
               }
               else
               {
                  var allTrue = true;
                  var values  = GetValuesFromView();
                  foreach ( var condition in ValidationConditions )
                  {
                     await condition.RevalidateSingleCondition( values.Item1, values.Item2 ).AndReturnToCallingContext();
                     if ( condition.IsValid.IsFalse() )
                     {
                        allTrue = false;

                        // IMPORTANT to continue to test all elements
                     }
                  }

                  await SetIsValid( allTrue ).AndReturnToCallingContext();
               }
            } );

         return Task.CompletedTask;
      }

      public async Task SetIsValid( bool isValid )
      {
         if ( BoundHost == null )
         {
            IsValid.SetFalse();
         }

         await IsValid.SetIsTrueOrFalse( isValid, IsValidChangedTask ).AndReturnToCallingContext();
      }

      protected virtual Task AfterAttached( VisualElement bindableAsVisualElement )
      {
         return Task.CompletedTask;
      }

      protected virtual Task AfterUnattached( VisualElement bindableAsVisualElement )
      {
         return Task.CompletedTask;
      }

      protected abstract IIsValidCondition_PI[] GetValidationConditions();

      protected abstract (object, object) GetValuesFromView();

      protected override void OnAttachedTo( BindableObject bindable )
      {
         base.OnAttachedTo( bindable );

         if ( bindable is VisualElement bindableAsVisualElement )
         {
            BoundHost           =  bindableAsVisualElement;
            BoundHost.Focused   += OnFocused;
            BoundHost.Unfocused += OnUnfocused;

            AfterAttached( bindableAsVisualElement ).FireAndFuhgetAboutIt();
         }
      }

      protected override void OnDetachingFrom( BindableObject bindable )
      {
         BoundHost.Focused   -= OnFocused;
         BoundHost.Unfocused -= OnUnfocused;

         base.OnDetachingFrom( bindable );

         BoundHost = default;

         if ( bindable is VisualElement bindableAsVisualElement )
         {
            AfterUnattached( bindableAsVisualElement ).FireAndFuhgetAboutIt();
         }
      }

      protected virtual void OnFocused
      (
         object         o,
         FocusEventArgs e
      )
      {
         IsFocused = true;
      }

      protected virtual void OnUnfocused
      (
         object         o,
         FocusEventArgs e
      )
      {
         IsFocused = false;


         RevalidateAllConditions().FireAndFuhgetAboutIt();
      }

      protected async Task ResetAndRevalidateAllConditions()
      {
         var previousValidationConditions = ValidationConditions;
         ValidationConditions         = GetValidationConditions();
         _validationConditionsSetOnce = true;

         if ( ValidationConditions.IsDifferentThan( previousValidationConditions ) )
         {
            await ValidationConditionsChanged.RunAllTasksUsingDefaults().AndReturnToCallingContext();
            await RevalidateAllConditions().AndReturnToCallingContext();
         }
      }
   }
}