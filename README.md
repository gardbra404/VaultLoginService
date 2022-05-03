# VaultLoginAPI

## Purpose
This application serves as a testing application for Hashicorp's Vault KeyManager application. This API
allows for an authenticated user to access credentials housed within a secret store, if the user is authorized to access said credential.

Administrative users are also able to add, delete and modify users, as well as add and modify credentials listed within the credential store. 

## Setup
This application requires that HashiCorp Vault be installed on your machine first, instructions for installing Vault can be found here https://www.vaultproject.io/. Once Vault is installed, you must start the Vault server by using the vault server command from the termial, including the -config flag pointed toward the configuration file for the Vault, which must be of the HCL (HasiCorp Configuration Language) format, see the config.hcl file found within this repo for an example.

Once the Vault has been started, it must then be opened by using the unseal command (if the vault has not been initialized, you must enter vault operator init to generate the unseal keys as well as the root auth token). Enter vault unseal {{unseal key}} into the terminal to begin unsealing the vault, the number of keys you will need to enter is based on how you have initialized the vault. At this point, it is time to start the application.

The application, being written in C#, requires that the dotnet framework be installed on the computer which can be installed from here https://dotnet.microsoft.com/en-us/download/dotnet/6.0. Once the framework is installed, the application can be started by either opening the solution file (.sln) in Visual Studio and selecting run, or by navigating to the project directory in the terminal and using the dotnet run command. 

## Using the application
In order to use the application, either utilize a separate application to perform HTTP requests to server, or, if the application is still in development mode, use the generated swagger page to perform calls on the various endpoints using the respective arguments and HTTP method. For more on swagger, please consult https://swagger.io/docs/.

## Application endpoints

### /api/Login
The login endpoint for all non admin users to retrieve permitted credentials, if the login allows for the requested credential to be returned, it will return it, a 403 otherwise

#### Method
POST

#### Paramters
* UID:string - the unique identifier for the user.
* Token:string - the Vault authorization Token assigned to UID.
* RequestedKey:string - the name of the credential the user wishes to access.

### /api/Admin
The administrative endpoint used to add a new user to the Vault and define their credentials.

#### Method
PUT

#### Parameters
* LoginToken:string - the Vault authorization token assigned to the admin.
* UID:string - the UID to create a new login for.
* Permissions:string list - the list of credentials that the UID is allowed access to.

### /api/Admin/DeleteItem
The administrative endpoint to delete a user's allowed credentials, removing their access

#### Method
DELETE

#### Parameters
* LoginToken:string - the Vault authorization token assigned to the admin.
* ItemID:string - the UID which will have its permissions revoke in Vault

### /api/Admin/UpdateItem
The administrative endpoint to add and/or delete allowed credentials for a specified user. 

#### Method
POST

#### Parameters
* LoginToken:string - the Vault authorization token assigned to the admin.
* Policy:string - the Vault UID that will have its credentials updated.
* AddedCreds:string list - the list of credentials that will be added to the user's allowed list.
* DeletedCreds: string list - the list of credentials that will be removed from the user's allowed list.

### /api/Admin/AddCred
The administrative endpoint to add and modify credentials stored within the Vault credential store.

#### Method
POST

#### Parameters
* LoginToken:string - the Vault authorization token assigned to the admin.
* SecretKey:string - the name of the credential which will be added or modified inside of the store.
* SecretValue:string - the credential value that will be set within the secret store.