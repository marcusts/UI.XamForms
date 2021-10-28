// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=ComparisonEntryValidatorBehavior_Forms.cs
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

namespace Com.MarcusTS.UI.XamForms.Common.Validations
{
   using System.Collections.Generic;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.Common.Behaviors;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Forms;

   public interface IComparisonEntryValidatorBehavior_Forms : IEntryValidationBehavior_Forms
   {
      Entry CompareEntry { get; set; }
      Task  SetCompareEntry( Entry compareEntry );
   }

   /// <summary>
   /// Can pass in an illegal character filter. Also, to be valid, the two strings *must* match. Implements the
   /// <see cref="EntryValidationBehavior_Forms" />
   /// </summary>
   /// <seealso cref="EntryValidationBehavior_Forms" />
   public class ComparisonEntryValidatorBehavior_Forms : EntryValidationBehavior_Forms,
                                                         IComparisonEntryValidatorBehavior_Forms
   {
      /// <summary>The compare entry</summary>
      private Entry _compareEntry;

      /// <summary>Gets or sets the compare entry.</summary>
      /// <value>The compare entry.</value>
      public Entry CompareEntry
      {
         get => _compareEntry;

         set =>
            SetCompareEntry( value ).FireAndFuhgetAboutIt();
      }

      public async Task SetCompareEntry( Entry compareEntry )
      {
         if ( _compareEntry.IsNotAnEqualReferenceTo( compareEntry ) )
         {
            if ( _compareEntry != null )
            {
               _compareEntry.TextChanged -= CompareEntryOnPropertyChanged;
            }

            _compareEntry = compareEntry;

            if ( _compareEntry != null )
            {
               _compareEntry.TextChanged += CompareEntryOnPropertyChanged;
            }
         }

         await RevalidateAllConditions().WithoutChangingContext();
      }

      protected override IIsValidCondition_PI[] GetValidationConditions()
      {
         // The only condition is that this matches the original entry
         var retConditions = new List<IIsValidCondition_PI>
                             {
                                new IsValidCondition_PI
                                {
                                   RuleDescription = ValidationUtils_PI.MATCHES_PASSWORD,
                                   IsValidFunc     = ( targetObj, compareObj ) => CompareEntryMatchesMainEntry(),
                                },
                             };

         return retConditions.ToArray();
      }

      private void CompareEntryOnPropertyChanged
      (
         object               sender,
         TextChangedEventArgs textChangedEventArgs
      )
      {
         RevalidateAllConditions().FireAndFuhgetAboutIt();
      }

      private bool CompareEntryMatchesMainEntry()
      {
         return
            ( CompareEntry != null )
          &&
            CurrentText.IsNotEmpty()
          &&
            CompareEntry.Text.IsSameAs( CurrentText );
      }
   }
}