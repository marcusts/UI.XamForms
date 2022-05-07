// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=ViewModelCustomAttibuteUtils_Forms.cs
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
   using System.Reflection;
   using Com.MarcusTS.PlatformIndependentShared.Annotations;
   using Com.MarcusTS.PlatformIndependentShared.Common.Interfaces;
   using Com.MarcusTS.PlatformIndependentShared.Common.Utils;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.SharedUtils.Utils;
   using Xamarin.Forms;

   public static class ViewModelCustomAttibuteUtils_Forms
   {
      public static (bool, Color?) CanBeConvertedToColor(this string str)
      {
         var parseResult = str.CanBeParsedFromHex<Color>();

         if (parseResult.Item1)
         {
            var retColor = Color.FromRgba(parseResult.Item2[0], parseResult.Item2[1], parseResult.Item2[2],
               parseResult.Item2[3]);
            return (true, retColor);
         }

         return (false, null);
      }

      public static void CopyCommmonPropertiesFromAttribute_Forms<TargetT, SourceT>
      (
         this TargetT   target,
         SourceT        sourceAttribute,
         PropertyInfo[] targetPropInfos = default,
         PropertyInfo[] sourcePropInfos = default
      )
         where SourceT : class
      {
         // Copy properties *if* they are set, else *ignore*
         target.CoerceSettablePropertyValuesFrom(
            sourceAttribute,
            CoerceProperty_Forms(),
            targetPropInfos,
            sourcePropInfos);

         // ----------------------------------------------------------------------------------------------------------------------------------------------------
         // PRIVATE METHODS
         // ----------------------------------------------------------------------------------------------------------------------------------------------------
         Func<PropertyInfo, SourceT, (bool, string, object)> CoerceProperty_Forms()
         {
            return
               (propInfo, source) =>
               {
                  var    includeIt    = false;
                  var    retPropName  = propInfo.Name;
                  object retPropValue = null;

                  // Get the current property value; the property name won't affect this "get"
                  retPropValue = source.IsNullOrDefault() ? null : propInfo.GetValue(source);

                  if (retPropValue is int retPropValueAsInt)
                  {
                     includeIt = !retPropValueAsInt.IsUnset();
                  }
                  else if (retPropValue is double retPropValueAsDouble)
                  {
                     includeIt = !retPropValueAsDouble.IsUnset();

                     if (includeIt)
                     {
                        // Corner case for certain doubles: the attribute value must be adjusted here for the operating system.
                        var isOsSensitive =
                           retPropName.EndsWith(ViewModelCustomAttribute_Static_PI.OPERATING_SYSTEM_SUFFIX);

                        if (isOsSensitive)
                        {
                           // If ending in OS suffix, remove that
                           retPropName = UIUtils_PI.RemoveSuffix(retPropName,
                              ViewModelCustomAttribute_Static_PI.OPERATING_SYSTEM_SUFFIX);
                           retPropValue = retPropValueAsDouble.AdjustForOsAndDevice();
                        }
                        else
                        {
                           retPropValue = retPropValueAsDouble;
                        }
                     }
                  }
                  else if (retPropValue is string retPropValueAsString)
                  {
                     retPropValue = retPropValueAsString;
                     includeIt    = !retPropValueAsString.IsUnset();
                  }

                  // ALL CASES LAND HERE
                  return (includeIt, retPropName, retPropValue);
               };
         }

         // ----------------------------------------------------------------------------------------------------------------------------------------------------
      }

      private static (bool, EnumT) EnumSuffixParsedOK<EnumT>
         (
            this string propStrVal, 
            string suffix,
            IViewModelCustomAttributeRoot_PI attribute
         )
         where EnumT : Enum
      {
         if (!attribute.ViewModelPropertyName.EndsWith(suffix))
         {
            return (false, default);
         }

         // ELSE

         // Look for ther enum using the assigned value
         var enumNames  = typeof(EnumT).GetEnumNames();
         var enumValues = typeof(EnumT).GetEnumValues();

         for (var enumIdx = 0; enumIdx < enumNames.Length; enumIdx++)
         {
            if (enumNames[enumIdx].IsSameAs(propStrVal))
            {
               // Get the actual enum
               return (true, (EnumT) enumValues.GetValue(enumIdx));
            }
         }

         // ELSE FAIL
         return (false, default);
      }

      /// <summary>
      /// To defeat the default, skip it. Transparent will then be returned.
      /// </summary>
      /// <param name="propStrVal"></param>
      /// <param name="attribute"></param>
      /// <param name="defaultColor"></param>
      /// <returns></returns>
      public static Color ToOsEquivalentColor(this string propStrVal, IViewModelCustomAttributeRoot_PI attribute,
                                              Color?      defaultColor = null)
      {
         // Source properties contain Hex colors as strings
         if (!propStrVal.IsUnset() && attribute.ViewModelPropertyName.EndsWith(ViewModelCustomAttribute_Static_PI.COLOR_HEX_SUFFIX))
         {
            // If ther assigned string value starts with "#", remove that too
            if (propStrVal.StartsWith(ViewModelCustomAttribute_Static_PI.POUND_SYMBOL))
            {
               propStrVal = propStrVal.Substring(1);
            }

            // ReSharper disable once PossibleNullReferenceException
            (var doIncludeIt, var parsedColor) = CanBeConvertedToColor(propStrVal);

            if (doIncludeIt && parsedColor.HasValue)
            {
               return parsedColor.Value;
            }
         }

         return defaultColor ?? Color.Transparent;
      }

      /// <summary>
      /// The default can be defeated by ignoring it
      /// </summary>
      public static double ToOsEquivalentPositiveNumber(this double dbl, IViewModelCustomAttributeRoot_PI attribute,
                                                        double      defaultDbl = 0.0)
      {
         if (dbl.IsUnset() || !dbl.IsANumberGreaterThanZero())
         {
            return defaultDbl;
         }


         // ELSE
         return
            attribute.ViewModelPropertyName.EndsWith(ViewModelCustomAttribute_Static_PI.OPERATING_SYSTEM_SUFFIX)
               ? dbl.AdjustForOsAndDevice()
               : dbl;
      }
      
      public static EnumT ToOsEquivalentEnum<EnumT>
         (
            this string                      propStrVal, 
            string                           suffix,
            IViewModelCustomAttributeRoot_PI attribute,
            EnumT                            defaultEnum = default
         )
         where EnumT : Enum
      {
         if (propStrVal.IsUnset())
         {
            return defaultEnum;
         }
         
         // ELSE
         
         (var parsedOK, var retEnumVal) = propStrVal.EnumSuffixParsedOK<EnumT>(suffix, attribute);
         
         if (parsedOK)
         {
            return retEnumVal;
         }


         // ELSE
         return defaultEnum;
      }
   }
}
