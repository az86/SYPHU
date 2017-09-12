using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SYPHU.Data;

namespace SYPHU.ViewModels
{
    public class WizardVM : VMBase
    {
        public static MensurationMethodVM MensurationMethod = new MensurationMethodVM();

        public static AssayDesignVM AssayDesign = new AssayDesignVM();

        public static CalculationModelVM CalculationModel = new CalculationModelVM();

        public static DataSizeInfoVM DataInfo = new DataSizeInfoVM();

        public static DataTransformationFormulaVM DataTransformationFormula = new DataTransformationFormulaVM();

        public static AbnormalDataCheckMethodVM AbnormalDataCheckMethod = new AbnormalDataCheckMethodVM();
    }
}
