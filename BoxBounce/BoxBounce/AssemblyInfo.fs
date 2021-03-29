namespace FSharpRhinoScripting

open System.Runtime.InteropServices
open System.Reflection
open Rhino.PlugIns

module AssemblyInfo =
    // Plug-in Description Attributes - all of these are optional
    // These will show in Rhino's option dialog, in the tab Plug-ins
    [<assembly: PlugInDescription(DescriptionType.Address, "-")>]
    [<assembly: PlugInDescription(DescriptionType.Country, "-")>]
    [<assembly: PlugInDescription(DescriptionType.Email, "x@y.z")>]
    [<assembly: PlugInDescription(DescriptionType.Phone, "-")>]
    [<assembly: PlugInDescription(DescriptionType.Fax, "-")>]
    [<assembly: PlugInDescription(DescriptionType.Organization, "-")>]
    [<assembly: PlugInDescription(DescriptionType.UpdateUrl, "-")>]
    [<assembly: PlugInDescription(DescriptionType.WebSite, "-")>]

    // General Information about an assembly is controlled through the following 
    // set of attributes. Change these attribute values to modify the information
    // associated with an assembly.
    [<assembly: AssemblyTitle("x")>] // Plug-In title is extracted from this
    [<assembly: AssemblyDescription("x")>]
    [<assembly: AssemblyConfiguration("")>]
    [<assembly: AssemblyCompany("x")>]
    [<assembly: AssemblyProduct("x")>]
    [<assembly: AssemblyCopyright("x")>]
    [<assembly: AssemblyTrademark("")>]
    [<assembly: AssemblyCulture("")>]

    // Setting ComVisible to false makes the types in this assembly not visible 
    // to COM components.  If you need to access a type in this assembly from 
    // COM, set the ComVisible attribute to true on that type.
    [<assembly: ComVisible(false)>]

    // The following GUID is for the ID of the typelib if this project is exposed to COM. call to get new: System.Guid.NewGuid()
    [<assembly: Guid("1d99f9a1-6bf9-4fd2-b68b-4017b6771a68")>] // This will also be the Guid of the Rhino plug-in

    // Version information for an assembly consists of the following four values:
    //      Major Version
    //      Minor Version 
    //      Build Number
    //      Revision
    // You can specify all the values or you can default the Build and Revision Numbers 
    // by using the '*' as shown below:
    // [assembly: AssemblyVersion("1.0.*")]
    [<assembly: AssemblyVersion("0.0.0.1")>]
    [<assembly: AssemblyFileVersion("0.0.0.1")>]
    ()

