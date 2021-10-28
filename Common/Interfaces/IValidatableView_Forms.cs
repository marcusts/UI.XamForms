// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=IValidatableView_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Common.Interfaces
{
   using System.Threading.Tasks;
using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.UI.XamForms.Common.Validations;
   using Com.MarcusTS.UI.XamForms.Views.Subviews;
   using Xamarin.Forms;

   /// <remarks>
   /// The base class comes from the view model attributes.
   /// This class adds Xamarin.Forms specific properties.
   /// </remarks>
   public interface IValidatableView_Forms : IValidatableEntryGeneral_PI, IHaveViewModelPropertyName
   {
      ShapeView_Forms               BorderView                 { get; }
      Color                         BorderViewBorderColor      { get; set; }
      BindingMode                   BoundMode                  { get; set; }
      BindableProperty              BoundProperty              { get; set; }
      IValueConverter               Converter                  { get; set; }
      object                        ConverterParameter         { get; set; }
      View                          EditableView               { get; }
      FontAttributes                InstructionsFontAttributes { get; set; }
      Label                         InstructionsLabel          { get; }
      Color                         InstructionsTextColor      { get; set; }
      Style                         InvalidBorderViewStyle     { get; set; }
      FontAttributes                InvalidFontAttributes      { get; set; }
      Style                         InvalidInstructionsStyle   { get; set; }
      Style                         InvalidPlaceholderStyle    { get; set; }
      Color                         InvalidTextColor           { get; set; }
      Color                         PlaceholderBackColor       { get; set; }
      Label                         PlaceholderLabel           { get; }
      Color                         PlaceholderTextColor       { get; set; }
      IValidationBehaviorBase_Forms Validator                  { get; set; }
      Style                         ValidBorderViewStyle       { get; set; }
      FontAttributes                ValidFontAttributes        { get; set; }
      Style                         ValidInstructionsStyle     { get; set; }
      Style                         ValidPlaceholderStyle      { get; set; }
      Color                         ValidTextColor             { get; set; }

      Task CallRevalidate();

      int SetTabIndexes(int incomingTabIndex);

      Task StopConstructionAndRefresh();
   }
}