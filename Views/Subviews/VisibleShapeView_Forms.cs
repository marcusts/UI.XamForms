// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=VisibleShapeView_Forms.cs
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
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;

   public interface IVisibleShapeView_Forms : IShapeView_Forms, IRespondToVisibilityChanges_PI
   { }

   public class VisibleShapeView_Forms : ShapeView_Forms, IVisibleShapeView_Forms
   {
      public VisibleShapeView_Forms()
      {
         CornerRadius = 0;
         SetCornerRadiusFactor(0).FireAndFuhgetAboutIt();
      }

      public IResponsiveTasks AfterUserVisibilityChangedTask { get; set; } = new ResponsiveTasks(1);

      public IResponsiveTasks BeforeUserVisibilityChangedTask { get; set; } = new ResponsiveTasks(1);

      public bool IsVisibleToUser { get; private set; }

      public async Task SetIsVisibleToUser(bool isVisible, bool forceSet = false)
      {
         await BeforeUserVisibilityChangedTask.RunAllTasksUsingDefaults(IsVisibleToUser).WithoutChangingContext();

         IsVisibleToUser = isVisible || forceSet;

         await OnSetIsVisibleToUser(isVisible).WithoutChangingContext();

         await AfterUserVisibilityChangedTask.RunAllTasksUsingDefaults(IsVisibleToUser).WithoutChangingContext();
      }

      protected virtual Task OnSetIsVisibleToUser(bool isVisible)
      {
         return Task.CompletedTask;
      }
   }
}