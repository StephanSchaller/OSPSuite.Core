﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.Assets;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Serialization;
using OSPSuite.Presentation.Settings;
using OSPSuite.Starter.Tasks;
using OSPSuite.Starter.Views;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Starter.Presenters
{
   public interface IChartTestPresenter : IPresenter<IChartTestView>, ICommandCollectorPresenter
   {
      void LoadChart();
      void SaveChart();
      void SaveSettings();
      void SaveChartWithDataWithoutValues();
      void LoadSettings();
      void RefreshDisplay();
      void ReloadMenus();
      void RemoveDatalessCurves();
      void InitializeRepositoriesWithOriginalData();
      void AddObservations(int numberOfObservations, int pointsPerCalculation, double? lloq);
      void AddCalculations(int numberOfCalculations, int pointsPerCalculation);
      void ClearChart();
      void AddObservationsWithArithmeticDeviation(int numberOfObservations, int pointsPerObservation, double? lloq);
      void AddObservationsWithGeometricDeviation(int numberOfObservations, int pointsPerObservation, double? lloq);
      void AddCalculationsWithGeometricMean(int numberOfCalculations, int pointsPerCalculation);
      void AddCalculationsWithArithmeticMean(int numberOfCalculations, int pointsPerCalculation);
   }

   public class ChartTestPresenter : AbstractCommandCollectorPresenter<IChartTestView, IChartTestPresenter>, IChartTestPresenter
   {
      private readonly IChartEditorAndDisplayPresenter _chartEditorAndDisplayPresenter;
      private readonly IContainer _model;
      private readonly IDataRepositoryCreator _dataRepositoryCreator;
      private readonly IOSPSuiteXmlSerializerRepository _ospSuiteXmlSerializerRepository;
      private readonly DataPersistor _dataPersistor;
      private readonly IChartFromTemplateService _chartFromTemplateService;
      private readonly IChartTemplatePersistor _chartTemplatePersistor;
      private readonly IDimensionFactory _dimensionFactory;
      private readonly IChartUpdater _chartUpdater;
      private ICache<string, DataRepository> _dataRepositories;
      private readonly IWithChartTemplates _simulationSettings;

      public ChartTestPresenter(IChartTestView view, IChartEditorAndDisplayPresenter chartEditorAndDisplayPresenter, TestEnvironment testEnvironment, IDataColumnToPathElementsMapper dataColumnToPathColumnValuesMapper,
         IDataRepositoryCreator dataRepositoryCreator, IOSPSuiteXmlSerializerRepository ospSuiteXmlSerializerRepository, DataPersistor dataPersistor, IChartFromTemplateService chartFromTemplateService,
         IChartTemplatePersistor chartTemplatePersistor, IDimensionFactory dimensionFactory, IChartUpdater chartUpdater) : base(view)
      {
         _model = testEnvironment.Model.Root;
         _dataRepositories = new Cache<string, DataRepository>(repository => repository.Name);
         _chartEditorAndDisplayPresenter = chartEditorAndDisplayPresenter;
         _dataRepositoryCreator = dataRepositoryCreator;
         _ospSuiteXmlSerializerRepository = ospSuiteXmlSerializerRepository;
         _dataPersistor = dataPersistor;
         _chartFromTemplateService = chartFromTemplateService;
         _chartTemplatePersistor = chartTemplatePersistor;
         _dimensionFactory = dimensionFactory;
         _chartUpdater = chartUpdater;
         _view.AddChartEditorView(chartEditorAndDisplayPresenter.EditorPresenter.View);
         _view.AddChartDisplayView(chartEditorAndDisplayPresenter.DisplayPresenter.View);
         _simulationSettings = new SimulationSettings();
         AddSubPresenters(chartEditorAndDisplayPresenter);

         configureChartEditorPresenter(dataColumnToPathColumnValuesMapper);

         bindCurveChartToEditor();

         configureChartEditorEvents();
         InitializeWith(new OSPSuiteMacroCommand<IOSPSuiteExecutionContext>());
      }

      private void configureChartEditorEvents()
      {
         ChartEditorPresenter.ColumnSettingsFor(BrowserColumns.Origin).Visible = true;

         ChartEditorPresenter.SetCurveNameDefinition(Helper.NameDefinition);
         ChartEditorPresenter.ColumnSettingsFor(CurveOptionsColumns.InterpolationMode).Visible = false;

         ChartEditorPresenter.ColumnSettingsFor(AxisOptionsColumns.NumberMode).Caption = "Number Representation";
         ChartEditorPresenter.ColumnSettingsFor(AxisOptionsColumns.NumberMode).VisibleIndex = 1;

         ChartDisplayPresenter.Edit(Chart);

         ChartDisplayPresenter.DragOver += onChartDisplayDragOver;
         ChartEditorPresenter.DragOver += onChartDisplayDragOver;

         ReloadMenus();
      }

      public void InitializeRepositoriesWithOriginalData()
      {
         var newRepositories = _dataRepositoryCreator.CreateOriginalDataRepositories(_model).ToList();
         ChartEditorPresenter.AddDataRepositories(newRepositories);
         addNewRepositories(newRepositories);
      }

      private void addNewRepositories(IEnumerable<DataRepository> newRepositories)
      {
         _dataRepositories.AddRange(newRepositories);
      }

      public void AddCalculationsWithGeometricMean(int numberOfCalculations, int pointsPerCalculation)
      {
         addRepositoryToChart(_dataRepositoryCreator.CreateCalculationsWithGeometricMean(numberOfCalculations, _model, _dataRepositories.Count, pointsPerCalculation));
      }

      public void AddCalculationsWithArithmeticMean(int numberOfCalculations, int pointsPerCalculation)
      {
         addRepositoryToChart(_dataRepositoryCreator.CreateCalculationsWithArithmeticMean(numberOfCalculations, _model, _dataRepositories.Count, pointsPerCalculation));
      }

      public void AddObservationsWithGeometricDeviation(int numberOfObservations, int pointsPerObservation, double? lloq)
      {
         addRepositoryToChart(_dataRepositoryCreator.CreateObservationWithGeometricDeviation(numberOfObservations, _model, _dataRepositories.Count, pointsPerObservation, lloq));
      }

      public void AddObservationsWithArithmeticDeviation(int numberOfObservations, int pointsPerObservation, double? lloq)
      {
         addRepositoryToChart(_dataRepositoryCreator.CreateObservationWithArithmenticDeviation(numberOfObservations, _model, _dataRepositories.Count, pointsPerObservation, lloq));
      }

      public void AddObservations(int numberOfObservations, int pointsPerCalculation, double? lloq)
      {
         addRepositoryToChart(_dataRepositoryCreator.CreateObservationRepository(numberOfObservations, _model, _dataRepositories.Count, pointsPerCalculation, lloq));
      }

      private void addRepositoryToChart(DataRepository newRepository)
      {
         var dataRepositories = new[] { newRepository };
         ChartEditorPresenter.AddDataRepositories(dataRepositories);
         addNewRepositories(dataRepositories);
         addNewCurvesToChart(dataRepositories);
      }

      private void addNewCurvesToChart(IEnumerable<DataRepository> newRepositories)
      {
         using (_chartUpdater.UpdateTransaction(Chart))
         {
            newRepositories.Each(repository => repository.AllButBaseGrid().Each(addColumnToChart));
         }
      }

      private void addColumnToChart(DataColumn dataColumn)
      {
         var curve = Chart.CreateCurve(dataColumn.BaseGrid, dataColumn, Helper.NameDefinition(dataColumn), _dimensionFactory);
         // Settings already in chart, make no changes
         if (Chart.HasCurve(curve.Id))
            return;

         Chart.UpdateCurveColorAndStyle(curve, dataColumn, _dataRepositories.SelectMany(x => x.AllButBaseGrid()).ToList());

         Chart.AddCurve(curve);
      }

      public void AddCalculations(int numberOfCalculations, int pointsPerCalculation)
      {
         addRepositoryToChart(_dataRepositoryCreator.CreateCalculationRepository(numberOfCalculations, _model, _dataRepositories.Count, pointsPerCalculation));
      }

      public void ClearChart()
      {
         using (_chartUpdater.UpdateTransaction(Chart))
         {
            ChartEditorPresenter.RemoveDataRepositories(_dataRepositories);
            _dataRepositories.Each(repository =>
            {
               Chart.RemoveCurvesForDataRepository(repository);
            });
            _dataRepositories.Clear();
         }
      }

      public IChartDisplayPresenter ChartDisplayPresenter => _chartEditorAndDisplayPresenter.DisplayPresenter;

      private void bindCurveChartToEditor()
      {
         Chart = new CurveChart
         {
            OriginText = Captions.ChartFingerprintDataFrom("Test Chart Project", "Test Chart Simulation", DateTime.Now.ToIsoFormat()),
            Title = "The Chart Title"
         };

         Chart.ChartSettings.BackColor = Color.White;
         ChartEditorPresenter.Edit(Chart);

      }

      public CurveChart Chart { get; set; }

      private void configureChartEditorPresenter(IDataColumnToPathElementsMapper dataColumnToPathColumnValuesMapper)
      {
         ChartEditorPresenter.ColumnSettingsFor(BrowserColumns.RepositoryName).GroupIndex = 0;
         ChartEditorPresenter.ColumnSettingsFor(BrowserColumns.DimensionName).GroupIndex = 1;
         ChartEditorPresenter.ColumnSettingsFor(BrowserColumns.Simulation).GroupIndex = -1;
         ChartEditorPresenter.ColumnSettingsFor(BrowserColumns.TopContainer).Caption = "TopC";
         ChartEditorPresenter.ColumnSettingsFor(BrowserColumns.Container).Caption = "Container";
         ChartEditorPresenter.ColumnSettingsFor(BrowserColumns.BottomCompartment).VisibleIndex = 2;
         ChartEditorPresenter.ColumnSettingsFor(BrowserColumns.Molecule).SortColumnName = BrowserColumns.OrderIndex.ToString();
         ChartEditorPresenter.SetDisplayQuantityPathDefinition(setQuantityPathDefinitions(_model, dataColumnToPathColumnValuesMapper));
         ChartEditorPresenter.SetShowDataColumnInDataBrowserDefinition(showDataColumnInDataBrowserDefinition);
         ChartEditorPresenter.ApplyAllColumnSettings();
      }

      public IChartEditorPresenter ChartEditorPresenter => _chartEditorAndDisplayPresenter.EditorPresenter;

      private static Func<DataColumn, PathElements> setQuantityPathDefinitions(IContainer model, IDataColumnToPathElementsMapper dataColumnToPathColumnValuesMapper)
      {
         return col => dataColumnToPathColumnValuesMapper.MapFrom(col, model.RootContainer);
      }

      private static bool showDataColumnInDataBrowserDefinition(DataColumn dataColumn)
      {
         if (dataColumn.Name.StartsWith("B")) return false;

         return true;
      }

      private static void onChartDisplayDragOver(object sender, DragEventArgs e)
      {
         e.Effect = e.Data.GetDataPresent(typeof(string)) ? DragDropEffects.Move : DragDropEffects.None;
      }

      public void SaveChart()
      {
         var fileName = getFileName(new SaveFileDialog());
         if (string.IsNullOrEmpty(fileName)) return;

         _dataPersistor.Save(_dataRepositories, fileName.Replace(".", "_d."));

         _dataPersistor.Save(Chart, fileName);
      }

      public void SaveSettings()
      {
         var fileName = getFileName(new SaveFileDialog());
         if (string.IsNullOrEmpty(fileName)) return;

         _dataPersistor.Save(_chartEditorAndDisplayPresenter.CreateSettings(), fileName);
      }

      public void SaveChartWithDataWithoutValues()
      {
         var fileName = getFileName(new SaveFileDialog());
         if (string.IsNullOrEmpty(fileName)) return;

         _chartTemplatePersistor.SerializeToFileBasedOn(Chart, fileName);
      }

      public void LoadSettings()
      {
         var fileDialog = new OpenFileDialog();
         var fileName = getFileName(fileDialog);
         if (string.IsNullOrEmpty(fileName) || !fileDialog.CheckFileExists)
            return;

         var settingsPersister = new DataPersistor(_ospSuiteXmlSerializerRepository);
         var settings = settingsPersister.Load<ChartEditorAndDisplaySettings>(fileName);
         _chartEditorAndDisplayPresenter.CopySettingsFrom(settings);
      }

      public void RefreshDisplay()
      {
         _chartUpdater.Update(Chart);
      }

      public void ReloadMenus()
      {
         ChartEditorPresenter.ClearButtons();
         var saveAsTemplate = new TestProgramButton("Save as Chart Template", onChartSave);

         ChartEditorPresenter.AddButton(saveAsTemplate);


         var groupMenu = CreateSubMenu.WithCaption("Layouts");
         EnumHelper.AllValuesFor<Layouts>().Each(
            layout => groupMenu.AddItem(new TestProgramButton(layout.ToString(), onLayouts)));

         groupMenu.AddItem(new TestProgramButton("Dummy", () => { }).AsGroupStarter());

         ChartEditorPresenter.AddButton(groupMenu);

         ChartEditorPresenter.AddUsedInMenuItem();

         ChartEditorPresenter.AddChartTemplateMenu(_simulationSettings, template => loadFromTemplate(template));
      }

      private void loadFromTemplate(CurveChartTemplate template)
      {
         _chartFromTemplateService.InitializeChartFromTemplate(
            Chart,
            ChartEditorPresenter.AllDataColumns,
            template,
            Helper.NameDefinition, false);
      }

      public void RemoveDatalessCurves()
      {
         Chart.RemoveDatalessCurves();
      }

      private void onLayouts()
      {
      }

      private void onChartSave()
      {
      }

      public void LoadChart()
      {
         var fileDialog = new OpenFileDialog();
         var fileName = getFileName(fileDialog);
         if (string.IsNullOrEmpty(fileName) || !fileDialog.CheckFileExists)
            return;

         var dataPersitor = new DataPersistor(_ospSuiteXmlSerializerRepository);
         _dataRepositories = dataPersitor.Load<ICache<string, DataRepository>>(fileName.Replace(".", "_d."), _dimensionFactory);

         Chart = dataPersitor.Load<CurveChart>(fileName, _dimensionFactory, _dataRepositories);

         ChartEditorPresenter.Clear();

         ChartEditorPresenter.AddDataRepositories(_dataRepositories);
         ChartEditorPresenter.Edit(Chart);
      }

      private string getFileName(FileDialog fileDialog)
      {
         fileDialog.InitialDirectory = @"c:\Temp\DataChartTest";
         fileDialog.Filter = "xml files (*.xml)|*.xml";
         fileDialog.FilterIndex = 2;
         fileDialog.RestoreDirectory = true;

         string fileName;

         if (fileDialog.ShowDialog() != DialogResult.OK)
            fileName = string.Empty;
         else
            fileName = fileDialog.FileName;

         return fileName;
      }
   }
}