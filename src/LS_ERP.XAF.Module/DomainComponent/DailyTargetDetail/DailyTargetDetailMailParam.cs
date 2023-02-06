using DevExpress.ExpressApp.DC;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class DailyTargetDetailMailParam
    {
        public string ToAddress { get; set; }
        public string CCs { get; set; }
        public string BCCs { get; set; }
        public string Subject { get; set; }
        public string ImportFilePath { get; set; }

    }
}
