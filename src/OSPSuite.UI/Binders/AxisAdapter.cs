﻿using System;
using System.Drawing;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Format;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Chart.Mappers;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.UI.Extensions;
using Axis = DevExpress.XtraCharts.Axis;

namespace OSPSuite.UI.Binders
{
   /// <summary>
   ///    Adapter from Axis (DataChart.Data) to a AxisView (DevExpress.XtraCharts).
   /// </summary>
   internal class AxisAdapter : IAxisAdapter
   {
      private const int DEVEXPRESS_DEFAULT_Y_MINOR_TICKS = 4;
      private const int DEVEXPRESS_DEFAULT_X_MINOR_TICKS = 1;
      private readonly Axis _axisView;
      private readonly INumericFormatterOptions _numericFormatterOptions;
      private readonly int _defaultMinorTickCount;
      private bool _explicitRange;
      private readonly UnitToMinorIntervalMapper _unitToMinorIntervalMapper;

      public AxisAdapter(IAxis axis, Axis axisView, INumericFormatterOptions numericFormatterOptions)
      {
         Axis = axis;
         _unitToMinorIntervalMapper = new UnitToMinorIntervalMapper();

         _defaultMinorTickCount = Axis.AxisType == AxisTypes.X ? DEVEXPRESS_DEFAULT_X_MINOR_TICKS : DEVEXPRESS_DEFAULT_Y_MINOR_TICKS;

         _axisView = axisView;
         _axisView.VisualRange.Auto = false;
         _numericFormatterOptions = numericFormatterOptions;
         Axis.Changed += onAxisChanged;
         refresh();
      }

      public AxisTypes AxisType => Axis.AxisType;

      public IAxis Axis { get; }

      public bool Visible
      {
         get { return _axisView.Visibility == DefaultBoolean.True; }
         set
         {
            var reallyVisible = value && Axis.Visible;
            _axisView.Visibility = reallyVisible ? DefaultBoolean.True : DefaultBoolean.False;
            _axisView.GridLines.Visible = reallyVisible && Axis.GridLines;
         }
      }

      public void Dispose()
      {
         Axis.Changed -= onAxisChanged;
      }

      private void onAxisChanged(object sender)
      {
         refresh();
      }

      private void refresh()
      {
         if (_axisView == null || Axis.Dimension == null || Axis.UnitName == null)
            return;

         _axisView.GridLines.Visible = Axis.GridLines;
         if(!Axis.Visible)
            Visible = false;
         setAxisTitle();
         setLabels();
      }

      public object AxisView => _axisView;

      public void RefreshRange(bool sideMarginsEnabled, Size diagramSize)
      {
         setAxisRange(sideMarginsEnabled, diagramSize);
      }

      /// <summary>
      ///    Cut the min value for logarithmic axis by the smallest positive value possible.
      /// </summary>
      private void adjustMinForLogScale(Range range)
      {
         if (range.MinValue == null)
            return;
         var doubleValue = Convert.ToDouble(range.MinValue);


         if (!(doubleValue < double.Epsilon)) return;
         range.Auto = false;
         range.MinValue = double.Epsilon;
      }

      private bool isAuto => (Axis.NumberMode == NumberModes.Relative || noLimitsSet);

      private bool noLimitsSet => (!Axis.Min.HasValue && !Axis.Max.HasValue);

      private bool allLimitsSet => (Axis.Min.HasValue && Axis.Max.HasValue);

      private void adjustAxisMinMax()
      {
         //for log scaling adjust the min value if neccessary to minimum positive value
         if (Axis.Scaling == Scalings.Log)
            if (Axis.Min.HasValue && Axis.Min < float.Epsilon)
               Axis.Min = float.Epsilon;

         //both limits are set
         if (allLimitsSet)
         {
            if (Axis.Min >= Axis.Max)
               Axis.SetRange(null, null);
            return;
         }

         //no limits are set
         if (noLimitsSet)
            return;

         if (_explicitRange) // do we come from explicit range? Somebody deleted a limit...
         {
            Axis.SetRange(null, null);
         }
         else // somebody inserted a limit...
         {
            if (!Axis.Min.HasValue) Axis.Min = Convert.ToSingle(_axisView.WholeRange.MinValue);
            if (!Axis.Max.HasValue) Axis.Max = Convert.ToSingle(_axisView.WholeRange.MaxValue);
         }
         _explicitRange = !_explicitRange;
      }

      private void setAxisRange(bool sideMarginsEnabled, Size diagramSize)
      {
         var axisWidthInPixel = Axis.AxisType == AxisTypes.X ? diagramSize.Width : diagramSize.Height;
     
         adjustAxisMinMax();
         setRange(isAuto, sideMarginsEnabled);

         configureAxisScale(axisWidthInPixel);

         _axisView.Logarithmic = (Axis.Scaling == Scalings.Log);
         _axisView.WholeRange.AlwaysShowZeroLevel = !_axisView.Logarithmic;

         // logarithmic scale depending settings
         if (!_axisView.Logarithmic) return;

         //8 minor counts in log mode
         _axisView.MinorCount = 8;
         adjustMinForLogScale();
      }

