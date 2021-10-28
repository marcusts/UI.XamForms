// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=CanSortComparer.cs
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

#define HACK_INVERTED_DATE_TIME_SORTS

namespace Com.MarcusTS.UI.XamForms.Common.Utils
{
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Reflection;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.SharedUtils.Utils;

   public interface ICanSortComparer : IComparer<object>
   {
      bool IsReady { get; }
   }

   public class CanSortComparer : ICanSortComparer
   {
      private const           int                     IS_EQUAL                  = 0;
      private const           int                     IS_LESS                   = -1;
      private const           int                     IS_MORE                   = 1;
      private static readonly CaseInsensitiveComparer CASE_INSENSITIVE_COMPARER = new CaseInsensitiveComparer();

      private readonly KeyValuePair<PropertyInfo, ICanSort>[] _sortKeyList;

      public CanSortComparer( KeyValuePair<PropertyInfo, ICanSort>[] sortKeyList )
      {
         _sortKeyList = sortKeyList;
      }

      public bool IsReady => _sortKeyList.IsNotAnEmptyList();

      /// <remarks>
      /// Returns -1 if less  than, 0 if equal, and 1 if greater than
      /// ICanSort's default sort order is ignored here, as we often insert a tapped value in front of it.
      /// </remarks>
      public int Compare( object x, object y )
      {
         if ( _sortKeyList.IsAnEmptyList() )
         {
            return IS_EQUAL;
         }


         // The contract with this object is that it is:
         //    - A gross variable (not a property within) such as ContentView.Content.BindingContext
         //    - Is described by the PropertyInfo list
         // Otherwise, we don't know (or care) about the object's type.

         var retComparison = IS_EQUAL;

         // For each sort, get the value() for both objects and decide which is less.
         foreach ( var condition in _sortKeyList )
         {
            var sortDescending = condition.Value.DefaultSortDescending.IsTrue();
            var sortType       = condition.Key.PropertyType;
            var xVal           = x.IsNullOrDefault() ? null : condition.Key.GetValue( x );
            var yVal           = y.IsNullOrDefault() ? null : condition.Key.GetValue( y );

            // Nullable<T> corner case
            var nullableBaseType = Nullable.GetUnderlyingType( sortType );

            if ( nullableBaseType.IsNotNullOrDefault() )
            {
               sortType = nullableBaseType;
            }

            // Null cases
            if ( xVal == null )
            {
               if ( yVal == null )
               {
                  return IS_EQUAL;
               }

               return sortDescending ? IS_MORE : IS_LESS;
            }

            // ELSE xVal != null
            if ( yVal == null )
            {
               return sortDescending ? IS_LESS : IS_MORE;
            }

            // ELSE

#if HACK_INVERTED_DATE_TIME_SORTS
            if ( sortType == typeof( DateTime ) )
            {
               // BACKWARDS
               sortDescending = !sortDescending;
            }
#endif

            // Handle first to protect strict string handling
            if ( sortType == typeof( string ) )
            {
               if ( CompareAsStrings( xVal, yVal, sortDescending ) )
               {
                  break;
               }
            }
            else if ( typeof( IComparable ).IsAssignableFrom( sortType ) )
            {
               var comparableComparison = ( (IComparable)xVal ).CompareTo( (IComparable)yVal );

               if ( comparableComparison != 0 )
               {
                  retComparison = sortDescending ? comparableComparison * -1 : comparableComparison;
                  break;
               }
            }

            // Check equality first, then perform a string comparison (last-ditch effort)
            else if ( xVal.IsNotAnEqualObjectTo( yVal ) )
            {
               if ( CompareAsStrings( xVal.ToString(), yVal.ToString(), sortDescending ) )
               {
                  break;
               }
            }
         }

         return retComparison;

         // ------------------------------------------------------------------------------------------------------------------
         // PRIVATE METHODS
         // ------------------------------------------------------------------------------------------------------------------
         bool CompareAsStrings( object xVal, object yVal, bool sortDescending )
         {
            var xStringVal       = xVal?.ToString().Trim();
            var yStringVal       = yVal?.ToString().Trim();
            var stringComparison = CASE_INSENSITIVE_COMPARER.Compare( xStringVal, yStringVal );

            if ( stringComparison != 0 )
            {
               retComparison = sortDescending ? stringComparison * -1 : stringComparison;
               return true;
            }

            return false;
         }

         // ------------------------------------------------------------------------------------------------------------------
      }
   }
}