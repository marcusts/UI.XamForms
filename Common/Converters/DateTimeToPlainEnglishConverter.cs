// Copyright (c) 2019  Marcus Technical Services, Inc. <marcus@marcusts.com>
//
// This file, DateTimeToPlainEnglishConverter.cs, is a part of a program called AccountViewMobile.
//
// AccountViewMobile is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Permission to use, copy, modify, and/or distribute this software
// for any purpose with or without fee is hereby granted, provided
// that the above copyright notice and this permission notice appear
// in all copies.
//
// AccountViewMobile is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// For the complete GNU General Public License,
// see <http://www.gnu.org/licenses/>.

namespace Com.MarcusTS.UI.XamForms.Common.Converters
{
   using System;
   using Com.MarcusTS.SharedUtils.Utils;

   /// <summary>
   /// Class DateTimeToPlainEnglishConverter.
   /// Implements the <see cref="string" />
   /// </summary>
   /// <seealso cref="string" />
   public class DateTimeToPlainEnglishConverter : OneWayConverter<DateTime?, string>
   {
      /// <summary>
      /// The ago
      /// </summary>
      private const string AGO = " ago";
      private const string FROM_NOW = " from now";

      /// <summary>
      /// Converts the specified value.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <param name="parameter">The parameter.</param>
      /// <returns>System.String.</returns>
      protected override string Convert(DateTime? value, object parameter)
      {
         if (!value.HasValue)
         {
            return "";
         }

         var isPast   = DateTime.Now > value;
         var timeDiff = DateTime.Now.Subtract(value.GetValueOrDefault());
         var days = Math.Abs(timeDiff.Days);

         // Start with the broadest cases
         if (days > 0)
         {
            var yearsRaw = days / 365.25;
            var years    = Math.Floor(yearsRaw).ToRoundedInt();

            if (years > 0)
            {
               return ConvertToPastOrPresent($"{years:0} year(s)");
            }

            var months = Math.Floor(yearsRaw * 12).ToRoundedInt();
            if (months > 0)
            {
               return ConvertToPastOrPresent($"{months:0} month(s)");
            }

            return ConvertToPastOrPresent($"{days:0} day(s)");
         }

         var hours = Math.Abs(timeDiff.Hours);
         if (hours > 0)
         {
            return ConvertToPastOrPresent($"{hours:0} hour(s)");
         }

         var minutes = Math.Abs(timeDiff.Minutes);
         if (minutes > 0)
         {
            return ConvertToPastOrPresent($"{minutes:0} minutes(s)");
         }

         return "Just now";
         

         // PRIVATE METHODS
         string ConvertToPastOrPresent(string str)
         {
            return isPast ? str + AGO : str + FROM_NOW;
         }
      }
   }
}
