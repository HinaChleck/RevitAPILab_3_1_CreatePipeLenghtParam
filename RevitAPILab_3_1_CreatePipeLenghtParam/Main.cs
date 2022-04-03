using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPILab_3_1_CreatePipeLenghtParam
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        //Устанавливает значение параметра для выбранных экземпляров с запасом 10%. Параметр должен быть уже создан в ФОП
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Application application = uiapp.Application;


            IList<Reference> selectedElementRefList = uidoc.Selection.PickObjects(ObjectType.Element, new PipeFilter(), "Выберите трубы"); //список ссылок на элементы
                       
            foreach (var selectedElement in selectedElementRefList)
            {
                Pipe oPipe = doc.GetElement(selectedElement) as Pipe;

                double lengthParam = oPipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();

                double extraLength = lengthParam * 1.1;

                var categorySet = new CategorySet();
                categorySet.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves));

                using (Transaction ts = new Transaction(doc, "Set parameter"))
                {
                    ts.Start();
                    Parameter lParameter = oPipe.LookupParameter("Длина с запасом");
                    lParameter.Set(extraLength);

                    ts.Commit();
                }
            }

            return Result.Succeeded;
        }

    }
}
