// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=ScrollableFlexView_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Views.Subviews
{
   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.Linq;
   using System.Reflection;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Controls;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Validations;
   using Com.MarcusTS.UI.XamForms.Views.Controls;
   using Xamarin.Essentials;
   using Xamarin.Forms;

   public interface IScrollableFlexView_Forms : IScrollableViewBase_Forms
   {
      Task SetDefaultEditCornerRadius( double radius );

      Task SetFontSize( double fontSize );

      Task SetItemHeight( double itemHeight );

      Task SetItemWidth( double itemWidth );
   }

   /// <summary>
   /// The original FlexView (from Com.Marcus.SharedForms), but adapted to ResponsiveTasks.
   /// This view displays input fields from a view model that provide ViewModelValidation attributes on (any) of their
   /// properties.
   /// A master scroller is optional (_useScrollView), and included by default.
   /// </summary>
   public class ScrollableFlexView_Forms : ScrollableViewBase_Forms, IScrollableFlexView_Forms
   {
      private readonly IList<ICanBeValid_PI> _allBehaviors = new List<ICanBeValid_PI>();
      private readonly IThreadSafeAccessor IsInitializing = new ThreadSafeAccessor( 0 );
      private          double _defaultEditCornerRadiusFactor = UIConst_PI.LARGE_CORNER_RADIUS_FACTOR;
      private          double _fontSize = UIConst_Forms.DEFAULT_ENTRY_FONT_SIZE;
      private          View _initiallyFocusedView;
      private          double _itemHeight = UIConst_PI.DEFAULT_ENTRY_HEIGHT;
      private          double _itemWidth = UIConst_PI.DEFAULT_ENTRY_WIDTH;
      private          IDictionary<PropertyInfo, ViewModelValidationAttribute_PI> _propInfoDict;

      public ScrollableFlexView_Forms( ICanShowProgressSpinner_Forms spinnerHost ) : base ( spinnerHost )
      {
         IsInitializing.SetTrue();

         // ReSharper disable once VirtualMemberCallInConstructor
         if ( AnimatableLayout is INotifyAfterAfterRequestingChildItemFlows animatableLayoutAsChildDrawingNotifier )
         {
            animatableLayoutAsChildDrawingNotifier.AfterFlowingToOnScreenPositionsTask.AddIfNotAlreadyThere( this,
               HandleAfterDrawingAllChildItemsTask );
         }

         EndOfConstruction();
      }

      protected virtual IValidationManager_Forms CurrentCustomizations { get; } = new ValidationManager_Forms();

      private IHaveValidationViewModelHelper ViewModelAsValidator =>
         BindingContext as IHaveValidationViewModelHelper;

      public async Task SetDefaultEditCornerRadius( double radius )
      {
         if ( _defaultEditCornerRadiusFactor.IsDifferentThan( radius ) )
         {
            _defaultEditCornerRadiusFactor = radius;
            await RecreateScrollableViewUI().WithoutChangingContext();
         }
      }

      public async Task SetFontSize( double fontSize )
      {
         if ( _fontSize.IsDifferentThan( fontSize ) )
         {
            _fontSize = fontSize;
            await RecreateScrollableViewUI().WithoutChangingContext();
         }
      }

      public async Task SetItemHeight( double itemHeight )
      {
         if ( _itemHeight.IsDifferentThan( itemHeight ) )
         {
            _itemHeight = itemHeight;
            await RecreateScrollableViewUI().WithoutChangingContext();
         }
      }

      public async Task SetItemWidth( double itemWidth )
      {
         if ( _itemWidth.IsDifferentThan( itemWidth ) )
         {
            _itemWidth = itemWidth;
            await RecreateScrollableViewUI().WithoutChangingContext();
         }
      }

      protected virtual async Task<(IValidatableView_Forms, ICanBeValid_PI, int)> CreateEditableEntry
         ( IViewModelValidationAttribute_PI attribute, int retTabIndex )
      {
         ( var validatableViewForms, var canBeValidPi, var tabIndex ) =
            await CurrentCustomizations.CreateValidatableEditorsForAttribute
            (
               ViewModelAsValidator,
               attribute,
               _itemHeight,
               _itemWidth,
               _fontSize,
               retTabIndex
            ).WithoutChangingContext();

         return ( validatableViewForms, canBeValidPi, tabIndex );
      }

      protected override async Task<(bool, BetterObservableCollection<View>)> CreateScrollableViews()
      {
         _initiallyFocusedView = default;

         if ( ViewModelAsValidator.IsNotNullOrDefault() && _allBehaviors.IsNotAnEmptyList() )
         {
            await ViewModelAsValidator.ValidationHelper.RemoveBehaviorsWithoutNotification( _allBehaviors.ToArray() )
                                      .WithoutChangingContext();
         }

         _allBehaviors.Clear();

         var retViews = new BetterObservableCollection<View>();

         if ( _propInfoDict.IsNotAnEmptyList() )
         {
            var nextTabIndex = 0;

            foreach ( var keyValuePair in _propInfoDict.OrderBy( kvp => kvp.Value.DisplayOrder ) )
            {
               ( var validatableViewForms, var viewValidator, var lastTabIndex ) =
                  await CreateEditableEntry( keyValuePair.Value, nextTabIndex ).WithoutChangingContext();
               nextTabIndex = lastTabIndex;

               var view = validatableViewForms as View;

               if ( view.IsNullOrDefault() )
               {
                  Debug.WriteLine( nameof( CreateScrollableViews )                   + ": " +
                                   "ERROR: Could not create an editable view for ->" +
                                   keyValuePair.Value.ViewModelPropertyName          + "<-" );
                  continue;
               }


               // ELSE SUCCESS
               if ( keyValuePair.Value.IsInitialFocus.IsTrue() &&
                    validatableViewForms.EditableView.IsNotNullOrDefault() )
               {
                  _initiallyFocusedView = validatableViewForms.EditableView;
               }

               await validatableViewForms.BorderView.SetCornerRadiusFactor( _defaultEditCornerRadiusFactor )
                                         .WithoutChangingContext();

               // ReSharper disable once PossibleNullReferenceException
               view.BindingContext = BindingContext;

               // HACK around broken binding
               if ( view is IValidatableDateTime_Forms viewAsValidatableDateTime &&
                    BindingContext is IHandleNullableDateTimeChangeTask bindingContextAsNullableDateTimeHandler )
               {
                  viewAsValidatableDateTime.NullableResultChangedTask.AddIfNotAlreadyThere(
                     bindingContextAsNullableDateTimeHandler,
                     bindingContextAsNullableDateTimeHandler.HandleNullableResultChangeTask );
               }

               retViews.Add( view );

               if ( viewValidator.IsNotNullOrDefault() )
               {
                  _allBehaviors.Add( viewValidator );
               }
               else
               {
                  // The validator does not apply to optional fields OR boolean-style check boxes. So just issuing a warning.
                  Debug.WriteLine(
                     nameof( CreateScrollableViews )                 +
                     ": did not require a validator for property ->" +
                     keyValuePair.Key.Name                           + "<-" );
               }
            }
         }

         // Bubble validations up to the view model and commands
         if ( ViewModelAsValidator.IsNotNullOrDefault() && _allBehaviors.IsNotAnEmptyList() )
         {
            await ViewModelAsValidator.ValidationHelper.AddBehaviors( _allBehaviors.ToArray() )
                                      .WithoutChangingContext();
         }

         return ( true, retViews );
      }

      protected override async Task EndInitialization()
      {
         if ( IsInitializing.IsTrue() )
         {
            return;
         }

         // ELSE
         await base.EndInitialization();
      }

      protected override async Task HandlePostBindingContextTask( IResponsiveTaskParams paramDict )
      {
         // Get our dictionary done FIRST
         _propInfoDict = BindingContext?.GetType()
                                        .CreateViewModelCustomAttributeDict<ViewModelValidationAttribute_PI>();

         // Then the base
         await base.HandlePostBindingContextTask( paramDict );
      }

      private void EndOfConstruction()
      {
         IsInitializing.SetFalse();
         EndInitialization().FireAndFuhgetAboutIt();
      }

      private Task HandleAfterDrawingAllChildItemsTask( IResponsiveTaskParams paramdict )
      {
         if ( _initiallyFocusedView.IsNotNullOrDefault() )
         {
            try
            {
               Task.Run
               (
                  () =>
                  {
                     MainThread.BeginInvokeOnMainThread(

                        // ReSharper disable once AsyncVoidLambda
                        () =>
                        {
                           // Go back to editing
                           _initiallyFocusedView.Focus();
                        } );
                  }
               );
            }
            catch ( Exception ex )
            {
               Debug.WriteLine( nameof( HandleAfterDrawingAllChildItemsTask ) + " error ->" + ex.Message + "<-" );
            }
         }

         return Task.CompletedTask;
      }
   }
}