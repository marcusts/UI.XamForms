// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=FlowableSettings_Forms.cs
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
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.UI.XamForms.Common.Utils;
   
   using Xamarin.Forms;

   public interface IFlowableSettings_Forms
   {
      Easing FadeInEasing                { get; set; }
      uint   FadeInMilliseconds          { get; set; }
      Easing TranslateBoundsEasing       { get; set; }
      uint   TranslateBoundsMilliseconds { get; set; }
   }

   public class FlowableSettings_Forms : IFlowableSettings_Forms
   {
      public Easing FadeInEasing                { get; set; } = UIConst_Forms.DEFAULT_EASING;
      public uint   FadeInMilliseconds          { get; set; } = FlowableConst_PI.DEFAULT_FADE_IN_MILLISECONDS;
      public Easing TranslateBoundsEasing       { get; set; } = UIConst_Forms.DEFAULT_EASING;
      public uint   TranslateBoundsMilliseconds { get; set; } = FlowableConst_PI.DEFAULT_TRANSLATE_BOUNDS_MILLISECONDS;
   }

   public interface IHaveFlowableSettings
   {
      IFlowableSettings_Forms FlowSettings { get; set; }
   }
}