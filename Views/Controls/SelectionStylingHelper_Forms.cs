// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=SelectionStylingHelper_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Views.Controls
{
   using System;
   using System.Diagnostics;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Xamarin.Forms;

   public interface IHaveSelectionStylingHelper_Forms
   {
      ISelectionStylingHelper_Forms StylingHelperForms { get; set; }

      Task AfterStyleApplied();
   }

   public interface ISelectionStylingHelper_Forms
   {
      Style AlternateDeselectedStyle { get; set; }

      Style DeselectedStyle { get; set; }

      Style SelectedStyle { get; set; }

      bool CanAttach( View hostView );
   }

   public class SelectionStylingHelper_Forms : ISelectionStylingHelper_Forms
   {
      public static readonly BindableProperty AlternateDeselectedStyleProperty =
         CreateSelectionStylingHelperProperty
         (
            nameof( AlternateDeselectedStyle ),
            default( Style )
         );

      public static readonly BindableProperty DeselectedStyleProperty =
         CreateSelectionStylingHelperProperty
         (
            nameof( DeselectedStyle ),
            default( Style )
         );

      public static readonly BindableProperty SelectedStyleProperty =
         CreateSelectionStylingHelperProperty
         (
            nameof( SelectedStyle ),
            default( Style )
         );

      private Style _alternateDeselectedStyle;

      private ICanAlternate_PI _alternatePiProvider;

      private Style _deselectedStyle;

      private View _hostView;

      private ICanOverrideSelectionAndAlternation_PI _overrideProvider;

      private Style _selectedStyle;

      private ICanBeSelected_PI _selectionProvider;

      private IHaveSelectionStylingHelper_Forms _stylingHost;

      public Style AlternateDeselectedStyle
      {
         get => _alternateDeselectedStyle;
         set
         {
            _alternateDeselectedStyle = value;
            AssignCurrentStyle();
         }
      }

      public Style DeselectedStyle
      {
         get => _deselectedStyle;
         set
         {
            _deselectedStyle = value;
            AssignCurrentStyle();
         }
      }

      public Style SelectedStyle
      {
         get => _selectedStyle;
         set
         {
            _selectedStyle = value;
            AssignCurrentStyle();
         }
      }

      public bool CanAttach( View hostView )
      {
         if ( hostView is ICanBeSelected_PI hostAsSelectable )
         {
            _selectionProvider = hostAsSelectable;
         }
         else if ( hostView is IHaveSelectionProvider_PI hostAsHavingSelectionProvider )
         {
            _selectionProvider = hostAsHavingSelectionProvider.SelectionProvider;
         }
         else
         {
            Debug.WriteLine( nameof( SelectionStylingHelper_Forms ) + " constructor: " + nameof( hostView ) +
                             " must be either selectable or hostView a view that is selectable." );
            return false;
         }

         // Corner case: override cases
         if ( hostView is IHaveSelectionAndAlternationProvider_PI
            selectionProviderAsHavingSelectionAndAlternationProvider )
         {
            _overrideProvider =
               selectionProviderAsHavingSelectionAndAlternationProvider.SelectionAndAlternationPiProvider;

            _overrideProvider?.ConsiderSelectionOrAlterationOverrideTask.AddIfNotAlreadyThere(
               this, HandleByAssigningCurrentStyle );
         }

         _hostView = hostView;

         // SUCCESS
         if ( _selectionProvider is IHaveSelectionStylingHelper_Forms selectionProviderAsStylingHost )
         {
            _stylingHost = selectionProviderAsStylingHost;
         }

         // Optional
         _alternatePiProvider = _selectionProvider as ICanAlternate_PI;

         AssignCurrentStyle();

         _selectionProvider.IsSelectedChangedTask.AddIfNotAlreadyThere( this, HandleByAssigningCurrentStyle );

         if ( _alternatePiProvider.IsNotNullOrDefault() )
         {
            // ReSharper disable once PossibleNullReferenceException
            _alternatePiProvider.IsAlternateChangedTask.AddIfNotAlreadyThere( this, HandleByAssigningCurrentStyle );
         }

         return true;
      }

      public static BindableProperty CreateSelectionStylingHelperProperty<PropertyTypeT>
      (
         string localPropName, PropertyTypeT defaultVal = default,
         BindingMode bindingMode = BindingMode.OneWay,
         Action<SelectionStylingHelper_Forms, PropertyTypeT, PropertyTypeT> callbackAction = null
      )
      {
         return BindableUtils_Forms.CreateBindableProperty( localPropName, defaultVal, bindingMode, callbackAction );
      }

      private Task AssignCurrentStyle()
      {
         if ( _selectionProvider.IsNullOrDefault() )
         {
            return Task.FromResult( false );
         }

         if ( _selectionProvider.IsSelected ||
              ( _overrideProvider.IsNotNullOrDefault() && _overrideProvider.IsSelected ) )
         {
            _hostView.Style = SelectedStyle;
         }
         else if ( ( _alternatePiProvider.IsNotNullOrDefault() && AlternateDeselectedStyle.IsNotNullOrDefault() ) ||
                   ( _overrideProvider.IsNotNullOrDefault()    && _overrideProvider.IsAnAlternate ) )
         {
            _hostView.Style = AlternateDeselectedStyle;
         }
         else
         {
            _hostView.Style = DeselectedStyle;
         }

         _hostView.ForceStyle( _hostView.Style );

         return _stylingHost?.AfterStyleApplied();
      }

      private Task HandleByAssigningCurrentStyle( IResponsiveTaskParams paramDict )
      {
         return AssignCurrentStyle();
      }
   }
}