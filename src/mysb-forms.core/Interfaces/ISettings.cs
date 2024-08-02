using mysb_forms.core.Models;

namespace mysb_forms.core.Interfaces {
    public interface ISettings {
        OfpmSetting OfpmSettings { get; }
        IPassSetting IPassSettings { get; }
    }
}

