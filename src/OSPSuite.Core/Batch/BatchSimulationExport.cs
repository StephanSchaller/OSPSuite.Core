﻿using System.Collections.Generic;

namespace OSPSuite.Core.Batch
{
   public class BatchSimulationExport
   {
      public string Name { get; set; }
      public float[] Time { get; set; }
      public List<BatchOutputValues> OutputValues { get; set; } = new List<BatchOutputValues>();
      public List<ParameterValue> ParameterValues { get; set; } = new List<ParameterValue>();
   }
}