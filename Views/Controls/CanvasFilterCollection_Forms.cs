// *********************************************************************************
// Copyright @2020 Marcus Technical Services, Inc.
// <copyright
// file=CanvasFilterCollection_Forms.cs
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
   using System.Threading.Tasks;
   using Com.MarcusTS.ResponsiveTasks;
   using Com.MarcusTS.SharedUtils.Utils;

   public interface ICanvasFilterCollection_Forms
   {
      // 1. ICanvasFilterCollection_Forms>
      IResponsiveTasks FilterCollectionChangedTask { get; set; }

      ICanvasFilter_Forms[] Filters { get; set; }
   }

   public class CanvasFilterCollection_Forms : ICanvasFilterCollection_Forms
   {
      private ICanvasFilter_Forms[] _filters;

      // 1. ICanvasFilterCollection_Forms<ChildViewModelT>>
      public IResponsiveTasks FilterCollectionChangedTask { get; set; } = new ResponsiveTasks(1);

      public ICanvasFilter_Forms[] Filters
      {
         get => _filters;
         set
         {
            if (_filters.IsNotAnEmptyList())
            {
               foreach (var filter in _filters)
               {
                  filter.FilterChangedTask.RemoveIfThere(this, HandleChildFilterChanged);
               }
            }

            _filters = value;

            if (_filters.IsNotAnEmptyList())
            {
               foreach (var filter in _filters)
               {
                  filter.FilterChangedTask.AddIfNotAlreadyThere(this, HandleChildFilterChanged);
               }
            }
         }
      }

      private Task HandleChildFilterChanged(IResponsiveTaskParams paramDict)
      {
         // Filters are ignored; they might be empty
         return FilterCollectionChangedTask.RunAllTasksUsingDefaults(this);
      }
   }
}