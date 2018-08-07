# No-Mans-GUI
A GUI based editor for MBINCompiler

## Planned/Completed Features ##
This is a rough list of the features that are planned/completed to be included.  If there's a check in the box it's included in the latest release.
 * [ ] GUI based editing of MBINs.  No more exml!
 * [ ] Sensible Input types
   * [ ] Boolean values will be checkboxes
   * [ ] Arrays will have all possible values in drop-down
   * [ ] Color Pickers!
   * [ ] Structs will show all possible child structs (will be super useful in `ENTITY` files and interactions) eventually
   * [ ] Paths to other MBINs will function as links to open them directly
   * [ ] Load models into NMS Model Viewer with one click (once NMS Model Viewer is updated)
   * [ ] Fields that reference text in the LANGUAGE files will automatically translate said text
 * [ ] Built-in documentation
   * [ ] Basic Program usage
   * [ ] Descriptions of structs with what they do and how to use them
 * [ ] Semi-Automated mod merging and compatibility patch building
   * [ ] Dynamic Lists will be merged together between 2 mods
   * [ ] Filter out and view just the items that have been changed from vanilla
   * [ ] Automate the merging or two mods into a compatibility patch if everything is compatible and flag that which is not
 * [ ] FOMOD Installer builder.
   * [ ] Have installation windows to present compatibility patches and mod options just like Bethesda games have
   * [ ] Only compatible with Nexus Mod Manager and Vortex
 * [ ] Quest Builder Interface
   * [ ] Won't be available for awhile but when released will make custom quests much easier to build.
 * [ ] Integration with *the entire game structure* not just single MBINs
   * [ ] Program will load up all the relecvant MBINs and you can work with them simultaneously
   * [ ] No more jumping between 100 exml files just to change one thing
 * [ ] Theme Support
 * [ ] Mass Editing Support
    * [ ] Make mass changes to multiple files and records based on rules (similar to Excel)
    * [ ] Add Prefixes or Append to the end of multiple strings
    * [ ] *Math Based Changes* (ex. multiply all values of fields with *xyz* name by 2)
 * [ ] New More Organized File Structure 
    * [ ] Planned to be used as the new normal.  A File Structure that separates Modded files from Vanilla files as much as possible
    * [ ] `CUSTOMMODELS` `CUSTOMMETADATA` folders
    * [ ] Files utilizing this new structure will be more compatible and will overwrite mods that do not follow this structure by default.  Ensuring compatibility and conformance.
 * [ ] Direct Integration with Modders Alliance
    * [ ] Modders Alliance will define a set of guidelines and Best Practices to ensure that mods don't negatively affect other players and promote compatibility and modularity.
    * [ ] Using this program will ensure your mod follows proper guidelines and Best Practices
    * [ ] Will allow for Mod Pack definitions so a player can just share a Bit.ly link which will direct users to a list of all the mods being used on their planet/game/etc so that different players can *sync* their mods easily and with zero compatibility issues.
    * [ ] Will assist in organizing the community and provide a framework to make learning how to mod and compatibility much easier.

