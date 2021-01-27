using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rhino.PlugIns;

// Plug-in Description Attributes - all of these are optional.
// These will show in Rhino's option dialog, in the tab Plug-ins.
[assembly: PlugInDescription(DescriptionType.Address, @"NE/NY")]
[assembly: PlugInDescription(DescriptionType.Country, "USA")]
[assembly: PlugInDescription(DescriptionType.Email, "hotcoffeegroup@gmail.com")]
[assembly: PlugInDescription(DescriptionType.Phone, "-")]
[assembly: PlugInDescription(DescriptionType.Fax, "-")]
[assembly: PlugInDescription(DescriptionType.Organization, "two individuals Tim and Will")]
[assembly: PlugInDescription(DescriptionType.UpdateUrl, "-")]
[assembly: PlugInDescription(DescriptionType.WebSite, @"https://github.com/tmwarchitecture/Tabl_")]

// Icons should be Windows .ico files and contain 32-bit images in the following sizes: 16, 24, 32, 48, and 256.
// This is a Rhino 6-only description.
[assembly: PlugInDescription(DescriptionType.Icon, "Tabl_.EmbeddedResources.plugin-utility.ico")]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Tabl_")]

// This will be used also for the plug-in description.
[assembly: AssemblyDescription("Tabl_ spreadsheet interface")]

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("two individuals Tim and Will")]
[assembly: AssemblyProduct("Tabl_")]
[assembly: AssemblyCopyright("Copyright ©Timothy Williams  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("878bfac7-46ac-4fe9-a087-bb083fdab088")] // This will also be the Guid of the Rhino plug-in

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

[assembly: AssemblyVersion("2.0")]
[assembly: AssemblyFileVersion("2.0")]

// Make compatible with Rhino Installer Engine
[assembly: AssemblyInformationalVersion("2")]
