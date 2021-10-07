This repository contains a sample ASP.NET Web application to demonstrate using Azure KeyValut to achieve enterprise level encryption and decryption.

Follow the blog here [Achieve Enterprise Level Encryption In Your Applications](https://www.altumlabs.com)

## To run this application locally

1. Clone the repo with the following command:

   ```bash
   git clone https://github.com/Altum-LLC/keyvault-encryption.git
   ```

2. Open solution in Visual Studio.

3. Update the parameters in appSettings.Development.json for local development. Optionally you can add secrets in the local secret store or environment variables.

	```bash
	"AzureAd": {
		"Instance": "https://login.microsoftonline.com/",
		"Domain": "DOMAIN NAME REGISTERED FOR YOUR AZURE TENANT",
		"ClientId": "CLIENT ID GENERATED FOR THE REGISTERED APP IN YOUR AZURE TENANT",
		"ClientSecret": "CLIENT SECRET GENERATED FOR THE REGISTERED APP IN YOUR AZURE TENANT",
		"TenantId": "YOUR AZURE TENANT ID",
		"CallbackPath": "/signin-oidc"
	},
	"KeyVault": {
		"VaultUri": "YOUR AZURE KEYVALUT URL",
		"KeyId": "YOUR AZURE KEYVALUT KEY URL"
	},

	```

4. Run the application using Visual Studio or by running the command `dotnet run`.

