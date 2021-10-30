// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=CustomCheckBox_Forms.cs
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
   using System.ComponentModel;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Converters;
   using Com.MarcusTS.UI.XamForms.Common.Images;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   using Xamarin.Forms;

   /// <summary>
   /// Interface ICustomCheckBox_Forms
   /// </summary>
   public interface ICustomCheckBox_Forms : INotifyPropertyChanged
   {
      /// <summary>
      /// Gets or sets a value indicating whether this <see cref="ICustomCheckBox_Forms" /> is checked.
      /// </summary>
      /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
      bool IsChecked { get; set; }

      /// <summary>
      /// Occurs when [checked changed].
      /// </summary>
      IResponsiveTasks IsCheckedChangedTask { get; set; }

      double WidthHeight { get; set; }
   }

   /// <summary>
   /// Class CustomCheckBox.
   /// Implements the <see cref="Xamarin.Forms.Image" />
   /// Implements the <see cref="ICustomCheckBox_Forms" />
   /// </summary>
   /// <seealso cref="Xamarin.Forms.Image" />
   /// <seealso cref="ICustomCheckBox_Forms" />
   public class CustomCheckBox_Forms : Image, ICustomCheckBox_Forms
   {
      /// <summary>
      /// The checkbox checked image
      /// </summary>
      private const string CHECKBOX_CHECKED_IMAGE = SharedImageUtils.IMAGE_PRE_PATH + "checkbox_checked.png";

      /// <summary>
      /// The checkbox un checked image
      /// </summary>
      private const string CHECKBOX_UN_CHECKED_IMAGE = SharedImageUtils.IMAGE_PRE_PATH + "checkbox_unchecked.png";

      public static readonly BindableProperty IsCheckedProperty =
         CreateCheckBoxBindableProperty
         (
            nameof( IsChecked ),
            default( bool ),
            BindingMode.TwoWay,
            ( checkBox, oldVal, newVal ) =>
            {
               checkBox.IsCheckedChangedTask.RunAllTasksUsingDefaults( checkBox.IsChecked ).FireAndFuhgetAboutIt();
            }
         );

      private static readonly double DEFAULT_WIDTH_HEIGHT = 24.0.AdjustForOsAndDevice();

      /// <summary>
      /// Initializes a new instance of the <see cref="CustomCheckBox_Forms" /> class.
      /// </summary>
      public CustomCheckBox_Forms()
      {
         WidthRequest  = DEFAULT_WIDTH_HEIGHT;
         HeightRequest = DEFAULT_WIDTH_HEIGHT;
         Source        = CHECKBOX_UN_CHECKED_IMAGE;
         Aspect        = Aspect.AspectFit;
         var imageTapGesture = new TapGestureRecognizer();
         imageTapGesture.Tapped += ImageTapGestureOnTapped;
         GestureRecognizers.Add( imageTapGesture );
         PropertyChanged += OnPropertyChanged;

         this.SetUpBinding( SourceProperty, nameof( IsChecked ),
            converter: IsCheckedToImageSourceConverter.INSTANCE, source: this, bindingMode: BindingMode.OneWay );
      }

      private double _widthHeight { get; set; } = DEFAULT_WIDTH_HEIGHT;

      /// <summary>
      /// Gets or sets a value indicating whether this <see cref="CustomCheckBox_Forms" /> is checked.
      /// </summary>
      /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
      public bool IsChecked
      {
         get => (bool)GetValue( IsCheckedProperty );
         set => SetValue( IsCheckedProperty, value );
      }

      /// <summary>
      /// The checked changed event.
      /// </summary>
      public IResponsiveTasks IsCheckedChangedTask { get; set; } = new ResponsiveTasks( 1 );

      public double WidthHeight
      {
         get => _widthHeight;
         set
         {
            _widthHeight  = value;
            WidthRequest  = _widthHeight;
            HeightRequest = _widthHeight;
         }
      }

      public static BindableProperty CreateCheckBoxBindableProperty<PropertyTypeT>
      (
         string                                                     localPropName,
         PropertyTypeT                                              defaultVal     = default,
         BindingMode                                                bindingMode    = BindingMode.OneWay,
         Action<CustomCheckBox_Forms, PropertyTypeT, PropertyTypeT> callbackAction = null
      )
      {
         return BindableUtils_Forms.CreateBindableProperty( localPropName, defaultVal, bindingMode, callbackAction );
      }

      private void ImageTapGestureOnTapped( object sender, EventArgs eventArgs )
      {
         if ( IsEnabled )
         {
            IsChecked = !IsChecked;
         }
      }

      private void OnPropertyChanged( object sender, PropertyChangedEventArgs e )
      {
         if ( e.IsNotNullOrDefault() && e.PropertyName.IsSameAs( nameof( IsEnabled ) ) )
         {
            Opacity = IsEnabled ? UIConst_PI.VISIBLE_OPACITY : UIConst_PI.VISIBLE_OPACITY / 2;
         }
      }

      private class IsCheckedToImageSourceConverter : OneWayConverter<bool, ImageSource>
      {
         public static readonly IsCheckedToImageSourceConverter INSTANCE = new IsCheckedToImageSourceConverter();

         protected override ImageSource Convert( bool value, object parameter )
         {
            if ( value )
            {
               return ImageSource.FromResource( CHECKBOX_CHECKED_IMAGE, typeof( SharedImageUtils ).Assembly );
            }

            return ImageSource.FromResource( CHECKBOX_UN_CHECKED_IMAGE, typeof( SharedImageUtils ).Assembly );
         }
      }
   }
}