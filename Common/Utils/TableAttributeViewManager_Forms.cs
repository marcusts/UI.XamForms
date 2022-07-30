// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=TableAttributeViewManager_Forms.cs
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
   using System.Linq;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Annotations;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Converters;
   using Com.MarcusTS.UI.XamForms.Common.Images;
   using Com.MarcusTS.UI.XamForms.Views.Controls;
   using Xamarin.Forms;

   public interface ITableAttributeViewManager_Forms
   {
      int AnimateSortOnTap { get; set; }

      int HapticFeedbackOnSortTap { get; set; }

      Task<View> CreateCellViewForAttribute( IViewModelTableColumnAttribute_PI attribute,
                                             object                            bindingContext );

      Task<View> CreateHeaderViewForAttribute( IViewModelTableColumnAttribute_PI attribute );
   }

   public class TableAttributeViewManager_Forms : ITableAttributeViewManager_Forms
   {
      public const string COMPLETED_TOGGLE = nameof( COMPLETED_TOGGLE );

      // ITEM_DISPLAY_KIND VARIATIONS
      public const string DISPLAY_KIND_INFO = "Info";

      public const string DISPLAY_KIND_NOTICE = "Notice";
      public const string DISPLAY_KIND_TASK   = "Task";
      public const string ITEM_DISPLAY_KIND   = nameof( ITEM_DISPLAY_KIND );

      /// <summary>
      /// A date that nis written out in "plain" English:
      /// 1 day from now
      /// 1 hour ago
      /// </summary>
      public const string PLAIN_ENGLISH_DATE = nameof( PLAIN_ENGLISH_DATE );

      public const            string   READ_ONLY_TEXT = nameof( READ_ONLY_TEXT );
      public static readonly  string[] DISPLAY_KINDS = { DISPLAY_KIND_INFO, DISPLAY_KIND_NOTICE, DISPLAY_KIND_TASK, };
      private static readonly double   DISPLAY_KIND_IMAGE_WIDTH_HEIGHT = 30.0.AdjustForOsAndDevice();
      private static readonly double   DISPLAY_KIND_LABEL_HEIGHT = 15.0.AdjustForOsAndDevice();
      private static readonly double   MINIMUM_ROW_HEIGHT = DISPLAY_KIND_IMAGE_WIDTH_HEIGHT + DISPLAY_KIND_LABEL_HEIGHT;
      private static readonly double   ONE_SIDE_MARGIN_PADDING = 3.0.AdjustForOsAndDevice();
      public static readonly double   HEADER_ROW_HEIGHT = UIConst_Forms.DEFAULT_ENTRY_HEIGHT * Math.Round( 2.0 / 3.0 );

      private static readonly double SMALL_FONT_SIZE =
         Device.GetNamedSize( NamedSize.Micro, typeof( Label ) ).AdjustForOsAndDevice( 0.8 );

      private static readonly Thickness STANDARD_MARGIN  = new Thickness( 0, 0, ONE_SIDE_MARGIN_PADDING, 0 );
      private static readonly Thickness STANDARD_PADDING = ONE_SIDE_MARGIN_PADDING;
      public                  int       AnimateSortOnTap { get; set; } = ViewModelCustomAttribute_Static_PI.TRUE_BOOL;
      public                  int       HapticFeedbackOnSortTap { get; set; }= ViewModelCustomAttribute_Static_PI.TRUE_BOOL;

      public virtual async Task<View> CreateCellViewForAttribute( IViewModelTableColumnAttribute_PI attribute,
                                                                  object                            bindingContext )
      {
         View retView;

         switch ( attribute.DisplayKind )
         {
            case COMPLETED_TOGGLE:
               retView = await CreateCellCompletedToggle( attribute, bindingContext ).AndReturnToCallingContext();
               break;

            case PLAIN_ENGLISH_DATE:
               retView = CreateCellHoursOrDaysLate( attribute, bindingContext );
               break;

            case ITEM_DISPLAY_KIND:
               retView = CreateCellItemDisplayKind( attribute, bindingContext );
               break;

            // case READ_ONLY_TEXT:
            default:
               retView = CreateLabelForAttribute( attribute, bindingContext );
               break;
         }

         return retView;
      }

      public Task<View> CreateHeaderViewForAttribute( IViewModelTableColumnAttribute_PI attribute )
      {
         return Task.FromResult( CreateHeaderButtonForAttribute( attribute ) as View );
      }

      private static View CreateLabelForAttribute( IViewModelTableColumnAttribute_PI attribute,
                                                   object                            bindingContext )
      {
         var label =
            UIUtils_Forms.GetSimpleLabel
            (
               "",
               attribute.CellTextColor_ColorHex.ToOsEquivalentColor( attribute, Color.Black ),
               attribute.CellHorizontalTextAlignment_TextAlignment.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.TEXT_ALIGNMENT_SUFFIX, attribute, TextAlignment.Start ),
               default,
               default,
               attribute.CellFontSize_OS.ToOsEquivalentPositiveNumber( attribute,
                  Device.GetNamedSize( NamedSize.Small, typeof( Label ) ) ),
               attribute.CellFontAttributes_FontAttributes.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.FONT_ATTRIBUTES_SUFFIX, attribute, FontAttributes.None ),
               -1.0,
               -1.0,
               attribute.ViewModelPropertyName,
               bindingContext,
               attribute.CellStringFormat.IsUnset() ? "" : attribute.CellStringFormat,
               attribute.CellLineBreakMode_LineBreakMode.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.LINE_BREAK_MODE_SUFFIX, attribute, LineBreakMode.WordWrap ),
               attribute.CellFontFamily.IsUnset() ? UIUtils_Forms.DefaultFontFamily() : attribute.CellFontFamily
            );

         // Fix some gaps
         label.BackgroundColor = Color.Transparent;

         if ( attribute.CellUniformMargin_OS.IsANumberGreaterThanZero() )
         {
            label.Margin = attribute.CellUniformMargin_OS.ToOsEquivalentPositiveNumber( attribute );
         }

         if ( attribute.CellUniformPadding_OS.IsANumberGreaterThanZero() )
         {
            label.Padding = attribute.CellUniformPadding_OS.ToOsEquivalentPositiveNumber( attribute );
         }

         // label.HorizontalTextAlignment = attribute.CellHorizontalTextAlignment ?? TextAlignment.Start;
         label.HorizontalOptions = label.HorizontalTextAlignment.ToLayoutOptions();
         label.VerticalTextAlignment =
            attribute.CellVerticalTextAlignment_TextAlignment.ToOsEquivalentEnum(
               ViewModelCustomAttribute_Static_PI.TEXT_ALIGNMENT_SUFFIX, attribute, TextAlignment.Center );
         label.VerticalOptions = label.VerticalTextAlignment.ToLayoutOptions();

         label.Margin  = STANDARD_MARGIN;
         label.Padding = 0;

         return label;
      }

      private ISimpleImageLabelButton_Forms CreateCellButtonForAttribute(
         IViewModelTableColumnAttribute_PI attribute,
         object                            bindingContext,
         [CanBeNull] IValueConverter       bindingConverter          = null,
         [CanBeNull] object                bindingConverterParameter = null
      )
      {
         var retLabelButton =
            SimpleImageLabelButton_Forms.CreateCompleteSimpleImageLabelButton
            (
               AnimateSortOnTap.IsTrue(),
               Color.Transparent,
               bindingContext,
               bindingConverter,
               bindingConverterParameter,
               attribute.BoundMode_BindingMode.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.BINDING_MODE_SUFFIX, attribute, BindingMode.TwoWay ),
               attribute.ViewModelPropertyName,
               bindingContext,
               Color.Transparent,
               0.0,
               attribute.CanSort.IsFalse(),
               attribute.CellFontAttributes_FontAttributes.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.FONT_ATTRIBUTES_SUFFIX, attribute, FontAttributes.None ),
               attribute.CellFontFamily.IsUnset() ? UIUtils_Forms.DefaultFontFamily() : attribute.CellFontFamily,
               attribute.CellFontSize_OS.ToOsEquivalentPositiveNumber( attribute,
                  Device.GetNamedSize( NamedSize.Small, typeof( SimpleImageLabelButton_Forms ) ) ),
               true,
               DISPLAY_KIND_LABEL_HEIGHT,
               default,
               attribute.CellHorizontalTextAlignment_TextAlignment.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.TEXT_ALIGNMENT_SUFFIX, attribute, TextAlignment.Center ),
               "",
               default,
               HapticFeedbackOnSortTap.IsTrue(),
               attribute.CellLineBreakMode_LineBreakMode.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.LINE_BREAK_MODE_SUFFIX, attribute, LineBreakMode.WordWrap ),
               ImageLabelButtonSelectionStyles.NoSelection,
               attribute.CellStringFormat.IsUnset() ? "" : attribute.CellStringFormat,
               "",
               attribute.CellTextColor_ColorHex.ToOsEquivalentColor( attribute, Color.Black ),
               false,
               attribute.CellUniformMargin_OS.ToOsEquivalentPositiveNumber( attribute ),
               attribute.CellUniformPadding_OS.ToOsEquivalentPositiveNumber( attribute ),
               default,
               attribute.CellVerticalTextAlignment_TextAlignment.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.TEXT_ALIGNMENT_SUFFIX, attribute, TextAlignment.Center ),
               -1
            );

         retLabelButton.ButtonLabel.HorizontalOptions =
            retLabelButton.ButtonLabel.HorizontalTextAlignment.ToLayoutOptions();
         retLabelButton.ButtonLabel.VerticalOptions =
            retLabelButton.ButtonLabel.VerticalTextAlignment.ToLayoutOptions();
         ( (SimpleImageLabelButton_Forms)retLabelButton ).Margin  = STANDARD_MARGIN;
         ( (SimpleImageLabelButton_Forms)retLabelButton ).Padding = 0;

         ( (SimpleImageLabelButton_Forms)retLabelButton ).MinimumHeightRequest = MINIMUM_ROW_HEIGHT;

         return retLabelButton;
      }

      private Task<View> CreateCellCompletedToggle( IViewModelTableColumnAttribute_PI attribute,
                                                    object                            bindingContext )
      {
         var retCheckBox = new CustomCheckBox_Forms
                           {
                              BackgroundColor   = Color.Transparent,
                              Margin            = 5.0.AdjustForOsAndDevice(),
                              HorizontalOptions = LayoutOptions.Center,
                              VerticalOptions   = LayoutOptions.Center,
                           };

         retCheckBox.ConsiderBindings( CustomCheckBox_Forms.IsCheckedProperty, attribute.ViewModelPropertyName,
            bindingContext, bindingMode: BindingMode.TwoWay );

         // Copy any possible attributes over to the check box so they will affect the control
         retCheckBox.CopyCommmonPropertiesFromAttribute_Forms( attribute );

         return Task.FromResult( (View)retCheckBox );
      }

      private View CreateCellHoursOrDaysLate( IViewModelTableColumnAttribute_PI attribute,
                                              object                            bindingContext )
      {
         // Use the number on the left and the two strings on the right
         var retLayout     = UIUtils_Forms.GetExpandingStackLayout();
         var cellTextColor = attribute.CellTextColor_ColorHex.ToOsEquivalentColor( attribute, Color.Black );

         var displayKindTimeValueLabel =
            UIUtils_Forms.GetSimpleLabel
            (
               textColor: cellTextColor,
               fontNamedSize: NamedSize.Medium,
               fontAttributes: FontAttributes.Bold,
               bindingMode: BindingMode.OneWay,
               bindingSource: bindingContext,
               bindingSourcePropName: attribute.ViewModelPropertyName,
               bindingConverter: new DateTimeToPlainEnglishConverter_TimeValue()
            );

         retLayout.Children.Add( displayKindTimeValueLabel );

         var displayKindTimeKindLabel =
            UIUtils_Forms.GetSimpleLabel
            (
               textColor: cellTextColor,
               fontSize: SMALL_FONT_SIZE,
               fontAttributes: FontAttributes.None,
               bindingSource: bindingContext,
               bindingSourcePropName: attribute.ViewModelPropertyName,
               bindingConverter: new DateTimeToPlainEnglishConverter_TimeUnit()
            );

         retLayout.Children.Add( displayKindTimeKindLabel );

         var displayKindTimePastPresentLabel =
            UIUtils_Forms.GetSimpleLabel
            (
               textColor: cellTextColor,
               fontSize: SMALL_FONT_SIZE,
               fontAttributes: FontAttributes.Bold,
               bindingSource: bindingContext,
               bindingSourcePropName: attribute.ViewModelPropertyName,
               bindingConverter: new DateTimeToPlainEnglishConverter_TimePastPresent()
            );

         retLayout.Children.Add( displayKindTimePastPresentLabel );

         return retLayout;
      }

      private View CreateCellItemDisplayKind( IViewModelTableColumnAttribute_PI attribute,
                                              object                            bindingContext )
      {
         // Use an image label button for the label and image
         // Set their positions by default (center-top image, center-bottom label)
         var imageLabelButton =
            CreateCellButtonForAttribute( attribute, bindingContext );

         // Bind the image source based on the current text value
         imageLabelButton.ButtonImage =
            UIUtils_Forms.GetImage( "", DISPLAY_KIND_IMAGE_WIDTH_HEIGHT, DISPLAY_KIND_IMAGE_WIDTH_HEIGHT );

         // TODO Integrate this withn the button creation
         imageLabelButton.ImageWidth = DISPLAY_KIND_IMAGE_WIDTH_HEIGHT;
         imageLabelButton.ImageHeight = DISPLAY_KIND_IMAGE_WIDTH_HEIGHT;

         // Bind the button text (which itself is bound to a row view model's data) to the image source so the images matches the display kind.
         imageLabelButton.ButtonImage.SetUpBinding
         (
            Image.SourceProperty,
            nameof( Label.Text ),
            BindingMode.OneWay,
            new DisplayKindToImageSourceConverter(),
            source: imageLabelButton.ButtonLabel );

         // Position the image and label
         imageLabelButton.ImageHorizontalAlign = LayoutOptions.Center;
         imageLabelButton.ImageVerticalAlign   = LayoutOptions.Start;
         // imageLabelButton.ImageMargin          = new Thickness( 0, -TEXT_IMAGE_SEPARATION, 0, TEXT_IMAGE_SEPARATION );

         imageLabelButton.ButtonLabel.HorizontalTextAlignment = TextAlignment.Center;
         imageLabelButton.ButtonLabel.HorizontalOptions =
            imageLabelButton.ButtonLabel.HorizontalTextAlignment.ToLayoutOptions();
         imageLabelButton.ButtonLabel.VerticalTextAlignment = TextAlignment.End;
         imageLabelButton.ButtonLabel.VerticalOptions =
            imageLabelButton.ButtonLabel.VerticalTextAlignment.ToLayoutOptions();
         imageLabelButton.ButtonLabel.FontSize = SMALL_FONT_SIZE;
         // imageLabelButton.ButtonLabel.Margin   = new Thickness( 0, TEXT_IMAGE_SEPARATION, 0, -TEXT_IMAGE_SEPARATION );

         return imageLabelButton as View;
      }

      private ISimpleImageLabelButton_Forms CreateHeaderButtonForAttribute(
         IViewModelTableColumnAttribute_PI attribute,
         [CanBeNull] IValueConverter       bindingConverter          = null,
         [CanBeNull] object                bindingConverterParameter = null )
      {
         var retButton =
            SimpleImageLabelButton_Forms.CreateCompleteSimpleImageLabelButton
            (
               AnimateSortOnTap.IsTrue(),
               attribute.HeaderBackColor_ColorHex.ToOsEquivalentColor( attribute, Color.Transparent ),
               attribute,
               bindingConverter,
               bindingConverterParameter,
               attribute.BoundMode_BindingMode.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.BINDING_MODE_SUFFIX, attribute, BindingMode.OneWay ),
               nameof( IViewModelTableColumnAttribute_PI.HeaderName ),
               attribute,
               attribute.HeaderBorderColor_ColorHex.ToOsEquivalentColor( attribute, Color.Transparent ),
               (float)attribute.HeaderBorderThickness_OS.ToOsEquivalentPositiveNumber( attribute ),
               attribute.CanSort.IsFalse(),
               attribute.HeaderFontAttributes_FontAttributes.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.FONT_ATTRIBUTES_SUFFIX, attribute, FontAttributes.None ),
               attribute.HeaderFontFamily.IsUnset() ? UIUtils_Forms.DefaultFontFamily() : attribute.HeaderFontFamily,
               attribute.HeaderFontSize_OS.ToOsEquivalentPositiveNumber( attribute, SMALL_FONT_SIZE ),
               true,
               attribute.HeaderHeight_OS.ToOsEquivalentPositiveNumber( attribute, HEADER_ROW_HEIGHT ),
               default,
               attribute.HeaderHorizontalTextAlignment_TextAlignment.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.TEXT_ALIGNMENT_SUFFIX, attribute, TextAlignment.Center ),
               "",
               default,
               HapticFeedbackOnSortTap.IsTrue(),
               attribute.HeaderLineBreakMode_LineBreakMode.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.LINE_BREAK_MODE_SUFFIX, attribute, LineBreakMode.CharacterWrap ),
               ImageLabelButtonSelectionStyles.NoSelection,
               attribute.HeaderStringFormat.IsUnset() ? "" : attribute.HeaderStringFormat,
               "",
               attribute.HeaderTextColor_ColorHex.ToOsEquivalentColor( attribute, Color.White ),
               false,
               attribute.HeaderUniformMargin_OS.ToOsEquivalentPositiveNumber( attribute ),
               attribute.HeaderUniformPadding_OS.ToOsEquivalentPositiveNumber( attribute ),
               default,
               attribute.HeaderVerticalTextAlignment_TextAlignment.ToOsEquivalentEnum(
                  ViewModelCustomAttribute_Static_PI.TEXT_ALIGNMENT_SUFFIX, attribute, TextAlignment.Center ),
               -1
            );

         ( (SimpleImageLabelButton_Forms)retButton ).HorizontalOptions = LayoutOptions.FillAndExpand;
         ( (SimpleImageLabelButton_Forms)retButton ).VerticalOptions   = LayoutOptions.FillAndExpand;

         // IMPORTANT The button back color over-rides the fill color, of the shape view below it !!!
         ( (SimpleImageLabelButton_Forms)retButton ).ButtonBackColor = UIConst_Forms.HEADER_AND_KEY_LINE_COLOR;

         ( (SimpleImageLabelButton_Forms)retButton ).SetCornerRadiusFactor( UIConst_Forms.MEDIUM_CORNER_RADIUS_FACTOR );
         ( (SimpleImageLabelButton_Forms)retButton ).Margin  = STANDARD_MARGIN;
         ( (SimpleImageLabelButton_Forms)retButton ).Padding = STANDARD_PADDING;

         return retButton;
      }

      private abstract class DateTimeToPlainEnglishConverter_Base : DateTimeToPlainEnglishConverter
      {
         protected abstract int WhichSegment { get; }

         protected override string Convert( DateTime? value, object parameter )
         {
            var baseResult = base.Convert( value, parameter );

            if ( baseResult.IsNotEmpty() )
            {
               var segments = baseResult.Split( UIUtils_PI.SPACE_CHAR );

               if ( ( segments.Length >= 3 ) && ( WhichSegment >= 0 ) && ( WhichSegment < 3 ) )
               {
                  // Corner case for the last (2nd) segment - might contain several words
                  if ( ( segments.Length > 3 ) && ( WhichSegment == 2 ) )
                  {
                     // Pile the last words together with spaces; the request #2 will get all of them.
                     var retString = string.Join( UIUtils_PI.SPACE_CHAR.ToString(), segments.Skip( 2 ) );
                     return retString;
                  }

                  return segments[ WhichSegment ];
               }
            }

            return default;
         }
      }

      private class DateTimeToPlainEnglishConverter_TimePastPresent : DateTimeToPlainEnglishConverter_Base
      {
         protected override int WhichSegment => 2;
      }

      private class DateTimeToPlainEnglishConverter_TimeUnit : DateTimeToPlainEnglishConverter_Base
      {
         protected override int WhichSegment => 1;
      }

      private class DateTimeToPlainEnglishConverter_TimeValue : DateTimeToPlainEnglishConverter_Base
      {
         protected override int WhichSegment => 0;
      }

      private class DisplayKindToImageSourceConverter : OneWayConverter<string, ImageSource>
      {
         protected override ImageSource Convert( string value, object parameter )
         {
            switch ( value )
            {
               case DISPLAY_KIND_INFO:
               case DISPLAY_KIND_NOTICE:
               case DISPLAY_KIND_TASK:
                  return ImageSource.FromResource( SharedImageUtils.IMAGE_PRE_PATH + value.ToLower() + ".png",
                     typeof( SharedImageUtils ).Assembly );
            }

            return default;
         }
      }
   }
}