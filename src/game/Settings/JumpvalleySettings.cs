using JumpvalleyGame.Settings.Display;

namespace JumpvalleyGame.Settings
{
    /// <summary>
    /// Main list of user settings for Jumpvalley
    /// </summary>
    public partial class JumpvalleySettings
    {
        public SettingGroup Group;

        public JumpvalleySettings()
        {
            SettingGroup group = new SettingGroup();

            group.AddSettingGroup(new DisplaySettings());
            group.ShouldDisplayTitle = false;

            Group = group;
        }
    }
}
