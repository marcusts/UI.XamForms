// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=FlowableUtils_Forms.cs
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
   using System.Collections.Generic;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Views.Controls;
   using Xamarin.Forms;

   /// <summary>
   /// Class StageUtils.
   /// </summary>
   /// <summary>
   /// Class StageUtils.
   /// </summary>
   public static class FlowableUtils_Forms
   {
      public static double CheckAgainstZero( this double dbl )
      {
         return Math.Max( 0, dbl );
      }

      public static ITriStateImageLabelButton_Forms CreateFlowableControlButton(
         string         text,
         Command        command,
         object         bindingContext,
         int            tabIndex,
         bool           allowDisable,
         Color?         textColor      = default,
         double?        fontSize       = null,
         FontAttributes fontAttributes = FontAttributes.None,
         Color?         backColor      = default,
         double?        extraTopSpace  = default
      )
      {
         var button =
            SimpleImageLabelButton_Forms.CreateSimpleImageLabelButton
            (
               text,
               textColor ?? Color.White,
               fontSize  ?? NamedSize.Small.ToFormsNamedSize().AdjustForOsAndDevice(), FlowableConst_Forms.BUTTON_WIDTH,
               FlowableConst_Forms.BUTTON_HEIGHT,
               bindingContext,
               backColor ?? Color.LimeGreen,
               default,
               LayoutOptions.Center,
               LayoutOptions.Center,
               labelFontAttributes: fontAttributes
            );

         button.ButtonCommand = command;
         button.SetCornerRadiusFactor( UIConst_Forms.MEDIUM_CORNER_RADIUS_FACTOR );
         ( (View)button ).IsTabStop = true;
         ( (View)button ).TabIndex  = tabIndex;
         button.CanSelect           = false;

         if ( allowDisable )
         {
            button.CanDisable          = true;
            button.ButtonDisabledStyle = ImageLabelButtonBase_Forms.CreateButtonStyle( Color.DarkGray );
            button.ButtonDeselectedStyle =
               ImageLabelButtonBase_Forms.CreateButtonStyle( backColor ?? ThemeUtils_Forms.DARK_THEME_COLOR );
         }

         if ( extraTopSpace != null )
         {
            // Cannot fail
            var buttonAsView = button as View;

            buttonAsView.Margin =
               new Thickness
               (
                  buttonAsView.Margin.Left,
                  buttonAsView.Margin.Top + extraTopSpace.GetValueOrDefault(),
                  buttonAsView.Margin.Right,
                  buttonAsView.Margin.Bottom
               );
         }

         return button;
      }

      public static async Task FadeInOrOutAndTranslateXYLoc(
         this View view,
         Rectangle nextBounds,
         uint      translateBoundsMilliseconds = FlowableConst_PI.DEFAULT_TRANSLATE_BOUNDS_MILLISECONDS,
         Easing    translateBoundsEasing       = null,
         uint      fadeInMilliseconds          = FlowableConst_PI.DEFAULT_FADE_IN_MILLISECONDS,
         Easing    fadeEasing                  = null,
         double    nextOpacity                 = UIUtils_Forms.VISIBLE_OPACITY
      )
      {
         // Defaults for x and y are 0, so they have to be considered potential changes
         if (
               nextBounds.X.IsEmpty()                        || 
               view.Bounds.X.IsDifferentThan( nextBounds.X ) || 
               nextBounds.Y.IsEmpty()                        || 
               view.Bounds.Y.IsDifferentThan( nextBounds.Y ) ||
               view.Opacity.IsDifferentThan( nextOpacity ) 
            )
         {
            var moveX = nextBounds.X - view.Bounds.X;
            var moveY = nextBounds.Y - view.Bounds.Y;

            if ( view.Opacity.IsSameAs( nextOpacity ) )
            {
               // Don't bother with the fade
               await view.TranslateTo( moveX, moveY, translateBoundsMilliseconds,
                             translateBoundsEasing ?? Easing.Linear )
                         .AndReturnToCallingContext();
            }
            else
            {
               var translateTasks = new List<Task>
                                    {
                                       view.TranslateTo( moveX, moveY, translateBoundsMilliseconds,
                                          translateBoundsEasing ?? Easing.Linear ),
                                       view.FadeTo( nextOpacity, fadeInMilliseconds, fadeEasing ?? Easing.Linear ),
                                    };
               await Task.WhenAll( translateTasks ).AndReturnToCallingContext();
            }
         }
      }

      public static Rectangle FixNegativeDimensions( this Rectangle rect )
      {
         return new Rectangle( CheckAgainstZero( rect.X ), CheckAgainstZero( rect.Y ), CheckAgainstZero( rect.Width ),
            CheckAgainstZero( rect.Height ) );
      }

      /// <summary>
      /// Musts the have at least one selection.
      /// </summary>
      /// <param name="selectionRule">The selection rule.</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
      public static bool MustHaveAtLeastOneSelection( this SelectionRules selectionRule )
      {
         return ( selectionRule == SelectionRules.MultiSelectionAtLeastOneRequired ) ||
                ( selectionRule == SelectionRules.SingleSelectionAtLeastOneRequired );
      }

      public static void SetChildViewInputTransparencies( this View view )
      {
         if ( view.IsNullOrDefault() )
         {
            return;
         }

         if ( view is ContentView viewAsContentView )
         {
            view.InputTransparent = false;
            SetChildViewInputTransparencies( viewAsContentView.Content );
         }
         else if ( view is ScrollView viewAsScrollView )
         {
            view.InputTransparent = false;
            SetChildViewInputTransparencies( viewAsScrollView.Content );
         }
         else if ( view is Layout<View> viewAsLayoutView )
         {
            view.InputTransparent = false;

            foreach ( var child in viewAsLayoutView.Children )
            {
               SetChildViewInputTransparencies( child );
            }
         }
         else if ( view is InputView viewAsInputView )
         {
            viewAsInputView.InputTransparent = false;
         }
         else
         {
            // Add a tap listener to reach top" view
            view.InputTransparent = true;
         }
      }

      //public static bool WillStayOffScreen( this IFlowableChild_Forms item )
      //{
      //   return
      //      ( item.PositionStatus == FlowableChildPositionStatuses.OffScreen )
      //      ||
      //      ( item.PositionStatus == FlowableChildPositionStatuses.FilteredOut );
      //}
   }
}