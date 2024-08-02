using mysb_forms.core.Interfaces;
using mysb_forms.core.Models;
using Microsoft.Extensions.Options;

namespace mysb_forms.core;

public class Settings : ISettings {
    public OfpmSetting OfpmSettings { get; }
    public IPassSetting IPassSettings { get; }
    public Settings(IOptionsSnapshot<OfpmSetting> ofpmSettings, IOptionsSnapshot<IPassSetting> ipassSettings) {
        OfpmSettings = ofpmSettings.Value;
        IPassSettings = ipassSettings.Value;
    }
}
