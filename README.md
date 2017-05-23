# An example .NET BIV connector
After generating the service from the WSDL the following changes needs to be done to the generated code.

## Update ServiceContractAttribute
### Before
```csharp
[System.ServiceModel.ServiceContractAttribute(Namespace="http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/", ConfigurationName="CSAanleverService.AanleverService_V1_2")]
```

### After
```csharp
[System.ServiceModel.ServiceContractAttribute(Namespace="http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/", ConfigurationName="CSAanleverService.AanleverService_V1_2", ProtectionLevel = System.Net.Security.ProtectionLevel.Sign)]
```

## Update OperationContractAttribute
### Before
```csharp
[System.ServiceModel.OperationContractAttribute(Action="http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/AanleverService_V1_2/aanle" +
```

### After
```csharp
[System.ServiceModel.OperationContractAttribute(Action="http://logius.nl/digipoort/wus/2.0/aanleverservice/1.2/AanleverService/aanle" +
```