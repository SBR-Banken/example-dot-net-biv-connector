# An example .NET BIV connector

## Configuration of sample code
Any configuration that is necessary for running this sample code is in the constants of the file AbstractService.cs and the files that are referenced there.

# Generating the services from WSDL
After generating the services from the WSDL the following changes need to be done to the generated code (Reference.cs).
This needs to be done for each Service Reference. As an example the changes for AanleverService are shown below. These lines can occur multiple times.

## Update ServiceContractAttribute: add ProtectionLevel
### Before
```csharp
[System.ServiceModel.ServiceContractAttribute(Namespace="http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/", ConfigurationName="CSAanleverService.AanleverService_V1_2")]
```

### After
```csharp
[System.ServiceModel.ServiceContractAttribute(Namespace="http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/", ConfigurationName="CSAanleverService.AanleverService_V1_2", ProtectionLevel = System.Net.Security.ProtectionLevel.Sign)]
```

## Update OperationContractAttribute: remove version
### Before
```csharp
[System.ServiceModel.OperationContractAttribute(Action="http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/AanleverService_V1_2/aanle" +
```

### After
```csharp
[System.ServiceModel.OperationContractAttribute(Action="http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/AanleverService/aanle" +
```