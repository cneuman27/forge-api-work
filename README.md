# forge-api-work

Temporary repository to hold testing and experimental code to access the Forge API.

You may need to set up a local nuget source to access ModelAccess.AHU.AspNet (in UnitViewer.API) and MOM.Shared.WPF (in ForgeTest).  If this is necessary, nuget packages are in the jci-packages folder...


Solution contains the following:

1.) _3rdParty 

    Inventor COM Interop and SpreadsheetGear DLLs

2.) AppBundles/StandardPartProcessing

    Design Automation Plugin for simple IPT processing
    
3.) ForgeAPI

    .NET(Standard) access wrappers for the Forge REST API
    Includes IoC Containers for ASP.NET and Autofac
    
4.) UnitViewer

    Angular client application and ASP.NET API server to test Forge Viewer capabilities 
    and integration with AHU data model
    
5.) ForgeTest
  
    .NET WPF client used to understand various ForgeAPI endpoints (Data Management, Model Derivative, Viewer) 
    and test access wrapper libraries 
    
6.) InventorLib

    Test harness and Inventor API facade for Design Automation testing and debugging    
    
7.) JSONFormatters

    Serialization/deserialization support for custom MIME types used by AHU data model.
    
