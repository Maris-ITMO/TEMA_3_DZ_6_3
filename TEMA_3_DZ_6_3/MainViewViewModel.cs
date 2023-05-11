using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using Prism.Commands;
using TEMA_3_DZ_6_3_Library;

namespace TEMA_3_DZ_6_3
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public List<FamilySymbol> FamilyTypes { get; } = new List<FamilySymbol>();
        public List<XYZ> Points { get; } = new List<XYZ>();
        public DelegateCommand SaveCommand { get; }
        public FamilySymbol SelectedFamilyType { get; set; }
        public int ElementCount { get; set; }
        public Level SelectedLevel { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            FamilyTypes = FamilySymbolUtils.GetFamilySymbols(commandData);
            Points = SelectionUtils.GetPoints(_commandData, "Выберите точки", ObjectSnapTypes.Endpoints);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            SelectedLevel = LevelUtils.GetLevels(commandData)[0];
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (Points.Count < 1 ||
                SelectedFamilyType == null)
                return;

            double xLength = Points[1].X - Points[0].X;
            double yLength = Points[1].Y - Points[0].Y;
            double zLength = Points[1].Z - Points[0].Z;


            List<XYZ> intermediatePointsList = new List<XYZ>();

            for (int i = 1; i < ElementCount + 1; i++)
            {
                var point = new XYZ(Points[0].X + (xLength / (ElementCount + 1)) * i, Points[0].Y + (yLength / (ElementCount + 1)) * i, Points[0].Z + (zLength / (ElementCount + 1)) * i);
                intermediatePointsList.Add(point);
            }

            foreach (var point in intermediatePointsList)
            {
                FamilyInstanceUtils.CreateFamilyInstance(_commandData, SelectedFamilyType, point, SelectedLevel);
            }

            RaiseCloseRequest();
        }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}