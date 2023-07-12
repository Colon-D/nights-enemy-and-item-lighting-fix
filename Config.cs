using System.ComponentModel;
using nights.test.enemyanditemlightingfix.Template.Configuration;

namespace nights.test.enemyanditemlightingfix.Configuration;

public class Config : Configurable<Config>
{
	/*
        User Properties:
            - Please put all of your configurable properties here.
    
        By default, configuration saves as "Config.json" in mod user config folder.    
        Need more config files/classes? See Configuration.cs
    
        Available Attributes:
        - Category
        - DisplayName
        - Description
        - DefaultValue

        // Technically Supported but not Useful
        - Browsable
        - Localizable

        The `DefaultValue` attribute is used as part of the `Reset` button in Reloaded-Launcher.
    */

	[DisplayName("Fix Enemy Lighting")]
    [DefaultValue(true)]
    public bool FixEnemyLighting { get; set; } = true;
	[DisplayName("Fix Item Lighting")]
	[DefaultValue(true)]
	public bool FixItemLighting { get; set; } = true;
}

/// <summary>
/// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
/// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
/// </summary>
public class ConfiguratorMixin : ConfiguratorMixinBase
{
    // 
}