      private bool shouldApplyPreferredMinorTicks()
      {
         if (Axis.Dimension == null)
            return false;

          return Axis.Dimension.IsTime() 
            && _unitToMinorIntervalMapper.HasPreferredMinorIntervalsFor(Axis.Unit);
      }

      private void automaticallyConfigureAxisScale()
      {
         _axisView.MinorCount = _defaultMinorTickCount;
         _axisView.NumericScaleOptions.AutoGrid = true;
      }

      private void configureAxisScale(int axisWidthInPixel)
      {
         var ticksConfig = new TicksConfig{AutoScale = true};

         if (shouldApplyPreferredMinorTicks())
            ticksConfig = calculateMinorIntervalsPerMajorInterval(axisWidthInPixel);

         if (ticksConfig.AutoScale)
         {
            automaticallyConfigureAxisScale();
         }
         else
         {
            _axisView.MinorCount = ticksConfig.MinorTicks;
            _axisView.Tickmarks.MinorVisible = true;

            _axisView.NumericScaleOptions.AutoGrid = false;
            _axisView.NumericScaleOptions.GridSpacing = ticksConfig.GridSpacing;
            _axisView.NumericScaleOptions.GridAlignment = NumericGridAlignment.Ones;
            _axisView.NumericScaleOptions.MeasureUnit = NumericMeasureUnit.Ones;
         }
      }

      private TicksConfig calculateMinorIntervalsPerMajorInterval(int axisWidthInPixel)
      {
         var axisWidthInUnit = Convert.ToSingle(_axisView.VisualRange.MaxValue) - Convert.ToSingle(_axisView.VisualRange.MinValue); 
         return _unitToMinorIntervalMapper.MapFrom(Axis.Unit, axisWidthInUnit, axisWidthInPixel);
      }

      private void adjustMinForLogScale()
      {
         adjustMinForLogScale(_axisView.WholeRange);
         adjustMinForLogScale(_axisView.VisualRange);
      }

      private void setRange(bool autoRange, bool sideMarginsEnabled)
      {
         // For DevExpress, setting the sideMargins affects the auto range calculation property. 
         // We want to decouple that relationship so set side margins first
         // then we set the auto range.
         _axisView.SetSideMarginsEnabled(sideMarginsEnabled);

         if (autoRange)
            enableAutoRange(true);
         else
            setExplicitRange();
      }

      private void setExplicitRange()
      {
         if (!allLimitsSet)
            return;

         enableAutoRange(false);
         var currentMin = Convert.ToDouble(_axisView.WholeRange.MinValue);
         var currentMax = Convert.ToDouble(_axisView.WholeRange.MaxValue);
         var settingsMin = Convert.ToDouble(Axis.Min);
         var settingsMax = Convert.ToDouble(Axis.Max);

         //first we might have to enlarge the whole range
         if (settingsMin < currentMin || settingsMax > currentMax)
            _axisView.WholeRange.SetMinMaxValues(settingsMin, settingsMax);

         //if limits are already set we avoid a new setting
         if (Convert.ToDouble(_axisView.VisualRange.MinValue).Equals(settingsMin) &&
             Convert.ToDouble(_axisView.VisualRange.MaxValue).Equals(settingsMax))
            return;
         _axisView.VisualRange.SetMinMaxValues(settingsMin, settingsMax);
      }

      private void enableAutoRange(bool autoRangeEnabled)
      {
         // modify the Auto property of WholeRange
         _axisView.WholeRange.Auto = autoRangeEnabled;

         if (!autoRangeEnabled)
            return;

         // we only reset the VisualRange to WholeRange if enabling autoRange
         // do not auto range the VisualRange because it has unwanted side effects
         _axisView.ResetVisualRange();
      }

      private void setLabels()
      {
         string pattern;
         switch (Axis.NumberMode)
         {
            case NumberModes.Scientific:
               pattern = "V:E{0}".FormatWith(_numericFormatterOptions.DecimalPlace);
               break;
            case NumberModes.Normal:
               pattern = "V:G";
               break;
            case NumberModes.Relative:
               pattern = "V:P";
               break;
            default:
               return;
         }
         updateLabelTextPattern(pattern);
      }

      private void updateLabelTextPattern(string pattern)
      {
         pattern = $"{{{pattern}}}";
         if (string.Equals(_axisView.Label.TextPattern, pattern))
            return;

         _axisView.Label.TextPattern = pattern;
      }

      private void setAxisTitle()
      {
         _axisView.Title.Visibility = DefaultBoolean.True;
         _axisView.Title.Alignment = StringAlignment.Center;

         _axisView.Title.Text = Constants.NameWithUnitFor(!string.IsNullOrEmpty(Axis.Caption) ? Axis.Caption : Axis.Dimension.DisplayName, Axis.UnitName);
      }
   }
}