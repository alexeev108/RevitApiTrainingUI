using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitApiTrainingUI
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand PipesQuantity { get; }
        public DelegateCommand WallsVolume { get; }
        public DelegateCommand DoorsQuantity { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
             _commandData = commandData;
            PipesQuantity = new DelegateCommand(OnSelectCommandPipes);
            WallsVolume = new DelegateCommand(OnSelectCommandWalls);
            DoorsQuantity = new DelegateCommand(OnSelectCommandDoors);
        }        

        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnSelectCommandPipes()
        {
            RaiseHideRequest();
            List<Element> selectedElement = SelectionUtils.PipesSelection(_commandData);

            TaskDialog.Show("Сообшение", $"Количество всех труб: {selectedElement.Count} шт.");

            RaiseShowRequest();
        }

        private void OnSelectCommandWalls()
        {
            RaiseHideRequest();
            List<Element> selectedElement = SelectionUtils.WallsSelection(_commandData);

            double sumVolume = 0;

            foreach (Element element in selectedElement)
            {
                sumVolume += element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble();
            }

            double fromUnits = Math.Round(UnitUtils.ConvertFromInternalUnits(sumVolume, UnitTypeId.CubicMeters), 2);

            TaskDialog.Show("Сообшение", $"Объем всех стен: {fromUnits} м3");

            RaiseShowRequest();
        }

        private void OnSelectCommandDoors()
        {
            RaiseHideRequest();
            List<Element> selectedElement = SelectionUtils.DoorsSelection(_commandData);

            TaskDialog.Show("Сообшение", $"Количество всех дверей: {selectedElement.Count} шт.");

            RaiseShowRequest();
        }
    }
}
