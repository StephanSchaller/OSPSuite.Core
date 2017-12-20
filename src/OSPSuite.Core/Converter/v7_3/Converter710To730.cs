﻿using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization;

namespace OSPSuite.Core.Converter.v7_3
{
   public class Converter710To730 : IObjectConverter
   {
      public bool IsSatisfiedBy(int version)
      {
         return version == PKMLVersion.V7_1_0;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToUpdate)
      {
         return (PKMLVersion.V7_3_0, false);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element)
      {
         var converted = false;
         //retrieve all elements with an attribute dimension
         var allValueDescriptionAttributes = from child in element.DescendantsAndSelf()
            where child.HasAttributes
            let attr = child.Attribute(Constants.Serialization.Attribute.VALUE_DESCRIPTION)
            where attr != null
            select attr;


         foreach (var valueDescriptionAttribute in allValueDescriptionAttributes)
         {
            var (description, parentElement) = (valueDescriptionAttribute.Value, valueDescriptionAttribute.Parent);
            valueDescriptionAttribute.Remove();
            parentElement.Add(valueOriginFor(description));
            converted = true;
         }

         return (PKMLVersion.V7_3_0, converted);
      }

      private XElement valueOriginFor(string valueDescriptioon)
      {
         var element = new XElement(Constants.Serialization.VALUE_ORIGIN);
         element.SetAttributeValue(Constants.Serialization.Attribute.DESCRIPTION, valueDescriptioon);
         return element;
      }
   }
}