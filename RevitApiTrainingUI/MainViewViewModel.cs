using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
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

        public DelegateCommand SaveCommand { get; }
        public List<Element> PickedObjects { get; } = new List<Element>();
        public List<PipingSystemType> PipeSystems { get; } = new List<PipingSystemType>();
        public PipingSystemType SelectedPipeSystem { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
             _commandData = commandData;
            SaveCommand = new DelegateCommand(OnSaveCommand);
            PickedObjects = SelectionUtils.PickObjects(commandData);
            PipeSystems = PipesUtils.GetPipeSystems(commandData);
        }

        private void OnSaveCommand()
        {
            UIApplication uiApplication = _commandData.Application;
            UIDocument uIDocument = uiApplication.ActiveUIDocument;
            Document document = uIDocument.Document;

            if (PickedObjects.Count() == 0 || SelectedPipeSystem == null)
                return;

            using (var ts = new Transaction(document, "Set system type"))
            {
                ts.Start();
                foreach (var pickedObject in PickedObjects)
                {
                    if (pickedObject is Pipe)
                    {
                        var pipe = pickedObject as Pipe;
                        pipe.SetSystemType(SelectedPipeSystem.Id);
                    }
                }
                ts.Commit();
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
