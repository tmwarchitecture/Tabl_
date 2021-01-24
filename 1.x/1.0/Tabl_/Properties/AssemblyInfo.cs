using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rhino.PlugIns;

// Plug-in Description Attributes - all of these are optional.
// These will show in Rhino's option dialog, in the tab Plug-ins.
[assembly: PlugInDescription(DescriptionType.Address, "-")]
[assembly: PlugInDescription(DescriptionType.Country, "USA")]
[assembly: PlugInDescription(DescriptionType.Email, "hotcoffeegroup@gmail.com")]
[assembly: PlugInDescription(DescriptionType.Phone, "-")]
[assembly: PlugInDescription(DescriptionType.Fax, "-")]
[assembly: PlugInDescription(DescriptionType.Organization, "-")]
[assembly: PlugInDescription(DescriptionType.UpdateUrl, "https://www.food4rhino.com/app/tabl")]
[assembly: PlugInDescription(DescriptionType.WebSite, "-")]

// Icons should be Windows .ico files and contain 32-bit images in the following sizes: 16, 24, 32, 48, and 256.
// This is a Rhino 6-only description.
[assembly: PlugInDescription(DescriptionType.Icon, "Tabl_.EmbeddedResources.plugin-utility.ico")]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Tabl_")]

// This will be used also for the plug-in description.
[assembly: AssemblyDescription(@"Tabl_ is a spreadsheet interface within Rhino for the viewing, editing, and exporting of object properties

This version was ported by Will Wang, in reference to Rhino SR 6.24

For more information see repo
https://github.com/tmwarchitecture/Tabl_

See link on Food4Rhino
https://www.food4rhino.com/app/tabl

 - Architecture schedules
 - Area take-offs
 - Object queries based on properties
 - Examining large number of geometries
 - Object information extraction
")]

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Tabl_")]
[assembly: AssemblyCopyright("Copyright ©  Timothy Williams 2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e7bb6668-fb3d-4925-ab84-bb6761eca565")] // This will also be the Guid of the Rhino plug-in

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("1.0")]
[assembly: AssemblyFileVersion("1.0")]

// Make compatible with Rhino Installer Engine
[assembly: AssemblyInformationalVersion("2")]