// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=UIUtils_Forms.cs
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
   using System.Diagnostics;
   using System.Globalization;
   using System.Linq;
   using System.Reflection;
   using System.Text;
   using System.Threading;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Annotations;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Views.Controls;
   using Com.MarcusTS.UI.XamForms.Views.Subviews;
   using Xamarin.Essentials;
   using Xamarin.Forms;

   public static class UIUtils_Forms
   {
      /// <summary>Enum OffScreenPositions</summary>
      public enum OffScreenPositions
      {
         /// <summary>The none</summary>
         NONE,


         /// <summary>The left</summary>
         LEFT,

         /// <summary>The top</summary>
         TOP,

         /// <summary>The right</summary>
         RIGHT,

         /// <summary>The bottom</summary>
         BOTTOM,
      }

      /// <summary>Enum OnScreenPositions</summary>
      public enum OnScreenPositions
      {
         /// <summary>The none</summary>
         NONE,

         /// <summary>The left center</summary>
         LEFT_CENTER,

         /// <summary>The top left</summary>
         TOP_LEFT, // Same as Left_Upper

         /// <summary>The top center</summary>
         TOP_CENTER,

         /// <summary>The top right</summary>
         TOP_RIGHT, // Same as Right_Upper

         /// <summary>The right center</summary>
         RIGHT_CENTER,

         /// <summary>The bottom left</summary>
         BOTTOM_LEFT, // Same as Left_Lower

         /// <summary>The bottom center</summary>
         BOTTOM_CENTER,

         /// <summary>The bottom right</summary>
         BOTTOM_RIGHT, // Same as Right_Lower

         /// <summary>The center</summary>
         CENTER,
      }

      /// <summary>Enum StageHeaderPositions</summary>
      public enum StageHeaderPositions
      {
         /// <summary>The none</summary>
         NONE,

         /// <summary>The top</summary>
         TOP,
      }

      /// <summary>Enum StageToolbarPositions</summary>
      public enum StageToolbarPositions
      {
         /// <summary>The none</summary>
         NONE,

         /// <summary>The bottom</summary>
         BOTTOM,

         /// <summary>The left</summary>
         LEFT,

         /// <summary>The top</summary>
         TOP,

         /// <summary>The right</summary>
         RIGHT,
      }

      /// <summary>Enum SubStageFlowDirections</summary>
      public enum SubStageFlowDirections
      {
         /// <summary>The left to right</summary>
         LEFT_TO_RIGHT,

         /// <summary>The right to left</summary>
         RIGHT_TO_LEFT,

         /// <summary>The top to bottom</summary>
         TOP_TO_BOTTOM,

         /// <summary>The bottom to top</summary>
         BOTTOM_TO_TOP,
      }

      /// <summary>Enum ViewAlignments</summary>
      public enum ViewAlignments
      {
         /// <summary>The start</summary>
         START,

         /// <summary>The middle</summary>
         MIDDLE,

         /// <summary>The end</summary>
         END,

         /// <summary>The justify</summary>
         JUSTIFY,
      }

      public const string CREDIT_CARD_TEXT_MASK = "XXXX-XXXX-XXXX-XXXX";

      public const string DATE_TIME_FORMAT = "{0:M/d/yy - h:mm tt}";

      /// <summary>The ios top margin</summary>
      public const float IOS_TOP_MARGIN = 40;

      public const string LONG_DATE_FORMAT = "{0:MMM d, yyyy}";

      public const double NO_SCALE = 0;

      public const double NORMAL_SCALE = 1;

      /// <summary>The not visible opacity</summary>
      public const double NOT_VISIBLE_OPACITY = 0;

      public const string SERVER_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";

      public const string SHORT_DATE_FORMAT = "{0:M/d/yy}";

      public const string SIMPLE_DATE_FORMAT = "{0:M/d/yyyy}";

      /// <summary>The visible opacity</summary>
      public const double VISIBLE_OPACITY = 1;

      // 0.85 works for denser copy
      private const double CHARACTERS_TO_FONT_SIZE_ESTIMATOR = 1;

      public static void AddAndSetRowsAndColumns( this Grid grid, View view, int? row = default,
                                                  int?      column  = default,
                                                  int?      rowSpan = default, int? colSpan = default )
      {
         grid.Children.Add( view );

         if ( row != null )
         {
            Grid.SetRow( view, row.GetValueOrDefault() );
         }

         if ( column != null )
         {
            Grid.SetColumn( view, column.GetValueOrDefault() );
         }

         if ( rowSpan != null )
         {
            Grid.SetRowSpan( view, rowSpan.GetValueOrDefault() );
         }

         if ( colSpan != null )
         {
            Grid.SetColumnSpan( view, colSpan.GetValueOrDefault() );
         }
      }

      /*
      /// <summary>
      /// Converts horizontal and vertical LayoutOptions into grid rows, columns and column spans, assuming two rows and two
      /// columns
      /// </summary>
      /// <param name="horizontalAlign"></param>
      /// <param name="verticalAlign"></param>
      /// <returns>Succeeded, Row, RowSpan, Column, ColumnSpan</returns>
      public static (bool, int, int, int, int) GetPositionInGridQuad( LayoutOptions horizontalAlign,
                                                                      LayoutOptions verticalAlign )
      {
         var retRow       = -1;
         var retRowSpan   = -1;
         var retColumn    = -1;
         var retColumnSpan = -1;

         if ( ( horizontalAlign.Alignment == LayoutAlignment.Center ) || ( horizontalAlign.Alignment == LayoutAlignment.Fill ) )
         {
            retColumn     = 0;
            retColumnSpan = 2;
         }

         if ( horizontalAlign.Alignment == LayoutAlignment.End )
         {
            retColumn     = 1;
            retColumnSpan = 1;
         }

         if ( horizontalAlign.Alignment == LayoutAlignment.Start )
         {
            retColumn     = 0;
            retColumnSpan = 1;
         }

         if ( ( verticalAlign.Alignment == LayoutAlignment.Center ) || ( verticalAlign.Alignment == LayoutAlignment.Fill ) )
         {
            retRow     = 0;
            retRowSpan = 2;
         }

         if ( verticalAlign.Alignment == LayoutAlignment.End )
         {
            retRow     = 1;
            retRowSpan = 1;
         }

         if ( verticalAlign.Alignment == LayoutAlignment.Start )
         {
            retRow     = 0;
            retRowSpan = 1;
         }

         var success = ( ( retRow >= 0 ) && ( retRowSpan >= 0 ) && ( retColumn >= 0 ) && ( retColumnSpan >= 0 ) );

         // FAIL CASE
         return 
            ( 
               success,
               retRow, 
               retRowSpan,
               retColumn, 
               retColumnSpan 
            );
      }
      */

      /// <summary>Adds the animation and haptic feedback.</summary>
      /// <param name="view">The view.</param>
      /// <param name="animate">if set to <c>true</c> [animate].</param>
      /// <param name="vibrate">if set to <c>true</c> [vibrate].</param>
      /// <returns>Task.</returns>
      public static async Task AddAnimationAndHapticFeedback
      (
         this View view,
         bool      animate = true,
         bool      vibrate = true
      )
      {
         if ( animate )
         {
            await view.ScaleTo( UIConst_PI.ANIMATION_BOUNCE_SCALE, UIConst_PI.BUTTON_BOUNCE_MILLISECONDS, Easing.CubicOut ).WithoutChangingContext();
            await view.ScaleTo( 1,    UIConst_PI.BUTTON_BOUNCE_MILLISECONDS, Easing.CubicIn ).WithoutChangingContext();
         }

         if ( vibrate )
         {
            // var v = CrossVibrate.Current; v.Vibration(TimeSpan.FromMilliseconds(UIConst_PI.HAPTIC_VIBRATION_MILLISECONDS));
            Vibration.Vibrate( TimeSpan.FromMilliseconds( UIConst_PI.HAPTIC_VIBRATION_MILLISECONDS ) );
         }
      }

      /// <summary>Adds the automatic column.</summary>
      /// <param name="grid">The grid.</param>
      public static void AddAutoColumn( this Grid grid )
      {
         grid.ColumnDefinitions.Add( new ColumnDefinition { Width = GridLength.Auto, } );
      }

      /// <summary>Adds the automatic row.</summary>
      /// <param name="grid">The grid.</param>
      public static void AddAutoRow( this Grid grid )
      {
         grid.RowDefinitions.Add( new RowDefinition { Height = GridLength.Auto, } );
      }

      /// <summary>Adds the fixed column.</summary>
      /// <param name="grid">The grid.</param>
      /// <param name="width">The width.</param>
      public static void AddFixedColumn
      (
         this Grid grid,
         double    width
      )
      {
         grid.ColumnDefinitions.Add( new ColumnDefinition { Width = width, } );
      }

      /// <summary>Adds the fixed row.</summary>
      /// <param name="grid">The grid.</param>
      /// <param name="height">The height.</param>
      public static void AddFixedRow
      (
         this Grid grid,
         double    height
      )
      {
         grid.RowDefinitions.Add( new RowDefinition { Height = height, } );
      }

      public static void AddIfNotEmpty( this StringBuilder mainStrBuilder, string appendStr, string prefix = "" )
      {
         if ( appendStr.IsNotEmpty() )
         {
            if ( mainStrBuilder.ToString().IsNotEmpty() )
            {
               mainStrBuilder.Append( prefix );
            }

            mainStrBuilder.Append( appendStr );
         }
      }

      public static void AddOverlayBasedOnPosition( this RelativeLayout layout,
                                                    View                view,
                                                    OnScreenPositions   position,
                                                    double              viewWidth, double viewHeight )
      {
         if ( position == OnScreenPositions.NONE )
         {
            return;
         }

         switch ( position )
         {
            case OnScreenPositions.CENTER:
               layout.Children.Add
               (
                  view,
                  GetHorizontallyCenteredConstraint(),
                  GetVerticallyCenteredConstraint()
               );
               break;

            case OnScreenPositions.BOTTOM_CENTER:
               layout.Children.Add
               (
                  view,
                  GetHorizontallyCenteredConstraint(),
                  GetBottomVerticalConstraint()
               );
               break;

            case OnScreenPositions.BOTTOM_LEFT:
               layout.Children.Add
               (
                  view,
                  GetLeftHorizontalConstraint(),
                  GetBottomVerticalConstraint()
               );
               break;

            case OnScreenPositions.BOTTOM_RIGHT:
               layout.Children.Add
               (
                  view,
                  GetRightHorizontalConstraint(),
                  GetBottomVerticalConstraint()
               );
               break;

            case OnScreenPositions.TOP_CENTER:
               layout.Children.Add
               (
                  view,
                  GetHorizontallyCenteredConstraint(),
                  GetTopVerticalConstraint()
               );
               break;

            case OnScreenPositions.TOP_LEFT:
               layout.Children.Add
               (
                  view,
                  GetLeftHorizontalConstraint(),
                  GetTopVerticalConstraint()
               );
               break;

            case OnScreenPositions.TOP_RIGHT:
               layout.Children.Add
               (
                  view,
                  GetRightHorizontalConstraint(),
                  GetTopVerticalConstraint()
               );
               break;

            case OnScreenPositions.LEFT_CENTER:
               layout.Children.Add
               (
                  view,
                  GetLeftHorizontalConstraint(),
                  GetVerticallyCenteredConstraint()
               );
               break;

            case OnScreenPositions.RIGHT_CENTER:
               layout.Children.Add
               (
                  view,
                  GetRightHorizontalConstraint(),
                  GetVerticallyCenteredConstraint()
               );
               break;
         }

         Constraint GetHorizontallyCenteredConstraint()
         {
            return Constraint.RelativeToParent( relativeLayout => ( relativeLayout.Width - viewWidth ) / 2 );
         }

         Constraint GetVerticallyCenteredConstraint()
         {
            return Constraint.RelativeToParent( relativeLayout => ( relativeLayout.Height - viewHeight ) / 2 );
         }

         Constraint GetBottomVerticalConstraint()
         {
            return Constraint.RelativeToParent( relativeLayout => relativeLayout.Height - viewHeight );
         }

         Constraint GetTopVerticalConstraint()
         {
            return Constraint.Constant( 0 );
         }

         Constraint GetLeftHorizontalConstraint()
         {
            return Constraint.Constant( 0 );
         }

         Constraint GetRightHorizontalConstraint()
         {
            return Constraint.RelativeToParent( relativeLayout => relativeLayout.Width - viewWidth );
         }
      }

      public static void AddRowContent( this Grid grid, View view )
      {
         // The count is zero-based, so before we add, the physical count is the same as the "next" count
         var nextRow = grid.Children.Count;
         grid.AddAutoRow();
         grid.Children.Add( view );
         Grid.SetRow( view, nextRow );
      }

      /// <summary>Adds the star column.</summary>
      /// <param name="grid">The grid.</param>
      /// <param name="factor">The factor.</param>
      public static void AddStarColumn
      (
         this Grid grid,
         double    factor = 1
      )
      {
         grid.ColumnDefinitions.Add( new ColumnDefinition { Width = new GridLength( factor, GridUnitType.Star ), } );
      }

      /// <summary>Adds the star row.</summary>
      /// <param name="grid">The grid.</param>
      /// <param name="factor">The factor.</param>
      public static void AddStarRow
      (
         this Grid grid,
         double    factor = 1
      )
      {
         grid.RowDefinitions.Add( new RowDefinition { Height = new GridLength( factor, GridUnitType.Star ), } );
      }

      /// <summary>Adjusts for screen height b u g.</summary>
      /// <param name="originalHeight">Height of the original.</param>
      /// <returns>System.Single.</returns>
      public static float AdjustedForScreenHeightBug( this float originalHeight )
      {
         switch ( Device.RuntimePlatform )
         {
            case Device.iOS:
               return originalHeight - 52;

            case Device.Android:
               // return originalHeight - 24;
               return originalHeight - 39;
         }

         return originalHeight;
      }

      public static void AnimateHeightChange(
         this View view,
         double    nextHeight,
         uint      fadeMilliseconds = 250,
         Easing    easing           = null )
      {
         // Nothing to do
         if ( view.Height.IsSameAs( nextHeight ) )
         {
            return;
         }

         var fadeAnimation = new Animation( f => view.HeightRequest = f,
            view.Opacity,
            nextHeight, easing );
         fadeAnimation.Commit( view, "fadeAnimation", length: fadeMilliseconds, easing: easing );
      }

      public static void AnimateWidthChange(
         this View view,
         double    nextWidth,
         uint      fadeMilliseconds = 250,
         Easing    easing           = null )
      {
         // Nothing to do
         if ( view.Width.IsSameAs( nextWidth ) )
         {
            return;
         }

         var fadeAnimation = new Animation( f => view.WidthRequest = f,
            view.Opacity,
            nextWidth, easing );
         fadeAnimation.Commit( view, "fadeAnimation", length: fadeMilliseconds, easing: easing );
      }

      /// <summary>Boundses the are valid and have changed.</summary>
      /// <param name="bounds">The bounds.</param>
      /// <param name="propName">Name of the property.</param>
      /// <param name="lastBounds">The last bounds.</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
      public static bool AreValidAndHaveChanged
      (
         this Rectangle bounds,
         string         propName,
         Rectangle      lastBounds
      )
      {
         return propName.IsBoundsRelatedPropertyChange() && bounds.IsPositive() && bounds.IsDifferentThan( lastBounds );
      }

      public static bool IsValidAndHasChanged
      (
         this double propVal,
         string         propName,
         double lastVal
      )
      {
         return propName.IsBoundsRelatedPropertyChange() && propVal.IsANumberGreaterThanZero() && propVal.IsDifferentThan( lastVal );
      }

      /// <summary>Assigns the internal date time properties.</summary>
      /// <param name="picker">The picker.</param>
      /// <param name="itemHeight">Height of the item.</param>
      /// <param name="fontSize">Size of the font.</param>
      /// <param name="nextTabIndex">Index of the next tab.</param>
      /// <returns>System.Int32.</returns>
      public static int AssignInternalDateTimeProperties( this DatePicker picker, double itemHeight, double fontSize,
                                                          int             nextTabIndex )
      {
         picker.FontSize = fontSize;

         nextTabIndex = picker.AssignInternalViewProperties( itemHeight, nextTabIndex );

         return nextTabIndex;
      }

      public static int AssignInternalEntryProperties( this Entry entry, double itemHeight, double fontSize,
                                                       int        nextTabIndex )
      {
         entry.FontSize = fontSize;

         nextTabIndex = entry.AssignInternalViewProperties( itemHeight, nextTabIndex );
         return nextTabIndex;
      }

      /// <summary>Assigns the internal picker properties.</summary>
      /// <param name="picker">The picker.</param>
      /// <param name="itemHeight">Height of the item.</param>
      /// <param name="fontSize">Size of the font.</param>
      /// <param name="nextTabIndex">Index of the next tab.</param>
      /// <returns>System.Int32.</returns>
      public static int AssignInternalPickerProperties( this Picker picker, double itemHeight, double fontSize,
                                                        int         nextTabIndex )
      {
         picker.FontSize = fontSize;
         nextTabIndex    = picker.AssignInternalViewProperties( itemHeight, nextTabIndex );

         return nextTabIndex;
      }

      /// <summary>Assigns the internal view properties.</summary>
      /// <param name="view">The view.</param>
      /// <param name="itemHeight">Height of the item.</param>
      /// <param name="nextTabIndex">Index of the next tab.</param>
      /// <returns>System.Int32.</returns>
      public static int AssignInternalViewProperties( this View view, double itemHeight, int nextTabIndex )
      {
         // view.HeightRequest     = itemHeight;
         view.HorizontalOptions = LayoutOptions.FillAndExpand;

         if ( view is IValidatableView_Forms viewAsValidatable )
         {
            nextTabIndex = viewAsValidatable.SetTabIndexes( nextTabIndex );
         }
         else
         {
            view.IsTabStop = true;
            view.TabIndex  = nextTabIndex++;
         }

         return nextTabIndex;
      }

      /// <summary>Checks the against zero.</summary>
      /// <param name="dbl">The double.</param>
      /// <returns>System.Double.</returns>
      public static double CheckAgainstZero( this double dbl )
      {
         return Math.Max( 0, dbl );
      }

      /// <summary>Clears the completely.</summary>
      /// <param name="grid">The grid.</param>
      public static void ClearCompletely( this Grid grid )
      {
         grid.Children.Clear();
         grid.ColumnDefinitions.Clear();
         grid.RowDefinitions.Clear();
      }

      /// <summary>Compresses the date.</summary>
      /// <param name="dt">The dt.</param>
      /// <returns>System.String.</returns>
      public static string CompressedDate( this DateTime dt )
      {
         return $"{dt:M/d/yy}";
      }

      public static void ConsiderBindings(
         this View                   view,
         BindableProperty            bindableProperty,
         string                      bindingSourcePropName,
         [CanBeNull] object          bindingSource      = null,
         [CanBeNull] string          stringFormat       = "",
         BindingMode                 bindingMode        = BindingMode.OneWay,
         [CanBeNull] IValueConverter converter          = null,
         [CanBeNull] object          converterParameter = null
      )
      {
         view.SetUpBinding
         (
            bindableProperty,
            bindingSourcePropName,
            bindingMode,
            converter,
            converterParameter,
            stringFormat,
            bindingSource
         );
      }

      public static void ConsiderBindings(
         this Label                  label,
         string                      bindingSourcePropName,
         [CanBeNull] object          labelBindingSource = null,
         [CanBeNull] string          stringFormat       = "",
         BindingMode                 bindingMode        = BindingMode.OneWay,
         [CanBeNull] IValueConverter converter          = null,
         [CanBeNull] object          converterParameter = null
      )
      {
         label.ConsiderBindings
         (
            Label.TextProperty,
            bindingSourcePropName,
            labelBindingSource,
            stringFormat,
            bindingMode,
            converter,
            converterParameter
         );
      }

      public static Style CreateLabelStyle(
         string          fontFamily     = "",
         double?         fontSize       = null, Color? backColor = null,
         Color?          textColor      = null,
         FontAttributes? fontAttributes = null )
      {
         var retStyle = new Style( typeof( Label ) );

         if ( fontFamily.IsNotEmpty() )
         {
            retStyle.Setters.Add( new Setter { Property = Label.FontFamilyProperty, Value = fontFamily, } );
         }

         if ( fontSize != null )
         {
            retStyle.Setters.Add( new Setter { Property = Label.FontSizeProperty, Value = fontSize, } );
         }

         if ( backColor != null )
         {
            retStyle.Setters.Add( new Setter { Property = VisualElement.BackgroundColorProperty, Value = backColor, } );
         }

         if ( textColor != null )
         {
            retStyle.Setters.Add( new Setter { Property = Label.TextColorProperty, Value = textColor, } );
         }

         if ( fontAttributes != null )
         {
            retStyle.Setters.Add( new Setter { Property = Label.FontAttributesProperty, Value = fontAttributes, } );
         }

         return retStyle;
      }

      public static void CreateRelativeOverlay
      (
         this RelativeLayout layout,
         View                viewToAdd,
         Thickness           padding = default
      )
      {
         layout.Children.Add(
            viewToAdd, Constraint.Constant( padding.Left ), Constraint.Constant( padding.Top ),
            Constraint.RelativeToParent( parent => parent.Width  - padding.Left - padding.Right ),
            Constraint.RelativeToParent( parent => parent.Height - padding.Top  - padding.Bottom ) );
      }

      /// <summary>Creates the shape view style.</summary>
      /// <param name="backColor">Color of the back.</param>
      /// <param name="cornerRadius">The corner radius.</param>
      /// <param name="borderColor">Color of the border.</param>
      /// <param name="borderThickness">The border thickness.</param>
      /// <returns>Style.</returns>
      public static Style CreateShapeViewStyle(
         Color?  backColor       = null,
         double? cornerRadius    = null,
         Color?  borderColor     = null,
         double? borderThickness = null )
      {
         var retStyle = new Style( typeof( ShapeView_Forms ) );
         retStyle.SetRoundedCornerContentViewStyle( backColor, cornerRadius, borderColor, borderThickness );
         return retStyle;
      }

      /// <summary>Creates the tasks.</summary>
      /// <param name="tasks">The tasks.</param>
      /// <returns>Task[].</returns>
      public static Task[] CreateTasks( params Task[] tasks )
      {
         var retTasks = new List<Task>();

         foreach ( var action in tasks )
         {
            retTasks.Add( action );
         }

         return retTasks.ToArray();
      }

      /// <summary>
      /// IMPORTANT: This currently returns null, but there is no fix.  The null is harmless.
      /// </summary>
      /// <returns></returns>
      public static string DefaultFontFamily()
      {
         var retFontFamily = Font.SystemFontOfSize( NamedSize.Default ).FontFamily;

         var test = Font.SystemFontOfSize( 1d ).FontFamily;

         return retFontFamily;
      }

      public static bool DoesNotRepeatCharacters( this string str, int maxCharsAllowed )
      {
         if ( str.IsEmpty() )
         {
            return false;
         }

         for ( var idx = maxCharsAllowed; idx < str.Length; idx++ )
         {
            // Test the idx and characters directly to its left The min idx is the exact characters allowed:
            // Assuming 2 consecutive characters are allowed - 3 is a violation --

            // ABCDDD

            // The idx finally gets to the end of the string, so is at idx 5 We start at 4, one to the left (no need to
            // check the idx we are on). We end at 3 because that's all we need for a violation: The character "D" at
            // indexes 3, 4 and 5
            for ( var subIdx = idx - maxCharsAllowed; subIdx <= idx; subIdx++ )
            {
               // Not the same, so not illegal
               if ( str[ idx ] != str[ subIdx ] )
               {
                  break;
               }

               // All characters matched
               if ( subIdx == idx )
               {
                  return false;
               }

               // Else continue checking
            }
         }

         return true;
      }

      //   return retColor;
      //}
      /// <remarks>This is rough and inductive.</remarks>
      public static double EstimateHeight( this Label label, double width )
      {
         if ( label.IsNullOrDefault() || !width.IsGreaterThan( 0 ) )
         {
            return 0;
         }

         var length      = label.Text.Length;
         var totalLength = length      * label.FontSize * CHARACTERS_TO_FONT_SIZE_ESTIMATOR;
         var totalLines  = totalLength / width;

         return totalLines * label.FontSize; // * (Math.Max(label.LineHeight, label.FontSize));
      }

      public static string Expanded( this string str, int spaceCount = 1 )
      {
         if ( str.IsEmpty() )
         {
            return "";
         }

         var strBuilder  = new StringBuilder();
         var lastCharIdx = str.Length - 1;

         for ( var charIdx = 0; charIdx < str.Length; charIdx++ )
         {
            var ch = str[ charIdx ];

            strBuilder.Append( ch );

            if ( charIdx == lastCharIdx )
            {
               break;
            }

            // ELSE if not the last character
            for ( var idx = 0; idx < spaceCount; idx++ )
            {
               strBuilder.Append( UIUtils_PI.SPACE_CHAR );
            }
         }

         // We end up with a trailing space
         return strBuilder.ToString().Trim();
      }

      public static
#if DEFEAT_FADES
#elif ANIMATE_FADES
#else
         async
#endif
         void
         FadeIn(
            this View view,
            uint      fadeMilliseconds = 250,
            Easing    easing           = null,
            double    maxOpacity       = VISIBLE_OPACITY)
      {
         // Nothing to do
         if (view.Opacity.IsSameAs(maxOpacity))
         {
            return;
         }

#if DEFEAT_FADES
         view.Opacity = maxOpacity;
#elif ANIMATE_FADES
         var fadeAnimation = new Animation(f => view.Opacity = f,
                                                 view.Opacity,
                                                 maxOpacity, easing);
         fadeAnimation.Commit(view, "fadeAnimation", length:fadeMilliseconds, easing:easing);
#else
         await view.FadeTo(maxOpacity, fadeMilliseconds, easing ?? Easing.CubicIn).WithoutChangingContext();
#endif
      }

      /// <summary>Fades the out.</summary>
      /// <param name="view">The view.</param>
      /// <param name="fadeMilliseconds"></param>
      /// <param name="easing"></param>
      /// <returns>Task.</returns>
      public static
#if DEFEAT_FADES
#elif ANIMATE_FADES
#else
         async
#endif
         void FadeOut(
            this View view,
            uint      fadeMilliseconds = 250,
            Easing    easing           = null)
      {
         // Nothing to do
         if (view.Opacity.IsSameAs(NOT_VISIBLE_OPACITY))
         {
            return;
         }

#if DEFEAT_FADES
         view.Opacity = NOT_VISIBLE_OPACITY;
#elif ANIMATE_FADES
         var fadeAnimation = new Animation(f => view.Opacity = f,
                                           view.Opacity,
                                           NOT_VISIBLE_OPACITY, easing);
         fadeAnimation.Commit(view, "fadeAnimation", length:fadeMilliseconds, easing:easing);
#else
         await view.FadeTo(NOT_VISIBLE_OPACITY, fadeMilliseconds, easing ?? Easing.CubicOut).WithoutChangingContext();
#endif
      }

      public static Rectangle ForceAspect( this Rectangle rect, double aspect )
      {
         var currentAspect = rect.Width / rect.Height;
         var newWidth      = rect.Width;
         var newHeight     = rect.Height;

         if ( currentAspect.IsSameAs( aspect ) )
         {
            return rect;
         }

         if ( currentAspect < aspect )
         {
            // Too narrow; must shorten
            newHeight = rect.Width / aspect;
         }
         else
         {
            // ELSE currentAspect > aspect
            newWidth = rect.Height * aspect;
         }

         var heightDiff = Math.Max( 0, rect.Height - newHeight );
         var widthDiff  = Math.Max( 0, rect.Width  - newWidth );

         return new Rectangle( rect.X + ( widthDiff / 2 ), rect.Y + ( heightDiff / 2 ), newWidth, newHeight );
      }

      /// <summary>Forces the style.</summary>
      /// <param name="view">The view.</param>
      /// <param name="style">The style.</param>
      public static void ForceStyle
      (
         this View view,
         Style     style
      )
      {
         if ( ( style == null ) || style.Setters.IsEmpty() )
         {
            return;
         }

#if DEBUG
         var viewProperties = view.GetType().GetProperties();
#endif

         // ReSharper disable once ForCanBeConvertedToForeach
         for ( var setterIdx = 0; setterIdx < style.Setters.Count; setterIdx++ )
         {
#if DEBUG
            var viewProperty =
               viewProperties.FirstOrDefault(
                  p => p.Name.IsSameAs( style.Setters[ setterIdx ].Property.PropertyName ) );

            if ( viewProperty.IsNullOrDefault() )
            {
               Debug.WriteLine( nameof( ForceStyle )                             +
                                ": could not find the property name ->"          +
                                style.Setters[ setterIdx ].Property.PropertyName + "<- on view" );
               continue;
            }

            var targetPropertyType = style.Setters[ setterIdx ].Property.ReturnType;

            // ReSharper disable once PossibleNullReferenceException
            if ( viewProperty.PropertyType != targetPropertyType )
            {
               Debug.WriteLine( nameof( ForceStyle )                                  +
                                ": view property ->"                                  +
                                style.Setters[ setterIdx ].Property.PropertyName      +
                                "<- shows as type ->"                                 +
                                viewProperty.PropertyType                             +
                                "<- which does not match the setter property type ->" +
                                targetPropertyType                                    + "<-" );
               continue;
            }

#endif

            view.SetValue( style.Setters[ setterIdx ].Property, style.Setters[ setterIdx ].Value );
         }
      }

      /// <summary>Gets the expanding absolute layout.</summary>
      /// <returns>AbsoluteLayout.</returns>
      public static AbsoluteLayout GetExpandingAbsoluteLayout()
      {
         return new AbsoluteLayout
                {
                   HorizontalOptions = LayoutOptions.FillAndExpand,
                   VerticalOptions   = LayoutOptions.FillAndExpand,
                   BackgroundColor   = Color.Transparent,
                };
      }

      /// <summary>Gets the expanding grid.</summary>
      /// <returns>Grid.</returns>
      public static Grid GetExpandingGrid()
      {
         var retGrid = new Grid();
         retGrid.SetDefaults();
         return retGrid;
      }

      /// <summary>
      /// Creates columns to enforce alignment
      /// </summary>
      /// <param name="view"></param>
      /// <returns></returns>
      public static Grid ConvertToHorizontallyAlignGrid( this View view )
      {
         var grid = GetExpandingGrid();

         if ( view.HorizontalOptions.Alignment == LayoutAlignment.Start )
         {
            grid.AddAutoColumn();
            grid.AddStarColumn();
            grid.AddAndSetRowsAndColumns( view );
         }
         else if ( view.HorizontalOptions.Alignment == LayoutAlignment.Center )
         {
            grid.AddStarColumn();
            grid.AddAutoColumn();
            grid.AddStarColumn();
            grid.AddAndSetRowsAndColumns( view, column: 1 );
         }
         else if ( view.HorizontalOptions.Alignment == LayoutAlignment.End )
         {
            grid.AddStarColumn();
            grid.AddAutoColumn();
            grid.AddAndSetRowsAndColumns( view, column: 1 );
         }

         return grid;
      }

      /// <summary>Gets the expanding relative layout.</summary>
      /// <returns>RelativeLayout.</returns>
      public static RelativeLayout GetExpandingRelativeLayout()
      {
         return new RelativeLayout
                {
                   HorizontalOptions = LayoutOptions.FillAndExpand,
                   VerticalOptions   = LayoutOptions.FillAndExpand,
                   BackgroundColor   = Color.Transparent,
                };
      }

      /// <summary>Gets the expanding scroll view.</summary>
      /// <returns>ScrollView.</returns>
      public static ScrollView GetExpandingScrollView()
      {
         return new ScrollView
                {
                   VerticalOptions   = LayoutOptions.FillAndExpand,
                   HorizontalOptions = LayoutOptions.FillAndExpand,
                   BackgroundColor   = Color.Transparent,
                   Orientation       = ScrollOrientation.Vertical,
                };
      }

      /// <summary>Gets the expanding stack layout.</summary>
      /// <returns>the Stack Layout.</returns>
      public static StackLayout GetExpandingStackLayout()
      {
         var retStackLayout = new StackLayout();
         retStackLayout.SetDefaults();
         return retStackLayout;
      }

      /// <summary>Gets the image.</summary>
      /// <param name="filePath">The file path.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <param name="aspect">The aspect.</param>
      /// <param name="margin"></param>
      /// <param name="getFromResources">if set to <c>true</c> [get from resources].</param>
      /// <param name="resourceClass">The resource class.</param>
      /// <param name="horizontalAlign"></param>
      /// <param name="verticalAlign"></param>
      /// <returns>Image.</returns>
      public static Image GetImage
      (
         string         filePath,
         double?        width            = default,
         double?        height           = default,
         Aspect         aspect           = Aspect.AspectFit,
         LayoutOptions? horizontalAlign  = default,
         LayoutOptions? verticalAlign    = default,
         Thickness?     margin           = default,
         bool           getFromResources = false,
         Type           resourceClass    = null
      )
      {
         var retImage =
            new Image
            {
               Aspect            = aspect,
               VerticalOptions   = verticalAlign   ?? LayoutOptions.Center,
               HorizontalOptions = horizontalAlign ?? LayoutOptions.Center,
               BackgroundColor   = Color.Transparent,
               InputTransparent  = true,
            };

         if ( filePath.IsNotEmpty() )
         {
            if ( getFromResources && ( resourceClass != null ) )
            {
               retImage.Source = ImageSource.FromResource( filePath, resourceClass.GetTypeInfo().Assembly );
            }
            else
            {
               retImage.Source = ImageSource.FromFile( filePath );
            }
         }

         if ( width != null )
         {
            retImage.WidthRequest = width.Value;
         }

         if ( height != null )
         {
            retImage.HeightRequest = height.Value;
         }

         if ( margin != null )
         {
            retImage.Margin = margin.Value;
         }

         return retImage;
      }

      public static Keyboard GetKeyboardFromString( string attributeKeyboardName )
      {
         switch ( attributeKeyboardName )
         {
            case nameof( Keyboard.Numeric ):
               return Keyboard.Numeric;

            case nameof( Keyboard.Default ):
               return Keyboard.Default;

            case nameof( Keyboard.Chat ):
               return Keyboard.Chat;

            case nameof( Keyboard.Email ):
               return Keyboard.Email;

            case nameof( Keyboard.Plain ):
               return Keyboard.Plain;

            case nameof( Keyboard.Telephone ):
               return Keyboard.Telephone;

            case nameof( Keyboard.Text ):
               return Keyboard.Text;

            case nameof( Keyboard.Url ):
               return Keyboard.Url;
         }

         return UIConst_Forms.STANDARD_KEYBOARD;
      }

      /// <summary>Gets the margin for runtime platform.</summary>
      /// <returns>Thickness.</returns>
      public static Thickness GetMarginForRuntimePlatform()
      {
         var top = GetStartingYForRuntimePlatform();
         return new Thickness( 0, top, 0, 0 );
      }

      /// <summary>Gets the shape view.</summary>
      /// <returns>ShapeView.</returns>
      public static ShapeView_Forms GetShapeView()
      {
         var retShapeView = new ShapeView_Forms();
         retShapeView.SetDefaults();
         return retShapeView;
      }

      /// <summary>Gets the simple label.</summary>
      /// <param name="text">The label text.</param>
      /// <param name="textColor">Color of the text.</param>
      /// <param name="horizontalAlignment">The text alignment.</param>
      /// <param name="verticalAlignment"></param>
      /// <param name="fontNamedSize">Size of the font named.</param>
      /// <param name="fontSize">Size of the font.</param>
      /// <param name="fontAttributes">The font attributes.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <param name="bindingSourcePropName">Name of the label binding property.</param>
      /// <param name="bindingSource">The label binding source.</param>
      /// <param name="stringFormat"></param>
      /// <param name="breakMode">The break mode.</param>
      /// <param name="fontFamilyOverride"></param>
      /// <param name="bindingMode"></param>
      /// <param name="bindingConverter"></param>
      /// <param name="converterParameter"></param>
      /// <returns>Label.</returns>
      public static Label GetSimpleLabel
      (
         [CanBeNull] string          text                  = "",
         Color?                      textColor             = null,
         TextAlignment               horizontalAlignment   = TextAlignment.Center,
         TextAlignment               verticalAlignment     = TextAlignment.Center,
         NamedSize                   fontNamedSize         = NamedSize.Small,
         double?                     fontSize              = default,
         FontAttributes              fontAttributes        = FontAttributes.None,
         double?                     width                 = default,
         double?                     height                = default,
         [CanBeNull] string          bindingSourcePropName = "",
         [CanBeNull] object          bindingSource         = null,
         [CanBeNull] string          stringFormat          = "",
         LineBreakMode               breakMode             = LineBreakMode.WordWrap,
         [CanBeNull] string          fontFamilyOverride    = "",
         BindingMode                 bindingMode           = BindingMode.OneWay,
         [CanBeNull] IValueConverter bindingConverter      = null,
         [CanBeNull] object          converterParameter    = null
      )
      {
         textColor = textColor ?? Color.Black;

         var retLabel =
            new Label
            {
               Text                    = text,
               TextColor               = textColor.GetValueOrDefault(),
               HorizontalTextAlignment = horizontalAlignment,
               VerticalTextAlignment   = verticalAlignment,
               HorizontalOptions       = ToLayoutOptions( horizontalAlignment ),
               VerticalOptions         = ToLayoutOptions( verticalAlignment ),
               BackgroundColor         = Color.Transparent,
               FontAttributes          = fontAttributes,
               FontSize =
                  fontSize.GetValueOrDefault().IsANumberGreaterThanZero()
                     ? fontSize.GetValueOrDefault()
                     : Device.GetNamedSize( fontNamedSize, typeof( Label ) ),
               LineBreakMode = breakMode,
            };

         if ( fontFamilyOverride.IsNotEmpty() )
         {
            retLabel.FontFamily = fontFamilyOverride;
         }

         retLabel.ConsiderBindings
         (
            bindingSourcePropName,
            bindingSource,
            stringFormat,
            bindingMode,
            bindingConverter,
            converterParameter
         );

         if ( width.GetValueOrDefault().IsANumberGreaterThanZero() )
         {
            retLabel.WidthRequest = width.GetValueOrDefault();
         }

         if ( height.GetValueOrDefault().IsANumberGreaterThanZero() )
         {
            retLabel.HeightRequest = height.GetValueOrDefault();
         }

         return retLabel;
      }

      // ------------------------------------------------------------------------------------------
      /// <summary>Gets the spacer.</summary>
      /// <param name="height">The height.</param>
      /// <returns>BoxView.</returns>
      public static BoxView GetSpacer( double height )
      {
         return new BoxView
                {
                   HeightRequest     = height,
                   BackgroundColor   = Color.Transparent,
                   HorizontalOptions = LayoutOptions.FillAndExpand,
                   VerticalOptions   = LayoutOptions.FillAndExpand,
                };
      }

      /// <summary>Gets the starting y for runtime platform.</summary>
      /// <returns>System.Single.</returns>
      public static double GetStartingYForRuntimePlatform()
      {
         return DeviceUtils_PI.IsIos() ? IOS_TOP_MARGIN : 0;
      }

      /*
      /// <summary>The bounds have become *invalid* (not changed necessarily) in relation to the last known bounds.</summary>
      /// <param name="bounds">The bounds.</param>
      /// <param name="propName">Name of the property.</param>
      /// <param name="lastBounds">The last bounds.</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
      public static bool HaveBecomeInvalid
      (
         this Rectangle bounds,
         string         propName,
         Rectangle      lastBounds
      )
      {
         return propName.IsBoundsRelatedPropertyChange() && bounds.IsNotValid() && lastBounds.IsPositive();
      }
      */

      /// <summary>Inserts the automatic column.</summary>
      /// <param name="grid">The grid.</param>
      /// <param name="insertionLoc">The insertion loc.</param>
      public static void InsertAutoColumn( this Grid grid, int insertionLoc )
      {
         grid.ColumnDefinitions.Insert( insertionLoc, new ColumnDefinition { Width = GridLength.Auto, } );
      }

      public static bool IntersectsWithInAnyWay( this Rectangle parentRect, Rectangle compareRect )
      {
         return parentRect.Intersect( compareRect ).IsNotEmpty();
      }

      /// <summary>Determines whether [is bounds related property change] [the specified property name].</summary>
      /// <param name="propName">Name of the property.</param>
      /// <returns><c>true</c> if [is bounds related property change] [the specified property name]; otherwise, <c>false</c>.</returns>
      public static bool IsBoundsRelatedPropertyChange
      (
         this string propName
      )
      {
         return propName.IsSameAs( UIConst_PI.WIDTH_PROPERTY_NAME )  ||
                propName.IsSameAs( UIConst_PI.HEIGHT_PROPERTY_NAME ) ||
                propName.IsSameAs( UIConst_PI.X_PROPERTY_NAME )      ||
                propName.IsSameAs( UIConst_PI.Y_PROPERTY_NAME );
      }

      /// <summary>Determines whether [is different than] [the specified second color].</summary>
      /// <param name="color">The color.</param>
      /// <param name="secondColor">Color of the second.</param>
      /// <returns><c>true</c> if [is different than] [the specified second color]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this Color color,
         Color      secondColor
      )
      {
         return color.IsNotAnEqualObjectTo( secondColor );
      }

      /// <summary>Determines whether [is different than] [the specified other size].</summary>
      /// <param name="size">The size.</param>
      /// <param name="otherSize">Size of the other.</param>
      /// <returns><c>true</c> if [is different than] [the specified other size]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan( this Size size, Size otherSize )
      {
         return !size.IsSameAs( otherSize );
      }

      /// <summary>Determines whether [is different than] [the specified other thickness].</summary>
      /// <param name="Thickness">The thickness.</param>
      /// <param name="otherThickness">The other thickness.</param>
      /// <returns><c>true</c> if [is different than] [the specified other thickness]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan( this Thickness Thickness, Thickness otherThickness )
      {
         return !Thickness.IsSameAs( otherThickness );
      }

      /// <summary>Determines whether [is different than] [the specified other rect].</summary>
      /// <param name="mainRect">The main rect.</param>
      /// <param name="otherRect">The other rect.</param>
      /// <returns><c>true</c> if [is different than] [the specified other rect]; otherwise, <c>false</c>.</returns>
      public static bool IsDifferentThan
      (
         this Rectangle mainRect,
         Rectangle      otherRect
      )
      {
         return !mainRect.IsSameAs( otherRect );
      }

      /// <summary>Determines whether the specified size is empty.</summary>
      /// <param name="size">The size.</param>
      /// <returns><c>true</c> if the specified size is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty( this Size size )
      {
         return size.Width.IsLessThanOrEqualTo( 0 ) && size.Height.IsLessThanOrEqualTo( 0 );
      }

      /// <summary>Determines whether the specified thickness is empty.</summary>
      /// <param name="Thickness">The thickness.</param>
      /// <returns><c>true</c> if the specified thickness is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty( this Thickness Thickness )
      {
         return Thickness.Bottom.IsLessThanOrEqualTo( 0 ) && Thickness.Left.IsLessThanOrEqualTo( 0 ) &&
                Thickness.Right.IsLessThanOrEqualTo( 0 )  && Thickness.Top.IsLessThanOrEqualTo( 0 );
      }

      /// <summary>Determines whether the specified main rect is empty.</summary>
      /// <param name="mainRect">The main rect.</param>
      /// <returns><c>true</c> if the specified main rect is empty; otherwise, <c>false</c>.</returns>
      public static bool IsEmpty
      (
         this Rectangle mainRect
      )
      {
         return mainRect.X.IsLessThanOrEqualTo( 0 )     &&
                mainRect.Y.IsLessThanOrEqualTo( 0 )     &&
                mainRect.Width.IsLessThanOrEqualTo( 0 ) &&
                mainRect.Height.IsLessThanOrEqualTo( 0 );
      }

      public static bool IsLessThan( this string mainStr, string compareStr )
      {
         return string.CompareOrdinal( mainStr, compareStr ) < 0;
      }

      /// <summary>Determines whether [is not empty] [the specified size].</summary>
      /// <param name="size">The size.</param>
      /// <returns><c>true</c> if [is not empty] [the specified size]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty( this Size size )
      {
         return !size.IsEmpty();
      }

      /// <summary>Determines whether [is not empty] [the specified thickness].</summary>
      /// <param name="Thickness">The thickness.</param>
      /// <returns><c>true</c> if [is not empty] [the specified thickness]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty( this Thickness Thickness )
      {
         return !Thickness.IsEmpty();
      }

      /// <summary>Determines whether [is not empty] [the specified main rect].</summary>
      /// <param name="mainRect">The main rect.</param>
      /// <returns><c>true</c> if [is not empty] [the specified main rect]; otherwise, <c>false</c>.</returns>
      public static bool IsNotEmpty
      (
         this Rectangle mainRect
      )
      {
         return !mainRect.IsEmpty();
      }

      /*
      /// <summary>Determines whether [is not valid] [the specified bounds].</summary>
      /// <param name="bounds">The bounds.</param>
      /// <returns><c>true</c> if [is not valid] [the specified bounds]; otherwise, <c>false</c>.</returns>
      public static bool IsNotValid( this Rectangle bounds )
      {
         return !bounds.IsPositive();
      }
      */

      //public static double IosHeightHack(bool isNested)
      //{
      //   return Device.RuntimePlatform.IsSameAs(Device.iOS) ? isNested ? 0 : IOS_TOP_MARGIN : 0;
      //}
      /// <summary>Determines whether [is not visible] [the specified view].</summary>
      /// <param name="view">The view.</param>
      /// <returns><c>true</c> if [is not visible] [the specified view]; otherwise, <c>false</c>.</returns>
      public static bool IsNotVisible( this View view )
      {
         return view.Opacity.IsDifferentThan( VISIBLE_OPACITY );
      }

      /// <summary>Returns true if ... is valid.</summary>
      /// <remarks>WARNING: If the element is being evaluated which might legally have a 0 width r height, this method will *prevent* it.</remarks>
      /// <param name="bounds">The bounds.</param>
      /// <returns><c>true</c> if the specified bounds is valid; otherwise, <c>false</c>.</returns>
      public static bool IsPositive( this Rectangle bounds )
      {
         return bounds.Width.IsANumberGreaterThanZero() && bounds.Height.IsANumberGreaterThanZero();
      }

      public static bool IsPositive( this Size size )
      {
         return size.Width.IsANumberGreaterThanZero() && size.Height.IsANumberGreaterThanZero();
      }

      /// <summary>Determines whether [is same as] [the specified other size].</summary>
      /// <param name="size">The size.</param>
      /// <param name="otherSize">Size of the other.</param>
      /// <returns><c>true</c> if [is same as] [the specified other size]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs( this Size size, Size otherSize )
      {
         return size.Width.IsSameAs( otherSize.Width ) && size.Height.IsSameAs( otherSize.Height );
      }

      /// <summary>Determines whether [is same as] [the specified other thickness].</summary>
      /// <param name="Thickness">The thickness.</param>
      /// <param name="otherThickness">The other thickness.</param>
      /// <returns><c>true</c> if [is same as] [the specified other thickness]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs( this Thickness Thickness, Thickness otherThickness )
      {
         return Thickness.Bottom.IsSameAs( otherThickness.Bottom ) && Thickness.Left.IsSameAs( otherThickness.Left ) &&
                Thickness.Right.IsSameAs( otherThickness.Right )   && Thickness.Top.IsSameAs( otherThickness.Top );
      }

      /// <summary>Determines whether [is same as] [the specified other rect].</summary>
      /// <param name="mainRect">The main rect.</param>
      /// <param name="otherRect">The other rect.</param>
      /// <returns><c>true</c> if [is same as] [the specified other rect]; otherwise, <c>false</c>.</returns>
      public static bool IsSameAs
      (
         this Rectangle mainRect,
         Rectangle      otherRect
      )
      {
         return mainRect.Width.IsSameAs( otherRect.Width )
              &&
                mainRect.Height.IsSameAs( otherRect.Height )
              &&
                mainRect.X.IsSameAs( otherRect.X )
              &&
                mainRect.Y.IsSameAs( otherRect.Y );
      }

      /// <summary>Merges the style.</summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="mainStyle">The main style.</param>
      /// <param name="newStyle">The new style.</param>
      /// <returns>Style.</returns>
      public static Style MergeStyle<T>
      (
         this Style mainStyle,
         Style      newStyle
      )
      {
         if ( ( newStyle == null ) || newStyle.Setters.IsEmpty() )
         {
            return mainStyle;
         }

         mainStyle = new Style( typeof( T ) );

         foreach ( var setter in newStyle.Setters )
         {
            var foundSetter =
               mainStyle.Setters.FirstOrDefault(
                  s => s.Property.PropertyName.IsSameAs( setter.Property.PropertyName ) );
            if ( foundSetter != null )
            {
               var foundSetterIdx = mainStyle.Setters.IndexOf( foundSetter );

               mainStyle.Setters[ foundSetterIdx ] = setter;
            }
            else
            {
               mainStyle.Setters.Add( setter );
            }
         }

         // The Style settings must also be considered in the assignment
         mainStyle.ApplyToDerivedTypes = newStyle.ApplyToDerivedTypes || mainStyle.ApplyToDerivedTypes;

         return mainStyle;
      }

      public static string NowToServerDateTimeStr()
      {
         return DateTime.Now.ToServerDateTimeStr();
      }

      /// <summary>Presentables the date.</summary>
      /// <param name="dt">The dt.</param>
      /// <returns>System.String.</returns>
      public static string PresentableDate( this DateTime dt )
      {
         return $"{dt:MMM d, yyyy}";
      }

      /// <summary>Presentables the dollar amount.</summary>
      /// <param name="dollars">The dollars.</param>
      /// <returns>System.String.</returns>
      public static string PresentableDollarAmount( this double dollars )
      {
         return $"{dollars:c}";
      }

      /// <summary>Presentables the whole long int.</summary>
      /// <param name="lng">The LNG.</param>
      /// <returns>System.String.</returns>
      public static string PresentableWholeLongInt( this long lng )
      {
         return $"{lng:0}";
      }

      // var retColor = Color.FromRgb ( color.R.MultiplyWithMax(multiplier, MAX_COLOR_ELEMENT),
      // color.G.MultiplyWithMax(multiplier, MAX_COLOR_ELEMENT), color.B.MultiplyWithMax(multiplier, MAX_COLOR_ELEMENT) );
      public static void SetAndForceStyle( this View view, Style style )
      {
         view.Style = style;
         view.ForceStyle( style );
      }

      public static void SetBasicViewDefaults( this View view )
      {
         view.VerticalOptions   = LayoutOptions.FillAndExpand;
         view.HorizontalOptions = LayoutOptions.FillAndExpand;
         view.BackgroundColor   = Color.Transparent;
      }

      public static void SetBorderViewHeightIfNecessary( this IValidatableEntryGeneral_PI entry, double width,
                                                         double                           height )
      {
         ( (View)entry ).WidthRequest = width;
         entry.BorderViewHeight       = entry.BorderViewHeight.SetWidthHeightConditionally( height );
      }

      public static void SetDefaults( this Grid grid )
      {
         grid.SetBasicViewDefaults();

         grid.ColumnSpacing     = 0;
         grid.RowSpacing        = 0;
         grid.Margin            = 0;
         grid.Padding           = 0;
         grid.IsClippedToBounds = true;
      }

      public static void SetDefaults( this ShapeView_Forms shapeView )
      {
         shapeView.SetBasicViewDefaults();

         shapeView.SetCornerRadiusFactor( UIConst_PI.DEFAULT_CORNER_RADIUS_FACTOR );
         shapeView.IsClippedToBounds = true;
         shapeView.Margin            = 0;
         shapeView.Padding           = 0;
         shapeView.FillColor         = Color.Transparent;
      }

      public static void SetDefaults( this ScrollView scrollView )
      {
         scrollView.SetBasicViewDefaults();
         scrollView.Orientation = ScrollOrientation.Vertical;
      }

      public static void SetDefaults( this StackLayout layout )
      {
         layout.SetBasicViewDefaults();
         layout.Spacing         = 0;
         layout.Orientation     = StackOrientation.Vertical;
         layout.VerticalOptions = LayoutOptions.StartAndExpand;
      }

      public static void SetRoundedCornerContentViewStyle(
         this Style retStyle,
         Color?     backColor       = null,
         double?    cornerRadius    = null,
         Color?     borderColor     = null,
         double?    borderThickness = null )
      {
         if ( backColor != null )
         {
            retStyle.Setters.Add( new Setter { Property = RoundedContentView.FillColorProperty, Value = backColor, } );
         }

         if ( cornerRadius != null )
         {
            retStyle.Setters.Add( new Setter
                                  {
                                     Property = RoundedContentView.CornerRadiusProperty,
                                     Value    = cornerRadius,
                                  } );
         }

         if ( borderColor != null )
         {
            retStyle.Setters.Add( new Setter
                                  {
                                     Property = RoundedContentView.BorderColorProperty,
                                     Value    = borderColor,
                                  } );
         }

         if ( borderThickness != null )
         {
            retStyle.Setters.Add( new Setter
                                  {
                                     Property = RoundedContentView.BorderWidthProperty,
                                     Value    = borderThickness,
                                  } );
         }
      }

      public static double SetWidthHeightConditionally( this double originalWidthHeight, double newWidthHeight )
      {
         if ( !originalWidthHeight.IsANumberGreaterThanZero() && newWidthHeight.IsANumberGreaterThanZero() )
         {
            return newWidthHeight;
         }

         return originalWidthHeight;
      }

      /// <summary>
      /// Temparily mimics Xamarin.Forms:
      /// https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/text/fonts
      /// </summary>
      /// <param name="namedSize"></param>
      /// <returns></returns>
      public static double ToFormsNamedSize( this NamedSize namedSize )
      {
         switch ( namedSize )
         {
            case NamedSize.Body:
               return DeviceUtils_PI.IsIos() ? 17 : 16;

            case NamedSize.Caption:
               return 12;

            case NamedSize.Header:
               /*
               // TODO challenge this wild number
               return DeviceUtils_PI.IsIos() ? 17 : 96;
               */
               // HACK
               return DeviceUtils_PI.IsIos() ? 17 : 19;

            case NamedSize.Large:
               return 22;

            case NamedSize.Medium:
               return 17;

            case NamedSize.Micro:
               return DeviceUtils_PI.IsIos() ? 12 : 10;

            case NamedSize.Small:
               return 14;

            case NamedSize.Subtitle:
               return DeviceUtils_PI.IsIos() ? 22 : 16;

            case NamedSize.Title:
               return DeviceUtils_PI.IsIos() ? 28 : 24;

            default: // NamedSize.Default:
               return DeviceUtils_PI.IsIos() ? 17 : 14;
         }
      }

      /*
      /// <summary>Enums the try parse.</summary>
      /// <typeparam name="EnumT">The type of the enum t.</typeparam>
      /// <param name="input">The input.</param>
      /// <param name="theEnum">The enum.</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
      public static bool EnumTryParse<EnumT>
      (
         string    input,
         out EnumT theEnum
      )
      {
         foreach (var enumName in Enum.GetNames(typeof(EnumT)))
         {
            if (enumName.IsSameAs(input))
            {
               theEnum = (EnumT)Enum.Parse(typeof(EnumT), input, true);
               return true;
            }
         }

         theEnum = default;

         return false;
      }
      */

      /// <summary>Horizontals the options from text alignment.</summary>
      /// <param name="textAlignment">The text alignment.</param>
      /// <returns>LayoutOptions.</returns>
      public static LayoutOptions ToLayoutOptions( this TextAlignment textAlignment )
      {
         switch ( textAlignment )
         {
            case TextAlignment.Center:
               // return LayoutOptions.CenterAndExpand;
               return LayoutOptions.Center;

            case TextAlignment.End:
               // return LayoutOptions.EndAndExpand;
               return LayoutOptions.End;

            // case TextAlignment.Start:
            default:
               // return LayoutOptions.StartAndExpand;
               return LayoutOptions.Start;
         }
      }

      /*
      /// <summary>Determines whether [is unset or default] [the specified color].</summary>
      /// <param name="color">The color.</param>
      /// <returns><c>true</c> if [is unset or default] [the specified color]; otherwise, <c>false</c>.</returns>
      public static bool IsUnsetOrDefault(this Color color)
      {
         // Default is not null -- ??
         return color.IsAnEqualObjectTo(Color.Default);
      }
      */
      /*
       CANNOT DETERMINE IF A COLOR IS VALID

      /// <summary>Returns true if ... is valid.</summary>
      /// <param name="color">The color.</param>
      /// <returns><c>true</c> if the specified color is valid; otherwise, <c>false</c>.</returns>
      public static bool IsPositive(this Color color)
      {
         return !color.IsUnsetOrDefault();
      }
      */
      /*
      // // Determine the maximum multiplication that allows us to keep the exact proportion of the three colors. var
      // maxCurrentIndividualColor = Math.Max(Math.Max(color.R, color.G), color.B); var multiplier = MAX_COLOR_ELEMENT *
      // howCloseToWhite / maxCurrentIndividualColor;
      /// <summary>Converts to enum.</summary>
      /// <typeparam name="EnumT">The type of the enum t.</typeparam>
      /// <param name="enumAsString">The enum as string.</param>
      /// <returns>EnumT.</returns>
      public static EnumT ToEnum<EnumT>(this string enumAsString)
         where EnumT : Enum
      {
         if (EnumTryParse(enumAsString, out EnumT foundEnum))
         {
            return foundEnum;
         }

         return default;
      }
      */

      //public static Color SuperPale(this Color color, double howCloseToWhite = VERY_PALE)
      //{
      //   if (color.IsAnEqualObjectTo(default))
      //   {
      //      return Color.Transparent;
      //   }
      public static DateTime? ToNullableDateTime( this string str )
      {
         if ( str.IsEmpty() )
         {
            return default;
         }

         if ( DateTime.TryParse( str, out var dateTime ) )
         {
            return dateTime;
         }

         return default;
      }

      //private static double MultiplyWithMax(this double num, double factor, double max)
      //{
      //   return Math.Min(max, (num * factor));
      //}
      /// <summary>Converts to off screen position.</summary>
      /// <param name="stageToolbarPosition">The stage toolbar position.</param>
      /// <returns>OffScreenPositions.</returns>
      public static OffScreenPositions ToOffScreenPosition( this StageToolbarPositions stageToolbarPosition )
      {
         switch ( stageToolbarPosition )
         {
            case StageToolbarPositions.BOTTOM: return OffScreenPositions.BOTTOM;

            case StageToolbarPositions.LEFT: return OffScreenPositions.LEFT;

            case StageToolbarPositions.TOP: return OffScreenPositions.TOP;

            case StageToolbarPositions.RIGHT: return OffScreenPositions.RIGHT;
         }

         return OffScreenPositions.NONE;
      }

      //private const double VERY_PALE = 0.9;
      //private const double MAX_COLOR_ELEMENT = 255;
      /// <summary></summary>
      /// <param name="flowDirection"></param>
      /// <returns></returns>
      public static OffScreenPositions ToOffScreenPosition( this SubStageFlowDirections flowDirection )
      {
         switch ( flowDirection )
         {
            case SubStageFlowDirections.BOTTOM_TO_TOP: return OffScreenPositions.BOTTOM;
            case SubStageFlowDirections.LEFT_TO_RIGHT: return OffScreenPositions.LEFT;
            case SubStageFlowDirections.TOP_TO_BOTTOM: return OffScreenPositions.TOP;
            case SubStageFlowDirections.RIGHT_TO_LEFT: return OffScreenPositions.RIGHT;
         }

         return OffScreenPositions.NONE;
      }

      /// <summary>Converts to on-screen position.</summary>
      /// <param name="stageToolbarPosition">The stage toolbar position.</param>
      /// <returns>OnScreenPositions.</returns>
      public static OnScreenPositions ToOnScreenPosition( this StageToolbarPositions stageToolbarPosition )
      {
         switch ( stageToolbarPosition )
         {
            case StageToolbarPositions.BOTTOM: return OnScreenPositions.BOTTOM_CENTER;

            case StageToolbarPositions.LEFT: return OnScreenPositions.LEFT_CENTER;

            case StageToolbarPositions.TOP: return OnScreenPositions.TOP_CENTER;

            case StageToolbarPositions.RIGHT: return OnScreenPositions.RIGHT_CENTER;
         }

         return OnScreenPositions.NONE;
      }

      /// <summary>Converts to options.</summary>
      /// <param name="alignment">The alignment.</param>
      /// <returns>LayoutOptions.</returns>
      public static LayoutOptions ToOptions( this LayoutAlignment alignment )
      {
         // Set the child margins to compel this alignment
         switch ( alignment )
         {
            case LayoutAlignment.Center:
               return LayoutOptions.Center;

            case LayoutAlignment.End:
               return LayoutOptions.End;

            case LayoutAlignment.Fill:
               return LayoutOptions.Fill;

            case LayoutAlignment.Start:
               return LayoutOptions.Start;
         }

         return default;
      }

      public static string ToServerDateTimeStr( this DateTime dateTime )
      {
         var retStr = dateTime.ToString( SERVER_DATE_FORMAT );

         return retStr;
      }

      /// <summary>Use the current thread's culture info for conversion</summary>
      public static string ToTitleCase( this string str )
      {
         if ( str.IsEmpty() )
         {
            return "";
         }

         var cultureInfo = Thread.CurrentThread.CurrentCulture;
         return cultureInfo.TextInfo.ToTitleCase( str.ToLower() );
      }

      /// <summary>Overload which uses the culture info with the specified name</summary>
      public static string ToTitleCase( this string str, string cultureInfoName )
      {
         if ( str.IsEmpty() )
         {
            return "";
         }

         var cultureInfo = new CultureInfo( cultureInfoName );
         return cultureInfo.TextInfo.ToTitleCase( str.ToLower() );
      }

      /// <summary>Overload which uses the specified culture info</summary>
      public static string ToTitleCase( this string str, CultureInfo cultureInfo )
      {
         if ( str.IsEmpty() )
         {
            return "";
         }

         return cultureInfo.TextInfo.ToTitleCase( str.ToLower() );
      }

      /// <summary>Gets the width of the forced.</summary>
      /// <param name="width">The width.</param>
      /// <param name="forceLongSize">if set to <c>true</c> [force long size].</param>
      /// <param name="currentSize">Size of the current.</param>
      /// <returns>System.Double.</returns>
      private static double GetForcedWidth
      (
         double width,
         bool   forceLongSize,
         Size   currentSize
      )
      {
         width = forceLongSize ? currentSize.Width : width;
         return width;
      }

      public static void RescaleDelays( this IFlowableCollectionCanvas_Forms canvas, double scaleFactor )
      {
         // IMPORTANT Setting some animation speeds here because they *must* follow the base class's HandlePostBindingContextTask
         canvas.FlowSettings.TranslateBoundsMilliseconds =
            (uint)( scaleFactor * FlowableConst_PI.DEFAULT_TRANSLATE_BOUNDS_MILLISECONDS ).ToRoundedInt();
         canvas.FlowSettings.FadeInMilliseconds =
            (uint)( scaleFactor * FlowableConst_PI.DEFAULT_FADE_IN_MILLISECONDS ).ToRoundedInt();
         canvas.AnimateInDelayMilliseconds =
            ( scaleFactor * FlowableConst_PI.MILLISECOND_DELAY_BETWEEN_ANIMATION_TRANSITIONS ).ToRoundedInt();
      }


      /*
      public static Color FromHexToFormsColor(this string hex)
      {
         if (hex.IsEmpty() || !hex.EndsWith(ViewModelCustomAttribute_Static_PI.COLOR_HEX_SUFFIX))
         {
            return default;
         }

         // Remove the "Hex" suffix
         var colorName =
            hex.Substring(0, hex.Length - ViewModelCustomAttribute_Static_PI.COLOR_HEX_SUFFIX.Length);

         colorName = colorName.Trim();

         return colorName.CanBeConvertedToColor().Item2;
      }
      */
   }
}